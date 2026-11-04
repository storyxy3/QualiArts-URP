using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace VL.Rendering
{

[Serializable]
[VolumeComponentMenu("VL/Star Streak")]
public class VLStarStreak : VolumeComponent, IPostProcessComponent
{
	public const int SampleCount = 4;

	[Tooltip("光条タイプ")]
	public StarStreakTypeParameter type = new StarStreakTypeParameter();

	[Tooltip("強度")]
	public ClampedFloatParameter atten = new ClampedFloatParameter(0.925f, 0.0f, 0.99f, false);

	[Tooltip("しきい値")]
	public FloatParameter threshold = new MinFloatParameter(1.0f, 0.0f, false);

	[Tooltip("再帰数(3=64px 4=256px 5=1024px)")]
	public ClampedIntParameter iteration = new ClampedIntParameter(3, 3, 5, false);

	[ColorUsage(false, true)]
	public ColorParameter color = new ColorParameter(new Color(1.0f, 1.0f, 1.0f, 1.0f), true, false, false, false);

	[Tooltip("レインボー")]
	public FloatParameter spectrum = new ClampedFloatParameter(0.0f, 0.0f, 1.0f, false);

	public FloatParameter rotation = new FloatParameter(0.0f, false);

	public bool IsActive()
	{
		return type != StarStreakType.None;
	}

	public bool IsTileCompatible()
	{
		return false;
	}

	public VLStarStreak()
	{
	}
}

}
