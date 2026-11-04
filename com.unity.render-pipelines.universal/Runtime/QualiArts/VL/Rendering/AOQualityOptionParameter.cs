using System;
using UnityEngine.Rendering;

namespace VL.Rendering
{

[Serializable]
public class AOQualityOptionParameter : VolumeParameter<AOQualityOption>
{
	public AOQualityOptionParameter(AOQualityOption value, bool overrideState = false) : base(value, overrideState)
	{
	}
}

}
