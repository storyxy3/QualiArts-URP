// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering {
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using VL.Rendering;

public class VLSkyPass : ScriptableRenderPass
{


	public static readonly int _MainTex;


	public static readonly int _Tint;


	public static readonly int _Exposure;


	public static readonly int _Rotation;


	public static readonly int _PixelCoordToViewDirWS;


	public static readonly int _SkyParam;


	public static readonly int _CloudColor;


	private int _skyPass;


	private Material _skyMaterial;


	private MaterialPropertyBlock _materialPropertyBlock;


	private MaterialPropertyBlock _cloudMaterialPropertyBlock;


	public VLSkyPass(RenderPassEvent evt, Material skyMaterial, int pass = 0)
	{
	}


	public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
	{
	}


	internal static Vector4 GetScaledScreenSize(ref CameraData cameraData)
	{
		return default(Vector4);
	}


	internal static Matrix4x4 ComputePixelCoordToWorldSpaceViewDirectionMatrix(Camera camera, Vector4 resolution, in Matrix4x4 viewMatrix, in Matrix4x4 gpuProj)
	{
		return default(Matrix4x4);
	}


	internal static Matrix4x4 ComputePixelCoordToWorldSpaceViewDirectionMatrix(float verticalFoV, Vector2 lensShift, Vector4 screenSize, Matrix4x4 worldToViewMatrix, bool renderToCubemap, float aspectRatio = -1f, bool isOrthographic = false)
	{
		return default(Matrix4x4);
	}


	private void DrawClouds(CommandBuffer cmd, VLCloud vlCloud)
	{
	}
}

}
