using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace VL.Rendering.Shaders
{
    [GenerateHLSL(needAccessors = false, generateCBuffer = true)]
    public unsafe struct ShaderVariablesScreenSpaceReflection
    {
        public Matrix4x4 _SSRPrevVPMatrix;
        [HLSLArray(14, typeof(Vector4))]
        public fixed float _MipMinMax[56];
        public Vector4 _Params;
        public Vector4 _Padding;

        public Span<Vector4> MipMinMaxSpan
        {
            get
            {
                fixed (float* buffer = _MipMinMax)
                    return new Span<Vector4>(buffer, 14);
            }
        }
    }
}
