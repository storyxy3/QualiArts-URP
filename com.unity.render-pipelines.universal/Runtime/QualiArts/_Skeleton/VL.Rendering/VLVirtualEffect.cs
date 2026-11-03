// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering {
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using VL.Rendering;

[Serializable]

[VolumeComponentMenu("VL/VirtualEffect")]
public class VLVirtualEffect : VolumeComponent, IPostProcessComponent
{


	[Tooltip("バーチャルエフェクトの有効無効")]
	public BoolParameter isEnabled;


	[Tooltip("キャラ：バーチャルカラー")]
	public ColorParameter virtualColor;


	[Tooltip("キャラ：スキャンラインの密度")]
	public FloatParameter scanLineDensity;


	[Tooltip("キャラ：スキャンラインの幅")]
	public ClampedFloatParameter scanlineRange;


	[Tooltip("キャラ：スキャンラインのアルファ値")]
	public ClampedFloatParameter scanLineAlpha;


	[Tooltip("キャラ：スキャンライン(自己発光)の色")]
	public ColorParameter emissiveScanlineColor;


	[Tooltip("キャラ：スキャンライン(自己発光)の速度")]
	public FloatParameter emissiveScanlineSpeed;


	[Tooltip("キャラ：スキャンライン(自己発光)の強さ")]
	public FloatParameter emissiveScanlinePower;


	[Tooltip("オーラ：範囲")]
	public ClampedIntParameter auraDiffusion;


	[Tooltip("オーラ：色")]
	public ColorParameter auraColor;


	[Tooltip("ノイズ：周期タイプ")]
	public IntervalTypeParameter intervalType;


	[Tooltip("ノイズ：頻度")]
	public ClampedFloatParameter frequency;


	[Tooltip("ノイズ：タイプ")]
	public NoiseTypeParameter noiseType;


	[Tooltip("ノイズ：速度")]
	public FloatParameter speed;


	[Tooltip("ノイズ：強度")]
	public ClampedFloatParameter intensity;


	[Tooltip("ノイズ：粒度")]
	public ClampedFloatParameter granularity;


	[Tooltip("ノイズ：色分散度")]
	public FloatParameter rgbSplit;


	[Tooltip("色収差：強度")]
	public ClampedFloatParameter chromaticAberrationIntensity;


	public bool IsActiveAura()
	{
		return default(bool);
	}


	public bool IsActiveNoise()
	{
		return default(bool);
	}


	public bool IsActiveChromaticAberration()
	{
		return default(bool);
	}


	public bool IsActive()
	{
		return default(bool);
	}


	public bool IsTileCompatible()
	{
		return default(bool);
	}


	public VLVirtualEffect()
	{
	}
}

}
