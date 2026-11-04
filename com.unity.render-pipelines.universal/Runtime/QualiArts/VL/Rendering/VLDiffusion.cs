using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace VL.Rendering
{

[Serializable]
[VolumeComponentMenu("VL/Diffusion")]
public class VLDiffusion : VolumeComponent, IPostProcessComponent
{
	[Tooltip("範囲")]
	public ClampedFloatParameter diffusion = new ClampedFloatParameter(1.0f, 0.0f, 1.0f, false);

	[Tooltip("コントラスト閾値")]
	public ClampedFloatParameter contrastThreshold = new ClampedFloatParameter(0.5f, 0.0f, 1.0f, false);

	[Tooltip("コントラスト強度")]
	public ClampedFloatParameter contrastPower = new ClampedFloatParameter(0.5f, 0.0f, 1.0f, false);

	[Tooltip("ブレンド強度")]
	public ClampedFloatParameter blend = new ClampedFloatParameter(0.0f, 0.0f, 1.0f, false);

	public bool IsActive()
	{
		return diffusion.value > 0f && blend.value > 0f;
	}

	public bool IsTileCompatible()
	{
		return false;
	}

	public VLDiffusion()
	{
	}
}

}
