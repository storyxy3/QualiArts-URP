using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
namespace VL.Rendering
{
    public class VLSimpleRenderer : ScriptableRenderer
    {
        private class SimpleFinalBlitPass : ScriptableRenderPass
        {
            private Material m_BlitMaterial;
            public SimpleFinalBlitPass(Material blitMaterial)
            {
                renderPassEvent = (RenderPassEvent)1001;
                m_BlitMaterial = blitMaterial;
            }
            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                var target = RenderingUtils.GetCameraTargetIdentifier(ref renderingData);
                var cmd = CommandBufferPool.Get();
                RTHandleStaticHelpers.SetRTHandleStaticWrapper(target);
                var destination = RTHandleStaticHelpers.s_RTHandleWrapper;
                RenderingUtils.FinalBlit(cmd, ref renderingData.cameraData, renderingData.cameraData.renderer.cameraColorTargetHandle,
                    destination, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, m_BlitMaterial, 1);
                renderingData.cameraData.renderer.ConfigureCameraTarget(renderingData.cameraData.renderer.cameraColorTargetHandle,
                    renderingData.cameraData.renderer.cameraColorTargetHandle);
                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }
        }
        private static readonly ProfilingSampler _profilingSampler = new ProfilingSampler("VL Simple Renderer");
        private RTHandle m_CameraColorAttachment;
        private SimpleFinalBlitPass m_FinalBlitPass;
        private CapturePass m_CapturePass;
        private Material m_BlitMaterial;
        private Material m_BlitHDRMaterial;
        private Material m_CopyDepthMaterial;
        public VLSimpleRenderer(VLSimpleRendererData data) : base(data)
        {
            m_BlitMaterial = CoreUtils.CreateEngineMaterial(data.coreBlitPS);
            m_BlitHDRMaterial = CoreUtils.CreateEngineMaterial(data.blitHDROverlay);
            m_CopyDepthMaterial = CoreUtils.CreateEngineMaterial(data.copyDepthPS);
            m_CapturePass = new CapturePass(RenderPassEvent.AfterRendering);
            m_FinalBlitPass = new SimpleFinalBlitPass(m_BlitMaterial);
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            ReleaseRenderTargets();
            CoreUtils.Destroy(m_BlitMaterial);
            CoreUtils.Destroy(m_CopyDepthMaterial);
        }
        internal override void ReleaseRenderTargets()
        {
            m_CameraColorAttachment?.Release();
            hasReleasedRTs = true;
        }
        public override void FinishRendering(CommandBuffer cmd) { }
        public override void Setup(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType != CameraType.Game) return;
            var descriptor = renderingData.cameraData.cameraTargetDescriptor;
            descriptor.useMipMap = false;
            descriptor.autoGenerateMips = false;
            descriptor.depthBufferBits = 0;
            RenderingUtils.ReAllocateIfNeeded(ref m_CameraColorAttachment, in descriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, name: "_CameraColorTexture");
            ConfigureCameraTarget(m_CameraColorAttachment, m_CameraColorAttachment);
            renderingData.cameraData.renderer.useDepthPriming = false;
            if (renderingData.cameraData.resolveFinalTarget)
            {
                if (renderingData.cameraData.captureActions != null) EnqueuePass(m_CapturePass);
                EnqueuePass(m_FinalBlitPass);
            }
        }
        public override void SetupCullingParameters(ref ScriptableCullingParameters cullingParameters, ref CameraData cameraData)
        {
            cullingParameters.cullingOptions = (CullingOptions)32;
            cullingParameters.isOrthographic = cameraData.camera.orthographic;
            cullingParameters.shadowDistance = 0;
        }
        internal override void SwapColorBuffer(CommandBuffer cmd) { throw new InvalidOperationException(); }
    }
}
