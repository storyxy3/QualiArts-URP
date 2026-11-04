using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace VL.Rendering
{

[Serializable]
[VolumeComponentMenu("VL/Screen Space Reflection")]
public class VLScreenSpaceReflection : VolumeComponent
{
	[Tooltip("Quality")]
	public VLSSRQualitySettingParameter quality = new VLSSRQualitySettingParameter((VLSSRQualitySetting)2, false);

	public VLScreenSpaceReflection()
	{
	}
}

}
