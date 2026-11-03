// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering {
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VLActorGBuffer
{


	private readonly ShaderTagId _actorTagId;


	private readonly ShaderTagId _hairTagId;


	private readonly ShaderTagId _outlineTagId;


	private readonly ShaderTagId _forwardTagId;


	private FilteringSettings _baseFilteringSettings;


	private FilteringSettings _hairFilteringSettings;


	private FilteringSettings _allFilteringSettings;


	private FilteringSettings _transparentFilteringSettings;


	private static RenderStateBlock renderStateActor;


	public VLActorGBuffer(LayerMask layerMask)
	{
	}


	private DrawingSettings CreateDrawingSettings(ShaderTagId shaderTagId, ref RenderingData renderingData, SortingCriteria sortingCriteria, bool useMotionVector)
	{
		return default(DrawingSettings);
	}


	public void ExecuteBase(ScriptableRenderContext context, ref RenderingData renderingData, bool useMotionVector)
	{
	}


	public void ExecuteHair(ScriptableRenderContext context, ref RenderingData renderingData, bool useMotionVector)
	{
	}


	public void ExecuteTransparent(ScriptableRenderContext context, ref RenderingData renderingData)
	{
	}
}

}
