using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
namespace VL.Rendering
{
    public class VLActorParameterPass : ScriptableRenderPass
    {
        private readonly ProfilingSampler _profilingSampler = new ProfilingSampler("VLSRPCamera Parameter");
        public static readonly int MATCAP_LIGHT_COLOR = Shader.PropertyToID("_MatCapLightColor");
        public static readonly int MATCAP_MAINLIGHT = Shader.PropertyToID("_MatCapMainLight");
        public static readonly int MATCAP_RIM = Shader.PropertyToID("_MatCapRimLight");
        public static readonly int MATCAP_RIM_COLOR = Shader.PropertyToID("_MatCapRimColor");
        public static readonly int MATCAP_PARAM = Shader.PropertyToID("_MatCapParam");
        public static readonly int SHADE_MULTIPLY_COLOR = Shader.PropertyToID("_ShadeMultiplyColor");
        public static readonly int SPEC_CUBE = Shader.PropertyToID("_VLSpecCube");
        public static readonly int SPEC_COLOR = Shader.PropertyToID("_VLSpecColor");
        public static readonly int EYE_SPEC_CUBE = Shader.PropertyToID("_VLEyeSpecCube");
        public static readonly int EYE_SPEC_COLOR = Shader.PropertyToID("_VLEyeSpecColor");
        public static readonly int _GlobalLightParameter = Shader.PropertyToID("_GlobalLightParameter");
        public static readonly string USE_REFLECTION_TEXTURE = "USE_REFLECTION_TEXTURE";
        public static readonly string USE_EYE_REFLECTION_TEXTURE = "USE_EYE_REFLECTION_TEXTURE";
        public static readonly int _FogParams = Shader.PropertyToID("_VLFogParams");
        public static readonly int _FogColor = Shader.PropertyToID("_VLFogColor");
        public static readonly int _FogColor2 = Shader.PropertyToID("_VLFogColor2");
        public static readonly int _FogSphere = Shader.PropertyToID("_VLFogSphere");
        public static readonly string VL_FOG = "VL_FOG";
        public static readonly int _LightmapScaleColor = Shader.PropertyToID("_LightmapScaleColor");
        private Cubemap _defaultReflection;
        public VLActorParameterPass()
        {
            renderPassEvent = RenderPassEvent.BeforeRendering;
            _defaultReflection = CoreUtils.blackCubeTexture;
            useNativeRenderPass = false;
        }
        private void ProcessCommand(CommandBuffer cmd, ref RenderingData renderingData)
        {
            var stack = VolumeManager.instance.stack;
            var actor = stack.GetComponent<VLActorParameter>();
            var legacyFog = stack.GetComponent<VLDensityFogVolume>();
            var lightmap = stack.GetComponent<VLLightmapVolume>();
            bool useFog = legacyFog != null && (legacyFog.density.value > 0 || legacyFog.sphereRadius.value > 0 || legacyFog.sphereMaxAmount.value > 0);
            var depthFog = stack.GetComponent<DepthFog>();
            var sphereFog = stack.GetComponent<SphereFog>();
            if (depthFog != null && depthFog.density.value > 0) useFog = false;
            if (sphereFog != null && sphereFog.density.value > 0) useFog = false;
            CoreUtils.SetKeyword(cmd, VL_FOG, useFog);
            Cubemap specCube = _defaultReflection;
            Cubemap eyeSpecCube = _defaultReflection;
            if (actor != null)
            {
                cmd.SetGlobalColor(MATCAP_LIGHT_COLOR, actor.lightColor.value);
                var cameraTransform = renderingData.cameraData.camera.transform;
                var angle = actor.mainLightAngle.value;
                var space = actor.mainLightSpace.value;
                Quaternion rotation;
                if ((int)space == 1)
                    rotation = Quaternion.Euler(-angle.x, angle.y + 180, 0);
                else
                    rotation = Quaternion.AngleAxis(-cameraTransform.eulerAngles.z, Vector3.forward) * Quaternion.Euler(-angle.y, angle.x, 0);
                var direction = rotation * Vector3.forward;
                cmd.SetGlobalVector(MATCAP_MAINLIGHT, new Vector4(direction.x, direction.y, direction.z, (int)space == 1 ? 1 : 0));
                cmd.SetGlobalVector(MATCAP_PARAM, new Vector4(actor.matCapOffset.value, actor.matCapSmoothScale.value, 1 - actor.shadeApplyRatio.value, actor.rimPower.value));
                cmd.SetGlobalColor(SHADE_MULTIPLY_COLOR, actor.shadeColorMultiply.value);
                var rimAngle = actor.rimAngle.value;
                cmd.SetGlobalVector(MATCAP_RIM, Quaternion.Euler(rimAngle.y, rimAngle.x, 0) * Vector3.forward);
                Color rimColor = actor.rimColor.value;
                cmd.SetGlobalColor(MATCAP_RIM_COLOR, new Color(rimColor.r, rimColor.g, rimColor.b, actor.rimBaseColorRatio.value));
                cmd.SetGlobalVector(_GlobalLightParameter, new Vector4(actor.giScale.value, actor.additiveLightScale.value, 0, 0));
                if (actor.reflectionTexture != null && actor.reflectionTexture.value != null) specCube = actor.reflectionTexture.value;
                if (actor.eyeReflectionTexture != null && actor.eyeReflectionTexture.value != null) eyeSpecCube = actor.eyeReflectionTexture.value;
                cmd.SetGlobalColor(SPEC_COLOR, actor.reflectionColor.value);
                cmd.SetGlobalColor(EYE_SPEC_COLOR, actor.eyeReflectionColor.value);
            }
            cmd.SetGlobalTexture(SPEC_CUBE, specCube);
            cmd.SetGlobalTexture(EYE_SPEC_CUBE, eyeSpecCube);
            if (useFog)
            {
                bool linear = QualitySettings.activeColorSpace == ColorSpace.Linear;
                float density = legacyFog.density.value;
                cmd.SetGlobalVector(_FogParams, new Vector4(density * density, 0, 0, 0));
                Color color = legacyFog.color.value;
                if (linear)
                {
                    color.r = Mathf.GammaToLinearSpace(color.r);
                    color.g = Mathf.GammaToLinearSpace(color.g);
                    color.b = Mathf.GammaToLinearSpace(color.b);
                }
                cmd.SetGlobalColor(_FogColor, new Color(color.r, color.g, color.b, legacyFog.maxAmount.value));
                Vector3 position = legacyFog.spherePosition.value;
                cmd.SetGlobalVector(_FogSphere, new Vector4(position.x, position.y, position.z, legacyFog.sphereRadius.value));
                color = legacyFog.sphereColor.value;
                if (linear)
                {
                    color.r = Mathf.GammaToLinearSpace(color.r);
                    color.g = Mathf.GammaToLinearSpace(color.g);
                    color.b = Mathf.GammaToLinearSpace(color.b);
                }
                cmd.SetGlobalColor(_FogColor2, new Color(color.r, color.g, color.b, legacyFog.sphereMaxAmount.value));
            }
            if (lightmap != null) cmd.SetGlobalColor(_LightmapScaleColor, lightmap.color.value * lightmap.intensity.value);
        }
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, _profilingSampler))
            {
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                ProcessCommand(cmd, ref renderingData);
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}
