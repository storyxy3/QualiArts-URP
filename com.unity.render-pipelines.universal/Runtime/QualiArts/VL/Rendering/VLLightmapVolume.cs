using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace VL.Rendering
{

[Serializable]
[VolumeComponentMenu("VL/Lightmap")]
public class VLLightmapVolume : VolumeComponent
{
	[Tooltip("ライトマップスケールカラー")]
	public ColorParameter color = new ColorParameter(new Color(1.0f, 1.0f, 1.0f, 1.0f), false);

	[Tooltip("ライトマップ強度")]
	public FloatParameter intensity = new MinFloatParameter(1.0f, 0.0f, false);

	[Tooltip("ReflectionBufferのスケール")]
	public FloatParameter reflectionScale = new ClampedFloatParameter(1.0f, 0.0f, 10.0f, false);

	public VLLightmapVolume()
	{
	}
}

}
