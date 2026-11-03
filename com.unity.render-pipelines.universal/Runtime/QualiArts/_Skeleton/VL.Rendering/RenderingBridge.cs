// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering {
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using VL.Rendering;

public static class RenderingBridge
{

	public enum EventType
	{

		PreDepth,

		GBuffer,

		MotionVector,

		LitLighting,

		AfterGBuffer,

		ActorDecal,

		Last
	}

	public delegate void RenderEvent(ScriptableRenderContext context, ref RenderingData renderingData);


	private static readonly List<RenderEvent>[] _renderEvents;


	internal static void Execute(EventType type, ScriptableRenderContext context, ref RenderingData renderingData)
	{
	}


	internal static void ClearEvents()
	{
	}


	public static void AddRenderEvent(EventType type, RenderEvent renderEvent)
	{
	}
}

}
