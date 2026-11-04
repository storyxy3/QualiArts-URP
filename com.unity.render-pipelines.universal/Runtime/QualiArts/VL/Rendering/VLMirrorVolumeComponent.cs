using System;
using UnityEngine;
using UnityEngine.Rendering;
namespace VL.Rendering
{
    [Serializable, VolumeComponentMenu("VL/Mirror Plane")]
    public class VLMirrorVolumeComponent : VolumeComponent
    {
        private static readonly Plane[] Planes = new Plane[6];
        public MirrorMeshParameter mesh = new MirrorMeshParameter(null);
        public NoInterpVector3Parameter position = new NoInterpVector3Parameter(Vector3.zero);
        public MirrorQuaternionParameter rotation = new MirrorQuaternionParameter(Quaternion.identity);
        public NoInterpVector3Parameter scale = new NoInterpVector3Parameter(Vector3.one);
        public NoInterpVector3Parameter normal = new NoInterpVector3Parameter(Vector3.forward);
        public BoolParameter useGlossy = new BoolParameter(false);
        public ClampedFloatParameter bumpScale = new ClampedFloatParameter(1f, 0f, 1f);
        public ClampedFloatParameter bumpLimit = new ClampedFloatParameter(1f, 0f, 1f);
        public MirrorBoundsParameter bounds = new MirrorBoundsParameter(new Bounds(Vector3.zero, Vector3.one));
        public bool IsActive(Camera c)
        {
            if (mesh.value == null) return false;
            if (Vector3.Dot(position.value - c.transform.position, normal.value) > 0f) return false;
            GeometryUtility.CalculateFrustumPlanes(c, Planes);
            return GeometryUtility.TestPlanesAABB(Planes, bounds.value);
        }
        public Vector4 plane
        {
            get
            {
                var n = normal.value;
                return new Vector4(n.x, n.y, n.z, -Vector3.Dot(n, position.value));
            }
        }
    }
}
