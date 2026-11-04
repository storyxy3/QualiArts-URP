using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.Universal.Internal;
namespace VL.Rendering
{
    internal class VLDecalPass : ScriptableRenderPass
    {
        private static readonly ProfilingSampler _profilingSampler = new ProfilingSampler("Draw VLDecal");
        private VLDecalDrawSystem _drawSystem;
        private RenderingBridge.EventType _eventType;
        private RenderingBridge.RenderEvent _renderEvent;
        public VLDecalPass(RenderingBridge.EventType eventType)
        {
            profilingSampler = new ProfilingSampler($"Setup VLDecal {eventType}");
            _eventType = eventType;
            _renderEvent = ExecuteDecal;
            renderPassEvent = RenderPassEvent.BeforeRendering;
        }
        public void SetDrawSystem(VLDecalDrawSystem drawSystem) { _drawSystem = drawSystem; }
        public void ExecuteDecal(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, _profilingSampler))
            {
                NormalReconstruction.SetupProperties(cmd, in renderingData.cameraData);
                _drawSystem?.Execute(cmd);
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) => RenderingBridge.AddRenderEvent(_eventType, _renderEvent);
    }
}
