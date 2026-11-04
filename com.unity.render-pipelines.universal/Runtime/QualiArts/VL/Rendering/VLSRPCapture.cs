using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
namespace VL.Rendering
{
    [RequireComponent(typeof(Camera))]
    public class VLSRPCapture : MonoBehaviour
    {
        public enum SourceType { Current, Depth }
        public struct RequestData
        {
            public RTHandle rtHandle;
            public RenderTargetIdentifier target;
            public RenderPassEvent renderPassEvent;
            public SourceType sourceType;
            public Action onCapture;
            public RequestData(RenderTargetIdentifier target, RenderPassEvent renderPassEvent, Action onCapture = null)
            {
                rtHandle = null;
                this.target = target;
                this.renderPassEvent = renderPassEvent;
                sourceType = SourceType.Current;
                this.onCapture = onCapture;
            }
            public bool Equals(RequestData other) => target.Equals(other.target) && renderPassEvent == other.renderPassEvent && sourceType == other.sourceType;
            public override bool Equals(object obj) => obj is RequestData other && Equals(other);
            public override int GetHashCode() => HashCode.Combine(target.GetHashCode(), (int)renderPassEvent, (int)sourceType);
        }
        private HashSet<RequestData> _captureRequests;
        public void Add(RenderTargetIdentifier target, RenderPassEvent renderPassEvent, Action onCapture = null)
        {
            _captureRequests ??= new HashSet<RequestData>();
            _captureRequests.Add(new RequestData(target, renderPassEvent, onCapture));
        }
        public void Add(RequestData requestData)
        {
            _captureRequests ??= new HashSet<RequestData>();
            _captureRequests.Add(requestData);
        }
        public bool Remove(RenderTargetIdentifier target) => _captureRequests != null && _captureRequests.RemoveWhere(request => request.target.Equals(target)) > 0;
        public bool TryGetRequests(out HashSet<RequestData>.Enumerator requests)
        {
            if (_captureRequests != null)
            {
                requests = _captureRequests.GetEnumerator();
                return true;
            }
            requests = default;
            return false;
        }
        public void Clear() { _captureRequests?.Clear(); }
    }
}
