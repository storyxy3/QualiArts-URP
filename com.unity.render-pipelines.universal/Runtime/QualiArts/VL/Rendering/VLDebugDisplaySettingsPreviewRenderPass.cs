using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;
namespace VL.Rendering
{
    public class VLDebugDisplaySettingsPreviewRenderPass
    {
        private Camera _selectedCamera;
        protected int _selectedCameraIndex = -1;
        private Camera[] _cameraCaches;
        private List<Camera> _cameras = new List<Camera>();
        private bool _captureRequested;
        public Camera selectedCamera => _selectedCamera;
        public int selectedCameraIndex
        {
            set
            {
                _selectedCameraIndex = value;
                int count = cameras.Count();
                if (count == 0)
                    _selectedCamera = null;
                else
                    _selectedCamera = _selectedCameraIndex < 0 || _selectedCameraIndex >= count
                        ? cameras.First() : cameras.ElementAt(_selectedCameraIndex);
            }
        }
        public IEnumerable<Camera> cameras
        {
            get
            {
                _cameras.Clear();
                if (_cameraCaches == null || Camera.allCamerasCount != _cameraCaches.Length)
                    _cameraCaches = new Camera[Camera.allCamerasCount];
                Camera.GetAllCameras(_cameraCaches);
                foreach (Camera camera in _cameraCaches)
                {
                    if (camera == null || camera.cameraType == CameraType.Preview || camera.cameraType == CameraType.Reflection)
                        continue;
                    AddCamera(camera);
                }
                return _cameras;

                void AddCamera(Camera camera)
                {
                    if (camera.TryGetComponent<UniversalAdditionalCameraData>(out var data) && data.scriptableRenderer is VLSRPRenderer)
                        _cameras.Add(camera);
                }
            }
        }
        public void CaptureRequest() { _captureRequested = true; }
    }
}
