using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace VL.Rendering
{
    public class SubWorldPass : ScriptableRenderPass
    {
        public static ShaderTagId SubWorldGBuffer = new ShaderTagId("SubWorldGBuffer");
        private readonly ProfilingSampler _profilingSampler = new ProfilingSampler("SubWorld");
        private readonly ProfilingSampler _maskProfilingSampler = new ProfilingSampler("Mask");
        private readonly ProfilingSampler _emissionProfilingSampler = new ProfilingSampler("SubWorld Emission");
        private RTHandle _subWorldBuffer;
        private Material _subWorldMaterial;
        private SubWorldVolume _subWorld;
        private bool _isDeferredRenderingMode;
        private RenderStateBlock _maskRenderStateBlock;
        private RenderStateBlock _subWorldGBufferRenderState;
        private Matrix4x4 _reflectionMatrix;
        private Matrix4x4 _projectionMatrix;
        private CustomRenderPassData _renderPass;
        private CustomSubPassData[] _subPasses;
        private VLSkyPass _skyPass;

        public SubWorldPass(Material subWorldMaterial)
        {
            renderPassEvent = RenderPassEvent.AfterRenderingPrePasses;
            _subWorldMaterial = subWorldMaterial;
            _maskRenderStateBlock = default;
            _subWorldGBufferRenderState = new RenderStateBlock(RenderStateMask.Stencil)
            {
                stencilReference = 8,
                stencilState = new StencilState(true, 32, 8, CompareFunction.NotEqual, StencilOp.Replace)
            };
            _skyPass = new VLSkyPass(renderPassEvent, _subWorldMaterial, 2);
            useNativeRenderPass = false;
            var attachments = new[]
            {
                new AttachmentDescriptor(GraphicsFormat.R8G8B8A8_SRGB),
                new AttachmentDescriptor(GraphicsFormat.D24_UNorm_S8_UInt)
            };
            attachments[0].ConfigureTarget(BuiltinRenderTextureType.None, false, true);
            attachments[0].ConfigureClear(Color.clear, 1f, 0);
            attachments[1].ConfigureTarget(BuiltinRenderTextureType.None, false, false);
            attachments[1].ConfigureClear(Color.clear, 1f, 0);
            _renderPass = new CustomRenderPassData
            {
                attachmentDescriptors = attachments,
                depthAttachmentIndex = 1,
                memoryless = new[] { false, true }
            };
            _subPasses = new[]
            {
                new CustomSubPassData { colors = new[] { 0 }, renderAction = RenderSubWorld }
            };
        }

        public void Dispose()
        {
            _subWorldBuffer?.Release();
        }

        public void Setup(SubWorldVolume volume, bool isDeferredRenderingMode)
        {
            _subWorld = volume;
            _isDeferredRenderingMode = isDeferredRenderingMode;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            var descriptor = renderingData.cameraData.cameraTargetDescriptor;
            if (_subWorld.mode == SubWorldMode.Default)
            {
                descriptor.width >>= 1;
                descriptor.height >>= 1;
                descriptor.graphicsFormat = GraphicsFormat.R8G8B8A8_SRGB;
            }
            RenderingUtils.ReAllocateIfNeeded(ref _subWorldBuffer, descriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, name: "_SubWorldBuffer");
            _renderPass.width = descriptor.width;
            _renderPass.height = descriptor.height;
            _renderPass.attachmentDescriptors[0].graphicsFormat = descriptor.graphicsFormat;
            _renderPass.attachmentDescriptors[0].loadStoreTarget = _subWorldBuffer.nameID;
            ResetTarget();
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, _profilingSampler))
            {
                bool additionalShadows = renderingData.shadowData.supportsAdditionalLightShadows;
                CoreUtils.SetKeyword(cmd, ShaderKeywordStrings.MainLightShadows, false);
                CoreUtils.SetKeyword(cmd, ShaderKeywordStrings.MainLightShadowCascades, false);
                CoreUtils.SetKeyword(cmd, ShaderKeywordStrings.SoftShadows, false);
                CoreUtils.SetKeyword(cmd, ShaderKeywordStrings.AdditionalLightShadows, false);
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                CustomRenderPassUtility.Execute(context, ref renderingData, _renderPass, _subPasses);
                CoreUtils.SetKeyword(cmd, ShaderKeywordStrings.MainLightShadows, renderingData.shadowData.mainLightShadowCascadesCount == 1);
                CoreUtils.SetKeyword(cmd, ShaderKeywordStrings.MainLightShadowCascades, renderingData.shadowData.mainLightShadowCascadesCount > 1);
                CoreUtils.SetKeyword(cmd, ShaderKeywordStrings.SoftShadows, renderingData.shadowData.isKeywordSoftShadowsEnabled);
                CoreUtils.SetKeyword(cmd, ShaderKeywordStrings.AdditionalLightShadows, additionalShadows);
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        private void RenderSubWorld(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get();
            var cullResults = renderingData.cullResults;
            var maskDrawing = CreateDrawingSettings(SubWorldUtility.SubWorldMask, ref renderingData, SortingCriteria.None);
            maskDrawing.overrideMaterial = _subWorldMaterial;
            maskDrawing.overrideMaterialPassIndex = 0;
            if (_subWorld.mode == SubWorldMode.Default)
            {
                using (new ProfilingScope(cmd, _maskProfilingSampler))
                {
                    context.ExecuteCommandBuffer(cmd);
                    cmd.Clear();
                    // This exception is present in the game's default-mask path.
                    throw new NotImplementedException("まだ未実装です Materialが必要です");
                }
            }
            renderingData.cameraData.GetViewMatrix();
            renderingData.cameraData.GetProjectionMatrix();
            // TODO(qa-recon): The game's discarded SubWorldVolume member read retains only a null check.
            _ = _subWorld.position;
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            var drawing = CreateDrawingSettings(RenderingUtility.ShaderTagIds.UniversalForwardPerformance, ref renderingData, SortingCriteria.OptimizeStateChanges);
            _maskRenderStateBlock.mask |= RenderStateMask.Stencil;
            _maskRenderStateBlock.stencilReference |= 128;
            var stencil = _maskRenderStateBlock.stencilState;
            SubWorldUtility.UpdateReflectionStencilState(ref stencil);
            _maskRenderStateBlock.stencilState = stencil;
            context.DrawRenderers(cullResults, ref drawing, ref _subWorld.filteringSettings, ref _maskRenderStateBlock);
            if (_subWorld.mode != SubWorldMode.Default)
                cmd.ClearRenderTarget(true, false, Color.clear);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}
