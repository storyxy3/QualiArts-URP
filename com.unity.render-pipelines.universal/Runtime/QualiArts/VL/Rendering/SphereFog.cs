using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace VL.Rendering
{

[Serializable]
[VolumeComponentMenu("Fog/Sphere")]
public class SphereFog : VolumeComponent
{
	[Tooltip("密度")]
	public FloatParameter density = new ClampedFloatParameter(0.0f, 0.0f, 1.0f, false);

	[Tooltip("最大適用量")]
	public FloatParameter maxAmount = new ClampedFloatParameter(1.0f, 0.0f, 1.0f, false);

	[Tooltip("フォグ色")]
	public ColorParameter color = new ColorParameter(new Color(0.5f, 0.6f, 0.7f, 1.0f), true, false, false, false);

	[Tooltip("中心座標")]
	public Vector3Parameter position = new Vector3Parameter(new Vector3(0.0f, 0.0f, 0.0f), false);

	[Tooltip("半径")]
	public FloatParameter radius = new MinFloatParameter(1.0f, 0.0f, false);

	public SphereFog()
	{
	}
}

}
