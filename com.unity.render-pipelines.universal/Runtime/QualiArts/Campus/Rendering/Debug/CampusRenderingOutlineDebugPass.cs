using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
namespace Campus.Rendering.Debug
{
    public class CampusRenderingOutlineDebugPass : ScriptableRenderPass
    {
        private readonly ShaderTagId _outlineTagId = new ShaderTagId("UniversalForwardOutline");
        private FilteringSettings _filteringSettings;
        private RenderStateBlock _stateBlock;
        public CampusRenderingOutlineDebugPass()
        {
            renderPassEvent = RenderPassEvent.BeforeRenderingOpaques + 1;
            _filteringSettings = new FilteringSettings(RenderQueueRange.opaque);
            _stateBlock = new RenderStateBlock(RenderStateMask.Nothing);
        }
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            _ = CampusDebugDisplaySettings.Instance;
            DebugHandler handler = GetActiveDebugHandler(ref renderingData);
            if (handler != null)
            {
                var drawing = CreateDrawingSettings(_outlineTagId, ref renderingData, SortingCriteria.OptimizeStateChanges);
                CommandBuffer cmd = CommandBufferPool.Get();
                handler.DrawWithDebugRenderState(context, cmd, ref renderingData, ref drawing, ref _filteringSettings, ref _stateBlock,
                    (ScriptableRenderContext ctx, ref RenderingData data, ref DrawingSettings ds, ref FilteringSettings fs, ref RenderStateBlock rsb) =>
                        ctx.DrawRenderers(data.cullResults, ref ds, ref fs, ref rsb));
                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }
        }
    }
}
