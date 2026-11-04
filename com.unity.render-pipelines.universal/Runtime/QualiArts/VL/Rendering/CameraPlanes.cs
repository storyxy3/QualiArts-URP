using System;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
namespace VL.Rendering
{
    [Serializable]
    public struct CameraPlanes
    {
        public float4 p0;
        public float4 p1;
        public float4 p2;
        public float4 p3;
        public float4 p4;
        public float4 p5;
        public unsafe ref float4 this[int index]
        {
            get { fixed (float4* p = &p0) return ref p[index]; }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CameraPlanes(in Matrix4x4 vpMatrix)
        {
            float4 row3 = new float4(vpMatrix[3], vpMatrix[7], vpMatrix[11], vpMatrix[15]);
            float4 row0 = new float4(vpMatrix[0], vpMatrix[4], vpMatrix[8], vpMatrix[12]);
            p0 = math.normalize(row3 + row0);
            p1 = math.normalize(row3 - row0);
            float4 row1 = new float4(vpMatrix[1], vpMatrix[5], vpMatrix[9], vpMatrix[13]);
            p2 = math.normalize(row3 + row1);
            p3 = math.normalize(row3 - row1);
            float4 row2 = new float4(vpMatrix[2], vpMatrix[6], vpMatrix[10], vpMatrix[14]);
            p4 = math.normalize(row3 + row2);
            p5 = math.normalize(row3 - row2);
        }
        public bool TestAABB(Bounds bounds)
        {
            float3 center = bounds.center;
            float3 extents = bounds.extents;
            for (int i = 0; i < 6; ++i)
            {
                var plane = this[i];
                if (GetDistanceToPoint(plane, center) + math.dot(math.abs(plane.xyz), extents) < 0) return false;
            }
            return true;
        }
        public bool TestBoundingSphere(float4 sphere)
        {
            float distance = float.MaxValue;
            for (int i = 0; i < 6; ++i)
                distance = math.min(distance, GetDistanceToPoint(this[i], sphere.xyz));
            return distance + sphere.w <= 0;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float GetDistanceToPoint(float4 plane, float3 point) { return math.dot(plane.xyz, point) + plane.w; }
    }
}
