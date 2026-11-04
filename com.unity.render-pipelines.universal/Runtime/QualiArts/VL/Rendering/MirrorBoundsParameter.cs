using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace VL.Rendering
{

[Serializable]
public class MirrorBoundsParameter : VolumeParameter<Bounds>
{
	public MirrorBoundsParameter(Bounds value, bool overrideState = false) : base(value, overrideState)
	{
	}
}

}
