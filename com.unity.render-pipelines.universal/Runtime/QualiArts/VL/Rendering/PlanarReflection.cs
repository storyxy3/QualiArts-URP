using System;
using UnityEngine;
using UnityEngine.Rendering;
using VL.EntitySystem;
namespace VL.Rendering
{
    [ExecuteAlways]
    [AddComponentMenu("Rendering/Planar Reflection")]
    public class PlanarReflection : MonoBehaviour, IEntityComponent
    {
        public enum MaskType { Box, Mesh }
        public struct ShaderTagIdList
        {
            private const int kMaxShaderPasses = 15;
            public int count;
            public unsafe fixed int _tagIds[15];
            public unsafe ShaderTagIdList(string[] shaderTagNames)
            {
                count = 0;
                if (shaderTagNames == null || shaderTagNames.Length == 0) return;
                fixed (int* ptr = _tagIds)
                {
                    var tags = new Span<ShaderTagId>(ptr, kMaxShaderPasses);
                    for (int i = 0; i < shaderTagNames.Length; ++i)
                        if (!string.IsNullOrEmpty(shaderTagNames[i]))
                            tags[count++] = new ShaderTagId(shaderTagNames[i]);
                }
            }
            public unsafe ReadOnlySpan<ShaderTagId> AsReadOnlySpan()
            {
                fixed (int* ptr = _tagIds)
                    return new ReadOnlySpan<ShaderTagId>(ptr, count);
            }
        }
        public MaskType maskType;
        public Vector2 size = Vector2.one;
        public bool fullResolution;
        public Mesh mesh;
        public LayerMask layerMask = -1;
        public int minimumRenderQueue;
        public int maximumRenderQueue = 2500;
        public uint renderingLayerMask;
        public bool additionalShaderTagId;
        public string[] shaderTagNames;
        private ShaderTagIdList _shaderTagIdList;
        private bool _isShaderTagIdListDirty = true;
        public ShaderTagIdList shaderTagIdList
        {
            get
            {
                if (_isShaderTagIdListDirty)
                {
                    _shaderTagIdList = additionalShaderTagId ? new ShaderTagIdList(shaderTagNames) : default;
                    _isShaderTagIdListDirty = false;
                }
                return _shaderTagIdList;
            }
        }
        public Entity entity { get; set; }
        public void SetDirty() { _isShaderTagIdListDirty = true; }
        private void OnEnable() { _isShaderTagIdListDirty = true; EntityComponentEvent<PlanarReflection>.InvokeAdd(this); }
        private void OnDisable() { EntityComponentEvent<PlanarReflection>.InvokeRemove(this); }
        private void OnValidate() { EntityComponentEvent<PlanarReflection>.InvokePropertyChange(this); }
        Transform IEntityComponent.transform => transform;
    }
}
