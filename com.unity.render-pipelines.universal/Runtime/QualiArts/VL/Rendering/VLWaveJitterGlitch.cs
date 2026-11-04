using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace VL.Rendering
{

[Serializable]
[VolumeComponentMenu("VL/Noise/WaveJitterGlitch")]
public class VLWaveJitterGlitch : VolumeComponent, IPostProcessComponent
{
	public IntervalTypeParameter intervalType = new IntervalTypeParameter((IntervalType)1, false);

	public ClampedFloatParameter frequency = new ClampedFloatParameter(1.0f, 0.0f, 25.0f, false);

	public ClampedFloatParameter intensity = new ClampedFloatParameter(0.0f, 0.0f, 2.0f, false);

	public ClampedFloatParameter granularity = new ClampedFloatParameter(0.01f, 0.001f, 0.1f, false);

	public ClampedFloatParameter waveSpeed = new ClampedFloatParameter(0.25f, 0.0f, 1.0f, false);

	public ClampedFloatParameter rgbSplit = new ClampedFloatParameter(20.0f, 0.0f, 50.0f, false);

	public bool IsActive()
	{
		return intensity.value > 0f;
	}

	public bool IsTileCompatible()
	{
		return true;
	}

	public VLWaveJitterGlitch()
	{
	}
}

}
