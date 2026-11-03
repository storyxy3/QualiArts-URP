// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering {
using UnityEngine;
using UnityEngine.Rendering;
using VL.Rendering;

[VolumeComponentMenu("VL/Screen Space Ambient Occlusion")]
public class VLScreenSpaceAmbientOcclusion : VolumeComponent
{


	[Tooltip("強度 3.0が標準")]
	public MinFloatParameter intensity;


	public ClampedFloatParameter directLightingStrength;


	public MinFloatParameter radius;


	public MinFloatParameter falloff;


	public AOQualityOptionParameter blurType;


	public AOQualityOptionParameter samples;


	public bool IsActive()
	{
		return default(bool);
	}


	public VLScreenSpaceAmbientOcclusion()
	{
	}
}

}
