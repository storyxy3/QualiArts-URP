using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.Rendering;
namespace VL.Rendering
{
    public class PlanarReflectionUpdateCachedSystem
    {
        [BurstCompile]
        public struct UpdateJob : IJobParallelForTransform
        {
            [ReadOnly] public NativeArray<Bounds> localBounds;
            [WriteOnly] public NativeArray<float3> positions;
            [WriteOnly] public NativeArray<quaternion> rotations;
            [WriteOnly] public NativeArray<float4x4> localToWorldMatrices;
            [WriteOnly] public NativeArray<Bounds> bounds;
            [WriteOnly] public NativeArray<float4> reflectionPlanes;
            public void Execute(int index, TransformAccess transform)
            {
                if (!transform.isValid) return;
                transform.GetPositionAndRotation(out Vector3 position, out Quaternion rotation);
                Matrix4x4 localToWorldMatrix = transform.localToWorldMatrix;
                bounds[index] = AABBUtility.TransformAABB(localBounds[index], localToWorldMatrix);
                float3 normal = math.mul((quaternion)rotation, new float3(0, 1, 0));
                float distance = -math.dot(normal, (float3)position);
                positions[index] = position;
                rotations[index] = rotation;
                localToWorldMatrices[index] = localToWorldMatrix;
                reflectionPlanes[index] = new float4(normal, distance);
            }
        }
        private PlanarReflectionEntityManager _entityManager;
        private ProfilingSampler _sampler;
        private ProfilingSampler _samplerJob;
        public PlanarReflectionUpdateCachedSystem(PlanarReflectionEntityManager entityManager)
        {
            _entityManager = entityManager;
            _sampler = new ProfilingSampler("PlanarReflection.Update");
            _samplerJob = new ProfilingSampler("PlanarReflection.UpdateJob");
        }
        public void Execute()
        {
            using (new ProfilingScope(null, _sampler))
            {
                if (_entityManager.combinedChunks == null) return;
                Execute(_entityManager.combinedChunks.entityChunk, _entityManager.combinedChunks.cachedChunk);
            }
        }
        private void Execute(PlanarReflectionEntityChunk entityChunk, PlanarReflectionCachedChunk cachedChunk)
        {
            if (entityChunk.count == 0) return;
            cachedChunk.currentJobHandle.Complete();
            using (new ProfilingScope(null, _samplerJob))
            {
                var job = new UpdateJob {
                    localBounds = cachedChunk.localBounds, positions = cachedChunk.positions, rotations = cachedChunk.rotations,
                    localToWorldMatrices = cachedChunk.localToWorldMatrices, bounds = cachedChunk.bounds, reflectionPlanes = cachedChunk.reflectionPlanes
                };
                cachedChunk.currentJobHandle = job.ScheduleReadOnly(entityChunk.transformAccessArray, 16);
            }
        }
    }
}
