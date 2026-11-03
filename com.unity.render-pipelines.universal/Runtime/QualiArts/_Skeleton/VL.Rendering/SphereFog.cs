// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering {
using System;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]

[VolumeComponentMenu("Fog/Sphere")]
public class SphereFog : VolumeComponent
{


	[Tooltip("密度")]
	public FloatParameter density;


	[Tooltip("最大適用量")]
	public FloatParameter maxAmount;


	[Tooltip("フォグ色")]
	public ColorParameter color;


	[Tooltip("中心座標")]
	public Vector3Parameter position;


	[Tooltip("半径")]
	public FloatParameter radius;


	public SphereFog()
	{
	}
}

}
