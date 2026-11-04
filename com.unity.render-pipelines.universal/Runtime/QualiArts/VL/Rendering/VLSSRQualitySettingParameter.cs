using System;
using UnityEngine.Rendering;

namespace VL.Rendering
{

[Serializable]
public sealed class VLSSRQualitySettingParameter : VolumeParameter<VLSSRQualitySetting>
{
	public VLSSRQualitySettingParameter(VLSSRQualitySetting value, bool overrideState = false) : base(value, overrideState)
	{
	}
}

}
