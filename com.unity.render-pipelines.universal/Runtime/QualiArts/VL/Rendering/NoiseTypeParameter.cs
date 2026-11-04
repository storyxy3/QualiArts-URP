using System;
using UnityEngine.Rendering;

namespace VL.Rendering
{

[Serializable]
public sealed class NoiseTypeParameter : VolumeParameter<NoiseType>
{
	public NoiseTypeParameter(NoiseType value, bool overrideState = false) : base(value, overrideState)
	{
	}
}

}
