// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering {
using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using VL.Rendering;

[Serializable]

[VolumeComponentMenu("VL/Noise/WaveJitterGlitch")]
public class VLWaveJitterGlitch : VolumeComponent, IPostProcessComponent
{


	public IntervalTypeParameter intervalType;


	public ClampedFloatParameter frequency;


	public ClampedFloatParameter intensity;


	public ClampedFloatParameter granularity;


	public ClampedFloatParameter waveSpeed;


	public ClampedFloatParameter rgbSplit;


	public bool IsActive()
	{
		return default(bool);
	}


	public bool IsTileCompatible()
	{
		return default(bool);
	}


	public VLWaveJitterGlitch()
	{
	}
}

}
