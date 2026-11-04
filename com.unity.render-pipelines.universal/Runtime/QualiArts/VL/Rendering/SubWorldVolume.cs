using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
namespace VL.Rendering
{
    [ExecuteAlways]
    public class SubWorldVolume : Volume
    {
        private static readonly Plane[] Planes = new Plane[6];
        public SubWorldMode mode = SubWorldMode.Default;
        public Mesh mesh;
        public LayerMask layerMask = -1;
        public int minimumRenderQueue;
        public int maximumRenderQueue = 2500;
        public uint renderingLayerMask;
        [NonSerialized] private Vector3 _position;
        [NonSerialized] private Bounds _bounds;
        [NonSerialized] private FilteringSettings _filteringSettings;
        [NonSerialized] private SubWorld _subWorld;
        public ref FilteringSettings filteringSettings => ref _filteringSettings;
        public bool maskMode => mode == SubWorldMode.Default;
        public Vector3 position => _position;
        private void OnDestroy()
        {
            if (_subWorld != null) DestroyImmediate(_subWorld);
            DestroyImmediate(profile);
        }
        private void LateUpdate()
        {
            if (_subWorld == null || transform.hasChanged)
            {
                transform.hasChanged = false;
                UpdateParameters();
            }
        }
        public void UpdateParameters()
        {
            if (_subWorld == null && !profile.TryGet(out _subWorld))
                _subWorld = profile.Add<SubWorld>();
            _subWorld.target.Override(this);
            _filteringSettings = new FilteringSettings(new RenderQueueRange(minimumRenderQueue, maximumRenderQueue), layerMask, renderingLayerMask);
            if (mode == SubWorldMode.Default) UpdateMeshParameter();
        }
        private void UpdateMeshParameter()
        {
            if (mesh == null)
            {
                _subWorld.target.value = null;
                return;
            }
            _position = transform.position;
            _bounds = mesh.bounds;
        }
        private void UpdateTextureParameter() { }
        public bool IsActive(Camera c, Vector3 cameraPosition)
        {
            if (mode == SubWorldMode.None) return false;
            if (mode == SubWorldMode.Texture) return true;
            GeometryUtility.CalculateFrustumPlanes(c, Planes);
            return GeometryUtility.TestPlanesAABB(Planes, _bounds);
        }
        public Rect GetViewportRect(ref CameraData cameraData) => new Rect(0, 0, cameraData.pixelWidth, cameraData.pixelHeight);
        private void OnValidate() { UpdateParameters(); }
    }
}
