using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace VL.Rendering
{

[Serializable]
[VolumeComponentMenu("VL/Density Fog")]
public class VLDensityFogVolume : VolumeComponent
{
	[Tooltip("距離密度")]
	public FloatParameter density = new ClampedFloatParameter(0.0f, 0.0f, 4.0f, false);

	[Tooltip("最大適用量")]
	public FloatParameter maxAmount = new ClampedFloatParameter(1.0f, 0.0f, 1.0f, false);

	[Tooltip("距離フォグ色")]
	public ColorParameter color = new ColorParameter(new Color(0.5f, 0.6f, 0.7f, 1.0f), true, true, false, false);

	[Tooltip("Volume球座標")]
	public Vector3Parameter spherePosition = new Vector3Parameter(new Vector3(0.0f, 0.0f, 0.0f), false);

	[Tooltip("Volume球半径")]
	public FloatParameter sphereRadius = new MinFloatParameter(0.0f, 0.0f, false);

	[Tooltip("Volume球最大適用量")]
	public FloatParameter sphereMaxAmount = new ClampedFloatParameter(1.0f, 0.0f, 1.0f, false);

	[Tooltip("Volume球色")]
	public ColorParameter sphereColor = new ColorParameter(new Color(1.0f, 0.9f, 0.7f, 1.0f), true, false, false, false);

	public VLDensityFogVolume()
	{
	}
}

}
