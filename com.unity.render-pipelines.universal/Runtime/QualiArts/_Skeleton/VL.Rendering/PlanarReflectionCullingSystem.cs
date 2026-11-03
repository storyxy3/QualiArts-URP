// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering {
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using VL.Rendering;

public class PlanarReflectionCullingSystem
{

	[BurstCompile]
	private struct CullingJob : IJobParallelFor
	{


		public float3 cameraPosition;


		public CameraPlanes planes;


		[ReadOnly]
		public NativeArray<float3> positions;


		[ReadOnly]
		public NativeArray<float4> reflectionPlanes;


		[ReadOnly]
		public NativeArray<Bounds> bounds;


		[WriteOnly]
		public NativeArray<int> visibleIndices;


		public void Execute(int index)
		{
		}
	}


	private PlanarReflectionEntityManager _entityManager;


	private ProfilingSampler _sampler;


	public PlanarReflectionCullingSystem(PlanarReflectionEntityManager entityManager)
	{
	}


	public void Execute(Vector3 cameraPosition, Matrix4x4 vpMatrix)
	{
	}


	private void Execute(PlanarReflectionCachedChunk cachedChunk, PlanarReflectionCulledChunk culledChunk, Vector3 cameraPosition, ref CameraPlanes planes)
	{
	}
}

}
