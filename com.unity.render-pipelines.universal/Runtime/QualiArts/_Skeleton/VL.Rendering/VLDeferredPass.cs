// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering {
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.Universal.Internal;
using VL;
using VL.Rendering;

internal class VLDeferredPass : ScriptableRenderPass
{


	public static readonly int _CameraNormalsTexture;


	public static readonly int _CameraDepthTexture;


	private static ShaderTagId s_ShaderTagLit;


	private static ShaderTagId s_ShaderTagSimpleLit;


	private static ShaderTagId s_ShaderTagUnlit;


	private static ShaderTagId s_ShaderTagUniversalGBuffer;


	private static ShaderTagId s_ShaderTagUniversalMaterialType;


	private static ShaderTagId s_ShaderTagMotionVectors;


	private static readonly ProfilingSampler _drawGBufferProfilingSampler;


	private static readonly ProfilingSampler _deferredProfilingSampler;


	private static readonly ProfilingSampler _deferredFogProfilingSampler;


	public static readonly GraphicsFormat[] GBufferFormats;


	public static readonly AttachmentDescriptor[] ColorAttachments;


	public static readonly AttachmentDescriptor DepthAttachment;


	private readonly VLActorGBuffer _actorGBuffer;


	private readonly VLVirtualEffectGBuffer _virtualEffectGBuffer;


	private readonly DrawObjectsPass _forwardOnlyPass;


	private readonly DrawObjectsPass _fullScreenTransparentPass;


	private readonly VLSkyPass _skyPass;


	private readonly Material _fogMaterial;


	private readonly Material _motionVectorObjectMaterial;


	private DeferredLights _deferredLights;


	private CustomRenderPassData _deferredRenderPass;


	private CustomSubPassData[] _deferredSubPassesNoLit;


	private CustomSubPassData[] _deferredSubPasses;


	private ShaderTagId[] _gBufferShaderTagValues;


	private RenderStateBlock[] _gBufferRenderStateBlocks;


	private FilteringSettings m_FilteringSettings;


	private RenderStateBlock m_RenderStateBlock;


	private FilteringSettings m_MotionVectorFilteringSettingsBefore;


	private FilteringSettings m_MotionVectorFilteringSettingsAfter;


	private RenderStateBlock m_MotionVectorRenderStateBlock;


	private RTHandle _cameraNormalsTexture;


	private RTHandle _cameraDepthTexture;


	private RTHandle _additionalInfoTexture;


	public VLDeferredPass(RenderPassEvent evt, LayerMask opaqueLayerMask, LayerMask transparentLayerMask, StencilState stencilState, int stencilReference, DeferredLights deferredLights, VLSkyPass skyPass, DrawObjectsPass forwardOnlyPass, Material fogMaterial, Material motionVectorObjectMaterial)
	{
	}


	public void Setup(RTHandle cameraDepthTexture, RTHandle cameraNormalsTexture, RTHandle additionalInfoTexture)
	{
	}


	private void SetupMotionVector(CommandBuffer cmd, ref CameraData cameraData)
	{
	}


	public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
	{
	}


	public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
	{
	}


	public void ExecuteGBuffer(ScriptableRenderContext context, ref RenderingData renderingData)
	{
	}


	private void RenderGeometry(ScriptableRenderContext context, ref RenderingData renderingData)
	{
	}


	private void RenderActor(ScriptableRenderContext context, ref RenderingData renderingData)
	{
	}


	private void RenderVirtualActor(ScriptableRenderContext context, ref RenderingData renderingData)
	{
	}


	private void RenderAdditionalInfo(ScriptableRenderContext context, ref RenderingData renderingData)
	{
	}


	private void DrawObjectMotionVectors(ScriptableRenderContext context, ref RenderingData renderingData, Material objectMaterial)
	{
	}


	private void RenderPreEmission(ScriptableRenderContext context, ref RenderingData renderingData)
	{
	}


	private void RenderEmission(ScriptableRenderContext context, ref RenderingData renderingData)
	{
	}


	private void RenderFog(CommandBuffer cmd)
	{
	}


	public override void OnCameraCleanup(CommandBuffer cmd)
	{
	}
}

}
