// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering {
using System;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]

[VolumeComponentMenu("VL/Density Fog")]
public class VLDensityFogVolume : VolumeComponent
{


	[Tooltip("距離密度")]
	public FloatParameter density;


	[Tooltip("最大適用量")]
	public FloatParameter maxAmount;


	[Tooltip("距離フォグ色")]
	public ColorParameter color;


	[Tooltip("Volume球座標")]
	public Vector3Parameter spherePosition;


	[Tooltip("Volume球半径")]
	public FloatParameter sphereRadius;


	[Tooltip("Volume球最大適用量")]
	public FloatParameter sphereMaxAmount;


	[Tooltip("Volume球色")]
	public ColorParameter sphereColor;


	public VLDensityFogVolume()
	{
	}
}

}
