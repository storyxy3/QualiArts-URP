// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering {
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using VL.Rendering;

[Serializable]

[VolumeComponentMenu("VL/Tonemapping")]
public sealed class VLTonemapping : VolumeComponent, IPostProcessComponent
{


	[Tooltip("Specifies the tonemapping algorithm to use for the color grading process.")]
	public VL.Rendering.TonemappingModeParameter mode;


	[Tooltip("Controls the transition between the toe and the mid section of the curve. A value of 0 results in no transition and a value of 1 results in a very hard transition.")]
	public ClampedFloatParameter toeStrength;


	[Tooltip("Controls how much of the dynamic range is in the toe. Higher values result in longer toes and therefore contain more of the dynamic range.")]
	public ClampedFloatParameter toeLength;


	[Tooltip("Controls the transition between the midsection and the shoulder of the curve. A value of 0 results in no transition and a value of 1 results in a very hard transition.")]
	public ClampedFloatParameter shoulderStrength;


	[Tooltip("Sets how many F-stops (EV) to add to the dynamic range of the curve.")]
	public MinFloatParameter shoulderLength;


	[Tooltip("Controls how much overshoot to add to the shoulder.")]
	public ClampedFloatParameter shoulderAngle;


	[Tooltip("Sets a gamma correction value that HDRP applies to the whole curve.")]
	public MinFloatParameter gamma;


	[Tooltip("A custom 3D texture lookup table to apply.")]
	public Texture3DParameter lutTexture;


	[Tooltip("How much of the lookup texture will contribute to the color grading effect.")]
	public ClampedFloatParameter lutContribution;


	public FloatParameter gtLinearSectionStart;


	public FloatParameter gtContrast;


	public FloatParameter gtBlackBrightness;


	public FloatParameter gtLinearSelectionLength;


	public bool IsActive()
	{
		return default(bool);
	}


	public bool IsTileCompatible()
	{
		return default(bool);
	}


	public bool ValidateLUT()
	{
		return default(bool);
	}


	public VLTonemapping()
	{
	}
}

}
