using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace VL.Rendering
{

[Serializable]
[VolumeComponentMenu("VL/Depth Of Field")]
public sealed class VLDOF : VolumeComponent, IPostProcessComponent
{
	[Tooltip("ブラー適用タイプ")]
	public Dof34QualitySettingParameter quality = new Dof34QualitySettingParameter((Dof34QualitySetting)0, false);

	[Tooltip("フォーカスポイントまでの距離(m)")]
	public MinFloatParameter focalPoint = new MinFloatParameter(1.0f, 0.1f, false);

	[Tooltip("F値")]
	public ClampedFloatParameter fNumber = new ClampedFloatParameter(2.8f, 1.0f, 64.0f, false);

	[Tooltip("最大ブラーサイズ")]
	public MinFloatParameter maxBlurSpread = new MinFloatParameter(2.0f, 0.0f, false);

	[Tooltip("手前ブラーの適用量")]
	public MinFloatParameter foregroundBlurExtrude = new MinFloatParameter(1.15f, 0.0f, false);

	[Tooltip("F値で指定するかどうか")]
	public BoolParameter useFNumber = new BoolParameter(true, false);

	[Tooltip("Legacy ぼかし距離(m)")]
	public MinFloatParameter smoothness = new MinFloatParameter(0.5f, 0.01f, false);

	[Tooltip("Legacy 焦点が合う距離(m)")]
	public MinFloatParameter focalSize = new MinFloatParameter(1.0f, 0.01f, false);

	public ClampedIntParameter bladeCount = new ClampedIntParameter(5, 3, 9, false);

	public ClampedFloatParameter bladeCurvature = new ClampedFloatParameter(1.0f, 0.0f, 1.0f, false);

	public ClampedFloatParameter bladeRotation = new ClampedFloatParameter(0.0f, -180.0f, 180.0f, false);

	private bool _initialized;

	private bool _isDeviceSupported;

	public bool IsActive()
	{
		if (!_initialized)
        {
            _isDeviceSupported = SystemInfo.graphicsShaderLevel >= 30 && SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf);
            _isDeviceSupported &= !SystemInfo.graphicsDeviceName.EndsWith("G6430");
            _initialized = true;
        }
        return _isDeviceSupported && quality.value != Dof34QualitySetting.None;
	}

	public bool IsTileCompatible()
	{
		return false;
	}

	public VLDOF()
	{
	}
}

}
