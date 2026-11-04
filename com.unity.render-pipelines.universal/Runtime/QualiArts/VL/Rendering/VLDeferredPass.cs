// VL deferred pass (APK 3.5.3, VL.Rendering.VLDeferredPass): one native render pass with GBuffer / pre-emission /
// actor / emission subpasses, reconstructed from the game.
using System;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.Universal.Internal;

namespace VL.Rendering
{
    internal class VLDeferredPass : ScriptableRenderPass
    {
        public static readonly int _CameraNormalsTexture = Shader.PropertyToID("_CameraNormalsTexture");
        public static readonly int _CameraDepthTexture = Shader.PropertyToID("_CameraDepthTexture");
        private static ShaderTagId s_ShaderTagLit = new ShaderTagId("Lit");
        private static ShaderTagId s_ShaderTagSimpleLit = new ShaderTagId("SimpleLit");
        private static ShaderTagId s_ShaderTagUnlit = new ShaderTagId("Unlit");
        private static ShaderTagId s_ShaderTagUniversalGBuffer = new ShaderTagId("UniversalGBuffer");
        private static ShaderTagId s_ShaderTagUniversalMaterialType = new ShaderTagId("UniversalMaterialType");
        private static ShaderTagId s_ShaderTagMotionVectors = new ShaderTagId("MotionVectors");
        private static readonly ProfilingSampler _drawGBufferProfilingSampler = new ProfilingSampler("Draw GBuffer");
        private static readonly ProfilingSampler _deferredProfilingSampler = new ProfilingSampler("VL Deferred");
        private static readonly ProfilingSampler _deferredFogProfilingSampler = new ProfilingSampler("Deferred Fog");
        public static readonly GraphicsFormat[] GBufferFormats = new GraphicsFormat[]
        {
            GraphicsFormat.R8G8B8A8_SRGB,
            GraphicsFormat.R8G8B8A8_UNorm,
            GraphicsFormat.R16G16B16A16_SFloat,
            GraphicsFormat.B10G11R11_UFloatPack32,
            GraphicsFormat.B10G11R11_UFloatPack32,
        };
        public static readonly AttachmentDescriptor[] ColorAttachments = GBufferFormats.Select(x =>
        {
            var attachment = new AttachmentDescriptor(x);
            attachment.loadAction = RenderBufferLoadAction.Clear;
            attachment.storeAction = RenderBufferStoreAction.DontCare;
            return attachment;
        }).ToArray();
        public static readonly AttachmentDescriptor DepthAttachment = new AttachmentDescriptor(GraphicsFormat.D24_UNorm_S8_UInt)
        {
            loadAction = RenderBufferLoadAction.Clear,
            storeAction = RenderBufferStoreAction.DontCare,
        };

        private readonly VLActorGBuffer _actorGBuffer;
        private readonly VLVirtualEffectGBuffer _virtualEffectGBuffer;
        private readonly DrawObjectsPass _forwardOnlyPass;
        private readonly DrawObjectsPass _fullScreenTransparentPass;
        private readonly VLSkyPass _skyPass;
        private readonly Material _fogMaterial;
        private readonly Material _motionVectorObjectMaterial;
        private DeferredLights _deferredLights;
        private CustomRenderPassData _deferredRenderPass;
        private CustomSubPassData[] _deferredSubPassesNoLit;
        private CustomSubPassData[] _deferredSubPasses;
        private ShaderTagId[] _gBufferShaderTagValues;
        private RenderStateBlock[] _gBufferRenderStateBlocks;
        private FilteringSettings m_FilteringSettings;
        private RenderStateBlock m_RenderStateBlock;
        private FilteringSettings m_MotionVectorFilteringSettingsBefore;
        private FilteringSettings m_MotionVectorFilteringSettingsAfter;
        private RenderStateBlock m_MotionVectorRenderStateBlock;
        private RTHandle _cameraNormalsTexture;
        private RTHandle _cameraDepthTexture;
        private RTHandle _additionalInfoTexture;

        public VLDeferredPass(RenderPassEvent evt, LayerMask opaqueLayerMask, LayerMask transparentLayerMask, StencilState stencilState, int stencilReference, DeferredLights deferredLights, VLSkyPass skyPass, DrawObjectsPass forwardOnlyPass, Material fogMaterial, Material motionVectorObjectMaterial)
        {
            base.profilingSampler = new ProfilingSampler(nameof(VLDeferredPass));
            m_FilteringSettings = new FilteringSettings(RenderQueueRange.opaque, opaqueLayerMask);
            m_RenderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);
            m_RenderStateBlock.stencilState = stencilState;
            m_RenderStateBlock.stencilReference = stencilReference;
            m_RenderStateBlock.mask = RenderStateMask.Stencil;
            m_MotionVectorFilteringSettingsBefore = new FilteringSettings(VLRenderQueue.MotionVectorRangeBefore, opaqueLayerMask, 1);
            m_MotionVectorFilteringSettingsAfter = new FilteringSettings(VLRenderQueue.MotionVectorRangeAfter, opaqueLayerMask, 1);
            m_MotionVectorRenderStateBlock = new RenderStateBlock(RenderStateMask.Blend);
            var blendState = new BlendState();
            blendState.blendState0 = new RenderTargetBlendState(ColorWriteMask.Red | ColorWriteMask.Green);
            m_MotionVectorRenderStateBlock.blendState = blendState;
            _gBufferShaderTagValues = new ShaderTagId[] { s_ShaderTagLit, s_ShaderTagUnlit, new ShaderTagId() };
            _gBufferRenderStateBlocks = new RenderStateBlock[]
            {
                new RenderStateBlock(RenderStateMask.Nothing),
                new RenderStateBlock(RenderStateMask.Nothing),
                new RenderStateBlock(RenderStateMask.Nothing),
            };
            _actorGBuffer = new VLActorGBuffer(opaqueLayerMask);
            _virtualEffectGBuffer = new VLVirtualEffectGBuffer(opaqueLayerMask);
            _forwardOnlyPass = forwardOnlyPass;
            _skyPass = skyPass;
            _fullScreenTransparentPass = new DrawObjectsPass("FullScreen Transparent",
                new ShaderTagId[] { new ShaderTagId("SRPDefaultUnlit"), new ShaderTagId("UniversalForward"), new ShaderTagId("UniversalForwardOnly") },
                false, RenderPassEvent.BeforeRenderingTransparents, VLRenderQueue.TransparentRange, transparentLayerMask, new StencilState(false), 0);
            _fogMaterial = fogMaterial;
            _motionVectorObjectMaterial = motionVectorObjectMaterial;
            _deferredLights = deferredLights;
            useNativeRenderPass = false;
            renderPassEvent = evt;
            _deferredRenderPass = new CustomRenderPassData
            {
                attachmentDescriptors = ColorAttachments.Take(GBufferFormats.Length).Append(DepthAttachment).ToArray(),
                depthAttachmentIndex = GBufferFormats.Length,
                memoryless = new bool[] { true, true, false, false, false, true },
            };
            _deferredSubPasses = new CustomSubPassData[]
            {
                new CustomSubPassData
                {
                    colors = new int[] { 0, 1, 2, 3, 4 },
                    renderAction = RenderGeometry,
                    profilingSampler = new ProfilingSampler("Geometry"),
                },
                new CustomSubPassData
                {
                    colors = new int[] { 4 },
                    inputs = new int[] { 0, 1, 2, 3 },
                    renderAction = RenderPreEmission,
                    profilingSampler = new ProfilingSampler("Pre Emission"),
                },
                new CustomSubPassData
                {
                    colors = new int[] { 2, 3 },
                    inputs = new int[] { 4 },
                    renderAction = RenderAdditionalInfo,
                    profilingSampler = new ProfilingSampler("Actor Geometry"),
                },
                new CustomSubPassData
                {
                    colors = new int[] { 3 },
                    inputs = new int[] { 2 },
                    renderAction = RenderEmission,
                    profilingSampler = new ProfilingSampler("Emission"),
                },
            };
            _deferredSubPassesNoLit = new CustomSubPassData[]
            {
                new CustomSubPassData
                {
                    colors = new int[] { 0, 1, 2, 4, 3 },
                    renderAction = RenderGeometry,
                    profilingSampler = new ProfilingSampler("Geometry"),
                },
                new CustomSubPassData
                {
                    colors = new int[] { 3 },
                    inputs = new int[] { 0, 1, 2, 4 },
                    renderAction = RenderPreEmission,
                    profilingSampler = new ProfilingSampler("Pre Emission"),
                },
                new CustomSubPassData
                {
                    colors = new int[] { 2 },
                    renderAction = RenderAdditionalInfo,
                    profilingSampler = new ProfilingSampler("Actor Geometry"),
                },
                new CustomSubPassData
                {
                    colors = new int[] { 3 },
                    inputs = new int[] { 2 },
                    renderAction = RenderEmission,
                    profilingSampler = new ProfilingSampler("Emission"),
                },
            };
        }

        public void Setup(RTHandle cameraDepthTexture, RTHandle cameraNormalsTexture, RTHandle additionalInfoTexture)
        {
            _cameraNormalsTexture = cameraNormalsTexture;
            _cameraDepthTexture = cameraDepthTexture;
            _additionalInfoTexture = additionalInfoTexture;
        }

        private void SetupMotionVector(CommandBuffer cmd, ref CameraData cameraData)
        {
            var motionData = RenderingUtility.GetMotionVectorData(ref cameraData);
            if (motionData == null)
                return;
            var passID = motionData.GetXRMultiPassId(ref cameraData);
            if (cameraData.xr.enabled && cameraData.xr.singlePassEnabled)
            {
                cmd.SetGlobalMatrixArray("_PrevViewProjMatrixStereo", motionData.previousViewProjectionStereo);
                cmd.SetGlobalMatrixArray("_NonJitteredViewProjMatrixStereo", motionData.viewProjectionStereo);
            }
            else
            {
                cmd.SetGlobalMatrix("_PrevViewProjMatrix", motionData.previousViewProjectionStereo[passID]);
                cmd.SetGlobalMatrix("_NonJitteredViewProjMatrix", motionData.viewProjectionStereo[passID]);
            }
            cmd.SetGlobalTexture("_MotionVectorTexture", _additionalInfoTexture.nameID);
            cameraData.camera.depthTextureMode |= DepthTextureMode.MotionVectors | DepthTextureMode.Depth;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, _deferredProfilingSampler))
            {
                var cameraTextureDescriptor = renderingData.cameraData.cameraTargetDescriptor;
                var renderer = renderingData.cameraData.renderer;
                Color clearColor = renderingData.cameraData.camera.clearFlags == CameraClearFlags.SolidColor
                    ? CoreUtils.ConvertSRGBToActiveColorSpace(renderingData.cameraData.camera.backgroundColor)
                    : Color.clear;
                RenderTargetIdentifier litColorTarget = BuiltinRenderTextureType.None;
                int cameraHash = renderingData.cameraData.camera.GetHashCode();

                var debugHandler = GetActiveDebugHandler(ref renderingData);
                if (debugHandler != null && debugHandler.DebugDisplaySettings.materialSettings.materialDebugMode == DebugMaterialMode.LightingComplexity)
                    _deferredLights.storeLitColor = false;

                if (renderer is VLSRPRenderer vlRenderer && _deferredLights.storeLitColor)
                {
                    litColorTarget = vlRenderer.GetLitColorTexture(cameraHash).nameID;
                    vlRenderer.MarkLitColorTextureAsRendered(cameraHash);
                }

                AttachmentDescriptor attachment;
                attachment = new AttachmentDescriptor(GBufferFormats[2]);
                attachment.loadStoreTarget = _additionalInfoTexture.nameID;
                attachment.loadAction = RenderBufferLoadAction.Clear;
                attachment.storeAction = RenderBufferStoreAction.Store;
                attachment.clearColor = Color.clear;
                _deferredRenderPass.attachmentDescriptors[2] = attachment;

                attachment = new AttachmentDescriptor(GBufferFormats[4]);
                RenderTargetIdentifier litTarget = _deferredLights.storeLitColor ? litColorTarget : BuiltinRenderTextureType.None;
                attachment.loadStoreTarget = litTarget;
                attachment.loadAction = RenderBufferLoadAction.Clear;
                attachment.storeAction = _deferredLights.storeLitColor ? RenderBufferStoreAction.Store : RenderBufferStoreAction.DontCare;
                attachment.clearColor = clearColor;
                _deferredRenderPass.attachmentDescriptors[4] = attachment;
                _deferredRenderPass.memoryless[4] = !_deferredLights.storeLitColor;

                attachment = new AttachmentDescriptor(cameraTextureDescriptor.graphicsFormat);
                attachment.loadStoreTarget = renderer.cameraColorTargetHandle.nameID;
                attachment.loadAction = RenderBufferLoadAction.Clear;
                attachment.storeAction = RenderBufferStoreAction.Store;
                attachment.clearColor = clearColor;
                _deferredRenderPass.attachmentDescriptors[3] = attachment;

                cmd.SetGlobalTexture(_CameraDepthTexture, _cameraDepthTexture.nameID);
                if (_cameraNormalsTexture != null && _cameraNormalsTexture.rt != null)
                    cmd.SetGlobalTexture(_CameraNormalsTexture, _cameraNormalsTexture.nameID);
                cmd.SetGlobalTexture(_additionalInfoTexture.name, _additionalInfoTexture.nameID);
                SetupMotionVector(cmd, ref renderingData.cameraData);
                CoreUtils.SetKeyword(cmd, "_USE_LIT_BUFFER", _deferredLights.storeLitColor);
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                CustomRenderPassUtility.Execute(context, ref renderingData, _deferredRenderPass, _deferredLights.storeLitColor ? _deferredSubPasses : _deferredSubPassesNoLit);
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public void ExecuteGBuffer(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, _drawGBufferProfilingSampler))
            {
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                if (_deferredLights.IsOverlay)
                {
                    _deferredLights.ClearStencilPartial(cmd);
                    context.ExecuteCommandBuffer(cmd);
                    cmd.Clear();
                }

                var drawingSettings = CreateDrawingSettings(s_ShaderTagUniversalGBuffer, ref renderingData, renderingData.cameraData.defaultOpaqueSortFlags);
                ShaderTagId universalMaterialTypeTag = s_ShaderTagUniversalMaterialType;
                var tagValues = new NativeArray<ShaderTagId>(_gBufferShaderTagValues, Allocator.Temp);
                var stateBlocks = new NativeArray<RenderStateBlock>(_gBufferRenderStateBlocks, Allocator.Temp);
                var debugHandler = GetActiveDebugHandler(ref renderingData);
                if (debugHandler != null && debugHandler.DebugDisplaySettings.renderingSettings.sceneOverrideMode == DebugSceneOverrideMode.Overdraw)
                {
                    foreach (DebugRenderSetup debugRenderSetup in debugHandler.CreateDebugRenderSetupEnumerable(context, cmd, m_FilteringSettings))
                    {
                        for (int i = 0; i < stateBlocks.Length; i++)
                            stateBlocks[i] = debugRenderSetup.GetRenderStateBlock(stateBlocks[i]);
                        context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref m_FilteringSettings, universalMaterialTypeTag, false, tagValues, stateBlocks);
                    }
                }
                else
                {
                    context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref m_FilteringSettings, universalMaterialTypeTag, false, tagValues, stateBlocks);
                }
                tagValues.Dispose();
                stateBlocks.Dispose();
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        private void RenderGeometry(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            ExecuteGBuffer(context, ref renderingData);
            RenderingBridge.Execute(RenderingBridge.EventType.GBuffer, context, ref renderingData);
            if (!_deferredLights.storeLitColor)
            {
                RenderActor(context, ref renderingData);
                RenderVirtualActor(context, ref renderingData);
            }
            RenderingBridge.Execute(RenderingBridge.EventType.AfterGBuffer, context, ref renderingData);
        }

        private void RenderActor(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (DebugDisplaySettings<VLDebugDisplaySettings>.Instance.data.disableActor)
                return;
            _actorGBuffer.ExecuteBase(context, ref renderingData, true);
            _actorGBuffer.ExecuteHair(context, ref renderingData, true);
        }

        private void RenderVirtualActor(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (DebugDisplaySettings<VLDebugDisplaySettings>.Instance.data.disableActor)
                return;
            if (!VolumeManager.instance.stack.GetComponent<VLVirtualEffect>().isEnabled.value)
                return;
            _virtualEffectGBuffer.ExecutePreDepth(context, ref renderingData);
            _virtualEffectGBuffer.ExecuteVirtual(context, ref renderingData, true);
            _virtualEffectGBuffer.ExecuteVirtualHair(context, ref renderingData, true);
        }

        private void RenderAdditionalInfo(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (_deferredLights.storeLitColor)
                RenderActor(context, ref renderingData);
            _deferredLights.ExecuteAdditionalInfo(context, _deferredLights.storeLitColor);
            if (_deferredLights.storeLitColor)
                RenderVirtualActor(context, ref renderingData);
            var debugHandler = GetActiveDebugHandler(ref renderingData);
            if (debugHandler != null && debugHandler.DebugDisplaySettings.materialSettings.materialDebugMode == DebugMaterialMode.LightingComplexity)
            {
                _deferredLights.ExecuteLightingComplexityPass(context, ref renderingData);
                return;
            }
            DrawObjectMotionVectors(context, ref renderingData, _motionVectorObjectMaterial);
        }

        private void DrawObjectMotionVectors(ScriptableRenderContext context, ref RenderingData renderingData, Material objectMaterial)
        {
            var sortingSettings = new SortingSettings(renderingData.cameraData.camera) { criteria = renderingData.cameraData.defaultOpaqueSortFlags };
            var drawingSettings = new DrawingSettings(s_ShaderTagMotionVectors, sortingSettings)
            {
                perObjectData = PerObjectData.MotionVectors,
                enableDynamicBatching = false,
                enableInstancing = true,
            };
            drawingSettings.fallbackMaterial = objectMaterial;
            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref m_MotionVectorFilteringSettingsBefore, ref m_MotionVectorRenderStateBlock);
            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref m_MotionVectorFilteringSettingsAfter, ref m_MotionVectorRenderStateBlock);
        }

        private void RenderPreEmission(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var vlDebugSettings = DebugDisplaySettings<VLDebugDisplaySettings>.Instance;
            var debugHandler = GetActiveDebugHandler(ref renderingData);
            _forwardOnlyPass.Execute(context, ref renderingData);
            if (debugHandler != null && debugHandler.DebugDisplaySettings.renderingSettings.sceneOverrideMode == DebugSceneOverrideMode.Overdraw)
                return;
            _deferredLights.ExecutePlanarReflection(context, ref renderingData);
            _deferredLights.ExecuteIndirectReflection(context, ref renderingData);
            if (!vlDebugSettings.data.disableDeferredLight)
                _deferredLights.ExecuteDeferredPass(context, ref renderingData);
            if (debugHandler != null)
            {
                if (debugHandler.AreAnySettingsActive)
                {
                    var cmd = CommandBufferPool.Get();
                    _deferredLights.ExecutePreEmissionDebugPass(cmd, ref renderingData);
                    context.ExecuteCommandBuffer(cmd);
                    CommandBufferPool.Release(cmd);
                }
                RenderingBridge.Execute(RenderingBridge.EventType.LitLighting, context, ref renderingData);
                if (debugHandler.DebugDisplaySettings.renderingSettings.overdrawMode != DebugOverdrawMode.None)
                    return;
            }
            else
            {
                RenderingBridge.Execute(RenderingBridge.EventType.LitLighting, context, ref renderingData);
            }
            _skyPass.Execute(context, ref renderingData);
        }

        private void RenderEmission(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var debugHandler = GetActiveDebugHandler(ref renderingData);
            RenderingBridge.Execute(RenderingBridge.EventType.ActorDecal, context, ref renderingData);
            var cmd = CommandBufferPool.Get();
            if (debugHandler != null && debugHandler.AreAnySettingsActive)
                _deferredLights.ExecuteEmissionDebugPass(cmd, ref renderingData);
            else
                RenderFog(cmd);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
            if (!DebugDisplaySettings<VLDebugDisplaySettings>.Instance.data.disableActor)
                _actorGBuffer.ExecuteTransparent(context, ref renderingData);
            _fullScreenTransparentPass.Execute(context, ref renderingData);
        }

        private void RenderFog(CommandBuffer cmd)
        {
            if (DebugDisplaySettings<VLDebugDisplaySettings>.Instance.data.disableDeferredFog)
                return;
            var stack = VolumeManager.instance.stack;
            FogUtility.UpdateProperties(cmd, stack.GetComponent<DepthFog>(), stack.GetComponent<SphereFog>());
            using (new ProfilingScope(cmd, _deferredFogProfilingSampler))
            {
                CoreUtils.DrawFullScreen(cmd, _fogMaterial);
            }
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            _deferredLights.OnCameraCleanup(cmd);
        }
    }
}
