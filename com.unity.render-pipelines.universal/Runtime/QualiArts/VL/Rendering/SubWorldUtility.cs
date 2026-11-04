using UnityEngine;
using UnityEngine.Rendering;
namespace VL.Rendering
{
    public static class SubWorldUtility
    {
        public static ShaderTagId SubWorldMask = new ShaderTagId("UniversalForward");
        public static readonly int _MirrorPlane = Shader.PropertyToID("_MirrorPlane");
        public static readonly int _MirrorWorldSpaceCameraPos = Shader.PropertyToID("_MirrorWorldSpaceCameraPos");
        public static readonly int _MirrorReflectionMatrix = Shader.PropertyToID("_MirrorReflectionMatrix");
        public static readonly int mirror_MatrixV = Shader.PropertyToID("mirror_MatrixV");
        public static readonly int glstate_matrix_mirror_projection = Shader.PropertyToID("glstate_matrix_mirror_projection");
        public static void UpdateReflectionStencilState(ref StencilState stencilState)
        {
            stencilState.enabled = true;
            stencilState.readMask |= 128;
            if (stencilState.compareFunctionBack == CompareFunction.Always)
                stencilState.compareFunctionBack = CompareFunction.Equal;
            else
                stencilState.compareFunctionBack = CompareFunction.LessEqual;
            if (stencilState.compareFunctionFront == CompareFunction.Always)
                stencilState.compareFunctionFront = CompareFunction.Equal;
            else
                stencilState.compareFunctionFront = CompareFunction.LessEqual;
            stencilState.SetPassOperation(StencilOp.Keep);
        }
    }
}
