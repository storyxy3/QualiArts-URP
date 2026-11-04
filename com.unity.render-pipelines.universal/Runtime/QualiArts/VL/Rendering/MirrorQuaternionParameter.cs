using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace VL.Rendering
{

[Serializable]
public class MirrorQuaternionParameter : VolumeParameter<Quaternion>
{
	public MirrorQuaternionParameter(Quaternion value, bool overrideState = false) : base(value, overrideState)
	{
	}
}

}
