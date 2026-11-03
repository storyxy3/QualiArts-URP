// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering {
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;
using VL.Rendering;

public class FogUtility
{
	[StructLayout((LayoutKind)3)]

	[CompilerGenerated]
	private struct _003C_003Ec__DisplayClass7_0
	{


		public bool isLinear;
	}


	public static readonly int _FogParams;


	public static readonly int _FogColor;


	public static readonly int _FogSphere;


	public static readonly int _FogSphereColor;


	private static readonly ProfilingSampler deferredFogProfilingSampler;


	private static readonly Dictionary<DeferredFogPasses, string> _passToPassName;


	private static readonly Dictionary<DeferredFogPasses, int> _passNameToIndex;


	public static void UpdateProperties(CommandBuffer cmd, DepthFog depthFog, SphereFog sphereFog, [Optional] VLDensityFogVolume legacyFog)
	{
	}


	[CompilerGenerated]
	private static Vector4 _003CUpdateProperties_003Eg__GetFogColor_007C7_0(Color color, float maxAmount, ref _003C_003Ec__DisplayClass7_0 P_2)
	{
		return default(Vector4);
	}
}

}
