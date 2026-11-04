using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace VL.Rendering
{

[Serializable]
[VolumeComponentMenu("VL/ChromaticAberration")]
public class VLChromaticAberration : VolumeComponent, IPostProcessComponent
{
	public Vector2Parameter redDirection = new Vector2Parameter(Vector2.left, false);

	public Vector2Parameter greenDirection = new Vector2Parameter(Vector2.up, false);

	public Vector2Parameter blueDirection = new Vector2Parameter(Vector2.right, false);

	public IntervalTypeParameter intervalType = new IntervalTypeParameter((IntervalType)1, false);

	public ClampedFloatParameter frequency = new ClampedFloatParameter(1.0f, 0.0f, 50.0f, false);

	public ClampedFloatParameter rgbSplit = new ClampedFloatParameter(0.0f, 0.0f, 50.0f, false);

	public bool IsActive()
	{
		return rgbSplit.value > 0f;
	}

	public bool IsTileCompatible()
	{
		return true;
	}

	public VLChromaticAberration()
	{
	}
}

}
