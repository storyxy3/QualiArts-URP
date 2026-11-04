using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace VL.Rendering
{

[Serializable]
[VolumeComponentMenu("VL/Noise/RGBBlockNoise")]
public class VLRGBBlockNoise : VolumeComponent, IPostProcessComponent
{
	public ClampedFloatParameter stepFrequency = new ClampedFloatParameter(1.0f, 0.0f, 5.0f, false);

	public ClampedFloatParameter rgbSplit = new ClampedFloatParameter(0.0f, 0.0f, 50.0f, false);

	public ClampedFloatParameter saturation = new ClampedFloatParameter(2.0f, 0.0f, 5.0f, false);

	public ClampedFloatParameter attenuation = new ClampedFloatParameter(1.0f, 0.0f, 1.0f, false);

	public FloatParameter tileOffsetX = new FloatParameter(1.0f, false);

	public FloatParameter tileOffsetY = new FloatParameter(1.0f, false);

	public bool IsActive()
	{
		return rgbSplit.value > 0f;
	}

	public bool IsTileCompatible()
	{
		return true;
	}

	public VLRGBBlockNoise()
	{
	}
}

}
