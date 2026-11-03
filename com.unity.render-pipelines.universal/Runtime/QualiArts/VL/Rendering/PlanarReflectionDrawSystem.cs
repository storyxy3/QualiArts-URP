using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
namespace VL.Rendering
{
    public class PlanarReflectionDrawSystem : IDisposable
    {
        public const int MaxPlanarReflection = 3;
        public const int StencilSSR = 2;
        public const int Stencil0 = 4;
        public const int Stencil1 = 8;
        public static readonly int[] StencilRefs = { 4, 8, 12 };
        public static readonly int _StencilRef = Shader.PropertyToID("_StencilRef");
        private readonly PlanarReflectionEntityManager _entityManager;
        private readonly ProfilingSampler _drawSampler;
        private Material[] _materials;
        private readonly RenderStateBlock[] _renderStateBlocks;
        private readonly int _stencilPass;
        private readonly int _maskPass;
        private readonly int _resetPass;
        private int _planarReflectionCount;
        public bool isSystemExecuted;
        public bool IsActive => isSystemExecuted && _planarReflectionCount > 0 && _entityManager.combinedChunks != null;
        public PlanarReflectionDrawSystem(PlanarReflectionEntityManager entityManager, Material sourceMaterial, int stencilPass, int maskPass, int resetPass)
        {
            _entityManager = entityManager;
            _materials = new Material[MaxPlanarReflection];
            _renderStateBlocks = new RenderStateBlock[MaxPlanarReflection];
            for (int i = 0; i < MaxPlanarReflection; ++i)
            {
                _materials[i] = new Material(sourceMaterial) { hideFlags = HideFlags.HideAndDontSave };
                _materials[i].SetInt(_StencilRef, StencilRefs[i]);
                var state = new RenderStateBlock(RenderStateMask.Stencil);
                state.stencilReference = StencilRefs[i];
                var stencil = state.stencilState;
                stencil.enabled = true;
                stencil.readMask = 12;
                stencil.writeMask = 2;
                stencil.SetCompareFunction(CompareFunction.Equal);
                stencil.SetPassOperation(StencilOp.Zero);
                state.stencilState = stencil;
                _renderStateBlocks[i] = state;
            }
            _stencilPass = stencilPass;
            _maskPass = maskPass;
            _resetPass = resetPass;
            _drawSampler = new ProfilingSampler("PlanarReflection.Draw");
            _planarReflectionCount = -1;
        }
        public void Dispose()
        {
            if (_materials == null) return;
            foreach (var material in _materials)
                if (material != null) CoreUtils.Destroy(material);
            _materials = null;
        }
        public void CompleteJob()
        {
            if (!isSystemExecuted || _entityManager.combinedChunks == null) return;
            var culledChunk = _entityManager.combinedChunks.culledChunk;
            var resultChunk = _entityManager.combinedChunks.resultChunk;
            culledChunk.currentJobHandle.Complete();
            resultChunk.currentJobHandle.Complete();
            _planarReflectionCount = Mathf.Min(culledChunk.culledCount[0], MaxPlanarReflection);
        }
        public void DrawPlanarReflections(CommandBuffer cmd, ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (!IsActive) return;
            using (new ProfilingScope(null, _drawSampler))
            {
                for (int i = 0; i < _planarReflectionCount; ++i)
                    PlanarReflectionUtility.RenderPlanarReflection(cmd, context, ref renderingData, _entityManager, i,
                        _materials[i], _stencilPass, _maskPass, _resetPass, ref _renderStateBlocks[i]);
            }
            isSystemExecuted = false;
        }
    }
}
