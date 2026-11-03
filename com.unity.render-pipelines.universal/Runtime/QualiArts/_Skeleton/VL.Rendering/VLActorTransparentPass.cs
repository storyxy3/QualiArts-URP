// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering {
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VLActorTransparentPass : ScriptableRenderPass
{


	private RenderStateBlock _stateBlock;


	private FilteringSettings _filteringSettings;


	private readonly ProfilingSampler _profilingSampler;


	private readonly ShaderTagId _prePassTagId;


	private readonly List<ShaderTagId> _shaderTagIds;


	private readonly ShaderTagId _outlineTagId;


	public VLActorTransparentPass(LayerMask layerMask)
	{
	}


	public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
	{
	}
}

}
