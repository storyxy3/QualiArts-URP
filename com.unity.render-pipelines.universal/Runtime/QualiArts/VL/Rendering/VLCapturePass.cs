using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
namespace VL.Rendering
{
    public class VLCapturePass : ScriptableRenderPass
    {
        private readonly RenderTargetIdentifier _target;
        private Action _onCapture;
        public VLCapturePass(VLSRPCapture.RequestData requestData)
        {
            _target = requestData.target;
            _onCapture = requestData.onCapture;
            renderPassEvent = requestData.renderPassEvent;
        }
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get();
            cmd.Blit(BuiltinRenderTextureType.CurrentActive, _target);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
            _onCapture?.Invoke();
        }
    }
}
