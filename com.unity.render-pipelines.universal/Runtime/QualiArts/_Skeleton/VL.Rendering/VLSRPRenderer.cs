// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering {
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.Universal.Internal;
using VL.Rendering;
using VL.Rendering.Internal;

public sealed class VLSRPRenderer : UniversalRenderer
{
	[Serializable]

	[CompilerGenerated]
	private sealed class _003C_003Ec
	{


		public static readonly _003C_003Ec _003C_003E9;


		public static Predicate<ScriptableRenderPass> _003C_003E9__27_0;


		public static Predicate<ScriptableRenderPass> _003C_003E9__27_1;


		public _003C_003Ec()
		{
		}


		internal bool _003CSetup_003Eb__27_0(ScriptableRenderPass x)
		{
			return default(bool);
		}


		internal bool _003CSetup_003Eb__27_1(ScriptableRenderPass x)
		{
			return default(bool);
		}
	}


	private VLMotionVectorRenderPass m_VLMotionVectorPass;


	private VLSkyPass m_DrawSkyboxPass;


	internal VLCameraBufferSystem m_LitColorTexture;


	internal RTHandle m_AdditionalInfoTexture;


	private Material m_SkyMaterial;


	private Material m_FogMaterial;


	private VLPostProcessPasses m_VLPostProcessPasses;


	private VLDeferredPass _vlDeferredPass;


	private VLActorForward m_ActorForward;


	private VLPreDepthIDPass _vlPreDepthIDPass;

	internal new ColorGradingLutPass colorGradingLutPass
	{


		get
		{
			return null;
		}
	}

	internal ColorGradingComputePass colorGradingComputePass
	{


		get
		{
			return null;
		}
	}

	internal new VLPostProcessPass postProcessPass
	{


		get
		{
			return null;
		}
	}

	internal new VLPostProcessPass finalPostProcessPass
	{


		get
		{
			return null;
		}
	}

	internal new RTHandle colorGradingLut
	{


		get
		{
			return null;
		}
	}

	public ForwardLights forwardLights
	{


		get
		{
			return null;
		}
	}


	public VLSRPRenderer(VLSRPRendererData data) : base(data) // TODO(qa-recon): original constructor chaining unavailable.
	{
	}


	protected override void Dispose(bool disposing)
	{
	}


	internal override void ReleaseRenderTargets()
	{
	}


	internal void SetupVLSRPFinalPassDebug(ref CameraData cameraData)
	{
	}


	public override void Setup(ScriptableRenderContext context, ref RenderingData renderingData)
	{
	}


	public override void SetupLights(ScriptableRenderContext context, ref RenderingData renderingData)
	{
	}


	public override void SetupCullingParameters(ref ScriptableCullingParameters cullingParameters, ref CameraData cameraData)
	{
	}


	public override void FinishRendering(CommandBuffer cmd)
	{
	}


	private void EnqueueDeferred(ref RenderingData renderingData, bool hasDepthPrepass, bool hasNormalPrepass, bool hasRenderingLayerPrepass, bool applyMainShadow, bool applyAdditionalShadow)
	{
	}


	public bool IsLitColorRendered(int cameraHash)
	{
		return default(bool);
	}


	public RTHandle GetLitColorTexture(int cameraHash)
	{
		return null;
	}


	public void MarkLitColorTextureAsRendered(int cameraHash)
	{
	}


	public int GetLightCookieShaderDataIndex(int visibleLightIndex)
	{
		return default(int);
	}
}

}
