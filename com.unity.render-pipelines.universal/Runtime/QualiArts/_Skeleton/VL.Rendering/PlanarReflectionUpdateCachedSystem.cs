// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering {
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.Rendering;
using VL.Rendering;

public class PlanarReflectionUpdateCachedSystem
{

	[BurstCompile]
	public struct UpdateJob : IJobParallelForTransform
	{


		[ReadOnly]
		public NativeArray<Bounds> localBounds;


		[WriteOnly]
		public NativeArray<float3> positions;


		[WriteOnly]
		public NativeArray<quaternion> rotations;


		[WriteOnly]
		public NativeArray<float4x4> localToWorldMatrices;


		[WriteOnly]
		public NativeArray<Bounds> bounds;


		[WriteOnly]
		public NativeArray<float4> reflectionPlanes;


		public void Execute(int index, TransformAccess transform)
		{
		}
	}


	private PlanarReflectionEntityManager _entityManager;


	private ProfilingSampler _sampler;


	private ProfilingSampler _samplerJob;


	public PlanarReflectionUpdateCachedSystem(PlanarReflectionEntityManager entityManager)
	{
	}


	public void Execute()
	{
	}


	private void Execute(PlanarReflectionEntityChunk entityChunk, PlanarReflectionCachedChunk cachedChunk)
	{
	}
}

}
