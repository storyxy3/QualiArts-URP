using UnityEngine.Rendering;
namespace VL.Rendering
{
    public static partial class RenderingUtility
    {
        public static class ShaderTagIds
        {
            public static ShaderTagId UniversalForwardPerformance = new ShaderTagId("UniversalForwardPerformance");
            public static ShaderTagId DepthOnly = new ShaderTagId("DepthOnly");
        }
    }
}
