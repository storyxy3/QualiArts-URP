using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace VL.Rendering
{

public class VLPostProcessData : PostProcessData
{
	public Shader vlDiffusion;

	public Shader vlBloom;

	public Shader vlFlare;

	public Shader vlDOF;

	public Shader vlStarStreak;

	public Shader vlMotionBlur;

	public Shader vlTextureBlur;

	public Shader vlVirtualEffect;

	public Shader vlNoise;

	public Shader vlChromaticAberration;

	public Shader vlJumpFloodOutline;

	public ShaderVariantCollection vlPostProcessVariant;

	public bool useComputeGrading;

	public ComputeShader lutCompute;

	public ComputeShader minMaxCocComputeShader;

	public VLPostProcessData()
	{
	}
}

}
