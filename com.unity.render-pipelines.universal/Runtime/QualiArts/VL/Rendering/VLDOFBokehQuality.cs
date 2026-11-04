using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace VL.Rendering
{

[Serializable]
[VolumeComponentMenu("VL/Internal/(使用不可)Bokeh Quality")]
public class VLDOFBokehQuality : VolumeComponent
{
	public DofBokehQualitySettingParameter bokehQuality = new DofBokehQualitySettingParameter((DofBokehQualitySetting)1, false);

	public VLDOFBokehQuality()
	{
	}
}

}
