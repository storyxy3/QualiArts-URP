using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
namespace VL.Rendering
{
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
                int culledIndex = 0;
                for (int i = 0; i < count; ++i)
                {
                    int index = visibleIndices[i];
                    if (index >= 0 && fullResolutions[i] == isFullResolution)
                    {
                        UpdateParams(index, culledIndex);
                        visibleIndices[culledIndex++] = index;
                    }
                }
                culledCount[0] = culledIndex;
            }
            private void UpdateParams(int index, int culledIndex)
            {
                float3 position = positions[index];
                quaternion rotation = rotations[index];
                float4 plane = reflectionPlanes[index];
                float4x4 reflection = GetReflectionMatrix(plane);
                float3 tangent = math.mul(rotation, new float3(1, 0, 0));
                float3 binormal = math.mul(rotation, new float3(0, 0, 1));
                float4x4 view = math.mul(cameraViewMatrix, reflection);
                float4 clipPlane = CameraSpacePlane(view, position, plane.xyz, 1);
                maskMeshDrawMatrices[culledIndex] = math.mul(localToWorldMatrices[index], float4x4.Scale(sizes[index]));
                worldToTangentMatrices[culledIndex] = math.inverse(new float4x4(
                    tangent.x, tangent.y, tangent.z, 0,
                    binormal.x, binormal.y, binormal.z, 0,
                    plane.x, plane.y, plane.z, 0,
                    0, 0, 0, 1));
                clipPlanes[culledIndex] = clipPlane;
                reflectionMatrices[culledIndex] = reflection;
                reflectionViewMatrices[culledIndex] = view;
                reflectionCameraPositions[culledIndex] = math.transform(reflection, worldSpaceCameraPos);
            }
            private float4x4 GetReflectionMatrix(float4 reflectionPlane)
            {
                return new float4x4(
                    1 - 2 * reflectionPlane[0] * reflectionPlane[0], -2 * reflectionPlane[0] * reflectionPlane[1], -2 * reflectionPlane[0] * reflectionPlane[2], -2 * reflectionPlane[3] * reflectionPlane[0],
                    -2 * reflectionPlane[1] * reflectionPlane[0], 1 - 2 * reflectionPlane[1] * reflectionPlane[1], -2 * reflectionPlane[1] * reflectionPlane[2], -2 * reflectionPlane[3] * reflectionPlane[1],
                    -2 * reflectionPlane[2] * reflectionPlane[0], -2 * reflectionPlane[2] * reflectionPlane[1], 1 - 2 * reflectionPlane[2] * reflectionPlane[2], -2 * reflectionPlane[3] * reflectionPlane[2],
                    0, 0, 0, 1);
            }
            private float4 CameraSpacePlane(float4x4 viewMatrix, float3 pos, float3 normal, float sideSign)
            {
                float3 offsetPos = pos + normal * 0.01f;
                float3 cameraPos = math.transform(viewMatrix, offsetPos);
                float3 cameraNormal = math.normalize(math.rotate(viewMatrix, normal)) * sideSign;
                return new float4(cameraNormal, -math.dot(cameraPos, cameraNormal));
            }
        }
        private readonly PlanarReflectionEntityManager _entityManager;
        private readonly ProfilingSampler _sampler;
        private readonly bool _isFullResolution;
        public PlanarReflectionResultSystem(PlanarReflectionEntityManager entityManager, bool isFullResolution)
        {
            _entityManager = entityManager;
            _sampler = new ProfilingSampler("PlanarReflection.Result");
            _isFullResolution = isFullResolution;
        }
        public void Execute(Vector3 cameraPosition, Matrix4x4 viewMatrix)
        {
            using (new ProfilingScope(null, _sampler))
            {
                if (_entityManager.combinedChunks == null) return;
                Execute(_entityManager.combinedChunks.cachedChunk, _entityManager.combinedChunks.culledChunk, _entityManager.combinedChunks.resultChunk, cameraPosition, in viewMatrix);
            }
        }
        private void Execute(PlanarReflectionCachedChunk cachedChunk, PlanarReflectionCulledChunk culledChunk, PlanarReflectionResultChunk resultChunk, Vector3 cameraPosition, in Matrix4x4 viewMatrix)
        {
            if (cachedChunk.count == 0) return;
            var job = new ResultJob {
                cameraViewMatrix = viewMatrix, worldSpaceCameraPos = cameraPosition, count = cachedChunk.count, isFullResolution = _isFullResolution,
                visibleIndices = culledChunk.visibleIndices, positions = cachedChunk.positions, rotations = cachedChunk.rotations, sizes = cachedChunk.sizes,
                localToWorldMatrices = cachedChunk.localToWorldMatrices, reflectionPlanes = cachedChunk.reflectionPlanes, fullResolutions = cachedChunk.fullResolutions,
                maskMeshDrawMatrices = resultChunk.maskMeshMatrices, clipPlanes = resultChunk.clipPlanes, worldToTangentMatrices = resultChunk.worldToTangentMatrices,
                reflectionMatrices = resultChunk.reflectionMatrices, reflectionViewMatrices = resultChunk.reflectionViewMatrices, reflectionCameraPositions = resultChunk.reflectionCameraPositions,
                culledCount = culledChunk.culledCount
            };
            var handle = job.Schedule(culledChunk.currentJobHandle);
            cachedChunk.currentJobHandle = handle;
            culledChunk.currentJobHandle = handle;
            resultChunk.currentJobHandle = handle;
        }
    }
}
