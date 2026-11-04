using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace VL.Rendering
{

[Serializable]
[VolumeComponentMenu("VL/Tonemapping")]
public sealed class VLTonemapping : VolumeComponent, IPostProcessComponent
{
	[Tooltip("Specifies the tonemapping algorithm to use for the color grading process.")]
	public TonemappingModeParameter mode = new TonemappingModeParameter((VLTonemappingMode)0, false);

	[Tooltip("Controls the transition between the toe and the mid section of the curve. A value of 0 results in no transition and a value of 1 results in a very hard transition.")]
	public ClampedFloatParameter toeStrength = new ClampedFloatParameter(0.0f, 0.0f, 1.0f, false);

	[Tooltip("Controls how much of the dynamic range is in the toe. Higher values result in longer toes and therefore contain more of the dynamic range.")]
	public ClampedFloatParameter toeLength = new ClampedFloatParameter(0.5f, 0.0f, 1.0f, false);

	[Tooltip("Controls the transition between the midsection and the shoulder of the curve. A value of 0 results in no transition and a value of 1 results in a very hard transition.")]
	public ClampedFloatParameter shoulderStrength = new ClampedFloatParameter(0.0f, 0.0f, 1.0f, false);

	[Tooltip("Sets how many F-stops (EV) to add to the dynamic range of the curve.")]
	public MinFloatParameter shoulderLength = new MinFloatParameter(0.5f, 0.0f, false);

	[Tooltip("Controls how much overshoot to add to the shoulder.")]
	public ClampedFloatParameter shoulderAngle = new ClampedFloatParameter(0.0f, 0.0f, 1.0f, false);

	[Tooltip("Sets a gamma correction value that HDRP applies to the whole curve.")]
	public MinFloatParameter gamma = new MinFloatParameter(1.0f, 0.001f, false);

	[Tooltip("A custom 3D texture lookup table to apply.")]
	public Texture3DParameter lutTexture = new Texture3DParameter(null, false);

	[Tooltip("How much of the lookup texture will contribute to the color grading effect.")]
	public ClampedFloatParameter lutContribution = new ClampedFloatParameter(1.0f, 0.0f, 1.0f, false);

	public FloatParameter gtLinearSectionStart = new ClampedFloatParameter(0.22f, 0.0f, 1.0f, false);

	public FloatParameter gtContrast = new ClampedFloatParameter(1.0f, 0.0f, 5.0f, false);

	public FloatParameter gtBlackBrightness = new ClampedFloatParameter(1.33f, 1.0f, 3.0f, false);

	public FloatParameter gtLinearSelectionLength = new ClampedFloatParameter(0.4f, 0.0f, 1.0f, false);

	public bool IsActive()
	{
		return mode.value == VLTonemappingMode.External ? ValidateLUT() && lutContribution.value > 0f : mode.value != VLTonemappingMode.None;
	}

	public bool IsTileCompatible()
	{
		return false;
	}

	public bool ValidateLUT()
	{
        if (lutTexture.value == null) return false;
        var asset = GraphicsSettings.defaultRenderPipeline as UniversalRenderPipelineAsset;
        if (asset == null || lutTexture.value.width != asset.colorGradingLutSize) return false;
        switch (lutTexture.value)
        {
            case Texture3D texture:
                return texture.width == texture.height && texture.height == texture.depth;
            case RenderTexture texture:
                return texture.dimension == UnityEngine.Rendering.TextureDimension.Tex3D && texture.width == texture.height && texture.height == texture.volumeDepth;
            default: return false;
        }
	}

	public VLTonemapping()
	{
	}
}

}
