using System;
using UnityEngine.Rendering;

namespace VL.Rendering
{

[Serializable]
public sealed class IntervalTypeParameter : VolumeParameter<IntervalType>
{
	public IntervalTypeParameter(IntervalType value, bool overrideState = false) : base(value, overrideState)
	{
	}
}

}
