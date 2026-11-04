using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
namespace VL.Rendering
{
    internal class VLDecalDrawSystem
    {
        public static readonly int _NormalToWorld = Shader.PropertyToID("_NormalToWorld");
        public static readonly int _DecalLayerMaskFromDecal = Shader.PropertyToID("_DecalLayerMaskFromDecal");
        public static readonly int _Color = Shader.PropertyToID("_Color");
        protected DecalEntityManager m_EntityManager;
        private Matrix4x4[] m_WorldToDecals;
        private Matrix4x4[] m_NormalToDecals;
        private float[] m_DecalLayerMasks;
        private float[] m_Colors;
        private ProfilingSampler m_Sampler;
        private DecalType _decalType;
        public VLDecalDrawSystem(string sampler, DecalType type, int layer, uint renderingLayerMask, DecalEntityManager entityManager)
        {
            m_EntityManager = entityManager;
            m_WorldToDecals = new Matrix4x4[250];
            m_NormalToDecals = new Matrix4x4[250];
            m_DecalLayerMasks = new float[250];
            m_Colors = new float[250];
            m_Sampler = new ProfilingSampler(sampler);
            _decalType = type;
        }
        public void Execute(CommandBuffer cmd)
        {
            using (new ProfilingScope(cmd, m_Sampler))
                for (int i = 0; i < m_EntityManager.chunkCount; ++i)
                    Execute(cmd, m_EntityManager.entityChunks[i], m_EntityManager.cachedChunks[i], m_EntityManager.drawCallChunks[i]);
        }
        private void Execute(CommandBuffer cmd, DecalEntityChunk decalEntityChunk, DecalCachedChunk decalCachedChunk, DecalDrawCallChunk decalDrawCallChunk)
        {
            decalCachedChunk.currentJobHandle.Complete();
            decalDrawCallChunk.currentJobHandle.Complete();
            if (decalEntityChunk.count == 0) return;
            var material = decalEntityChunk.material;
            if (material.HasProperty(DecalProjector._DecalType) && (DecalType)material.GetFloat(DecalProjector._DecalType) == _decalType)
                Draw(cmd, decalEntityChunk, decalCachedChunk.propertyBlock, decalDrawCallChunk);
        }
        private void Draw(CommandBuffer cmd, DecalEntityChunk decalEntityChunk, MaterialPropertyBlock materialPropertyBlock, DecalDrawCallChunk decalDrawCallChunk)
        {
            var mesh = m_EntityManager.decalProjectorMesh;
            int subCallCount = decalDrawCallChunk.subCallCount;
            if (SystemInfo.supportsInstancing && decalEntityChunk.material.enableInstancing)
            {
                for (int i = 0; i < subCallCount; ++i)
                {
                    var subCall = decalDrawCallChunk.subCalls[i];
                    var worlds = decalDrawCallChunk.decalToWorlds.Reinterpret<Matrix4x4>();
                    NativeArray<Matrix4x4>.Copy(worlds, subCall.start, m_WorldToDecals, 0, subCall.count);
                    var normals = decalDrawCallChunk.normalToDecals.Reinterpret<Matrix4x4>();
                    NativeArray<Matrix4x4>.Copy(normals, subCall.start, m_NormalToDecals, 0, subCall.count);
                    NativeArray<float>.Copy(decalDrawCallChunk.renderingLayerMasks, subCall.start, m_DecalLayerMasks, 0, subCall.count);
                    NativeArray<float>.Copy(decalDrawCallChunk.colors, subCall.start, m_Colors, 0, subCall.count);
                    materialPropertyBlock.SetMatrixArray(_NormalToWorld, m_NormalToDecals);
                    materialPropertyBlock.SetFloatArray(_DecalLayerMaskFromDecal, m_DecalLayerMasks);
                    materialPropertyBlock.SetFloatArray(_Color, m_Colors);
                    cmd.DrawMeshInstanced(mesh, 0, decalEntityChunk.material, 0, m_WorldToDecals, subCall.end - subCall.start, materialPropertyBlock);
                }
            }
        }
    }
}
