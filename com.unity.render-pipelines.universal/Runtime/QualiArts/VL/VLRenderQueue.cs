using UnityEngine.Rendering;
namespace VL
{
    public static class VLRenderQueue
    {
        public static RenderQueueRange MotionVectorRangeBefore => new RenderQueueRange(0, 2200);
        public static RenderQueueRange MotionVectorRangeAfter => new RenderQueueRange(2301, 2500);
        public static RenderQueueRange TransparentRange => new RenderQueueRange(2701, 3500);
        public static RenderQueueRange DownscaleTransparentRange => new RenderQueueRange(3501, 5000);
    }
}
