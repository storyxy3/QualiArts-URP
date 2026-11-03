// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering {
using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable]

[VolumeComponentMenu("VL/Noise/RGBBlockNoise")]
public class VLRGBBlockNoise : VolumeComponent, IPostProcessComponent
{


	public ClampedFloatParameter stepFrequency;


	public ClampedFloatParameter rgbSplit;


	public ClampedFloatParameter saturation;


	public ClampedFloatParameter attenuation;


	public FloatParameter tileOffsetX;


	public FloatParameter tileOffsetY;


	public bool IsActive()
	{
		return default(bool);
	}


	public bool IsTileCompatible()
	{
		return default(bool);
	}


	public VLRGBBlockNoise()
	{
	}
}

}
