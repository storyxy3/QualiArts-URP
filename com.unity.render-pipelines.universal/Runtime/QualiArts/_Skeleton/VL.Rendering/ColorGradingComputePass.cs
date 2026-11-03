// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering {
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using VL.Rendering;

public class ColorGradingComputePass : ScriptableRenderPass
{

	private class PassData
	{


		internal ProfilingSampler sampler;


		internal ComputeShader lutBuilder;


		internal HableCurve hableCurve;


		internal bool allowColorGradingACESHDR;


		public PassData()
		{
		}
	}

	private static class ShaderConstants
	{


		public static readonly int _Lut_Params;


		public static readonly int _ColorBalance;


		public static readonly int _ColorFilter;


		public static readonly int _ChannelMixerRed;


		public static readonly int _ChannelMixerGreen;


		public static readonly int _ChannelMixerBlue;


		public static readonly int _HueSatCon;


		public static readonly int _Lift;


		public static readonly int _Gamma;


		public static readonly int _Gain;


		public static readonly int _Shadows;


		public static readonly int _Midtones;


		public static readonly int _Highlights;


		public static readonly int _ShaHiLimits;


		public static readonly int _SplitShadows;


		public static readonly int _SplitHighlights;


		public static readonly int _CurveMaster;


		public static readonly int _CurveRed;


		public static readonly int _CurveGreen;


		public static readonly int _CurveBlue;


		public static readonly int _CurveHueVsHue;


		public static readonly int _CurveHueVsSat;


		public static readonly int _CurveLumVsSat;


		public static readonly int _CurveSatVsSat;


		public static readonly int _OutputTexture;


		public static readonly int _Size;


		public static readonly int _Params;


		public static readonly int _CustomToneCurve;


		public static readonly int _ToeSegmentA;


		public static readonly int _ToeSegmentB;


		public static readonly int _MidSegmentA;


		public static readonly int _MidSegmentB;


		public static readonly int _ShoSegmentA;


		public static readonly int _ShoSegmentB;


		public static readonly int _LogLut3D;


		public static readonly int _LogLut3D_Params;


		public static readonly int _gtParamsA;
	}


	private readonly ComputeShader _lutCompute;


	private readonly GraphicsFormat _lutFormat;


	private PassData m_PassData;


	private RTHandle m_InternalLut;


	private bool m_AllowColorGradingACESHDR;


	public static bool IsAvailable(VLPostProcessData data)
	{
		return default(bool);
	}


	public ColorGradingComputePass(RenderPassEvent evt, VLPostProcessData data)
	{
	}


	public void Setup(in RTHandle internalLut)
	{
	}


	public void ConfigureDescriptor(in PostProcessingData postProcessingData, out RenderTextureDescriptor descriptor, out FilterMode filterMode)
	{
            descriptor = default;
            filterMode = default;

	}


	public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
	{
	}


	private static void ExecutePass(ScriptableRenderContext context, PassData passData, ref RenderingData renderingData, RTHandle internalLutTarget)
	{
	}
}

}
