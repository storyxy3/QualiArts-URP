using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

namespace VL.Rendering
{

[Serializable]
[VolumeComponentMenu("VL/Actor Parameter")]
public class VLActorParameter : VolumeComponent, IPostProcessComponent
{
	[Tooltip("ライトカラー")]
	public ColorParameter lightColor = new ColorParameter(new Color(1.0f, 1.0f, 1.0f, 1.0f), true, false, true, false);

	[Tooltip("陰影ライト角度空間")]
	public LightSpaceTypeParameter mainLightSpace = new LightSpaceTypeParameter((LightSpaceType)0, false);

	[Tooltip("陰影ライト角度")]
	public Vector2Parameter mainLightAngle = new Vector2Parameter(new Vector2(-7.0f, 7.0f), false);

	[Tooltip("陰の量オフセット")]
	public FloatParameter matCapOffset = new ClampedFloatParameter(0.3f, -1.0f, 1.0f, false);

	[Tooltip("陰のぼかし量スケール")]
	public FloatParameter matCapSmoothScale = new ClampedFloatParameter(1.0f, 0.0f, 2.0f, false);

	[Tooltip("陰の追加乗算カラー")]
	public ColorParameter shadeColorMultiply = new ColorParameter(new Color(1.0f, 1.0f, 1.0f, 1.0f), false, false, false, false);

	[Tooltip("陰の追加加算カラー")]
	public ColorParameter shadeColorAdditive = new ColorParameter(new Color(0.0f, 0.0f, 0.0f, 1.0f), true, false, false, false);

	[Tooltip("陰の適用量")]
	public FloatParameter shadeApplyRatio = new ClampedFloatParameter(1.0f, 0.0f, 1.0f, false);

	[Tooltip("リムの角度")]
	public Vector2Parameter rimAngle = new Vector2Parameter(new Vector2(0.0f, 0.0f), false);

	[FormerlySerializedAs("rimAdditive")]
	[Tooltip("リムのbaseColor乗算割合")]
	public FloatParameter rimBaseColorRatio = new ClampedFloatParameter(1.0f, 0.0f, 1.0f, false);

	[Tooltip("リムの掛け方")]
	public FloatParameter rimPower = new ClampedFloatParameter(8.0f, 1.0f, 32.0f, false);

	[Tooltip("リムの色 (出さない場合は黒)")]
	public ColorParameter rimColor = new ColorParameter(new Color(0.0f, 0.0f, 0.0f, 1.0f), true, true, false, false);

	[Tooltip("giスケール")]
	public FloatParameter giScale = new ClampedFloatParameter(0.0f, 0.0f, 1.0f, false);

	[Tooltip("加算ライトの適用量")]
	public FloatParameter additiveLightScale = new ClampedFloatParameter(1.0f, 0.0f, 1.0f, false);

	[Tooltip("加算ライトスペキュラー強度")]
	public FloatParameter additiveLightSpecularScale = new ClampedFloatParameter(1.0f, 0.0f, 1.0f, false);

	[Tooltip("肌の彩度上げ")]
	public FloatParameter skinSaturation = new ClampedFloatParameter(0.0f, 0.0f, 1.0f, false);

	[Tooltip("瞳ハイライトの色")]
	public ColorParameter eyeHighlightColor = new ColorParameter(new Color(1.0f, 1.0f, 1.0f, 1.0f), true, false, false, false);

	[Tooltip("疑似スペキュラテクスチャ")]
	public NoInterpCubemapParameter reflectionTexture = new NoInterpCubemapParameter(null, false);

	[Tooltip("疑似スペキュラカラー")]
	public ColorParameter reflectionColor = new ColorParameter(new Color(1.0f, 1.0f, 1.0f, 1.0f), true, false, false, false);

	[Tooltip("目の疑似スペキュラテクスチャ")]
	public NoInterpCubemapParameter eyeReflectionTexture = new NoInterpCubemapParameter(null, false);

	[Tooltip("目の疑似スペキュラカラー")]
	public ColorParameter eyeReflectionColor = new ColorParameter(new Color(1.0f, 1.0f, 1.0f, 1.0f), true, false, false, false);

	public bool IsActive()
	{
		return true;
	}

	public bool IsTileCompatible()
	{
		return false;
	}

	public VLActorParameter()
	{
	}
}

}
