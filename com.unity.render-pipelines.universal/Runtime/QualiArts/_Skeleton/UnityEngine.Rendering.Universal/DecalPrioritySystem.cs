// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace UnityEngine.Rendering.Universal {
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

internal class DecalPrioritySystem
{

	[BurstCompile]
	private struct PrioritySortJob : IJob
	{


		[ReadOnly]
		public NativeArray<int> layerMasks;


		[ReadOnly]
		public NativeArray<int> priorities;


		public NativeArray<uint> sortIndices;


		public NativeArray<int> visibleDecalIndices;


		public int visibleDecalCount;


		public void Execute()
		{
		}
	}


	private DecalEntityManager m_EntityManager;


	private ProfilingSampler m_Sampler;


	public DecalPrioritySystem(DecalEntityManager entityManager)
	{
	}


	public void Execute()
	{
	}


	private void Execute(DecalCachedChunk cachedChunk, DecalCulledChunk culledChunk)
	{
	}
}

}
