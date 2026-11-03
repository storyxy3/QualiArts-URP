// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering.Internal {
using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using VL.Rendering;
using VL.Rendering.Internal;
using VL.Rendering.Shaders;

[Serializable]

internal class VLPostProcessPass : PostProcessPass
{

	private class DOFHandles
	{


		private RTHandle m_Far;


		private RTHandle m_Near;

		public RTHandle far
		{


			get
			{
				return null;
			}
		}

		public RTHandle near
		{


			get
			{
				return null;
			}
		}


		public DOFHandles(string name)
		{
		}


		public bool ReAllocateIfNeeded(int width, int height, GraphicsFormat colorFormat)
		{
			return default(bool);
		}


		public void Release()
		{
		}
	}

	private class VLMaterialLibrary
	{


		public readonly Material vlDiffusion;


		public readonly Material vlBloom;


		public readonly Material vlFlare;


		public readonly Material vlDOF;


		public readonly Material vlStarStreak;


		public readonly Material vlMotionBlur;


		public readonly Material vlTextureBlur;


		public readonly Material vlVirtualEffect;


		public readonly Material vlNoise;


		public readonly Material vlChromaticAberration;


		public readonly Material vlJumpFloodOutline;


		public VLMaterialLibrary(VLPostProcessData data)
		{
		}


		private Material Load(Shader shader)
		{
			return null;
		}


		internal void Cleanup()
		{
		}
	}

	private enum VLDOFPasses
	{

		CalcCoc,

		CalcBokehCoc,

		CocReprojection,

		InflateCocFirst,

		InflateCocSecond,

		DownsampleMRT,

		Downsample2MRT,

		BlurFirstMRT,

		BlurSecondMRT,

		BlurFirst2MRT,

		UpsampleMRT,

		ApplyMRT,

		BokehPrefilter,

		BokehBlurMRT,

		BokehFloodFill,

		BokehPostBlurMRT,

		BokehComposite
	}

	private static class VLDOFShaderConstants
	{


		public static readonly int ShaderVariablesVLDOFBokeh;


		public static readonly int _ForegroundBlurExtrude;


		public static readonly int _DOFFrontTexture;


		public static readonly int _DOFBackTexture;


		public static readonly int _HistoryCoCTexture;


		public static readonly int _CocReprojectionParams;


		public static readonly int _DOFMediumRezWorkTexture;


		public static readonly int _DOFLowNearTexture;


		public static readonly int _InvRenderTargetSize;


		public static readonly int _DOFBlurOffsets;


		public static readonly int _DOFCurveParams;


		public static readonly int _DOFCocOffset;
	}

	private class VLJumpFloodOutlineShaderConstants
	{


		internal static readonly int _DistanceFieldMap;


		internal static readonly int _StepWidth;


		internal static readonly int _MainOutlineAmount;


		internal static readonly int _MainOutlineColor;


		internal static readonly int _MainOffsetShiftUV;


		internal static readonly int _SubOutlineAmount;


		internal static readonly int _SubOutlineColor;


		internal static readonly int _SubOffsetShiftUV;


		internal static readonly int _OutlineMaxWidth;


		internal static readonly int _SmoothDelta;


		internal static readonly int _HistoryDistanceFieldTexture;


		internal static readonly int _DistanceFiledReprojectionParams;
	}

	private class VLMotionBlurShaderConstants
	{


		internal static readonly int VelocityScale;


		internal static readonly int MaxBlurRadius;


		internal static readonly int RcpMaxBlurRadius;


		internal static readonly int TileMaxOffs;


		internal static readonly int TileMaxLoop;


		internal static readonly int LoopCount;
	}

	private class VLShaderConstants
	{


		public static readonly int _TextureBlurIntensity;


		public static readonly int _TextureBlurTexture;


		public static readonly int _Bloom_Settings;


		public static readonly int _Bloom_Color;


		public static readonly int _Flare_Settings;


		public static readonly int _Flare_Color0;


		public static readonly int _Flare_Color1;


		public static readonly int _StarStreakParam;


		public static readonly int _StarStreakBlurParams;


		public static readonly int _Diffusion_Settings;


		public static readonly int _Diffusion_Contrasts;


		public static readonly int _BlurOffset;
	}

	private class VLVirtualEffectShaderConstants
	{


		internal static readonly int _VirtualEffect_Mask;


		internal static readonly int _AuraColor;


		internal static readonly int _NoiseParams;


		internal static readonly int _Granularity;


		internal static readonly int _RGBSplit;


		internal static readonly int _ChromaticAberrationIntensity;
	}

	private class VLChromaticAberrationShaderConstants
	{


		internal static readonly int _RedDirection;


		internal static readonly int _GreenDirection;


		internal static readonly int _BlueDirection;
	}

	private class VLRGBBockNoiseShaderConstants
	{


		internal static readonly int _OffsetUV;


		internal static readonly int _StepFrequency;


		internal static readonly int _TileOffsetX;


		internal static readonly int _TileOffsetY;


		internal static readonly int _Aspect;


		internal static readonly int _Attenuation;


		internal static readonly int _Saturation;
	}

	private class VLWaveJitterGlitchShaderContents
	{


		internal static readonly int _Frequency;


		internal static readonly int _Intensity;


		internal static readonly int _Granularity;


		internal static readonly int _WaveSpeed;


		internal static readonly int _RGBSplit;
	}


	private static readonly ProfilingSampler fsrUpscalingSampler;


	private RTHandle m_DownscaleAdditiveTexture;


	private RTHandle m_DiffusionTexture;


	private RTHandle m_DiffusionTemp;


	private RTHandle m_DOFCocTexture;


	private RTHandle m_DOFInflateCoc1;


	private RTHandle m_DOFInflateCoc2;


	private DOFHandles m_DOFMedium;


	private DOFHandles m_DOFLow;


	private DOFHandles m_DOFTemp1;


	private DOFHandles m_DOFTemp2;


	private RTHandle m_VelocityTex;


	private RTHandle[] m_TileRTs;


	private RTHandle m_TileVRT;


	private RTHandle m_NeighborMaxTex;


	private VLMaterialLibrary m_VLMaterials;


	private bool _computeGradingAvailable;


	private bool _vlDOFSupported;


	private GraphicsFormat m_RequestHDRFormat;


	private RTHandle m_ChromaticAberrationTexture;


	private readonly ProfilingSampler _vlDOFProfilingSampler;


	private static RenderTargetBinding DofMRT2Bindings;


	private static RenderTargetBinding DofMRT3Bindings;


	private ShaderVariablesVLDOFBokeh _bokehData;


	private static readonly ProfilingSampler _vlJumpFloodOutlineSampler;


	private RTHandle m_OutlineMaskHalfRT;


	private RTHandle m_OutlineMaskQuarterRT;


	private RTHandle m_PingBuffer;


	private RTHandle m_PongBuffer;


	private RTHandle m_DistanceFieldPingRT;


	private RTHandle m_DistanceFieldPongRT;


	private bool m_distanceFieldPingPong;


	private Vector2 _mainOutlineOffsetShiftUV;


	private Vector2 _subOutlineOffsetShiftUV;


	private readonly ProfilingSampler _blendDownscaleProfilingSampler;


	private readonly ProfilingSampler _vlDiffusionProfilingSampler;


	private readonly ProfilingSampler _vlBloomProfilingSampler;


	private readonly ProfilingSampler _vlParaffinProfilingSampler;


	private readonly ProfilingSampler _vlMotionBlurSampler;


	private readonly ProfilingSampler _vlTextureBlurSampler;


	private Vector4[] _starStreakParams;


	private static readonly ProfilingSampler _vlVirtualEffectSampler;


	private RTHandle m_AdditiveTextureToAdd;


	private RTHandle[] m_AuraMipDown;


	private RTHandle[] m_AuraMipUp;


	private static readonly ProfilingSampler _vlChromaticAberrationSampler;


	private Vector2 _offsetUV;


	private Vector2 _redDirection;


	private Vector2 _greenDirection;


	private Vector2 _blueDirection;


	private static readonly ProfilingSampler _vlNoiseSampler;

	public bool UseComputeGrading
	{


		get
		{
			return default(bool);
		}
	}


	public VLPostProcessPass(RenderPassEvent evt, VLPostProcessData data, ref PostProcessParams postProcessParams, bool computeGradingAvailable) : base(default, default, ref postProcessParams) // TODO(qa-recon): original constructor chaining unavailable.
	{
	}


	private void SetDefaultColorFormat(bool needsAlphaChannel)
	{
	}


	public override void Cleanup()
	{
	}


	public override void Dispose()
	{
	}


	public override void Setup(in RenderTextureDescriptor baseDescriptor, in RTHandle source, bool resolveToScreen, in RTHandle depth, in RTHandle internalLut, in RTHandle motionVectors, bool hasFinalPass, bool enableColorEncoding)
	{
	}


	internal override void Render(CommandBuffer cmd, ref RenderingData renderingData)
	{
	}


	private void RenderFinalPassWithFSR(CommandBuffer cmd, ref RenderingData renderingData, RTHandle sourceTex)
	{
	}


	internal override void RenderFinalPass(CommandBuffer cmd, ref RenderingData renderingData)
	{
	}


	protected override void SetupColorGrading(CommandBuffer cmd, ref RenderingData renderingData, Material material)
	{
	}


	private bool DoVLDOF(CommandBuffer cmd, RTHandle source, RTHandle destination, ref CameraData cameraData)
	{
		return default(bool);
	}


	public static void SetDOFCurveProperty(CommandBuffer cmd, VLDOF dof, Camera camera, Vector3 worldCameraPosition, out bool blurBackground, out bool blurForeground)
	{
            blurBackground = default;
            blurForeground = default;

	}


	private bool DrawVLDOFMRT(VLDOF vlDof, CommandBuffer cmd, ref CameraData cameraData, RTHandle source, RTHandle destination)
	{
		return default(bool);
	}


	private void VLDOFDownsampleMRT(CommandBuffer cmd, RTHandle source, Material dofMaterial, int width, int height, VLDOFPasses pass)
	{
	}


	private void VLDOFBlurMRT(CommandBuffer cmd, Material dofMaterial, RTHandle srcBack, RTHandle srcFront, RTHandle dstBack, RTHandle dstFront, RTHandle tempBack, RTHandle tempFront, float maxBlurSpread, float oneOverBaseSize, float widthOverHeight, bool pass2)
	{
	}


	protected void PrepareBokehKernel(VLDOF vlDof, Span<Vector4> bokehKernel, float maxRadius, float rcpAspect)
	{
	}


	private void SetBokehRenderTarget(CommandBuffer cmd, RTHandle far, RTHandle near, bool blurNearField)
	{
	}


	private bool DrawVLDOFBokeh(VLDOF vlDof, CommandBuffer cmd, ref CameraData cameraData, RTHandle source, RTHandle destination)
	{
		return default(bool);
	}


	public void DoJumpFloodOutline(CommandBuffer cmd, RTHandle source, RTHandle destination, CameraData cameraData)
	{
	}


	public void DisposeJumpFloodOutline()
	{
	}


	public void GrapDistanceFieldTexture(out RTHandle prevDistanceField, out RTHandle nextDistanceField)
	{
            prevDistanceField = default;
            nextDistanceField = default;

	}


	public bool DoVLMotionBlur(CommandBuffer cmd, RTHandle source, RTHandle destination)
	{
		return default(bool);
	}


	public float Exp2(float x)
	{
		return default(float);
	}


	private void SetupVLDiffusion(CommandBuffer cmd, Material material, RTHandle source, RenderTextureDescriptor descriptor)
	{
	}


	private void DoVLAdditive(CommandBuffer cmd, RTHandle source, Camera camera)
	{
	}


	private void SetupVLBloom(CommandBuffer cmd, RTHandle source, VLBloom vlBloom, VLStarStreak vlStarStreak)
	{
	}


	private void DrawStarStreak(CommandBuffer cmd, RTHandle source, RTHandle destination, VLStarStreak vlStarStreak)
	{
	}


	private void DrawStarStreak(CommandBuffer cmd, Vector2 dir, Color color, int iteration, float atten, RTHandle source, RTHandle destination)
	{
	}


	private void SetupVLParaffin(CommandBuffer cmd, float aspect, VLParaffin vlParaffin)
	{
	}


	private bool DrawFlare(CommandBuffer cmd, Material flareMaterial, FlareSetting flare, float baseAspect, float aspect)
	{
		return default(bool);
	}


	private bool DoVLTextureBlur(CommandBuffer cmd, RTHandle source, RTHandle destination)
	{
		return default(bool);
	}


	public void InitializeVirtualEffect()
	{
	}


	public void DisposeVirtualEffect()
	{
	}


	public void DoVLVirtualEffectAdditiveTextureToAdd(CommandBuffer cmd, RTHandle source, Camera camera)
	{
	}


	private void SetupVLVirtualEffect(CommandBuffer cmd, RTHandle source, VLVirtualEffect vlVirtualEffect)
	{
	}


	public void DoVLVirtualEffectAdditiveTextureToBloom(CommandBuffer cmd, RTHandle source, RTHandle destination)
	{
	}


	public void DoVLVirtualEffectNoise(CommandBuffer cmd, RTHandle source, RTHandle destination)
	{
	}


	public void DoVLVirtualEffectChromaticAberration(CommandBuffer cmd, RTHandle source, RTHandle destination)
	{
	}


	public void DoChromaticAberration(CommandBuffer cmd, RTHandle source, RTHandle destination)
	{
	}


	public void DoRGBBlockNoise(CommandBuffer cmd, RTHandle source, RTHandle destination)
	{
	}


	public void DoWaveJitterGlitch(CommandBuffer cmd, RTHandle source, RTHandle destination)
	{
	}
}

}
