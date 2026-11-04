// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering.Internal {
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.Universal.Internal;
using VL.Rendering;
using VL.Rendering.Internal;

internal struct VLPostProcessPasses : IDisposable
{


	private ColorGradingLutPass m_ColorGradingLutPass;


	private ColorGradingComputePass m_ColorGradingComputePass;


	private VLPostProcessPass m_PostProcessPass;


	private VLPostProcessPass m_FinalPostProcessPass;


	internal RTHandle m_AfterPostProcessColor;


	internal RTHandle m_ColorGradingLut;


	private VLPostProcessData m_RendererPostProcessData;


	private VLPostProcessData m_CurrentPostProcessData;


	private Material m_BlitMaterial;


	private bool m_ComputeGradingAvailable;

	public ColorGradingLutPass colorGradingLutPass => m_ColorGradingLutPass;

	public ColorGradingComputePass colorGradingComputePass => m_ColorGradingComputePass;

	public VLPostProcessPass postProcessPass => m_PostProcessPass;

	public VLPostProcessPass finalPostProcessPass => m_FinalPostProcessPass;

	public RTHandle colorGradingLut => m_ColorGradingLut;

	public bool isCreated
	{


		get
		{
			return default(bool);
		}
	}


	public VLPostProcessPasses(VLPostProcessData rendererPostProcessData, ref PostProcessParams postProcessParams) : this()
	{
	}


	public void Recreate(VLPostProcessData data, ref PostProcessParams ppParams)
	{
	}


	public void Dispose()
	{
	}


	internal void ReleaseRenderTargets()
	{
	}
}

}
