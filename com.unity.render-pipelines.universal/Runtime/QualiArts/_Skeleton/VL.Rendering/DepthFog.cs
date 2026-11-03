// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering {
using System;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]

[VolumeComponentMenu("Fog/Depth")]
public class DepthFog : VolumeComponent
{


	[Tooltip("密度")]
	public FloatParameter density;


	[Tooltip("最大適用量")]
	public FloatParameter maxAmount;


	[Tooltip("フォグ色")]
	public ColorParameter color;


	[Tooltip("スカイボックスに適用するかどうか")]
	public BoolParameter applySkybox;


	public DepthFog()
	{
	}
}

}
