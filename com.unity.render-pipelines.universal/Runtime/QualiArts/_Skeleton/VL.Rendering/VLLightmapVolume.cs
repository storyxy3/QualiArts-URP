// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering {
using System;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]

[VolumeComponentMenu("VL/Lightmap")]
public class VLLightmapVolume : VolumeComponent
{


	[Tooltip("ライトマップスケールカラー")]
	public ColorParameter color;


	[Tooltip("ライトマップ強度")]
	public FloatParameter intensity;


	[Tooltip("ReflectionBufferのスケール")]
	public FloatParameter reflectionScale;


	public VLLightmapVolume()
	{
	}
}

}
