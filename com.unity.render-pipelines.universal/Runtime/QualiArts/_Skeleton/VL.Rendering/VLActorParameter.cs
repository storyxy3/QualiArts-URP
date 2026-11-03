// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering {
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using VL.Rendering;

[Serializable]

[VolumeComponentMenu("VL/Actor Parameter")]
public class VLActorParameter : VolumeComponent, IPostProcessComponent
{


	[Tooltip("ライトカラー")]
	public ColorParameter lightColor;


	[Tooltip("陰影ライト角度空間")]
	public LightSpaceTypeParameter mainLightSpace;


	[Tooltip("陰影ライト角度")]
	public Vector2Parameter mainLightAngle;


	[Tooltip("陰の量オフセット")]
	public FloatParameter matCapOffset;


	[Tooltip("陰のぼかし量スケール")]
	public FloatParameter matCapSmoothScale;


	[Tooltip("陰の追加乗算カラー")]
	public ColorParameter shadeColorMultiply;


	[Tooltip("陰の追加加算カラー")]
	public ColorParameter shadeColorAdditive;


	[Tooltip("陰の適用量")]
	public FloatParameter shadeApplyRatio;


	[Tooltip("リムの角度")]
	public Vector2Parameter rimAngle;


	[FormerlySerializedAs("rimAdditive")]
	[Tooltip("リムのbaseColor乗算割合")]
	public FloatParameter rimBaseColorRatio;


	[Tooltip("リムの掛け方")]
	public FloatParameter rimPower;


	[Tooltip("リムの色 (出さない場合は黒)")]
	public ColorParameter rimColor;


	[Tooltip("giスケール")]
	public FloatParameter giScale;


	[Tooltip("加算ライトの適用量")]
	public FloatParameter additiveLightScale;


	[Tooltip("加算ライトスペキュラー強度")]
	public FloatParameter additiveLightSpecularScale;


	[Tooltip("肌の彩度上げ")]
	public FloatParameter skinSaturation;


	[Tooltip("瞳ハイライトの色")]
	public ColorParameter eyeHighlightColor;


	[Tooltip("疑似スペキュラテクスチャ")]
	public NoInterpCubemapParameter reflectionTexture;


	[Tooltip("疑似スペキュラカラー")]
	public ColorParameter reflectionColor;


	[Tooltip("目の疑似スペキュラテクスチャ")]
	public NoInterpCubemapParameter eyeReflectionTexture;


	[Tooltip("目の疑似スペキュラカラー")]
	public ColorParameter eyeReflectionColor;


	public bool IsActive()
	{
		return default(bool);
	}


	public bool IsTileCompatible()
	{
		return default(bool);
	}


	public VLActorParameter()
	{
	}
}

}
