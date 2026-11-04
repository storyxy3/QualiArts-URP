using System;
using UnityEngine.Rendering;

namespace VL.Rendering
{

[Serializable]
public class LightSpaceTypeParameter : VolumeParameter<LightSpaceType>
{
	public LightSpaceTypeParameter(LightSpaceType value, bool overrideState = false) : base(value, overrideState)
	{
	}
}

}
