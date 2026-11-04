using Unity.Mathematics;
using UnityEngine;
namespace VL.Rendering
{
    public static class AABBUtility
    {
        public static readonly Vector3 MinVector3 = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        public static readonly Vector3 MaxVector3 = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        public static readonly Bounds MinBounds = new Bounds(Vector3.zero, MinVector3);
        public static Bounds TransformAABB(Bounds bounds, Matrix4x4 transform)
        {
            Vector3 min = bounds.min;
            Vector3 max = bounds.max;
            Vector3 resultMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 resultMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            for (int i = 0; i < 8; ++i)
            {
                Vector3 point = transform.MultiplyPoint3x4(new Vector3((i & 1) != 0 ? max.x : min.x, (i & 2) != 0 ? max.y : min.y, (i & 4) != 0 ? max.z : min.z));
                resultMin = Vector3.Min(resultMin, point);
                resultMax = Vector3.Max(resultMax, point);
            }
            bounds.SetMinMax(resultMin, resultMax);
            return bounds;
        }
        public static Bounds TransformAABB(Bounds bounds, float4x4 transform)
        {
            Vector3 min = bounds.min;
            Vector3 max = bounds.max;
            float3 resultMin = new float3(float.MaxValue);
            float3 resultMax = new float3(float.MinValue);
            for (int i = 0; i < 8; ++i)
            {
                float3 corner = new float3(
                    (i & 1) == 0 ? min.x : max.x,
                    (i & 2) == 0 ? min.y : max.y,
                    (i & 4) == 0 ? min.z : max.z);
                float3 point = math.transform(transform, corner);
                resultMin = math.min(resultMin, point);
                resultMax = math.max(resultMax, point);
            }
            bounds.SetMinMax(resultMin, resultMax);
            return bounds;
        }
        public static bool TestSphereAABB(in Bounds bounds, in BoundingSphere sphere)
        {
            return bounds.SqrDistance(sphere.position) <= sphere.radius * sphere.radius;
        }
    }
}
