namespace UnityEngine.Rendering.Universal
{
    // QualiArts (VL SRP): lighting model of an additional light (UniversalAdditionalLightData), read by Hidden/Campus/StencilDeferred.
    public enum CustomLightingType
    {
        [InspectorName("標準(スペキュラ)")]
        Default = 0,
        [InspectorName("Emission(高速)")]
        Emission = 1,
        [InspectorName("影領域乗算")]
        Multiply = 2,
    }
}
