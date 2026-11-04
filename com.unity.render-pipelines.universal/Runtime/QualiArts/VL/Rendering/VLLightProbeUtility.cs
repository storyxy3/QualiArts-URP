using UnityEngine;
using UnityEngine.Rendering;
namespace VL.Rendering
{
    public static class VLLightProbeUtility
    {
        public static int _VLGlobalLightProbeSHAr = Shader.PropertyToID("_VLGlobalLightProbeSHAr");
        public static int _VLGlobalLightProbeSHAg = Shader.PropertyToID("_VLGlobalLightProbeSHAg");
        public static int _VLGlobalLightProbeSHAb = Shader.PropertyToID("_VLGlobalLightProbeSHAb");
        public static int _VLGlobalOcclusionProbe = Shader.PropertyToID("_VLGlobalOcclusionProbe");
        private static Vector3[] _lightProbePosition = new Vector3[1];
        private static SphericalHarmonicsL2[] _lightProbeSh = new SphericalHarmonicsL2[1];
        private static Vector4[] _lightProbeOcclusion = new Vector4[1];
        public static void SetupGlobalLightProbe(CommandBuffer cmd, Vector3 position)
        {
            _lightProbePosition[0] = position;
            LightProbes.CalculateInterpolatedLightAndOcclusionProbes(_lightProbePosition, _lightProbeSh, _lightProbeOcclusion);
            var sh = _lightProbeSh[0];
            cmd.SetGlobalVector(_VLGlobalLightProbeSHAr, new Vector4(sh[0, 3], sh[0, 1], sh[0, 2], sh[0, 0] - sh[0, 6]));
            cmd.SetGlobalVector(_VLGlobalLightProbeSHAg, new Vector4(sh[1, 3], sh[1, 1], sh[1, 2], sh[1, 0] - sh[1, 6]));
            cmd.SetGlobalVector(_VLGlobalLightProbeSHAb, new Vector4(sh[2, 3], sh[2, 1], sh[2, 2], sh[2, 0] - sh[2, 6]));
            cmd.SetGlobalVector(_VLGlobalOcclusionProbe, _lightProbeOcclusion[0]);
        }
    }
}
