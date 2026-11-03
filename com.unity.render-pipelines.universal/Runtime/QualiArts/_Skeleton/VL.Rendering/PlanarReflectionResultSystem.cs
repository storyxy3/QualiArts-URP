// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering {
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using VL.Rendering;

public class PlanarReflectionResultSystem
{

	[BurstCompile]
	public struct ResultJob : IJob
	{


		public float4x4 cameraViewMatrix;


		public float3 worldSpaceCameraPos;


		public int count;


		public bool isFullResolution;


		public NativeArray<int> visibleIndices;


		[ReadOnly]
		public NativeArray<float3> positions;


		[ReadOnly]
		public NativeArray<quaternion> rotations;


		[ReadOnly]
		public NativeArray<float3> sizes;


		[ReadOnly]
		public NativeArray<float4x4> localToWorldMatrices;


		[ReadOnly]
		public NativeArray<float4> reflectionPlanes;


		[ReadOnly]
		public NativeArray<bool> fullResolutions;


		[WriteOnly]
		public NativeArray<float4x4> maskMeshDrawMatrices;


		[WriteOnly]
		public NativeArray<float4> clipPlanes;


		[WriteOnly]
		public NativeArray<float4x4> worldToTangentMatrices;


		[WriteOnly]
		public NativeArray<float4x4> reflectionMatrices;


		[WriteOnly]
		public NativeArray<float4x4> reflectionViewMatrices;


		[WriteOnly]
		public NativeArray<float3> reflectionCameraPositions;


		[WriteOnly]
		public NativeArray<int> culledCount;


		public void Execute()
		{
		}


		private void UpdateParams(int index, int culledIndex)
		{
		}


		private float4x4 GetReflectionMatrix(float4 reflectionPlane)
		{
			return default(float4x4);
		}


		private float4 CameraSpacePlane(float4x4 viewMatrix, float3 pos, float3 normal, float sideSign)
		{
			return default(float4);
		}
	}


	private readonly PlanarReflectionEntityManager _entityManager;


	private readonly ProfilingSampler _sampler;


	private readonly bool _isFullResolution;


	public PlanarReflectionResultSystem(PlanarReflectionEntityManager entityManager, bool isFullResolution)
	{
	}


	public void Execute(Vector3 cameraPosition, Matrix4x4 viewMatrix)
	{
	}


	private void Execute(PlanarReflectionCachedChunk cachedChunk, PlanarReflectionCulledChunk culledChunk, PlanarReflectionResultChunk resultChunk, Vector3 cameraPosition, in Matrix4x4 viewMatrix)
	{
	}
}

}
