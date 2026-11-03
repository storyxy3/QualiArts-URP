// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering {
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable]

[VolumeComponentMenu("VL/Bloom")]
public class VLBloom : VolumeComponent, IPostProcessComponent
{


	[Tooltip("Bloom強度")]
	public FloatParameter intensity;


	[Tooltip("しきい値")]
	public FloatParameter threshold;


	[Tooltip("範囲")]
	public ClampedIntParameter diffusion;


	[Tooltip("Bloomに乗算する色")]
	public ColorParameter color;


	public bool IsActive()
	{
		return default(bool);
	}


	public bool IsTileCompatible()
	{
		return default(bool);
	}


	public VLBloom()
	{
	}
}

}
