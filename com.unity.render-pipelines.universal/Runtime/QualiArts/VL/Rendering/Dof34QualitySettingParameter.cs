using System;
using UnityEngine.Rendering;

namespace VL.Rendering
{

[Serializable]
public sealed class Dof34QualitySettingParameter : VolumeParameter<Dof34QualitySetting>
{
	public Dof34QualitySettingParameter(Dof34QualitySetting value, bool overrideState = false) : base(value, overrideState)
	{
	}
}

}
