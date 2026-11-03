// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering.Shaders {
using System;
using UnityEngine;
using UnityEngine.Rendering;

[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, "/github/workspace/campus-client/campus-submodule/vl-unity/Rendering/Runtime/ShaderVariablesVLDOFBokeh.cs", needAccessors = false, generateCBuffer = true)]
public struct ShaderVariablesVLDOFBokeh
{


	public Vector4 _SourceSize;


	public Vector4 _CoCParams;


	public Vector4 _BokehConstants;


	public Vector4 _Padding;


	[HLSLArray(42, typeof(Vector4))]
	public unsafe fixed float _BokehKernel[168];

	public Span<Vector4> BokehKernelSpan
	{


		get
		{
			return default(Span<Vector4>);
		}
	}
}

}
