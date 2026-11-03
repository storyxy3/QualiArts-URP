// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering {
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using VL;

public class VLPreDepthIDPass : ScriptableRenderPass
{


	private static readonly ShaderTagId _depthIDShaderTagId;


	private static readonly int _CameraNormalsTexture;


	private static readonly int _CameraDepthTexture;


	private RTHandle _depthTextureHandle;


	private RTHandle _normalAttachmentHandle;


	private FilteringSettings _filteringSettings;


	private CustomRenderPassData _renderPassData;


	private CustomSubPassData[] _subPassData;


	public VLPreDepthIDPass(RenderPassEvent evt, RenderQueueRange renderQueueRange, LayerMask layerMask)
	{
	}


	public void Setup(RTHandle depthTextureHandle, RTHandle normalAttachmentHandle)
	{
	}


	public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
	{
	}


	private void OnRenderDepthAndNormal(ScriptableRenderContext context, ref RenderingData renderingData)
	{
	}


	public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
	{
	}
}

}
