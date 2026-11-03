// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering {
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using VL;
using VL.Rendering;

public class SubWorldPass : ScriptableRenderPass
{


	public static ShaderTagId SubWorldGBuffer;


	private readonly ProfilingSampler _profilingSampler;


	private readonly ProfilingSampler _maskProfilingSampler;


	private readonly ProfilingSampler _emissionProfilingSampler;


	private RTHandle _subWorldBuffer;


	private Material _subWorldMaterial;


	private SubWorldVolume _subWorld;


	private bool _isDeferredRenderingMode;


	private RenderStateBlock _maskRenderStateBlock;


	private RenderStateBlock _subWorldGBufferRenderState;


	private Matrix4x4 _reflectionMatrix;


	private Matrix4x4 _projectionMatrix;


	private CustomRenderPassData _renderPass;


	private CustomSubPassData[] _subPasses;


	private VLSkyPass _skyPass;


	public SubWorldPass(Material subWorldMaterial)
	{
	}


	public void Dispose()
	{
	}


	public void Setup(SubWorldVolume volume, bool isDeferredRenderingMode)
	{
	}


	public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
	{
	}


	public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
	{
	}


	private void RenderSubWorld(ScriptableRenderContext context, ref RenderingData renderingData)
	{
	}
}

}
