using Unity.Collections;
using Unity.Jobs;

namespace UnityEngine.Rendering.Universal
{
    internal class DecalPrioritySystem
    {
        private DecalEntityManager m_EntityManager;
        private ProfilingSampler m_Sampler;

        public DecalPrioritySystem(DecalEntityManager entityManager)
        {
            m_EntityManager = entityManager;
            m_Sampler = new ProfilingSampler("DecalPrioritySystem.Execute");
        }

        public void Execute()
        {
            using (new ProfilingScope(null, m_Sampler))
                for (int i = 0; i < m_EntityManager.chunkCount; ++i)
                    Execute(m_EntityManager.cachedChunks[i], m_EntityManager.culledChunks[i]);
        }

        private void Execute(DecalCachedChunk cachedChunk, DecalCulledChunk culledChunk)
        {
            if (culledChunk.visibleDecalCount < 2)
                return;
            var job = new PrioritySortJob
            {
                layerMasks = cachedChunk.layerMasks,
                priorities = cachedChunk.priorities,
                sortIndices = culledChunk.sortIndices,
                visibleDecalIndices = culledChunk.visibleDecalIndices,
                visibleDecalCount = culledChunk.visibleDecalCount
            };
            cachedChunk.currentJobHandle = job.Schedule(cachedChunk.currentJobHandle);
        }

        private struct PrioritySortJob : IJob
        {
            [ReadOnly] public NativeArray<int> layerMasks;
            [ReadOnly] public NativeArray<int> priorities;
            public NativeArray<uint> sortIndices;
            public NativeArray<int> visibleDecalIndices;
            public int visibleDecalCount;

            public void Execute()
            {
                for (int i = 0; i < visibleDecalCount; ++i)
                {
                    int index = visibleDecalIndices[i];
                    sortIndices[i] = (((uint)index & 0xff) | ((uint)priorities[index] << 8)) + 12800u
                        | ((uint)layerMasks[index] << 16);
                }
                CoreUnsafeUtils.InsertionSort(sortIndices, visibleDecalCount);
                for (int i = 0; i < visibleDecalCount; ++i)
                    visibleDecalIndices[i] = (int)(sortIndices[i] & 0xff);
            }
        }
    }
}
