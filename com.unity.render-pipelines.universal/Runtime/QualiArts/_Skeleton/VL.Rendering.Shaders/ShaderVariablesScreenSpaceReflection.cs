// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering.Shaders {
using System;
using UnityEngine;
using UnityEngine.Rendering;

[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, "/github/workspace/campus-client/campus-submodule/vl-unity/Rendering/Runtime/ShaderVariablesScreenSpaceReflection.cs", needAccessors = false, generateCBuffer = true)]
public struct ShaderVariablesScreenSpaceReflection
{


	public Matrix4x4 _SSRPrevVPMatrix;


	[HLSLArray(14, typeof(Vector4))]
	public unsafe fixed float _MipMinMax[56];


	public Vector4 _Params;


	public Vector4 _Padding;

	public Span<Vector4> MipMinMaxSpan
	{


		get
		{
			return default(Span<Vector4>);
		}
	}
}

}
