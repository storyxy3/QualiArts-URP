using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
namespace VL.Rendering
{
    public class VLPreDepthIDPass : ScriptableRenderPass
    {
        private static readonly ShaderTagId _depthIDShaderTagId = new ShaderTagId("DepthID");
        private static readonly int _CameraNormalsTexture = Shader.PropertyToID("_CameraNormalsTexture");
        private static readonly int _CameraDepthTexture = Shader.PropertyToID("_CameraDepthTexture");
        private RTHandle _depthTextureHandle;
        private RTHandle _normalAttachmentHandle;
        private FilteringSettings _filteringSettings;
        private CustomRenderPassData _renderPassData;
        private CustomSubPassData[] _subPassData;
        public VLPreDepthIDPass(RenderPassEvent evt, RenderQueueRange renderQueueRange, LayerMask layerMask)
        {
            profilingSampler = new ProfilingSampler("VLPreDepthIDPass");
            _filteringSettings = new FilteringSettings(renderQueueRange, layerMask, 1);
            renderPassEvent = evt;
            useNativeRenderPass = false;
            _renderPassData = new CustomRenderPassData
            {
                attachmentDescriptors = new[]
                {
                    new AttachmentDescriptor(GraphicsFormat.R8G8B8A8_UNorm)
                    {
                        clearColor = Color.clear,
                        loadAction = RenderBufferLoadAction.Clear,
                        storeAction = RenderBufferStoreAction.Store
                    },
                    new AttachmentDescriptor(GraphicsFormat.D24_UNorm)
                    {
                        loadAction = RenderBufferLoadAction.Clear,
                        storeAction = RenderBufferStoreAction.Store
                    }
                },
                depthAttachmentIndex = 1
            };
            _subPassData = new[]
            {
                new CustomSubPassData
                {
                    colors = new[] { 0 },
                    profilingSampler = new ProfilingSampler("DepthID And Normal"),
                    renderAction = OnRenderDepthAndNormal
                }
            };
        }
        public void Setup(RTHandle depthTextureHandle, RTHandle normalAttachmentHandle)
        {
            _depthTextureHandle = depthTextureHandle;
            _normalAttachmentHandle = normalAttachmentHandle;
        }
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData) { ResetTarget(); }
        private void OnRenderDepthAndNormal(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var settings = CreateDrawingSettings(_depthIDShaderTagId, ref renderingData, renderingData.cameraData.defaultOpaqueSortFlags);
            settings.perObjectData = PerObjectData.None;
            context.DrawRenderers(renderingData.cullResults, ref settings, ref _filteringSettings);
            RenderingBridge.Execute(RenderingBridge.EventType.PreDepth, context, ref renderingData);
        }
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, profilingSampler))
            {
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                _renderPassData.attachmentDescriptors[0].loadStoreTarget = _normalAttachmentHandle.nameID;
                _renderPassData.attachmentDescriptors[1].loadStoreTarget = _depthTextureHandle.nameID;
                CustomRenderPassUtility.Execute(context, ref renderingData, _renderPassData, _subPassData);
                cmd.SetGlobalTexture(_CameraNormalsTexture, _normalAttachmentHandle.nameID);
                cmd.SetGlobalTexture(_CameraDepthTexture, _depthTextureHandle.nameID);
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}
