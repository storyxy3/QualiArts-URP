using UnityEngine;

namespace VL.Rendering
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    public class VLSRPAdditionalCameraData : MonoBehaviour
    {
        public RenderTexture targetTexture { get; set; }
    }
}
