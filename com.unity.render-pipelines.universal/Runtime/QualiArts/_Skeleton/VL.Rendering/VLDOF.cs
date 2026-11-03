// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering {
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using VL.Rendering;

[Serializable]

[VolumeComponentMenu("VL/Depth Of Field")]
public sealed class VLDOF : VolumeComponent, IPostProcessComponent
{


	[Tooltip("ブラー適用タイプ")]
	public Dof34QualitySettingParameter quality;


	[Tooltip("フォーカスポイントまでの距離(m)")]
	public MinFloatParameter focalPoint;


	[Tooltip("F値")]
	public ClampedFloatParameter fNumber;


	[Tooltip("最大ブラーサイズ")]
	public MinFloatParameter maxBlurSpread;


	[Tooltip("手前ブラーの適用量")]
	public MinFloatParameter foregroundBlurExtrude;


	[Tooltip("F値で指定するかどうか")]
	public BoolParameter useFNumber;


	[Tooltip("Legacy ぼかし距離(m)")]
	public MinFloatParameter smoothness;


	[Tooltip("Legacy 焦点が合う距離(m)")]
	public MinFloatParameter focalSize;


	public ClampedIntParameter bladeCount;


	public ClampedFloatParameter bladeCurvature;


	public ClampedFloatParameter bladeRotation;


	private bool _initialized;


	private bool _isDeviceSupported;


	public bool IsActive()
	{
		return default(bool);
	}


	public bool IsTileCompatible()
	{
		return default(bool);
	}


	public VLDOF()
	{
	}
}

}
