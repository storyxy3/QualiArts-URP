// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering {
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VLVirtualEffectGBuffer
{


	private readonly ShaderTagId _virtualTagId;


	private readonly ShaderTagId _virtualHairTagId;


	private readonly ShaderTagId _virtualOutlineTagId;


	private readonly ShaderTagId _preDepthTagId;


	private FilteringSettings _baseFilteringSettings;


	private FilteringSettings _hairFilteringSettings;


	private static RenderStateBlock renderStateActor;

	public static RenderQueueRange VirtualEffectRangeBefore
	{


		get
		{
			return default(RenderQueueRange);
		}
	}

	public static RenderQueueRange VirtualEffectRangeFaceParts
	{


		get
		{
			return default(RenderQueueRange);
		}
	}

	public static RenderQueueRange VirtualEffectRangeAfter
	{


		get
		{
			return default(RenderQueueRange);
		}
	}


	public VLVirtualEffectGBuffer(LayerMask layerMask)
	{
	}


	private DrawingSettings CreateDrawingSettings(ShaderTagId shaderTagId, ref RenderingData renderingData, SortingCriteria sortingCriteria, bool useMotionVector)
	{
		return default(DrawingSettings);
	}


	public void ExecuteVirtual(ScriptableRenderContext context, ref RenderingData renderingData, bool useMotionVector)
	{
	}


	public void ExecuteVirtualHair(ScriptableRenderContext context, ref RenderingData renderingData, bool useMotionVector)
	{
	}


	public void ExecutePreDepth(ScriptableRenderContext context, ref RenderingData renderingData)
	{
	}
}

}
