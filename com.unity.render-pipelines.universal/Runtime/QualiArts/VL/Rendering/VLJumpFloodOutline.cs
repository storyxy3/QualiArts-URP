using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace VL.Rendering
{

[Serializable]
[VolumeComponentMenu("VL/JumpFloodOutline")]
public class VLJumpFloodOutline : VolumeComponent, IPostProcessComponent
{
	[Tooltip("メインアウトラインの大きさ")]
	public ClampedFloatParameter mainOutlineAmount = new ClampedFloatParameter(0.0f, 0.0f, 1.0f, false);

	[Tooltip("メインアウトラインの色")]
	public ColorParameter mainOutlineColor = new ColorParameter(new Color(1.0f, 1.0f, 1.0f, 1.0f), false);

	[Tooltip("メインアウトラインの位置")]
	public Vector2Parameter mainOutlinePosition = new Vector2Parameter(Vector2.zero, false);

	[Tooltip("サブアウトラインの大きさ")]
	public ClampedFloatParameter subOutlineAmount = new ClampedFloatParameter(0.0f, 0.0f, 1.0f, false);

	[Tooltip("サブアウトラインの色")]
	public ColorParameter subOutlineColor = new ColorParameter(new Color(1.0f, 1.0f, 1.0f, 1.0f), false);

	[Tooltip("サブアウトラインの位置")]
	public Vector2Parameter subOutlinePosition = new Vector2Parameter(Vector2.zero, false);

	public bool IsActive()
	{
		return mainOutlineAmount.value > 0f;
	}

	public bool IsTileCompatible()
	{
		return true;
	}

	public VLJumpFloodOutline()
	{
	}
}

}
