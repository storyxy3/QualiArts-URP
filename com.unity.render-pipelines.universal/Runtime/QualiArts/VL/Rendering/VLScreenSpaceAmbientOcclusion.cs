using UnityEngine;
using UnityEngine.Rendering;

namespace VL.Rendering
{

[VolumeComponentMenu("VL/Screen Space Ambient Occlusion")]
public class VLScreenSpaceAmbientOcclusion : VolumeComponent
{
	[Tooltip("強度 3.0が標準")]
	public MinFloatParameter intensity = new MinFloatParameter(0.0f, 0.0f, false);

	public ClampedFloatParameter directLightingStrength = new ClampedFloatParameter(0.25f, 0.0f, 1.0f, false);

	public MinFloatParameter radius = new MinFloatParameter(0.035f, 0.0f, false);

	public MinFloatParameter falloff = new MinFloatParameter(100.0f, 0.1f, false);

	public AOQualityOptionParameter blurType = new AOQualityOptionParameter((AOQualityOption)1, false);

	public AOQualityOptionParameter samples = new AOQualityOptionParameter((AOQualityOption)1, false);

	public bool IsActive()
	{
		return intensity.value > 0f && radius.value > 0f;
	}

	public VLScreenSpaceAmbientOcclusion()
	{
	}
}

}
