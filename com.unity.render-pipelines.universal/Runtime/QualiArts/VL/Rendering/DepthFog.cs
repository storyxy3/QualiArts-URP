using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace VL.Rendering
{

[Serializable]
[VolumeComponentMenu("Fog/Depth")]
public class DepthFog : VolumeComponent
{
	[Tooltip("密度")]
	public FloatParameter density = new ClampedFloatParameter(0.0f, 0.0f, 2.0f, false);

	[Tooltip("最大適用量")]
	public FloatParameter maxAmount = new ClampedFloatParameter(1.0f, 0.0f, 1.0f, false);

	[Tooltip("フォグ色")]
	public ColorParameter color = new ColorParameter(new Color(0.5f, 0.6f, 0.7f, 1.0f), true, false, false, false);

	[Tooltip("スカイボックスに適用するかどうか")]
	public BoolParameter applySkybox = new BoolParameter(false, false);

	public DepthFog()
	{
	}
}

}
