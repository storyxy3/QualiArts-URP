using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
namespace VL.Rendering
{
    public class VLActorGBuffer
    {
        private readonly ShaderTagId _actorTagId = new ShaderTagId("UniversalGBufferActor");
        private readonly ShaderTagId _hairTagId = new ShaderTagId("UniversalGBufferActorHair");
        private readonly ShaderTagId _outlineTagId = new ShaderTagId("UniversalGBufferOutline");
        private readonly ShaderTagId _forwardTagId = new ShaderTagId("VLActor");
        private FilteringSettings _baseFilteringSettings;
        private FilteringSettings _hairFilteringSettings;
        private FilteringSettings _allFilteringSettings;
        private FilteringSettings _transparentFilteringSettings;
        private static RenderStateBlock renderStateActor = new RenderStateBlock(RenderStateMask.Stencil)
        {
            stencilReference = 64,
            stencilState = new StencilState(true, 108, 108, CompareFunction.Always, StencilOp.Replace,
                StencilOp.Keep, StencilOp.Keep, CompareFunction.Always, StencilOp.Replace, StencilOp.Keep, StencilOp.Keep)
        };
        public VLActorGBuffer(LayerMask layerMask)
        {
            _baseFilteringSettings = new FilteringSettings(RenderQueueRange.opaque, layerMask, 45056);
            _hairFilteringSettings = new FilteringSettings(RenderQueueRange.opaque, layerMask, 16384);
            _allFilteringSettings = new FilteringSettings(RenderQueueRange.opaque, layerMask, 61440);
            _transparentFilteringSettings = new FilteringSettings(RenderQueueRange.transparent, layerMask, 61440);
        }
        private DrawingSettings CreateDrawingSettings(ShaderTagId shaderTagId, ref RenderingData renderingData, SortingCriteria sortingCriteria, bool useMotionVector)
        {
            var sortingSettings = new SortingSettings(renderingData.cameraData.camera) { criteria = sortingCriteria };
            return new DrawingSettings(shaderTagId, sortingSettings)
            {
                perObjectData = renderingData.perObjectData | (useMotionVector ? PerObjectData.MotionVectors : PerObjectData.None),
                mainLightIndex = renderingData.lightData.mainLightIndex,
                enableDynamicBatching = false,
                enableInstancing = false
            };
        }
        public void ExecuteBase(ScriptableRenderContext context, ref RenderingData renderingData, bool useMotionVector)
        {
            var actor = CreateDrawingSettings(_actorTagId, ref renderingData, renderingData.cameraData.defaultOpaqueSortFlags, useMotionVector);
            context.DrawRenderers(renderingData.cullResults, ref actor, ref _baseFilteringSettings);
            var outline = CreateDrawingSettings(_outlineTagId, ref renderingData, SortingCriteria.OptimizeStateChanges, useMotionVector);
            context.DrawRenderers(renderingData.cullResults, ref outline, ref _baseFilteringSettings);
        }
        public void ExecuteHair(ScriptableRenderContext context, ref RenderingData renderingData, bool useMotionVector)
        {
            var actor = CreateDrawingSettings(_actorTagId, ref renderingData, renderingData.cameraData.defaultOpaqueSortFlags, useMotionVector);
            context.DrawRenderers(renderingData.cullResults, ref actor, ref _hairFilteringSettings);
            var hair = CreateDrawingSettings(_hairTagId, ref renderingData, SortingCriteria.QuantizedFrontToBack, useMotionVector);
            context.DrawRenderers(renderingData.cullResults, ref hair, ref _hairFilteringSettings);
            var outline = CreateDrawingSettings(_outlineTagId, ref renderingData, SortingCriteria.OptimizeStateChanges, useMotionVector);
            context.DrawRenderers(renderingData.cullResults, ref outline, ref _hairFilteringSettings, ref renderStateActor);
        }
        public void ExecuteTransparent(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var settings = CreateDrawingSettings(_forwardTagId, ref renderingData, SortingCriteria.CommonTransparent, false);
            context.DrawRenderers(renderingData.cullResults, ref settings, ref _transparentFilteringSettings);
        }
    }
}
