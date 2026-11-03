// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering {
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable]

[VolumeComponentMenu("VL/Diffusion")]
public class VLDiffusion : VolumeComponent, IPostProcessComponent
{


	[Tooltip("範囲")]
	public ClampedFloatParameter diffusion;


	[Tooltip("コントラスト閾値")]
	public ClampedFloatParameter contrastThreshold;


	[Tooltip("コントラスト強度")]
	public ClampedFloatParameter contrastPower;


	[Tooltip("ブレンド強度")]
	public ClampedFloatParameter blend;


	public bool IsActive()
	{
		return default(bool);
	}


	public bool IsTileCompatible()
	{
		return default(bool);
	}


	public VLDiffusion()
	{
	}
}

}
