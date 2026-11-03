// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering {
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public sealed class VLMotionVectorRenderPass : MotionVectorRenderPass
{


	internal static readonly int kPreviousWorldCameraPosition;


	internal static readonly int kPreviousWorldCameraPositionStereo;


	public static readonly ShaderTagId motionVectorTagId;


	public static readonly ShaderTagId outlineMotionVectorTagId;


	internal VLMotionVectorRenderPass(RenderPassEvent evt, Material cameraMaterial, Material objectMaterial, LayerMask opaqueLayerMask) : base(evt, cameraMaterial, objectMaterial, opaqueLayerMask) // TODO(qa-recon): original constructor chaining unavailable.
	{
	}


	private static void ExecutePass(ScriptableRenderContext context, PassData passData, ref RenderingData renderingData)
	{
	}


	public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
	{
	}


	private static void DrawObjectMotionVectors(ScriptableRenderContext context, ref RenderingData renderingData, Camera camera, Material objectMaterial, CommandBuffer cmd, ref FilteringSettings filteringSettings)
	{
	}
}

}
