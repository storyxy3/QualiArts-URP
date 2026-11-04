using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
namespace VL.Rendering
{
    public class VLVirtualEffectGBuffer
    {
        private readonly ShaderTagId _virtualTagId = new ShaderTagId("UniversalGBufferVirtualEffect");
        private readonly ShaderTagId _virtualHairTagId = new ShaderTagId("UniversalGBufferVirtualHair");
        private readonly ShaderTagId _virtualOutlineTagId = new ShaderTagId("UniversalGBufferVirtualOutline");
        private readonly ShaderTagId _preDepthTagId = new ShaderTagId("UniversalGBufferPreDepth");
        private FilteringSettings _baseFilteringSettings;
        private FilteringSettings _hairFilteringSettings;
        private static RenderStateBlock renderStateActor = new RenderStateBlock(RenderStateMask.Stencil)
        {
            stencilReference = 64,
            stencilState = new StencilState(true, 108, 108, CompareFunction.Always, StencilOp.Replace,
                StencilOp.Keep, StencilOp.Keep, CompareFunction.Always, StencilOp.Replace, StencilOp.Keep, StencilOp.Keep)
        };
        public VLVirtualEffectGBuffer(LayerMask layerMask)
        {
            _baseFilteringSettings = new FilteringSettings(RenderQueueRange.opaque, layerMask, 45056);
            _hairFilteringSettings = new FilteringSettings(RenderQueueRange.opaque, layerMask, 16384);
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
        public void ExecuteVirtual(ScriptableRenderContext context, ref RenderingData renderingData, bool useMotionVector)
        {
            var actor = CreateDrawingSettings(_virtualTagId, ref renderingData, renderingData.cameraData.defaultOpaqueSortFlags, useMotionVector);
            context.DrawRenderers(renderingData.cullResults, ref actor, ref _baseFilteringSettings);
            var outline = CreateDrawingSettings(_virtualOutlineTagId, ref renderingData, SortingCriteria.OptimizeStateChanges, useMotionVector);
            context.DrawRenderers(renderingData.cullResults, ref outline, ref _baseFilteringSettings);
        }
        public void ExecuteVirtualHair(ScriptableRenderContext context, ref RenderingData renderingData, bool useMotionVector)
        {
            var actor = CreateDrawingSettings(_virtualTagId, ref renderingData, renderingData.cameraData.defaultOpaqueSortFlags, useMotionVector);
            context.DrawRenderers(renderingData.cullResults, ref actor, ref _hairFilteringSettings);
            var hair = CreateDrawingSettings(_virtualHairTagId, ref renderingData, SortingCriteria.QuantizedFrontToBack, useMotionVector);
            context.DrawRenderers(renderingData.cullResults, ref hair, ref _hairFilteringSettings);
            var outline = CreateDrawingSettings(_virtualOutlineTagId, ref renderingData, SortingCriteria.OptimizeStateChanges, useMotionVector);
            context.DrawRenderers(renderingData.cullResults, ref outline, ref _hairFilteringSettings, ref renderStateActor);
        }
        public static RenderQueueRange VirtualEffectRangeBefore => new RenderQueueRange(0, 2001);
        public static RenderQueueRange VirtualEffectRangeFaceParts => new RenderQueueRange(2003, 2003);
        public static RenderQueueRange VirtualEffectRangeAfter => new RenderQueueRange(2301, 2500);
        public void ExecutePreDepth(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var settings = CreateDrawingSettings(_preDepthTagId, ref renderingData, renderingData.cameraData.defaultOpaqueSortFlags, false);
            var before = new FilteringSettings(VirtualEffectRangeBefore, renderingData.cameraData.camera.cullingMask);
            var face = new FilteringSettings(VirtualEffectRangeFaceParts, renderingData.cameraData.camera.cullingMask);
            var after = new FilteringSettings(VirtualEffectRangeAfter, renderingData.cameraData.camera.cullingMask);
            context.DrawRenderers(renderingData.cullResults, ref settings, ref before);
            context.DrawRenderers(renderingData.cullResults, ref settings, ref face);
            context.DrawRenderers(renderingData.cullResults, ref settings, ref after);
        }
    }
}
