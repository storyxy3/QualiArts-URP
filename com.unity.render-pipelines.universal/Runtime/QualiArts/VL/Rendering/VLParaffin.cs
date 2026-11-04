using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace VL.Rendering
{

[Serializable]
[VolumeComponentMenu("VL/Paraffin")]
public class VLParaffin : VolumeComponent, IPostProcessComponent
{
	public FloatParameter baseAspect = new FloatParameter(1.7778f, false);

	public FlareParameter flare0 = new FlareParameter();

	public FlareParameter flare1 = new FlareParameter();

	public bool IsActive()
	{
		return (flare0.value.type != FlareType.None || flare1.value.type != FlareType.None) && IsValidAspect(baseAspect.value);
	}

	public bool IsTileCompatible()
	{
		return true;
	}

	public bool IsValidAspect(float aspect)
	{
		return float.IsFinite(aspect) && aspect >= 0.001f;
	}

	public VLParaffin()
	{
	}
}

}
