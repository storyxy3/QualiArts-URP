using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
namespace Campus.Rendering.Debug
{
    public class CampusRenderingDebugPass : ScriptableRenderPass
    {
        private static readonly int _ActorDebugMaterialMode = Shader.PropertyToID("_ActorDebugMaterialMode");
        public CampusRenderingDebugPass() { renderPassEvent = RenderPassEvent.BeforeRendering; }
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var settings = CampusDebugDisplaySettings.Instance;
            CommandBuffer cmd = CommandBufferPool.Get();
            cmd.SetGlobalInteger(_ActorDebugMaterialMode, (int)settings.commonData.actorDebugMaterialMode);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}
