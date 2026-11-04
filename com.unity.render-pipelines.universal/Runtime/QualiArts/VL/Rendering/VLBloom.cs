using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace VL.Rendering
{

[Serializable]
[VolumeComponentMenu("VL/Bloom")]
public class VLBloom : VolumeComponent, IPostProcessComponent
{
	[Tooltip("Bloom強度")]
	public FloatParameter intensity = new MinFloatParameter(0.0f, 0.0f, false);

	[Tooltip("しきい値")]
	public FloatParameter threshold = new MinFloatParameter(1.0f, 0.0f, false);

	[Tooltip("範囲")]
	public ClampedIntParameter diffusion = new ClampedIntParameter(10, 0, 10, false);

	[Tooltip("Bloomに乗算する色")]
	public ColorParameter color = new ColorParameter(new Color(1.0f, 1.0f, 1.0f, 1.0f), false);

	public bool IsActive()
	{
		return intensity.value > 0f;
	}

	public bool IsTileCompatible()
	{
		return false;
	}

	public VLBloom()
	{
	}
}

}
