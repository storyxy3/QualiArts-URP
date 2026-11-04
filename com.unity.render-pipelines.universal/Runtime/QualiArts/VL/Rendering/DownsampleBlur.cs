using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
namespace VL.Rendering
{
    public class DownsampleBlur : IDisposable
    {
        private static readonly int _BlurOffset = Shader.PropertyToID("_BlurOffset");
        private const int MaxPyramidSize = 5;
        private RTHandle[] _pyramidTextures;
        private string[] _pyramidNames;
        private string _resultName;
        private Material _blurMaterial;
        private int _downPass;
        private int _upPass;
        public DownsampleBlur(string propertyName, Material material, int downPass, int upPass)
        {
            _blurMaterial = material;
            _pyramidTextures = new RTHandle[MaxPyramidSize];
            _pyramidNames = Enumerable.Range(0, MaxPyramidSize).Select(x => $"{propertyName}_Mip{x}").ToArray();
            _resultName = propertyName + "_Result";
            _downPass = downPass;
            _upPass = upPass;
        }
        public void Dispose()
        {
            if (_pyramidTextures != null)
            {
                foreach (var texture in _pyramidTextures) texture?.Release();
                _pyramidTextures = null;
            }
        }
        public void ApplyBlur(CommandBuffer cmd, RTHandle source, in RenderTextureDescriptor sourceDescriptor, ref RTHandle target, float size)
        {
            ApplyBlur(cmd, source, in sourceDescriptor, ref target, size, false, out int iterations);
        }
        private void ApplyBlur(CommandBuffer cmd, RTHandle source, in RenderTextureDescriptor sourceDescriptor, ref RTHandle target, float size, bool writeMips, out int iterations)
        {
            var desc = sourceDescriptor;
            desc.width >>= 1;
            desc.height >>= 1;
            desc.depthBufferBits = 0;
            float logh = Mathf.Log(Mathf.Max(desc.width, desc.height), 2f) + Mathf.Min(size, 10f) - 10f;
            int loghInt = Mathf.FloorToInt(logh);
            iterations = Mathf.Clamp(loghInt, 2, MaxPyramidSize);
            SetBlurOffset(cmd, 1f);
            if (!writeMips)
            {
                var targetDesc = desc;
                targetDesc.width >>= iterations - 2;
                targetDesc.height >>= iterations - 2;
                RenderingUtils.ReAllocateIfNeeded(ref target, in targetDesc, FilterMode.Bilinear, TextureWrapMode.Clamp, name: _resultName);
            }
            for (int i = 0; i < iterations; ++i)
            {
                RenderingUtils.ReAllocateIfNeeded(ref _pyramidTextures[i], in desc, FilterMode.Bilinear, TextureWrapMode.Clamp, name: _pyramidNames[i]);
                Blitter.BlitCameraTexture(cmd, source, _pyramidTextures[i], RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, _blurMaterial, _downPass);
                desc.width >>= 1;
                desc.height >>= 1;
                source = _pyramidTextures[i];
            }
            if (writeMips)
            {
                for (int i = iterations - 2; i >= 0; --i)
                {
                    var texture = _pyramidTextures[i + 1];
                    var scale = texture.useScaling ? new Vector2(texture.rtHandleProperties.rtHandleScale.x, texture.rtHandleProperties.rtHandleScale.y) : Vector2.one;
                    CoreUtils.SetRenderTarget(cmd, target, ClearFlag.None, i + 1);
                    Blitter.BlitTexture(cmd, _pyramidTextures[i + 1], new Vector4(scale.x, scale.y, 0f, 0f), _blurMaterial, _upPass);
                }
            }
            else
            {
                SetBlurOffset(cmd, (logh + 0.5f - loghInt) * 0.5f);
                Blitter.BlitCameraTexture(cmd, _pyramidTextures[iterations - 1], target, _blurMaterial, _upPass);
            }
            --iterations;
        }
        private void SetBlurOffset(CommandBuffer cmd, float scale)
        {
            cmd.SetGlobalVector(_BlurOffset, new Vector4(-scale, -scale, scale, scale));
        }
    }
}
