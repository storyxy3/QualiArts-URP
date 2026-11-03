// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering {
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.Rendering;
using VL.Rendering;

public class FogUtility
{


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
}

}
