using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace VL.Rendering
{

[Serializable]
public class MirrorMeshParameter : VolumeParameter<Mesh>
{
	public MirrorMeshParameter(Mesh value, bool overrideState = false) : base(value, overrideState)
	{
	}
}

}
