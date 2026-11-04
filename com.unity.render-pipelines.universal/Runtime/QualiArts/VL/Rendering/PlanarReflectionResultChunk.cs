using System;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using VL.EntitySystem;
namespace VL.Rendering
{
    public class PlanarReflectionResultChunk : Chunk
    {
        public NativeArray<float4x4> maskMeshMatrices;
        public NativeArray<float4x4> worldToTangentMatrices;
        public NativeArray<float4> clipPlanes;
        public NativeArray<float4x4> reflectionMatrices;
        public NativeArray<float4x4> reflectionViewMatrices;
        public NativeArray<float3> reflectionCameraPositions;
        public override void RemoveAtSwapBack(int entityIndex)
        {
            RemoveAtSwapBack(ref maskMeshMatrices, entityIndex, count);
            RemoveAtSwapBack(ref worldToTangentMatrices, entityIndex, count);
            RemoveAtSwapBack(ref clipPlanes, entityIndex, count);
            RemoveAtSwapBack(ref reflectionMatrices, entityIndex, count);
            RemoveAtSwapBack(ref reflectionViewMatrices, entityIndex, count);
            RemoveAtSwapBack(ref reflectionCameraPositions, entityIndex, count);
            --count;
        }
        public override void SetCapacity(int newCapacity)
        {
            ArrayExtensions.ResizeArray(ref maskMeshMatrices, newCapacity);
            ArrayExtensions.ResizeArray(ref worldToTangentMatrices, newCapacity);
            ArrayExtensions.ResizeArray(ref clipPlanes, newCapacity);
            ArrayExtensions.ResizeArray(ref reflectionMatrices, newCapacity);
            ArrayExtensions.ResizeArray(ref reflectionViewMatrices, newCapacity);
            ArrayExtensions.ResizeArray(ref reflectionCameraPositions, newCapacity);
            capacity = newCapacity;
        }
        public override void Dispose()
        {
            if (capacity == 0) return;
            maskMeshMatrices.Dispose();
            worldToTangentMatrices.Dispose();
            clipPlanes.Dispose();
            reflectionMatrices.Dispose();
            reflectionViewMatrices.Dispose();
            reflectionCameraPositions.Dispose();
            count = 0;
            capacity = 0;
        }
    }
}
