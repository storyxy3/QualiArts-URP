#!/usr/bin/env python3
"""Game metadata API amendments not represented by additional C# members.
Each replacement is declaration-only and idempotent; evidence is the matching
Gakumas 3.5.3 DummyDll type in .qa-work/urp-fields.
"""
from pathlib import Path
import subprocess,argparse
root=Path(__file__).resolve().parents[2]
p=argparse.ArgumentParser();p.add_argument('--commit',action='store_true');a=p.parse_args()
repairs=[
('UniversalRenderPipelineAsset','Data/UniversalRenderPipelineAsset.cs',[
 ('internal ReadOnlySpan<ScriptableRenderer> renderers','public ReadOnlySpan<ScriptableRenderer> renderers'),
 ('internal int[] rendererIndexList','public int[] rendererIndexList'),
 ('        /// <inheritdoc/>\n        public override string renderPipelineShaderTag => UniversalRenderPipeline.k_ShaderTagName;\n#endif','#endif\n        /// <inheritdoc/>\n        public override string renderPipelineShaderTag => UniversalRenderPipeline.k_ShaderTagName;')]),
('MotionVectorsPersistentData','MotionVectors.cs',[('internal Matrix4x4 previousViewProjection\n','public Matrix4x4 previousViewProjection\n')]),
('PostProcessPass','Passes/PostProcessPass.cs',[('        class MaterialLibrary','        protected class MaterialLibrary'),('        static class ShaderConstants','        protected static class ShaderConstants')]),
('PostProcessUtils','PostProcessUtils.cs',[('internal static void SetSourceSize','public static void SetSourceSize')]),
('TemporalAA.Settings','TemporalAA.cs',[('[NonSerialized] internal int resetHistoryFrames','[NonSerialized] public int resetHistoryFrames')]),
('UniversalAdditionalCameraData','UniversalAdditionalCameraData.cs',[('internal VolumeFrameworkUpdateMode volumeFrameworkUpdateMode','public VolumeFrameworkUpdateMode volumeFrameworkUpdateMode'),('internal MotionVectorsPersistentData motionVectorsPersistentData','public MotionVectorsPersistentData motionVectorsPersistentData')]),
('CameraData','UniversalRenderPipelineCore.cs',[('internal int pixelWidth;','public int pixelWidth;'),('internal int pixelHeight;','public int pixelHeight;'),('internal TemporalAA.Settings taaSettings;','public TemporalAA.Settings taaSettings;'),('internal Matrix4x4 GetProjectionMatrixNoJitter','public Matrix4x4 GetProjectionMatrixNoJitter')]),
('ShaderPropertyId','UniversalRenderPipelineCore.cs',[('internal static class ShaderPropertyId','public static class ShaderPropertyId')]),
('UniversalRenderer','UniversalRenderer.cs',[('internal RenderingMode renderingModeRequested','public RenderingMode renderingModeRequested'),('internal RenderingMode renderingModeActual','public RenderingMode renderingModeActual'),('internal DeferredLights deferredLights','public DeferredLights deferredLights'),('private struct RenderPassInputSummary','internal struct RenderPassInputSummary')]),
('DeferredLights','DeferredLights.cs',[('internal bool storeLitColor','public bool storeLitColor'),('internal bool renderedDirectReflection','public bool renderedDirectReflection')]),
]
for owner,relative,reps in repairs:
 path=root/'com.unity.render-pipelines.universal/Runtime'/relative
 s=old=path.read_text()
 for before,after in reps:s=s.replace(before,after)
 if s==old:continue
 path.write_text(s)
 if a.commit:
  subprocess.run(['git','-C',str(root),'add',str(path)],check=True)
  subprocess.run(['git','-C',str(root),'commit','-m','Match game exported API on '+owner],check=True,stdout=subprocess.DEVNULL)
# Declarations established during compilation of the original subclass APIs.
extra=[
('MotionVectorsPersistentData','com.unity.render-pipelines.universal/Runtime/MotionVectors.cs',[('internal sealed class MotionVectorsPersistentData','public sealed class MotionVectorsPersistentData')]),
('MotionVectorRenderPass','com.unity.render-pipelines.universal/Runtime/Passes/MotionVectorRenderPass.cs',[('    sealed class MotionVectorRenderPass','    public class MotionVectorRenderPass'),('private class PassData','protected class PassData')]),
('PostProcessPass','com.unity.render-pipelines.universal/Runtime/Passes/PostProcessPass.cs',[('public void Cleanup()', 'public virtual void Cleanup()'),('public void Dispose()', 'public virtual void Dispose()'),('public void Setup(in RenderTextureDescriptor','public virtual void Setup(in RenderTextureDescriptor'),('        void Render(CommandBuffer','        internal virtual void Render(CommandBuffer'),('        void RenderFinalPass(CommandBuffer','        internal virtual void RenderFinalPass(CommandBuffer'),('        void SetupColorGrading(CommandBuffer','        protected virtual void SetupColorGrading(CommandBuffer')]),
('DeferredLights','com.unity.render-pipelines.universal/Runtime/DeferredLights.cs',[('internal class DeferredLights','public class DeferredLights')]),
('UniversalAdditionalLightData','com.unity.render-pipelines.universal/Runtime/UniversalAdditionalLightData.cs',[('LightLayerEnum m_LightLayerMask = LightLayerEnum.LightLayerDefault','uint m_LightLayerMask = (uint)LightLayerEnum.LightLayerDefault'),('public LightLayerEnum lightLayerMask','public uint lightLayerMask')]),
('Volume','com.unity.render-pipelines.core/Runtime/Volume/Volume.cs',[('        void OnEnable()','        protected virtual void OnEnable()'),('        void OnDisable()','        protected virtual void OnDisable()')]),
]
for relative in ['UniversalRenderer.cs','UniversalRendererRenderGraph.cs','Deprecated.cs']:
 extra.append(('UniversalRenderer','com.unity.render-pipelines.universal/Runtime/'+relative,[('sealed partial class UniversalRenderer','partial class UniversalRenderer')]))
for owner,relative,reps in extra:
 path=root/relative;s=old=path.read_text()
 for before,after in reps:s=s.replace(before,after)
 if s!=old:path.write_text(s)
path=root/'com.unity.render-pipelines.universal/Runtime/ScriptableRenderer.cs';s=path.read_text()
field='private Matrix4x4[] _screenToWorld;'
if field in s and s.index(field)>s.index('public RenderingFeatures supportedRenderingFeatures'):
 s=s.replace('        // TODO(qa-recon): game declaration; DummyDll body remains a placeholder.\n'+field+'\n','')
 s=s.replace('        public RenderingFeatures supportedRenderingFeatures','        // Game field order: follows DebugHandler and precedes supportedRenderingFeatures.\n        '+field+'\n\n        public RenderingFeatures supportedRenderingFeatures')
 path.write_text(s)
path=root/'com.unity.render-pipelines.universal/Runtime/Debug/DebugDisplaySettingsRendering.cs';s=path.read_text()
if '_003CgameViewOnly_003Ek__BackingField' not in s:
 anchor='        public float validationRangeMax { get; set; } = 1.0f;'
 s=s.replace(anchor,anchor+'\n\n        // TODO(qa-recon): escaped metadata field; original name <gameViewOnly>k__BackingField.\n        [System.Runtime.CompilerServices.CompilerGenerated]\n        private bool _003CgameViewOnly_003Ek__BackingField;');path.write_text(s)
path=root/'com.unity.render-pipelines.universal/Runtime/Data/UniversalRenderPipelineAsset.cs';s=path.read_text()
if 'UseMainCamera = 10' not in s:s=s.replace('        UsePipelineSettings = 2,','        UsePipelineSettings = 2,\n\n        [InspectorName("Use MainCamera")]\n        UseMainCamera = 10,')
start=s.index('public enum ShadowResolution');end=s.index('\n    }',start)
if '_8192' not in s[start:end]:s=s[:start]+s[start:end].replace('_4096 = 4096','_4096 = 4096,\n\n        /// <summary>Game 3.5.3 shadow-map resolution.</summary>\n        _8192 = 8192')+s[end:]
path.write_text(s)

# Stock URP14 declared this unused handle; the game has no corresponding field.
path=root/"com.unity.render-pipelines.universal/Runtime/UniversalRenderer.cs"
s=path.read_text()
if s.count("m_ColorFrontBuffer")==1:s=s.replace("        RTHandle m_ColorFrontBuffer;\n", "")
path.write_text(s)
path=root/'com.unity.render-pipelines.universal/Runtime/Data/UniversalRenderPipelineAsset.cs'
s=path.read_text().replace('UniversalRenderPipelineAsset : RenderPipelineAsset, ISerializationCallbackReceiver','UniversalRenderPipelineAsset : RenderPipelineAsset<UniversalRenderPipeline>, ISerializationCallbackReceiver');path.write_text(s)
path=root/'com.unity.render-pipelines.universal/Runtime/ScriptableRenderer.cs';s=path.read_text()
if 'Game declaration order, including constants and static storage.' not in s:
 for line in ['public const int ScreenToWorldDownsampleCount = 3;','public static readonly int _ScreenToWorld;']:
  s=s.replace('        // TODO(qa-recon): game declaration; DummyDll body remains a placeholder.\n'+line+'\n','')
 s=s.replace('        // Game field order: follows DebugHandler and precedes supportedRenderingFeatures.\n        private Matrix4x4[] _screenToWorld;\n','')
 anchor='        internal static ScriptableRenderer current = null;'
 s=s.replace(anchor,anchor+'\n\n        // Game declaration order, including constants and static storage.\n        public const int ScreenToWorldDownsampleCount = 3;\n        private Matrix4x4[] _screenToWorld;\n        public static readonly int _ScreenToWorld;')
 path.write_text(s)

# More dead stock storage absent from native game metadata.
path=root/'com.unity.render-pipelines.core/Runtime/RenderGraph/RenderGraphDefaultResources.cs'
s=path.read_text()
s=s.replace('        bool m_IsValid;\n','')
path.write_text(s)
path=root/'com.unity.render-pipelines.core/Runtime/Common/DynamicResolutionHandler.cs'
s=path.read_text()
s=s.replace('        private Vector2Int cachedOriginalSize;\n','')
s=s.replace('            cachedOriginalSize = size;\n','')
path.write_text(s)
