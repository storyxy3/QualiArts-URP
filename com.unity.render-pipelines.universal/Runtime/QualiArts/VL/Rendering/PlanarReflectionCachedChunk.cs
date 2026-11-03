using System;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using VL.EntitySystem;
namespace VL.Rendering
{
    public class PlanarReflectionCachedChunk : Chunk
    {
        public Mesh[] meshes;
        public NativeArray<PlanarReflection.ShaderTagIdList> additionalShaderTagIdLists;
        public NativeArray<FilteringSettings> filteringSettings;
        public NativeArray<float3> positions;
        public NativeArray<quaternion> rotations;
        public NativeArray<float3> sizes;
        public NativeArray<float4x4> localToWorldMatrices;
        public NativeArray<Bounds> localBounds;
        public NativeArray<Bounds> bounds;
        public NativeArray<float4> reflectionPlanes;
        public NativeArray<float4x4> worldToTangentMatrices;
        public NativeArray<bool> fullResolutions;
        public PlanarReflectionCachedChunk() : base() { }

        public override void RemoveAtSwapBack(int entityIndex)
        {
            RemoveAtSwapBack(ref meshes, entityIndex, count);
            RemoveAtSwapBack(ref additionalShaderTagIdLists, entityIndex, count);
            RemoveAtSwapBack(ref filteringSettings, entityIndex, count);
            RemoveAtSwapBack(ref positions, entityIndex, count);
            RemoveAtSwapBack(ref rotations, entityIndex, count);
            RemoveAtSwapBack(ref sizes, entityIndex, count);
            RemoveAtSwapBack(ref localToWorldMatrices, entityIndex, count);
            RemoveAtSwapBack(ref localBounds, entityIndex, count);
            RemoveAtSwapBack(ref bounds, entityIndex, count);
            RemoveAtSwapBack(ref reflectionPlanes, entityIndex, count);
            RemoveAtSwapBack(ref worldToTangentMatrices, entityIndex, count);
            RemoveAtSwapBack(ref fullResolutions, entityIndex, count);
            --count;
        }
        public override void SetCapacity(int newCapacity)
        {
            Array.Resize(ref meshes, newCapacity);
            ArrayExtensions.ResizeArray(ref additionalShaderTagIdLists, newCapacity);
            ArrayExtensions.ResizeArray(ref filteringSettings, newCapacity);
            ArrayExtensions.ResizeArray(ref positions, newCapacity);
            ArrayExtensions.ResizeArray(ref rotations, newCapacity);
            ArrayExtensions.ResizeArray(ref sizes, newCapacity);
            ArrayExtensions.ResizeArray(ref localToWorldMatrices, newCapacity);
            ArrayExtensions.ResizeArray(ref localBounds, newCapacity);
            ArrayExtensions.ResizeArray(ref bounds, newCapacity);
            ArrayExtensions.ResizeArray(ref reflectionPlanes, newCapacity);
            ArrayExtensions.ResizeArray(ref worldToTangentMatrices, newCapacity);
            ArrayExtensions.ResizeArray(ref fullResolutions, newCapacity);
            capacity = newCapacity;
        }
        public override void Dispose()
        {
            if (capacity == 0) return;
            meshes = null;
            additionalShaderTagIdLists.Dispose();
            filteringSettings.Dispose();
            positions.Dispose();
            rotations.Dispose();
            sizes.Dispose();
            localToWorldMatrices.Dispose();
            localBounds.Dispose();
            bounds.Dispose();
            reflectionPlanes.Dispose();
            worldToTangentMatrices.Dispose();
            fullResolutions.Dispose();
            count = 0;
            capacity = 0;
        }
    }
}
