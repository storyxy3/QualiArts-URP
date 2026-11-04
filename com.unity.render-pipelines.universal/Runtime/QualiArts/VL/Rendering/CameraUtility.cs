using UnityEngine;
using UnityEngine.Rendering;
namespace VL.Rendering
{
    public static class CameraUtility
    {
        public static Plane GetPlane(Vector4 vector)
        {
            float length = Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
            return new Plane(new Vector3(vector.x / length, vector.y / length, vector.z / length), vector.w / length);
        }
        public static void CalcCullingPlane(ref ScriptableCullingParameters parameters, Matrix4x4 vpMatrix)
        {
            Vector4 row0 = vpMatrix.GetRow(0);
            Vector4 row1 = vpMatrix.GetRow(1);
            Vector4 row2 = vpMatrix.GetRow(2);
            Vector4 row3 = vpMatrix.GetRow(3);
            parameters.SetCullingPlane(0, GetPlane(row2 + row3));
            parameters.SetCullingPlane(1, GetPlane(row3 - row2));
            parameters.SetCullingPlane(2, GetPlane(row1 + row3));
            parameters.SetCullingPlane(3, GetPlane(row3 - row1));
            parameters.SetCullingPlane(4, GetPlane(row0 + row3));
            parameters.SetCullingPlane(5, GetPlane(row3 - row0));
            parameters.cullingMatrix = vpMatrix;
        }
    }
}
