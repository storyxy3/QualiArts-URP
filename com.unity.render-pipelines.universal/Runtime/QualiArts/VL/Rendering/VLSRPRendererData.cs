using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
namespace VL.Rendering
{
    [Serializable, ReloadGroup, ExcludeFromPreset]
    public class VLSRPRendererData : UniversalRendererData
    {
        public Shader deferredFogShader;
        public Shader skyShader;
        public Material defaultLitMaterial;
        public Material defaultDecalMaterial;
        protected override void OnEnable() { base.OnEnable(); }
        protected override ScriptableRenderer Create() { return new VLSRPRenderer(this); }
    }
}
