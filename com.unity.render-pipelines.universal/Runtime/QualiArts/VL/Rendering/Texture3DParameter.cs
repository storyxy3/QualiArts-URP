using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Rendering;

namespace VL.Rendering
{

[Serializable]
[DebuggerDisplay("{m_Value} ({m_OverrideState})")]
public class Texture3DParameter : VolumeParameter<Texture>
{
	public Texture3DParameter(Texture value, bool overrideState = false) : base(value, overrideState)
	{
	}
}

}
