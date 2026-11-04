using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
namespace VL.Rendering
{
    public class ConvolutionBlur
    {
        public const int MaxMipCount = 6;
        public const int MinResolution = 64;
        private static readonly float[] GaussOffset = { 1.3846154f, 3.2307692f };
        private static readonly int _BlurOffset = Shader.PropertyToID("_BlurOffset");
        private static readonly int _MipLevel = Shader.PropertyToID("_MipLevel");
        private static readonly int[] _TempConvolutionMips = Enumerable.Range(0, 6).Select(x => Shader.PropertyToID($"Convolution Blur{x}")).ToArray();
        private static readonly ProfilingSampler _profilingSampler = new ProfilingSampler("Convolution Blur");
        private readonly Material _material;
        private readonly int _mipCount;
        private readonly int _blurPass;
        private RenderTextureDescriptor _descriptor;
        private RTHandle[] _tempRTs;
        public ConvolutionBlur(Material material, int blurPass = 1, int mipCount = 6)
        {
            _mipCount = mipCount;
            _material = material;
            _blurPass = blurPass;
            _tempRTs = new RTHandle[mipCount];
            for (int i = 0; i < _tempRTs.Length; ++i)
            {
                string name = $"_ConvolutionBlurTemp{i}";
                _tempRTs[i] = RTHandles.Alloc(name, name: name);
            }
        }
        public void Dispose()
        {
            for (int i = 0; i < _tempRTs.Length; ++i) _tempRTs[i]?.Release();
        }
        public void Execute(CommandBuffer cmd, RTHandle destination, RenderTextureDescriptor cameraTextureDescriptor)
        {
            if (_material == null || destination == null || destination.rt == null) return;
            using (new ProfilingScope(cmd, _profilingSampler))
            {
                cameraTextureDescriptor.width = Mathf.Max(cameraTextureDescriptor.width, MinResolution);
                cameraTextureDescriptor.height = Mathf.Max(cameraTextureDescriptor.height, MinResolution);
                cameraTextureDescriptor.depthBufferBits = 0;
                cameraTextureDescriptor.msaaSamples = 1;
                cameraTextureDescriptor.useMipMap = false;
                cameraTextureDescriptor.mipCount = 0;
                for (int i = 0; i < _mipCount; ++i)
                {
                    cameraTextureDescriptor.width >>= 1;
                    cameraTextureDescriptor.height >>= 1;
                    cmd.SetGlobalVector(_BlurOffset, new Vector4(0, GaussOffset[0] / cameraTextureDescriptor.height, 0, GaussOffset[1] / cameraTextureDescriptor.height));
                    cmd.SetGlobalFloat(_MipLevel, i);
                    RenderingUtils.ReAllocateIfNeeded(ref _tempRTs[i], in cameraTextureDescriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, name: _tempRTs[i].name);
                    Blitter.BlitCameraTexture(cmd, destination, _tempRTs[i], RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, _material, _blurPass);
                    cmd.SetGlobalVector(_BlurOffset, new Vector4(GaussOffset[0] / cameraTextureDescriptor.width, 0, GaussOffset[1] / cameraTextureDescriptor.width, 0));
                    cmd.SetGlobalFloat(_MipLevel, 0);
                    Vector2 scale = _tempRTs[i].useScaling ? new Vector2(_tempRTs[i].rtHandleProperties.rtHandleScale.x, _tempRTs[i].rtHandleProperties.rtHandleScale.y) : Vector2.one;
                    CoreUtils.SetRenderTarget(cmd, destination, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, ClearFlag.None, Color.clear, i + 1);
                    Blitter.BlitTexture(cmd, _tempRTs[i], new Vector4(scale.x, scale.y, 0, 0), _material, _blurPass);
                }
            }
        }
    }
}
