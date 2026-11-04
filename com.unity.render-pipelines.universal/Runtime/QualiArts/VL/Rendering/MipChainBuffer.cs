using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
namespace VL.Rendering
{
    public class MipChainBuffer : IDisposable
    {
        public const int MaxMipCount = 14;
        public static readonly string PrepareKernelName = "Prepare";
        public static readonly string MipChainKernelName = "MipChain";
        public static readonly int _SrcOffset = Shader.PropertyToID("_SrcOffset");
        public static readonly int _DstOffset = Shader.PropertyToID("_DstOffset");
        public static readonly int _Input = Shader.PropertyToID("_Input");
        public static readonly int _Output = Shader.PropertyToID("_Output");
        private readonly ProfilingSampler _profilingSampler;
        private readonly ComputeShader _compute;
        private readonly GraphicsFormat _format;
        private readonly int _threadSize;
        private readonly string _bufferName;
        private readonly int _prepareKernelIndex;
        private readonly int _mipChainKernelIndex;
        private int[] _srcOffset = new int[4];
        private int[] _dstOffset = new int[4];
        private RTHandle _mipBuffer;
        public int MipCount { get; private set; }
        public Vector2Int[] MipMin { get; } = new Vector2Int[MaxMipCount];
        public Vector2Int[] MipMax { get; } = new Vector2Int[MaxMipCount];
        public Vector2Int[] Size { get; } = new Vector2Int[MaxMipCount];
        public RenderTargetIdentifier nameID => _mipBuffer.nameID;
        public MipChainBuffer(ComputeShader compute, GraphicsFormat format, string bufferName)
        {
            _profilingSampler = new ProfilingSampler("MipChainBuffer (" + bufferName + ")");
            _compute = compute;
            _format = format;
            _bufferName = bufferName;
            _prepareKernelIndex = compute.FindKernel(PrepareKernelName);
            _mipChainKernelIndex = compute.FindKernel(MipChainKernelName);
            compute.GetKernelThreadGroupSizes(_mipChainKernelIndex, out uint x, out uint y, out uint z);
            _threadSize = (int)x;
        }
        public void Dispose() { _mipBuffer?.Release(); }
        public void UpdateMipChainParams(int width, int height, int maxMipCount = -1)
        {
            MipCount = Mathf.FloorToInt(Mathf.Log(Mathf.Max(width, height), 2f)) + 1;
            if (maxMipCount > 0) MipCount = Mathf.Min(MipCount, maxMipCount);
            MipMin[0] = new Vector2Int(0, 0);
            MipMax[0] = new Vector2Int(width, height) - Vector2Int.one;
            Size[0] = new Vector2Int(width, height);
            int minX = 0, minY = 0;
            int maxX = width, maxY = height;
            for (int i = 1; i < MipCount; ++i)
            {
                int w = Mathf.Max(1, Size[i - 1].x >> 1);
                int h = Mathf.Max(1, Size[i - 1].y >> 1);
                if ((i & 1) != 0) minY = maxY;
                else minX = maxX;
                MipMin[i] = new Vector2Int(minX, minY);
                maxX = minX + w;
                maxY = minY + h;
                MipMax[i] = new Vector2Int(maxX, maxY) - Vector2Int.one;
                Size[i] = new Vector2Int(w, h);
            }
        }
        private void SetComputeOffsetParams(CommandBuffer cmd, Vector2Int srcMin, Vector2Int srcMax, Vector2Int dstMin, Vector2Int srcSize)
        {
            _srcOffset[0] = srcMin.x;
            _srcOffset[1] = srcMin.y;
            _srcOffset[2] = srcMax.x;
            _srcOffset[3] = srcMax.y;
            _dstOffset[0] = dstMin.x;
            _dstOffset[1] = dstMin.y;
            _dstOffset[2] = srcSize.x & 1;
            _dstOffset[3] = srcSize.y & 1;
            cmd.SetComputeIntParams(_compute, _SrcOffset, _srcOffset);
            cmd.SetComputeIntParams(_compute, _DstOffset, _dstOffset);
        }
        public void Execute(CommandBuffer cmd, int width, int height, RTHandle source = null)
        {
            int GetExtraPixel(int x)
            {
                int remainder = x % _threadSize;
                return remainder == 0 ? 0 : _threadSize - remainder;
            }
            static int DivRoundUp(int x, int y) => (x + y - 1) / y;
            using (new ProfilingScope(cmd, _profilingSampler))
            {
                var desc = new RenderTextureDescriptor(width, height, _format, 0);
                desc.width += GetExtraPixel(width);
                desc.height += Mathf.CeilToInt(height * 0.5f) + GetExtraPixel(height);
                desc.enableRandomWrite = true;
                RenderingUtils.ReAllocateIfNeeded(ref _mipBuffer, in desc, FilterMode.Point, TextureWrapMode.Repeat, name: _bufferName);
                cmd.SetGlobalTexture(_bufferName, _mipBuffer.nameID);
                if (source != null) cmd.SetComputeTextureParam(_compute, _prepareKernelIndex, _Input, source.nameID);
                cmd.SetComputeTextureParam(_compute, _prepareKernelIndex, _Output, _mipBuffer.nameID, 0);
                cmd.SetComputeTextureParam(_compute, _mipChainKernelIndex, _Output, _mipBuffer.nameID);
                var size = new Vector2Int(width, height);
                SetComputeOffsetParams(cmd, Vector2Int.zero, size, Vector2Int.zero, size);
                cmd.DispatchCompute(_compute, _prepareKernelIndex, DivRoundUp(width, _threadSize), DivRoundUp(height, _threadSize), 1);
                for (int i = 1; i < MipCount; ++i)
                {
                    var mipSize = Size[i];
                    SetComputeOffsetParams(cmd, MipMin[i - 1], MipMax[i - 1], MipMin[i], Size[i - 1]);
                    cmd.DispatchCompute(_compute, _mipChainKernelIndex, DivRoundUp(mipSize.x, _threadSize), DivRoundUp(mipSize.y, _threadSize), 1);
                }
            }
        }
        public void CopyToMipMinMax(Span<Vector4> mipMinMax)
        {
            for (int i = 0; i < MipCount; ++i)
                mipMinMax[i] = new Vector4(MipMin[i].x, MipMin[i].y, MipMax[i].x, MipMax[i].y);
        }
        public static bool IsSupported(ComputeShader computeShader, GraphicsFormat format)
        {
            if (!RenderingUtils.SupportsGraphicsFormat(format, FormatUsage.LoadStore) || !SystemInfo.supportsComputeShaders) return false;
            if (computeShader == null) return false;
            return computeShader.HasKernel(PrepareKernelName) && computeShader.HasKernel(MipChainKernelName)
                && computeShader.IsSupported(computeShader.FindKernel(MipChainKernelName));
        }
    }
}
