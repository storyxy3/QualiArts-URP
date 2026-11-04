using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
namespace VL.Rendering
{
    public class PlanarReflectionCullingSystem
    {
        [BurstCompile]
        private struct CullingJob : IJobParallelFor
        {
            public float3 cameraPosition;
            public CameraPlanes planes;
            [ReadOnly] public NativeArray<float3> positions;
            [ReadOnly] public NativeArray<float4> reflectionPlanes;
            [ReadOnly] public NativeArray<Bounds> bounds;
            [WriteOnly] public NativeArray<int> visibleIndices;
            public void Execute(int index) { visibleIndices[index] = planes.TestAABB(bounds[index]) ? index : -1; }
        }
        private PlanarReflectionEntityManager _entityManager;
        private ProfilingSampler _sampler;
        public PlanarReflectionCullingSystem(PlanarReflectionEntityManager entityManager)
        {
            _entityManager = entityManager;
            _sampler = new ProfilingSampler("PlanarReflection.Cull");
        }
        public void Execute(Vector3 cameraPosition, Matrix4x4 vpMatrix)
        {
            using (new ProfilingScope(null, _sampler))
            {
                var planes = new CameraPlanes(vpMatrix);
                if (_entityManager.combinedChunks == null) return;
                Execute(_entityManager.combinedChunks.cachedChunk, _entityManager.combinedChunks.culledChunk, cameraPosition, ref planes);
            }
        }
        private void Execute(PlanarReflectionCachedChunk cachedChunk, PlanarReflectionCulledChunk culledChunk, Vector3 cameraPosition, ref CameraPlanes planes)
        {
            if (cachedChunk.count == 0) return;
            var job = new CullingJob {
                cameraPosition = cameraPosition, planes = planes, positions = cachedChunk.positions,
                reflectionPlanes = cachedChunk.reflectionPlanes, bounds = cachedChunk.bounds, visibleIndices = culledChunk.visibleIndices
            };
            var handle = job.Schedule(cachedChunk.count, 16, cachedChunk.currentJobHandle);
            culledChunk.currentJobHandle = handle;
            cachedChunk.currentJobHandle = handle;
        }
    }
}
