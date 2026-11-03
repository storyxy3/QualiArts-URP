// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering {
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using VL.Rendering;

[Serializable]

[VolumeComponentMenu("VL/Star Streak")]
public class VLStarStreak : VolumeComponent, IPostProcessComponent
{

	public const int SampleCount = 4;


	[Tooltip("光条タイプ")]
	public StarStreakTypeParameter type;


	[Tooltip("強度")]
	public ClampedFloatParameter atten;


	[Tooltip("しきい値")]
	public FloatParameter threshold;


	[Tooltip("再帰数(3=64px 4=256px 5=1024px)")]
	public ClampedIntParameter iteration;


	[ColorUsage(false, true)]
	public ColorParameter color;


	[Tooltip("レインボー")]
	public FloatParameter spectrum;


	public FloatParameter rotation;


	public bool IsActive()
	{
		return default(bool);
	}


	public bool IsTileCompatible()
	{
		return default(bool);
	}


	public VLStarStreak()
	{
	}
}

}
