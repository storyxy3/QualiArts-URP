// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering {
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable]

[VolumeComponentMenu("VL/JumpFloodOutline")]
public class VLJumpFloodOutline : VolumeComponent, IPostProcessComponent
{


	[Tooltip("メインアウトラインの大きさ")]
	public ClampedFloatParameter mainOutlineAmount;


	[Tooltip("メインアウトラインの色")]
	public ColorParameter mainOutlineColor;


	[Tooltip("メインアウトラインの位置")]
	public Vector2Parameter mainOutlinePosition;


	[Tooltip("サブアウトラインの大きさ")]
	public ClampedFloatParameter subOutlineAmount;


	[Tooltip("サブアウトラインの色")]
	public ColorParameter subOutlineColor;


	[Tooltip("サブアウトラインの位置")]
	public Vector2Parameter subOutlinePosition;


	public bool IsActive()
	{
		return default(bool);
	}


	public bool IsTileCompatible()
	{
		return default(bool);
	}


	public VLJumpFloodOutline()
	{
	}
}

}
