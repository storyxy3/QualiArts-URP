using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace VL.Rendering
{

[Serializable]
[VolumeComponentMenu("VL/VL Motion Blur")]
public class VLMotionBlur : VolumeComponent
{
	public ClampedFloatParameter shutterAngle = new ClampedFloatParameter(270.0f, 0.0f, 360.0f, false);

	public ClampedIntParameter sampleCount = new ClampedIntParameter(8, 4, 32, false);

	public bool IsActive()
	{
		return active && shutterAngle.overrideState && shutterAngle.value > 0f && sampleCount.value > 0;
	}

	public VLMotionBlur()
	{
	}
}

}
