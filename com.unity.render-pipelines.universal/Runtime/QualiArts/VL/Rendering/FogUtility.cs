using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
namespace VL.Rendering
{
    public class FogUtility
    {
        public static readonly int _FogParams = Shader.PropertyToID("_VLFogParams");
        public static readonly int _FogColor = Shader.PropertyToID("_VLFogColor");
        public static readonly int _FogSphere = Shader.PropertyToID("_VLFogSphere");
        public static readonly int _FogSphereColor = Shader.PropertyToID("_VLFogSphereColor");
        private static readonly ProfilingSampler deferredFogProfilingSampler = new ProfilingSampler("Deferred Fog");
        private static readonly Dictionary<DeferredFogPasses, string> _passToPassName = new Dictionary<DeferredFogPasses, string>
        {
            { DeferredFogPasses.Full, "Deferred Fog" },
            { DeferredFogPasses.ActorOnly, "Deferred ActorFog" },
            { DeferredFogPasses.Downscale, "Deferred Fog (Downscale)" },
        };
        private static readonly Dictionary<DeferredFogPasses, int> _passNameToIndex = new Dictionary<DeferredFogPasses, int>();
        public static void UpdateProperties(CommandBuffer cmd, DepthFog depthFog, SphereFog sphereFog, VLDensityFogVolume legacyFog = null)
        {
            float density = Mathf.Pow(depthFog.density.value, 4f);
            float sphereDensity = Mathf.Pow(sphereFog.density.value, 4f);
            bool applySkybox = depthFog.applySkybox.value;
            Color color = depthFog.color.value;
            float maxAmount = depthFog.maxAmount.value;
            Vector3 spherePosition = sphereFog.position.value;
            float sphereRadius = sphereFog.radius.value;
            Color sphereColor = sphereFog.color.value;
            float sphereMaxAmount = sphereFog.maxAmount.value;
            if (legacyFog != null && (legacyFog.density.value > 0f || legacyFog.sphereRadius.value > 0f))
            {
                density = legacyFog.density.value * legacyFog.density.value;
                sphereDensity = 1f;
                applySkybox = true;
                color = legacyFog.color.value;
                maxAmount = legacyFog.maxAmount.value;
                spherePosition = legacyFog.spherePosition.value;
                sphereRadius = legacyFog.sphereRadius.value;
                sphereColor = legacyFog.sphereColor.value;
                sphereMaxAmount = legacyFog.sphereMaxAmount.value;
            }
            bool isLinear = QualitySettings.activeColorSpace == ColorSpace.Linear;
            cmd.SetGlobalVector(_FogParams, new Vector4(density, applySkybox ? 1f : 0f, sphereDensity, 0f));
            cmd.SetGlobalVector(_FogColor, GetFogColor(color, maxAmount));
            cmd.SetGlobalVector(_FogSphere, new Vector4(spherePosition.x, spherePosition.y, spherePosition.z, sphereRadius));
            cmd.SetGlobalVector(_FogSphereColor, GetFogColor(sphereColor, sphereMaxAmount));
            Vector4 GetFogColor(Color c, float amount)
            {
                if (isLinear)
                {
                    c.r = Mathf.GammaToLinearSpace(c.r);
                    c.g = Mathf.GammaToLinearSpace(c.g);
                    c.b = Mathf.GammaToLinearSpace(c.b);
                }
                return new Vector4(c.r, c.g, c.b, amount);
            }
        }
    }
}
