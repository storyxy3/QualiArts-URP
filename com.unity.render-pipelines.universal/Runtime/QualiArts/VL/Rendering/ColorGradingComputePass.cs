// VL compute colour-grading LUT pass (APK 3.5.3, VL.Rendering.ColorGradingComputePass): URP 14 ColorGradingLutPass
// rewritten as a compute dispatch (kernel "KBuild") with the VL tonemapping modes; reconstructed from the game.
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace VL.Rendering
{
    public class ColorGradingComputePass : ScriptableRenderPass
    {
        private class PassData
        {
            internal ProfilingSampler sampler;
            internal ComputeShader lutBuilder;
            internal HableCurve hableCurve;
            internal bool allowColorGradingACESHDR;
        }

        private static class ShaderConstants
        {
            public static readonly int _Lut_Params = Shader.PropertyToID("_Lut_Params");
            public static readonly int _ColorBalance = Shader.PropertyToID("_ColorBalance");
            public static readonly int _ColorFilter = Shader.PropertyToID("_ColorFilter");
            public static readonly int _ChannelMixerRed = Shader.PropertyToID("_ChannelMixerRed");
            public static readonly int _ChannelMixerGreen = Shader.PropertyToID("_ChannelMixerGreen");
            public static readonly int _ChannelMixerBlue = Shader.PropertyToID("_ChannelMixerBlue");
            public static readonly int _HueSatCon = Shader.PropertyToID("_HueSatCon");
            public static readonly int _Lift = Shader.PropertyToID("_Lift");
            public static readonly int _Gamma = Shader.PropertyToID("_Gamma");
            public static readonly int _Gain = Shader.PropertyToID("_Gain");
            public static readonly int _Shadows = Shader.PropertyToID("_Shadows");
            public static readonly int _Midtones = Shader.PropertyToID("_Midtones");
            public static readonly int _Highlights = Shader.PropertyToID("_Highlights");
            public static readonly int _ShaHiLimits = Shader.PropertyToID("_ShaHiLimits");
            public static readonly int _SplitShadows = Shader.PropertyToID("_SplitShadows");
            public static readonly int _SplitHighlights = Shader.PropertyToID("_SplitHighlights");
            public static readonly int _CurveMaster = Shader.PropertyToID("_CurveMaster");
            public static readonly int _CurveRed = Shader.PropertyToID("_CurveRed");
            public static readonly int _CurveGreen = Shader.PropertyToID("_CurveGreen");
            public static readonly int _CurveBlue = Shader.PropertyToID("_CurveBlue");
            public static readonly int _CurveHueVsHue = Shader.PropertyToID("_CurveHueVsHue");
            public static readonly int _CurveHueVsSat = Shader.PropertyToID("_CurveHueVsSat");
            public static readonly int _CurveLumVsSat = Shader.PropertyToID("_CurveLumVsSat");
            public static readonly int _CurveSatVsSat = Shader.PropertyToID("_CurveSatVsSat");
            public static readonly int _OutputTexture = Shader.PropertyToID("_OutputTexture");
            public static readonly int _Size = Shader.PropertyToID("_Size");
            public static readonly int _Params = Shader.PropertyToID("_Params");
            public static readonly int _CustomToneCurve = Shader.PropertyToID("_CustomToneCurve");
            public static readonly int _ToeSegmentA = Shader.PropertyToID("_ToeSegmentA");
            public static readonly int _ToeSegmentB = Shader.PropertyToID("_ToeSegmentB");
            public static readonly int _MidSegmentA = Shader.PropertyToID("_MidSegmentA");
            public static readonly int _MidSegmentB = Shader.PropertyToID("_MidSegmentB");
            public static readonly int _ShoSegmentA = Shader.PropertyToID("_ShoSegmentA");
            public static readonly int _ShoSegmentB = Shader.PropertyToID("_ShoSegmentB");
            public static readonly int _LogLut3D = Shader.PropertyToID("_LogLut3D");
            public static readonly int _LogLut3D_Params = Shader.PropertyToID("_LogLut3D_Params");
            public static readonly int _gtParamsA = Shader.PropertyToID("_GtParamsA");
        }

        private readonly ComputeShader _lutCompute;
        private readonly GraphicsFormat _lutFormat;
        private PassData m_PassData;
        private RTHandle m_InternalLut;
        private bool m_AllowColorGradingACESHDR = true;

        public static bool IsAvailable(VLPostProcessData data)
        {
            if (!RenderingUtils.SupportsGraphicsFormat(GraphicsFormat.B10G11R11_UFloatPack32, FormatUsage.LoadStore))
                return false;
            if (!RenderingUtility.IsSupportedDevice() || !SystemInfo.supportsComputeShaders || SystemInfo.supportedRandomWriteTargetCount < 1)
                return false;
            if (data == null)
                return false;
            if (data.lutCompute == null)
                return false;
            return data.lutCompute.HasKernel("KBuild");
        }

        public ColorGradingComputePass(RenderPassEvent evt, VLPostProcessData data)
        {
            base.profilingSampler = new ProfilingSampler(nameof(ColorGradingComputePass));
            renderPassEvent = evt;
            overrideCameraTarget = false;
            _lutCompute = data.lutCompute;
            useNativeRenderPass = false;
            _lutFormat = GraphicsFormat.B10G11R11_UFloatPack32;
            if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES3 && Graphics.minOpenGLESVersion <= OpenGLESVersion.OpenGLES30 && SystemInfo.graphicsDeviceName.StartsWith("Adreno (TM) 3"))
                m_AllowColorGradingACESHDR = false;
            m_PassData = new PassData();
            m_PassData.hableCurve = new HableCurve();
            m_PassData.sampler = profilingSampler;
            m_PassData.lutBuilder = _lutCompute;
        }

        public void Setup(in RTHandle internalLut)
        {
            m_InternalLut = internalLut;
        }

        public void ConfigureDescriptor(in PostProcessingData postProcessingData, out RenderTextureDescriptor descriptor, out FilterMode filterMode)
        {
            int lutHeight = postProcessingData.lutSize;
            descriptor = new RenderTextureDescriptor(lutHeight, lutHeight, _lutFormat, 0);
            descriptor.vrUsage = VRTextureUsage.None;
            descriptor.dimension = TextureDimension.Tex3D;
            descriptor.volumeDepth = lutHeight;
            descriptor.enableRandomWrite = true;
            filterMode = FilterMode.Trilinear;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            m_PassData.allowColorGradingACESHDR = m_AllowColorGradingACESHDR;
            if (renderingData.cameraData.xr.supportsFoveatedRendering)
                renderingData.commandBuffer.SetFoveatedRenderingMode(FoveatedRenderingMode.Disabled);
            ExecutePass(context, m_PassData, ref renderingData, m_InternalLut);
        }

        private static void ExecutePass(ScriptableRenderContext context, PassData passData, ref RenderingData renderingData, RTHandle internalLutTarget)
        {
            var cmd = renderingData.commandBuffer;
            var lutBuilder = passData.lutBuilder;
            int kernel = lutBuilder.FindKernel("KBuild");
            var hableCurve = passData.hableCurve;
            var allowColorGradingACESHDR = passData.allowColorGradingACESHDR;
            using (new ProfilingScope(cmd, passData.sampler))
            {
                var stack = VolumeManager.instance.stack;
                var channelMixer = stack.GetComponent<ChannelMixer>();
                var colorAdjustments = stack.GetComponent<ColorAdjustments>();
                var curves = stack.GetComponent<ColorCurves>();
                var liftGammaGain = stack.GetComponent<LiftGammaGain>();
                var shadowsMidtonesHighlights = stack.GetComponent<ShadowsMidtonesHighlights>();
                var splitToning = stack.GetComponent<SplitToning>();
                var tonemapping = stack.GetComponent<Tonemapping>();
                var vlTonemapping = stack.GetComponent<VLTonemapping>();
                var whiteBalance = stack.GetComponent<WhiteBalance>();

                ref var postProcessingData = ref renderingData.postProcessingData;
                bool hdr = postProcessingData.gradingMode == ColorGradingMode.HighDynamicRange;
                ref CameraData cameraData = ref renderingData.cameraData;

                var lmsColorBalance = ColorUtils.ColorBalanceToLMSCoeffs(whiteBalance.temperature.value, whiteBalance.tint.value);
                var hueSatCon = new Vector4(colorAdjustments.hueShift.value / 360f, colorAdjustments.saturation.value / 100f + 1f, colorAdjustments.contrast.value / 100f + 1f, 0f);
                var channelMixerR = new Vector4(channelMixer.redOutRedIn.value / 100f, channelMixer.redOutGreenIn.value / 100f, channelMixer.redOutBlueIn.value / 100f, 0f);
                var channelMixerG = new Vector4(channelMixer.greenOutRedIn.value / 100f, channelMixer.greenOutGreenIn.value / 100f, channelMixer.greenOutBlueIn.value / 100f, 0f);
                var channelMixerB = new Vector4(channelMixer.blueOutRedIn.value / 100f, channelMixer.blueOutGreenIn.value / 100f, channelMixer.blueOutBlueIn.value / 100f, 0f);

                var shadowsHighlightsLimits = new Vector4(
                    shadowsMidtonesHighlights.shadowsStart.value,
                    shadowsMidtonesHighlights.shadowsEnd.value,
                    shadowsMidtonesHighlights.highlightsStart.value,
                    shadowsMidtonesHighlights.highlightsEnd.value
                );

                var (shadows, midtones, highlights) = ColorUtils.PrepareShadowsMidtonesHighlights(
                    shadowsMidtonesHighlights.shadows.value,
                    shadowsMidtonesHighlights.midtones.value,
                    shadowsMidtonesHighlights.highlights.value
                );

                var (lift, gamma, gain) = ColorUtils.PrepareLiftGammaGain(
                    liftGammaGain.lift.value,
                    liftGammaGain.gamma.value,
                    liftGammaGain.gain.value
                );

                var (splitShadows, splitHighlights) = ColorUtils.PrepareSplitToning(
                    splitToning.shadows.value,
                    splitToning.highlights.value,
                    splitToning.balance.value
                );

                int lutHeight = postProcessingData.lutSize;
                cmd.SetComputeVectorParam(lutBuilder, ShaderConstants._Size, new Vector4(lutHeight, 1f / (lutHeight - 1f), 0f, 0f));
                cmd.SetComputeVectorParam(lutBuilder, ShaderConstants._ColorBalance, lmsColorBalance);
                cmd.SetComputeVectorParam(lutBuilder, ShaderConstants._ColorFilter, colorAdjustments.colorFilter.value.linear);
                cmd.SetComputeVectorParam(lutBuilder, ShaderConstants._ChannelMixerRed, channelMixerR);
                cmd.SetComputeVectorParam(lutBuilder, ShaderConstants._ChannelMixerGreen, channelMixerG);
                cmd.SetComputeVectorParam(lutBuilder, ShaderConstants._ChannelMixerBlue, channelMixerB);
                cmd.SetComputeVectorParam(lutBuilder, ShaderConstants._HueSatCon, hueSatCon);
                cmd.SetComputeVectorParam(lutBuilder, ShaderConstants._Lift, lift);
                cmd.SetComputeVectorParam(lutBuilder, ShaderConstants._Gamma, gamma);
                cmd.SetComputeVectorParam(lutBuilder, ShaderConstants._Gain, gain);
                cmd.SetComputeVectorParam(lutBuilder, ShaderConstants._Shadows, shadows);
                cmd.SetComputeVectorParam(lutBuilder, ShaderConstants._Midtones, midtones);
                cmd.SetComputeVectorParam(lutBuilder, ShaderConstants._Highlights, highlights);
                cmd.SetComputeVectorParam(lutBuilder, ShaderConstants._ShaHiLimits, shadowsHighlightsLimits);
                cmd.SetComputeVectorParam(lutBuilder, ShaderConstants._SplitShadows, splitShadows);
                cmd.SetComputeVectorParam(lutBuilder, ShaderConstants._SplitHighlights, splitHighlights);
                cmd.SetComputeVectorParam(lutBuilder, ShaderConstants._Params, new Vector4(1f, 0f, 0f, 0f));

                if (hdr)
                {
                    lutBuilder.shaderKeywords = null;
                    if (vlTonemapping.IsActive())
                    {
                        switch (vlTonemapping.mode.value)
                        {
                            case VLTonemappingMode.Neutral:
                                CoreUtils.SetKeyword(lutBuilder, "_TONEMAP_NEUTRAL", true);
                                break;
                            case VLTonemappingMode.ACES:
                                CoreUtils.SetKeyword(lutBuilder, allowColorGradingACESHDR ? "_TONEMAP_ACES" : "_TONEMAP_NEUTRAL", true);
                                break;
                            case VLTonemappingMode.Custom:
                                CoreUtils.SetKeyword(lutBuilder, "_TONEMAP_CUSTOM", true);
                                hableCurve.Init(
                                    vlTonemapping.toeStrength.value,
                                    vlTonemapping.toeLength.value,
                                    vlTonemapping.shoulderStrength.value,
                                    vlTonemapping.shoulderLength.value,
                                    vlTonemapping.shoulderAngle.value,
                                    vlTonemapping.gamma.value);
                                cmd.SetComputeVectorParam(lutBuilder, ShaderConstants._CustomToneCurve, hableCurve.uniforms.curve);
                                cmd.SetComputeVectorParam(lutBuilder, ShaderConstants._ToeSegmentA, hableCurve.uniforms.toeSegmentA);
                                cmd.SetComputeVectorParam(lutBuilder, ShaderConstants._ToeSegmentB, hableCurve.uniforms.toeSegmentB);
                                cmd.SetComputeVectorParam(lutBuilder, ShaderConstants._MidSegmentA, hableCurve.uniforms.midSegmentA);
                                cmd.SetComputeVectorParam(lutBuilder, ShaderConstants._MidSegmentB, hableCurve.uniforms.midSegmentB);
                                cmd.SetComputeVectorParam(lutBuilder, ShaderConstants._ShoSegmentA, hableCurve.uniforms.shoSegmentA);
                                cmd.SetComputeVectorParam(lutBuilder, ShaderConstants._ShoSegmentB, hableCurve.uniforms.shoSegmentB);
                                break;
                            case VLTonemappingMode.External:
                                CoreUtils.SetKeyword(lutBuilder, "_TONEMAP_EXTERNAL", true);
                                cmd.SetComputeTextureParam(lutBuilder, kernel, ShaderConstants._LogLut3D, vlTonemapping.lutTexture.value);
                                cmd.SetComputeVectorParam(lutBuilder, ShaderConstants._LogLut3D_Params, new Vector4(1f / lutHeight, lutHeight - 1f, vlTonemapping.lutContribution.value, 0f));
                                break;
                            case VLTonemappingMode.GT:
                                CoreUtils.SetKeyword(lutBuilder, "_TONEMAP_GT", true);
                                cmd.SetComputeVectorParam(lutBuilder, ShaderConstants._gtParamsA, new Vector4(
                                    vlTonemapping.gtLinearSectionStart.value,
                                    vlTonemapping.gtContrast.value,
                                    vlTonemapping.gtBlackBrightness.value,
                                    vlTonemapping.gtLinearSelectionLength.value));
                                break;
                        }
                    }
                    else
                    {
                        switch (tonemapping.mode.value)
                        {
                            case TonemappingMode.Neutral:
                                CoreUtils.SetKeyword(lutBuilder, "_TONEMAP_NEUTRAL", true);
                                break;
                            case TonemappingMode.ACES:
                                CoreUtils.SetKeyword(lutBuilder, allowColorGradingACESHDR ? "_TONEMAP_ACES" : "_TONEMAP_NEUTRAL", true);
                                break;
                        }
                    }

                    if (cameraData.isHDROutputActive)
                    {
                        UniversalRenderPipeline.GetHDROutputLuminanceParameters(cameraData.hdrDisplayInformation, cameraData.hdrDisplayColorGamut, tonemapping, out Vector4 hdrOutputLuminanceParams);
                        UniversalRenderPipeline.GetHDROutputGradingParameters(tonemapping, out Vector4 hdrOutputGradingParams);
                        cmd.SetComputeVectorParam(lutBuilder, ShaderPropertyId.hdrOutputLuminanceParams, hdrOutputLuminanceParams);
                        cmd.SetComputeVectorParam(lutBuilder, ShaderPropertyId.hdrOutputGradingParams, hdrOutputGradingParams);
                        HDROutputUtils.ConfigureHDROutput(lutBuilder, cameraData.hdrDisplayColorGamut, HDROutputUtils.Operation.ColorConversion);
                    }
                }

                cmd.SetComputeTextureParam(lutBuilder, kernel, ShaderConstants._OutputTexture, internalLutTarget.nameID);
                cameraData.xr.StopSinglePass(cmd);
                if (cameraData.xr.supportsFoveatedRendering)
                    cmd.SetFoveatedRenderingMode(FoveatedRenderingMode.Disabled);
                int groups = (lutHeight + 3) / 4;
                cmd.DispatchCompute(lutBuilder, kernel, groups, groups, groups);
                cameraData.xr.StartSinglePass(cmd);
            }
        }
    }
}
