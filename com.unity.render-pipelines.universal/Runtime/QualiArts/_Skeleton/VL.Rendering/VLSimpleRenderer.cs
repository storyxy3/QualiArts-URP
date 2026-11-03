// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering {
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using VL.Rendering;

public class VLSimpleRenderer : ScriptableRenderer
{

	private class SimpleFinalBlitPass : ScriptableRenderPass
	{


		private Material m_BlitMaterial;


		public SimpleFinalBlitPass(Material blitMaterial)
		{
		}


		public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
		}
	}


	private static readonly ProfilingSampler _profilingSampler;


	private RTHandle m_CameraColorAttachment;


	private SimpleFinalBlitPass m_FinalBlitPass;


	private CapturePass m_CapturePass;


	private Material m_BlitMaterial;


	private Material m_BlitHDRMaterial;


	private Material m_CopyDepthMaterial;


	public VLSimpleRenderer(VLSimpleRendererData data) : base(data) // TODO(qa-recon): original constructor chaining unavailable.
	{
	}


	protected override void Dispose(bool disposing)
	{
	}


	internal override void ReleaseRenderTargets()
	{
	}


	public override void FinishRendering(CommandBuffer cmd)
	{
	}


	public override void Setup(ScriptableRenderContext context, ref RenderingData renderingData)
	{
	}


	public override void SetupCullingParameters(ref ScriptableCullingParameters cullingParameters, ref CameraData cameraData)
	{
	}


	internal override void SwapColorBuffer(CommandBuffer cmd)
	{
	}
}

}
