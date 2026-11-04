using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace VL.Rendering.Shaders
{
    [GenerateHLSL(needAccessors = false, generateCBuffer = true)]
    public unsafe struct ShaderVariablesVLDOFBokeh
    {
        public Vector4 _SourceSize;
        public Vector4 _CoCParams;
        public Vector4 _BokehConstants;
        public Vector4 _Padding;
        [HLSLArray(42, typeof(Vector4))]
        public fixed float _BokehKernel[168];

        public Span<Vector4> BokehKernelSpan
        {
            get
            {
                fixed (float* buffer = _BokehKernel)
                    return new Span<Vector4>(buffer, 42);
            }
        }
    }
}
