using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
namespace VL.Rendering
{
    public class VLCameraBufferSystem
    {
        private class RenderData
        {
            private string _name;
            private RTHandle _rtHandle;
            private int _renderedFrame = -1;
            public string name => _name;
            public RTHandle rtHandle => _rtHandle;
            public bool isRendered => _renderedFrame >= 0;
            public RenderData(string name) { _name = name; }
            public void ReAllocateIfNeeded(in RenderTextureDescriptor descriptor, FilterMode filterMode, TextureWrapMode wrapMode, string name)
            {
                if (RenderingUtils.ReAllocateIfNeeded(ref _rtHandle, in descriptor, filterMode, wrapMode, name: name))
                    _renderedFrame = -1;
            }
            public void Dispose() { _rtHandle?.Release(); }
            public void MarkRendered() { _renderedFrame = Time.renderedFrameCount; }
        }
        private Dictionary<int, RenderData> _renderData = new Dictionary<int, RenderData>();
        public RTHandle ReAllocateIfNeeded(int cameraHash, in RenderTextureDescriptor descriptor, FilterMode filterMode, TextureWrapMode wrapMode, string name)
        {
            if (!_renderData.TryGetValue(cameraHash, out var data))
            {
                data = new RenderData($"{name}:{cameraHash}");
                _renderData.Add(cameraHash, data);
            }
            data.ReAllocateIfNeeded(in descriptor, filterMode, wrapMode, data.name);
            return data.rtHandle;
        }
        public RTHandle GetTexture(int cameraHash) => _renderData.TryGetValue(cameraHash, out var data) ? data.rtHandle : null;
        public void MarkAsRendered(int cameraHash) { if (_renderData.TryGetValue(cameraHash, out var data)) data.MarkRendered(); }
        public bool IsRendered(int cameraHash) => _renderData.TryGetValue(cameraHash, out var data) && data.isRendered;
        public void Release()
        {
            if (_renderData == null) return;
            foreach (var entry in _renderData) entry.Value?.Dispose();
            _renderData.Clear();
        }
    }
}
