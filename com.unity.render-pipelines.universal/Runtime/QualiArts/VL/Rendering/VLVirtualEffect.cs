using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace VL.Rendering
{

[Serializable]
[VolumeComponentMenu("VL/VirtualEffect")]
public class VLVirtualEffect : VolumeComponent, IPostProcessComponent
{
	[Tooltip("バーチャルエフェクトの有効無効")]
	public BoolParameter isEnabled = new BoolParameter(false, false);

	[Tooltip("キャラ：バーチャルカラー")]
	public ColorParameter virtualColor = new ColorParameter(new Color(1.0f, 1.0f, 1.0f, 1.0f), true, false, true, false);

	[Tooltip("キャラ：スキャンラインの密度")]
	public FloatParameter scanLineDensity = new FloatParameter(1000.0f, false);

	[Tooltip("キャラ：スキャンラインの幅")]
	public ClampedFloatParameter scanlineRange = new ClampedFloatParameter(0.2f, 0.0f, 1.0f, false);

	[Tooltip("キャラ：スキャンラインのアルファ値")]
	public ClampedFloatParameter scanLineAlpha = new ClampedFloatParameter(0.5f, 0.0f, 1.0f, false);

	[Tooltip("キャラ：スキャンライン(自己発光)の色")]
	public ColorParameter emissiveScanlineColor = new ColorParameter(new Color(1.0f, 1.0f, 1.0f, 1.0f), true, false, true, false);

	[Tooltip("キャラ：スキャンライン(自己発光)の速度")]
	public FloatParameter emissiveScanlineSpeed = new FloatParameter(0.8f, false);

	[Tooltip("キャラ：スキャンライン(自己発光)の強さ")]
	public FloatParameter emissiveScanlinePower = new FloatParameter(15.0f, false);

	[Tooltip("オーラ：範囲")]
	public ClampedIntParameter auraDiffusion = new ClampedIntParameter(10, 0, 10, false);

	[Tooltip("オーラ：色")]
	public ColorParameter auraColor = new ColorParameter(new Color(1.0f, 1.0f, 1.0f, 1.0f), true, false, true, false);

	[Tooltip("ノイズ：周期タイプ")]
	public IntervalTypeParameter intervalType = new IntervalTypeParameter((IntervalType)0, false);

	[Tooltip("ノイズ：頻度")]
	public ClampedFloatParameter frequency = new ClampedFloatParameter(1.0f, 0.0f, 25.0f, false);

	[Tooltip("ノイズ：タイプ")]
	public NoiseTypeParameter noiseType = new NoiseTypeParameter((NoiseType)0, false);

	[Tooltip("ノイズ：速度")]
	public FloatParameter speed = new FloatParameter(0.5f, false);

	[Tooltip("ノイズ：強度")]
	public ClampedFloatParameter intensity = new ClampedFloatParameter(0.1f, 0.0f, 1.0f, false);

	[Tooltip("ノイズ：粒度")]
	public ClampedFloatParameter granularity = new ClampedFloatParameter(0.01f, 0.001f, 0.1f, false);

	[Tooltip("ノイズ：色分散度")]
	public FloatParameter rgbSplit = new FloatParameter(20.0f, false);

	[Tooltip("色収差：強度")]
	public ClampedFloatParameter chromaticAberrationIntensity = new ClampedFloatParameter(0.01f, 0.0f, 1.0f, false);

	public bool IsActiveAura()
	{
		return auraDiffusion.value > 0 && isEnabled.value;
	}

	public bool IsActiveNoise()
	{
		return intensity.value > 0f && isEnabled.value;
	}

	public bool IsActiveChromaticAberration()
	{
		return chromaticAberrationIntensity.value > 0f && isEnabled.value;
	}

	public bool IsActive()
	{
		return isEnabled.value;
	}

	public bool IsTileCompatible()
	{
		return true;
	}

	public VLVirtualEffect()
	{
	}
}

}
