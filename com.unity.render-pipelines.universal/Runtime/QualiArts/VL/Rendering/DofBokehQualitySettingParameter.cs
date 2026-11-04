using System;
using UnityEngine.Rendering;

namespace VL.Rendering
{

[Serializable]
public sealed class DofBokehQualitySettingParameter : VolumeParameter<DofBokehQualitySetting>
{
	public DofBokehQualitySettingParameter(DofBokehQualitySetting value, bool overrideState = false) : base(value, overrideState)
	{
	}
}

}
