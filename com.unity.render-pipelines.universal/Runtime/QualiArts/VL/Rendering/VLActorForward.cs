using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
namespace VL.Rendering
{
    public class VLActorForward : ScriptableRenderPass
    {
        private RenderStateBlock _stateBlock;
        private FilteringSettings _filteringSettings;
        private readonly ProfilingSampler _profilingSampler = new ProfilingSampler("Actor Forward");
        private readonly List<ShaderTagId> _shaderTagIds = new List<ShaderTagId> { new ShaderTagId("VLActor"), new ShaderTagId("VLActorCoverZPrePass"), new ShaderTagId("VLActorCover") };
        private readonly List<ShaderTagId> _shaderTagIdsCapture = new List<ShaderTagId> { new ShaderTagId("VLActor"), new ShaderTagId("VLActorCoverZPrePass"), new ShaderTagId("VLActorCover"), new ShaderTagId("VLActorCoverAlphaFillPass") };
        private readonly ShaderTagId _outlineTagId = new ShaderTagId("VLOutline");
        public VLActorForward(LayerMask layerMask)
        {
            _filteringSettings = new FilteringSettings(RenderQueueRange.opaque, layerMask);
            renderPassEvent = (RenderPassEvent)250;
            _stateBlock = new RenderStateBlock(RenderStateMask.Nothing);
        }
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var sortFlags = renderingData.cameraData.defaultOpaqueSortFlags;
            var tags = GraphicsFormatUtility.HasAlphaChannel(renderingData.cameraData.cameraTargetDescriptor.graphicsFormat) ? _shaderTagIdsCapture : _shaderTagIds;
            var actor = CreateDrawingSettings(tags, ref renderingData, sortFlags);
            var outline = CreateDrawingSettings(_outlineTagId, ref renderingData, SortingCriteria.OptimizeStateChanges);
            var cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, _profilingSampler))
            {
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                var filterSettings = _filteringSettings;
                var debug = GetActiveDebugHandler(ref renderingData);
                if (debug != null)
                {
                    debug.DrawWithDebugRenderState(context, cmd, ref renderingData, ref actor, ref filterSettings, ref _stateBlock,
                        (ScriptableRenderContext ctx, ref RenderingData data, ref DrawingSettings ds, ref FilteringSettings fs, ref RenderStateBlock rsb) =>
                        {
                            ctx.DrawRenderers(data.cullResults, ref ds, ref fs, ref rsb);
                        });
                }
                else
                {
                    context.DrawRenderers(renderingData.cullResults, ref actor, ref filterSettings, ref _stateBlock);
                    context.DrawRenderers(renderingData.cullResults, ref outline, ref filterSettings, ref _stateBlock);
                }
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}
