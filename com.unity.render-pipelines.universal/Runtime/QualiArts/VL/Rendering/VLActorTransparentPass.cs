using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
namespace VL.Rendering
{
    public class VLActorTransparentPass : ScriptableRenderPass
    {
        private RenderStateBlock _stateBlock;
        private FilteringSettings _filteringSettings;
        private readonly ProfilingSampler _profilingSampler = new ProfilingSampler("VLActorTransparent");
        private readonly ShaderTagId _prePassTagId = new ShaderTagId("VLActorTransparentZPrePass");
        private readonly List<ShaderTagId> _shaderTagIds = new List<ShaderTagId> { new ShaderTagId("VLActorTransparent"), new ShaderTagId("VLActorCoverZPrePassTransparent"), new ShaderTagId("VLActorCoverTransparent") };
        private readonly ShaderTagId _outlineTagId = new ShaderTagId("VLTransparentOutline");
        public VLActorTransparentPass(LayerMask layerMask)
        {
            _filteringSettings = new FilteringSettings(RenderQueueRange.opaque, layerMask);
            renderPassEvent = (RenderPassEvent)400;
            _stateBlock = new RenderStateBlock(RenderStateMask.Nothing);
        }
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var sortFlags = renderingData.cameraData.defaultOpaqueSortFlags;
            var prePass = CreateDrawingSettings(_prePassTagId, ref renderingData, sortFlags);
            var actor = CreateDrawingSettings(_shaderTagIds, ref renderingData, sortFlags);
            var outline = CreateDrawingSettings(_outlineTagId, ref renderingData, sortFlags);
            var cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, _profilingSampler))
            {
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                var debug = GetActiveDebugHandler(ref renderingData);
                if (debug != null)
                {
                    debug.DrawWithDebugRenderState(context, cmd, ref renderingData, ref actor, ref _filteringSettings, ref _stateBlock,
                        (ScriptableRenderContext ctx, ref RenderingData data, ref DrawingSettings ds, ref FilteringSettings fs, ref RenderStateBlock rsb) =>
                        {
                            ctx.DrawRenderers(data.cullResults, ref ds, ref fs, ref rsb);
                        });
                }
                else
                {
                    context.DrawRenderers(renderingData.cullResults, ref prePass, ref _filteringSettings, ref _stateBlock);
                    context.DrawRenderers(renderingData.cullResults, ref actor, ref _filteringSettings, ref _stateBlock);
                    context.DrawRenderers(renderingData.cullResults, ref outline, ref _filteringSettings, ref _stateBlock);
                }
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}
