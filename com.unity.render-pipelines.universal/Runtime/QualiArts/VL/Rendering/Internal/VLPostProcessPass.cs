// Reconstructed from the Gakumas 3.5.3 IL2CPP binary.
namespace VL.Rendering.Internal
{
    using System;
    using Unity.Mathematics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Experimental.Rendering;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.Universal;
    using VL.Rendering;
    using VL.Rendering.Internal;
    using VL.Rendering.Shaders;

    [Serializable]

    internal class VLPostProcessPass : PostProcessPass
    {

        private class DOFHandles
        {

            private RTHandle m_Far;

            private RTHandle m_Near;

            public RTHandle far
            {
                get { return m_Far; }
            }

            public RTHandle near
            {
                get { return m_Near; }
            }

            public DOFHandles(string name)
            {
                string farName = name + "_Far";
                string nearName = name + "_Near";
                m_Far = RTHandles.Alloc(farName, farName);
                m_Near = RTHandles.Alloc(nearName, nearName);
            }

            public bool ReAllocateIfNeeded(int width, int height, GraphicsFormat colorFormat)
            {
                var descriptor = new RenderTextureDescriptor(width, height, colorFormat, 0);
                return RenderingUtils.ReAllocateIfNeeded(ref m_Far, descriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, name: m_Far.name)
                    | RenderingUtils.ReAllocateIfNeeded(ref m_Near, descriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, name: m_Near.name);
            }

            public void Release()
            {
                m_Far?.Release();
                m_Near?.Release();
            }
        }

        private class VLMaterialLibrary
        {

            public readonly Material vlDiffusion;

            public readonly Material vlBloom;

            public readonly Material vlFlare;

            public readonly Material vlDOF;

            public readonly Material vlStarStreak;

            public readonly Material vlMotionBlur;

            public readonly Material vlTextureBlur;

            public readonly Material vlVirtualEffect;

            public readonly Material vlNoise;

            public readonly Material vlChromaticAberration;

            public readonly Material vlJumpFloodOutline;

            public VLMaterialLibrary(VLPostProcessData data)
            {
                vlDiffusion = Load(data.vlDiffusion);
                vlBloom = Load(data.vlBloom);
                vlFlare = Load(data.vlFlare);
                vlDOF = Load(data.vlDOF);
                vlStarStreak = Load(data.vlStarStreak);
                vlMotionBlur = Load(data.vlMotionBlur);
                vlTextureBlur = Load(data.vlTextureBlur);
                vlVirtualEffect = Load(data.vlVirtualEffect);
                vlNoise = Load(data.vlNoise);
                vlChromaticAberration = Load(data.vlChromaticAberration);
                vlJumpFloodOutline = Load(data.vlJumpFloodOutline);
            }

            private Material Load(Shader shader)
            {
                if (shader == null)
                {
                    Debug.LogErrorFormat($"Missing shader. {GetType().DeclaringType.Name} render pass will not execute. Check for missing reference in the renderer resources.");
                    return null;
                }
                if (!shader.isSupported)
                    return null;
                return CoreUtils.CreateEngineMaterial(shader);
            }

            internal void Cleanup()
            {
                CoreUtils.Destroy(vlBloom);
                CoreUtils.Destroy(vlFlare);
                CoreUtils.Destroy(vlDOF);
                CoreUtils.Destroy(vlStarStreak);
                CoreUtils.Destroy(vlMotionBlur);
                CoreUtils.Destroy(vlTextureBlur);
                CoreUtils.Destroy(vlVirtualEffect);
                CoreUtils.Destroy(vlNoise);
                CoreUtils.Destroy(vlChromaticAberration);
            }
        }

        private enum VLDOFPasses
        {

            CalcCoc,

            CalcBokehCoc,

            CocReprojection,

            InflateCocFirst,

            InflateCocSecond,

            DownsampleMRT,

            Downsample2MRT,

            BlurFirstMRT,

            BlurSecondMRT,

            BlurFirst2MRT,

            UpsampleMRT,

            ApplyMRT,

            BokehPrefilter,

            BokehBlurMRT,

            BokehFloodFill,

            BokehPostBlurMRT,

            BokehComposite
        }

        private static class VLDOFShaderConstants
        {

            public static readonly int ShaderVariablesVLDOFBokeh = Shader.PropertyToID("ShaderVariablesVLDOFBokeh");

            public static readonly int _ForegroundBlurExtrude = Shader.PropertyToID("_ForegroundBlurExtrude");

            public static readonly int _DOFFrontTexture = Shader.PropertyToID("_DOFFrontTexture");

            public static readonly int _DOFBackTexture = Shader.PropertyToID("_DOFBackTexture");

            public static readonly int _HistoryCoCTexture = Shader.PropertyToID("_HistoryCoCTexture");

            public static readonly int _CocReprojectionParams = Shader.PropertyToID("_CocReprojectionParams");

            public static readonly int _DOFMediumRezWorkTexture = Shader.PropertyToID("_DOFMediumRezWorkTexture");

            public static readonly int _DOFLowNearTexture = Shader.PropertyToID("_DOFLowNearTexture");

            public static readonly int _InvRenderTargetSize = Shader.PropertyToID("_InvRenderTargetSize");

            public static readonly int _DOFBlurOffsets = Shader.PropertyToID("_DOFBlurOffsets");

            public static readonly int _DOFCurveParams = Shader.PropertyToID("_DOFCurveParams");

            public static readonly int _DOFCocOffset = Shader.PropertyToID("_DOFCocOffset");
        }

        private class VLJumpFloodOutlineShaderConstants
        {

            internal static readonly int _DistanceFieldMap = Shader.PropertyToID("_DistanceFieldMap");

            internal static readonly int _StepWidth = Shader.PropertyToID("_StepWidth");

            internal static readonly int _MainOutlineAmount = Shader.PropertyToID("_MainOutlineAmount");

            internal static readonly int _MainOutlineColor = Shader.PropertyToID("_MainOutlineColor");

            internal static readonly int _MainOffsetShiftUV = Shader.PropertyToID("_MainOffsetShiftUV");

            internal static readonly int _SubOutlineAmount = Shader.PropertyToID("_SubOutlineAmount");

            internal static readonly int _SubOutlineColor = Shader.PropertyToID("_SubOutlineColor");

            internal static readonly int _SubOffsetShiftUV = Shader.PropertyToID("_SubOffsetShiftUV");

            internal static readonly int _OutlineMaxWidth = Shader.PropertyToID("_OutlineMaxWidth");

            internal static readonly int _SmoothDelta = Shader.PropertyToID("_SmoothDelta");

            internal static readonly int _HistoryDistanceFieldTexture = Shader.PropertyToID("_HistoryDistanceFieldTexture");

            internal static readonly int _DistanceFiledReprojectionParams = Shader.PropertyToID("_DistanceFiledReprojectionParams");
        }

        private class VLMotionBlurShaderConstants
        {

            internal static readonly int VelocityScale = Shader.PropertyToID("_VelocityScale");

            internal static readonly int MaxBlurRadius = Shader.PropertyToID("_MaxBlurRadius");

            internal static readonly int RcpMaxBlurRadius = Shader.PropertyToID("_RcpMaxBlurRadius");

            internal static readonly int TileMaxOffs = Shader.PropertyToID("_TileMaxOffs");

            internal static readonly int TileMaxLoop = Shader.PropertyToID("_TileMaxLoop");

            internal static readonly int LoopCount = Shader.PropertyToID("_LoopCount");
        }

        private class VLShaderConstants
        {

            public static readonly int _TextureBlurIntensity = Shader.PropertyToID("_TextureBlurIntensity");

            public static readonly int _TextureBlurTexture = Shader.PropertyToID("_TextureBlurTex");

            public static readonly int _Bloom_Settings = Shader.PropertyToID("_Bloom_Settings");

            public static readonly int _Bloom_Color = Shader.PropertyToID("_Bloom_Color");

            public static readonly int _Flare_Settings = Shader.PropertyToID("_Flare_Settings");

            public static readonly int _Flare_Color0 = Shader.PropertyToID("_Flare_Color0");

            public static readonly int _Flare_Color1 = Shader.PropertyToID("_Flare_Color1");

            public static readonly int _StarStreakParam = Shader.PropertyToID("_StarStreakParam");

            public static readonly int _StarStreakBlurParams = Shader.PropertyToID("_StarStreakBlurParams");

            public static readonly int _Diffusion_Settings = Shader.PropertyToID("_Diffusion_Settings");

            public static readonly int _Diffusion_Contrasts = Shader.PropertyToID("_Diffusion_Contrasts");

            public static readonly int _BlurOffset = Shader.PropertyToID("_BlurOffset");
        }

        private class VLVirtualEffectShaderConstants
        {

            internal static readonly int _VirtualEffect_Mask = Shader.PropertyToID("_VirtualEffect_Mask");

            internal static readonly int _AuraColor = Shader.PropertyToID("_AuraColor");

            internal static readonly int _NoiseParams = Shader.PropertyToID("_NoiseParams");

            internal static readonly int _Granularity = Shader.PropertyToID("_Granularity");

            internal static readonly int _RGBSplit = Shader.PropertyToID("_RGBSplit");

            internal static readonly int _ChromaticAberrationIntensity = Shader.PropertyToID("_ChromaticAberrationIntensity");
        }

        private class VLChromaticAberrationShaderConstants
        {

            internal static readonly int _RedDirection = Shader.PropertyToID("_RedDirection");

            internal static readonly int _GreenDirection = Shader.PropertyToID("_GreenDirection");

            internal static readonly int _BlueDirection = Shader.PropertyToID("_BlueDirection");
        }

        private class VLRGBBockNoiseShaderConstants
        {

            internal static readonly int _OffsetUV = Shader.PropertyToID("_OffsetUV");

            internal static readonly int _StepFrequency = Shader.PropertyToID("_StepFrequency");

            internal static readonly int _TileOffsetX = Shader.PropertyToID("_TileOffsetX");

            internal static readonly int _TileOffsetY = Shader.PropertyToID("_TileOffsetY");

            internal static readonly int _Aspect = Shader.PropertyToID("_Aspect");

            internal static readonly int _Attenuation = Shader.PropertyToID("_Attenuation");

            internal static readonly int _Saturation = Shader.PropertyToID("_Saturation");
        }

        private class VLWaveJitterGlitchShaderContents
        {

            internal static readonly int _Frequency = Shader.PropertyToID("_Frequency");

            internal static readonly int _Intensity = Shader.PropertyToID("_WaveIntensity");

            internal static readonly int _Granularity = Shader.PropertyToID("_Granularity");

            internal static readonly int _WaveSpeed = Shader.PropertyToID("_WaveSpeed");

            internal static readonly int _RGBSplit = Shader.PropertyToID("_RGBSplit");
        }


        private static readonly ProfilingSampler fsrUpscalingSampler = new ProfilingSampler("FSR Upscaling");

        private RTHandle m_DownscaleAdditiveTexture;

        private RTHandle m_DiffusionTexture;

        private RTHandle m_DiffusionTemp;

        private RTHandle m_DOFCocTexture;

        private RTHandle m_DOFInflateCoc1;

        private RTHandle m_DOFInflateCoc2;

        private DOFHandles m_DOFMedium;

        private DOFHandles m_DOFLow;

        private DOFHandles m_DOFTemp1;

        private DOFHandles m_DOFTemp2;

        private RTHandle m_VelocityTex;

        private RTHandle[] m_TileRTs;

        private RTHandle m_TileVRT;

        private RTHandle m_NeighborMaxTex;

        private VLMaterialLibrary m_VLMaterials;

        private bool _computeGradingAvailable;

        private bool _vlDOFSupported;

        private GraphicsFormat m_RequestHDRFormat;

        private RTHandle m_ChromaticAberrationTexture;

        private readonly ProfilingSampler _vlDOFProfilingSampler = new ProfilingSampler("VL DOF");

        private static RenderTargetBinding DofMRT2Bindings = new RenderTargetBinding(
            new RenderTargetIdentifier[2],
            new[] { RenderBufferLoadAction.DontCare, RenderBufferLoadAction.DontCare },
            new RenderBufferStoreAction[2], BuiltinRenderTextureType.None,
            RenderBufferLoadAction.DontCare, RenderBufferStoreAction.DontCare);

        private static RenderTargetBinding DofMRT3Bindings = new RenderTargetBinding(
            new RenderTargetIdentifier[3],
            new[] { RenderBufferLoadAction.DontCare, RenderBufferLoadAction.DontCare, RenderBufferLoadAction.DontCare },
            new RenderBufferStoreAction[3], BuiltinRenderTextureType.None,
            RenderBufferLoadAction.DontCare, RenderBufferStoreAction.DontCare);

        private ShaderVariablesVLDOFBokeh _bokehData;

        private static readonly ProfilingSampler _vlJumpFloodOutlineSampler = new ProfilingSampler("VL Jump Flood Outline");

        private RTHandle m_OutlineMaskHalfRT;

        private RTHandle m_OutlineMaskQuarterRT;

        private RTHandle m_PingBuffer;

        private RTHandle m_PongBuffer;

        private RTHandle m_DistanceFieldPingRT;

        private RTHandle m_DistanceFieldPongRT;

        private bool m_distanceFieldPingPong;

        private Vector2 _mainOutlineOffsetShiftUV;

        private Vector2 _subOutlineOffsetShiftUV;

        private readonly ProfilingSampler _blendDownscaleProfilingSampler = new ProfilingSampler("Blend Downscale");

        private readonly ProfilingSampler _vlDiffusionProfilingSampler = new ProfilingSampler("VL Diffusion");

        private readonly ProfilingSampler _vlBloomProfilingSampler = new ProfilingSampler("VL Bloom");

        private readonly ProfilingSampler _vlParaffinProfilingSampler = new ProfilingSampler("VL Paraffin");

        private readonly ProfilingSampler _vlMotionBlurSampler = new ProfilingSampler("VL Motion Blur");

        private readonly ProfilingSampler _vlTextureBlurSampler = new ProfilingSampler("VL Texture Blur");

        private Vector4[] _starStreakParams;

        private static readonly ProfilingSampler _vlVirtualEffectSampler = new ProfilingSampler("VL Virtual Effect");

        private RTHandle m_AdditiveTextureToAdd;

        private RTHandle[] m_AuraMipDown;

        private RTHandle[] m_AuraMipUp;

        private static readonly ProfilingSampler _vlChromaticAberrationSampler = new ProfilingSampler("VL ChromaticAberration");

        private Vector2 _offsetUV;

        private Vector2 _redDirection;

        private Vector2 _greenDirection;

        private Vector2 _blueDirection;

        private static readonly ProfilingSampler _vlNoiseSampler = new ProfilingSampler("VL Noise");

        public bool UseComputeGrading
        {
            get { return _computeGradingAvailable && m_Data is VLPostProcessData data && data.useComputeGrading; }
        }

        public VLPostProcessPass(RenderPassEvent evt, VLPostProcessData data, ref PostProcessParams postProcessParams, bool computeGradingAvailable) : base(evt, data, ref postProcessParams)
        {
            m_VLMaterials = new VLMaterialLibrary(data);
            m_RequestHDRFormat = postProcessParams.requestHDRFormat;
            SetDefaultColorFormat(false);
            InitializeVirtualEffect();
            bool supportsHDR = RenderingUtils.SupportsGraphicsFormat(GraphicsFormat.B10G11R11_UFloatPack32, FormatUsage.Blend);
            _vlDOFSupported = VL.Rendering.RenderingUtility.IsSupportedDevice() && (supportsHDR & (SystemInfo.supportedRenderTargetCount > 2));
            m_DOFMedium = new DOFHandles("_DOFMedium");
            m_DOFLow = new DOFHandles("_DOFLow");
            m_DOFTemp1 = new DOFHandles("_DOFTemp1");
            m_DOFTemp2 = new DOFHandles("_DOFTemp2");
            m_TileRTs = new RTHandle[3];
            for (int i = 0; i < m_TileRTs.Length; i++)
            {
                string name = string.Format("_TileRT{0}", 1 << (i + 1));
                m_TileRTs[i] = RTHandles.Alloc(Shader.PropertyToID(name), name);
            }
            _computeGradingAvailable = computeGradingAvailable;
        }

        private void SetDefaultColorFormat(bool needsAlphaChannel)
        {
            if (needsAlphaChannel)
            {
                m_DefaultHDRFormat = GraphicsFormat.R16G16B16A16_SFloat;
                m_UseRGBM = false;
            }
            else if (RenderingUtils.SupportsGraphicsFormat(m_RequestHDRFormat, FormatUsage.Blend))
            {
                m_DefaultHDRFormat = m_RequestHDRFormat;
                m_UseRGBM = false;
            }
            else if (RenderingUtils.SupportsGraphicsFormat(GraphicsFormat.B10G11R11_UFloatPack32, FormatUsage.Blend))
            {
                m_DefaultHDRFormat = GraphicsFormat.B10G11R11_UFloatPack32;
                m_UseRGBM = false;
            }
            else
            {
                m_DefaultHDRFormat = QualitySettings.activeColorSpace == ColorSpace.Linear
                    ? GraphicsFormat.R8G8B8A8_SRGB : GraphicsFormat.R8G8B8A8_UNorm;
                m_UseRGBM = true;
            }
        }

        public override void Cleanup()
        {
            base.Cleanup();
            m_VLMaterials.Cleanup();
        }

        public override void Dispose()
        {
            base.Dispose();
            m_DownscaleAdditiveTexture?.Release();
            m_DiffusionTexture?.Release();
            m_DiffusionTemp?.Release();
            m_DOFCocTexture?.Release();
            m_DOFMedium?.Release();
            m_DOFLow?.Release();
            m_DOFTemp1?.Release();
            m_DOFTemp2?.Release();
            m_DOFInflateCoc1?.Release();
            m_DOFInflateCoc2?.Release();
            DisposeVirtualEffect();
            DisposeJumpFloodOutline();
            m_VelocityTex?.Release();
            for (int i = 0; i < m_TileRTs.Length; i++)
                m_TileRTs[i]?.Release();
            m_TileVRT?.Release();
            m_NeighborMaxTex?.Release();
            m_ChromaticAberrationTexture?.Release();
        }

        public override void Setup(in RenderTextureDescriptor baseDescriptor, in RTHandle source, bool resolveToScreen, in RTHandle depth, in RTHandle internalLut, in RTHandle motionVectors, bool hasFinalPass, bool enableColorEncoding)
        {
            base.Setup(baseDescriptor, source, resolveToScreen, depth, internalLut, motionVectors, hasFinalPass, enableColorEncoding);
            SetDefaultColorFormat(GraphicsFormatUtility.HasAlphaChannel(baseDescriptor.graphicsFormat));
        }

        internal override void Render(CommandBuffer cmd, ref RenderingData renderingData)
        {
            ref CameraData cameraData = ref renderingData.cameraData;
            ref ScriptableRenderer renderer = ref cameraData.renderer;
            bool isSceneViewCamera = cameraData.isSceneViewCamera;

            bool useStopNan = cameraData.isStopNaNEnabled && m_Materials.stopNaN != null;
            bool useSubPixeMorpAA = cameraData.antialiasing == AntialiasingMode.SubpixelMorphologicalAntiAliasing && SystemInfo.graphicsDeviceType != GraphicsDeviceType.OpenGLES2;
            var dofMaterial = m_DepthOfField.mode.value == DepthOfFieldMode.Gaussian ? m_Materials.gaussianDepthOfField : m_Materials.bokehDepthOfField;
            bool useDepthOfField = m_DepthOfField.IsActive() && !isSceneViewCamera && dofMaterial != null;
            bool useLensFlare = !LensFlareCommonSRP.Instance.IsEmpty() && m_SupportDataDrivenLensFlare;
            bool useMotionBlur = m_MotionBlur.IsActive() && !isSceneViewCamera;
            bool usePaniniProjection = m_PaniniProjection.IsActive() && !isSceneViewCamera;

            var stack = VolumeManager.instance.stack;
            var virtualEffect = stack.GetComponent<VLVirtualEffect>();
            bool useVirtualNoise = virtualEffect.IsActiveNoise();
            bool useVirtualChromaticAberration = virtualEffect.IsActiveChromaticAberration();
            bool useRGBBlockNoise = stack.GetComponent<VLRGBBlockNoise>().IsActive();
            bool useWaveJitterGlitch = stack.GetComponent<VLWaveJitterGlitch>().IsActive();
            bool useJumpFloodOutline = stack.GetComponent<VLJumpFloodOutline>().IsActive();
            bool useTemporalAA = cameraData.IsTemporalAAEnabled();
            if (cameraData.antialiasing == AntialiasingMode.TemporalAntiAliasing && !useTemporalAA)
                TemporalAA.ValidateAndWarn(ref cameraData);

            int amountOfPassesRemaining = (useStopNan ? 1 : 0) + (useSubPixeMorpAA ? 1 : 0) + (useDepthOfField ? 1 : 0) + (useLensFlare ? 1 : 0) + (useTemporalAA ? 1 : 0) + (useMotionBlur ? 1 : 0) + (usePaniniProjection ? 1 : 0)
                + (useVirtualNoise ? 1 : 0) + (useVirtualChromaticAberration ? 1 : 0)
                + (useRGBBlockNoise ? 1 : 0) + (useWaveJitterGlitch ? 1 : 0) + (useJumpFloodOutline ? 1 : 0);

            if (m_UseSwapBuffer && amountOfPassesRemaining != 0)
            {
                renderer.EnableSwapBufferMSAA(false);
            }

            RTHandle source = m_UseSwapBuffer ? renderer.cameraColorTargetHandle : m_Source;
            RTHandle destination = m_UseSwapBuffer ? renderer.GetCameraColorFrontBuffer(cmd) : null;

            RTHandle GetSource() => source;

            RTHandle GetDestination()
            {
                if (destination == null)
                {
                    RenderingUtils.ReAllocateIfNeeded(ref m_TempTarget, GetCompatibleDescriptor(), FilterMode.Bilinear, TextureWrapMode.Clamp, name: "_TempTarget");
                    destination = m_TempTarget;
                }
                else if (destination == m_Source && m_Descriptor.msaaSamples > 1)
                {
                    RenderingUtils.ReAllocateIfNeeded(ref m_TempTarget2, GetCompatibleDescriptor(), FilterMode.Bilinear, TextureWrapMode.Clamp, name: "_TempTarget2");
                    destination = m_TempTarget2;
                }
                return destination;
            }

            void Swap(ref ScriptableRenderer r)
            {
                --amountOfPassesRemaining;
                if (m_UseSwapBuffer)
                {
                    r.SwapColorBuffer(cmd);
                    source = r.cameraColorTargetHandle;
                    if (amountOfPassesRemaining == 0 && !m_HasFinalPass)
                        r.EnableSwapBufferMSAA(true);
                    destination = r.GetCameraColorFrontBuffer(cmd);
                }
                else
                {
                    CoreUtils.Swap(ref source, ref destination);
                }
            }

            cmd.SetGlobalMatrix(ShaderConstants._FullscreenProjMat, GL.GetGPUProjectionMatrix(Matrix4x4.identity, true));

            if (useStopNan)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(URPProfileId.StopNaNs)))
                {
                    Blitter.BlitCameraTexture(cmd, GetSource(), GetDestination(), RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, m_Materials.stopNaN, 0);
                    Swap(ref renderer);
                }
            }

            if (useSubPixeMorpAA)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(URPProfileId.SMAA)))
                {
                    DoSubpixelMorphologicalAntialiasing(ref cameraData, cmd, GetSource(), GetDestination());
                    Swap(ref renderer);
                }
            }

            if (useTemporalAA)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(URPProfileId.TemporalAA)))
                {

                    TemporalAA.ExecutePass(cmd, m_Materials.temporalAntialiasing, ref cameraData, source, destination, m_MotionVectors.rt);
                    Swap(ref renderer);
                }
            }

            if (useDepthOfField)
            {
                var markerName = m_DepthOfField.mode.value == DepthOfFieldMode.Gaussian
                    ? URPProfileId.GaussianDepthOfField
                    : URPProfileId.BokehDepthOfField;

                using (new ProfilingScope(cmd, ProfilingSampler.Get(markerName)))
                {
                    DoDepthOfField(cameraData.camera, cmd, GetSource(), GetDestination(), cameraData.pixelRect);
                    Swap(ref renderer);
                }
            }

            else if (!cameraData.isSceneViewCamera && _vlDOFSupported)
            {
                using (new ProfilingScope(cmd, _vlDOFProfilingSampler))
                {
                    if (DoVLDOF(cmd, GetSource(), GetDestination(), ref cameraData))
                        Swap(ref renderer);
                }
            }
            if (!isSceneViewCamera && !cameraData.resetHistory)
            {
                using (new ProfilingScope(cmd, _vlMotionBlurSampler))
                {
                    if (DoVLMotionBlur(cmd, GetSource(), GetDestination()))
                        Swap(ref renderer);
                }
            }
            using (new ProfilingScope(cmd, _vlTextureBlurSampler))
            {
                if (DoVLTextureBlur(cmd, GetSource(), GetDestination()))
                    Swap(ref renderer);
            }

            if (useMotionBlur)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(URPProfileId.MotionBlur)))
                {
                    DoMotionBlur(cmd, GetSource(), GetDestination(), ref cameraData);
                    Swap(ref renderer);
                }
            }

            if (usePaniniProjection)
            {
                using (new ProfilingScope(cmd, ProfilingSampler.Get(URPProfileId.PaniniProjection)))
                {
                    DoPaniniProjection(cameraData.camera, cmd, GetSource(), GetDestination());
                    Swap(ref renderer);
                }
            }

            using (new ProfilingScope(cmd, ProfilingSampler.Get(URPProfileId.UberPostProcess)))
            {
                m_Materials.uber.shaderKeywords = null;
                if (useJumpFloodOutline)
                {
                    DoJumpFloodOutline(cmd, GetSource(), GetDestination(), cameraData);
                    Swap(ref renderer);
                }

                bool bloomActive = m_Bloom.IsActive();
                if (bloomActive)
                {
                    using (new ProfilingScope(cmd, ProfilingSampler.Get(URPProfileId.Bloom)))
                        SetupBloom(cmd, GetSource(), m_Materials.uber);
                }

                DoVLVirtualEffectAdditiveTextureToAdd(cmd, GetSource(), cameraData.camera);
                DoVLAdditive(cmd, GetSource(), cameraData.camera);
                DoVLVirtualEffectAdditiveTextureToBloom(cmd, m_AdditiveTextureToAdd, m_DownscaleAdditiveTexture);
                if (useVirtualNoise)
                {
                    DoVLVirtualEffectNoise(cmd, GetSource(), GetDestination());
                    Swap(ref renderer);
                }
                if (useVirtualChromaticAberration)
                {
                    DoVLVirtualEffectChromaticAberration(cmd, GetSource(), GetDestination());
                    Swap(ref renderer);
                }
                if (useRGBBlockNoise)
                {
                    DoRGBBlockNoise(cmd, GetSource(), GetDestination());
                    Swap(ref renderer);
                }
                if (useWaveJitterGlitch)
                {
                    DoWaveJitterGlitch(cmd, GetSource(), GetDestination());
                    Swap(ref renderer);
                }

                if (useLensFlare)
                {
                    bool usePanini;
                    float paniniDistance;
                    float paniniCropToFit;
                    if (m_PaniniProjection.IsActive())
                    {
                        usePanini = true;
                        paniniDistance = m_PaniniProjection.distance.value;
                        paniniCropToFit = m_PaniniProjection.cropToFit.value;
                    }
                    else
                    {
                        usePanini = false;
                        paniniDistance = 1.0f;
                        paniniCropToFit = 1.0f;
                    }

                    using (new ProfilingScope(cmd, ProfilingSampler.Get(URPProfileId.LensFlareDataDrivenComputeOcclusion)))
                    {
                        LensFlareDataDrivenComputeOcclusion(cameraData.camera, cmd, GetSource(), usePanini, paniniDistance, paniniCropToFit);
                    }
                    using (new ProfilingScope(cmd, ProfilingSampler.Get(URPProfileId.LensFlareDataDriven)))
                    {
                        LensFlareDataDriven(cameraData.camera, cmd, GetSource(), usePanini, paniniDistance, paniniCropToFit);
                    }
                }

                SetupLensDistortion(m_Materials.uber, isSceneViewCamera);
                SetupChromaticAberration(m_Materials.uber);
                SetupVignette(m_Materials.uber, cameraData.xr);
                SetupColorGrading(cmd, ref renderingData, m_Materials.uber);

                SetupGrain(ref cameraData, m_Materials.uber);
                SetupDithering(ref cameraData, m_Materials.uber);

                cmd.SetProjectionMatrix(cameraData.GetProjectionMatrixNoJitter());

                if (RequireSRGBConversionBlitToBackBuffer(ref cameraData))
                    m_Materials.uber.EnableKeyword(ShaderKeywordStrings.LinearToSRGBConversion);

                bool requireHDROutput = RequireHDROutput(ref cameraData);
                if (requireHDROutput)
                {
                    HDROutputUtils.Operation hdrOperation = !m_HasFinalPass && m_EnableColorEncodingIfNeeded ? HDROutputUtils.Operation.ColorEncoding : HDROutputUtils.Operation.None;
                    SetupHDROutput(cameraData.hdrDisplayInformation, cameraData.hdrDisplayColorGamut, m_Materials.uber, hdrOperation);
                }

                if (m_UseFastSRGBLinearConversion)
                {
                    m_Materials.uber.EnableKeyword(ShaderKeywordStrings.UseFastSRGBLinearConversion);
                }

                DebugHandler debugHandler = GetActiveDebugHandler(ref renderingData);
                bool resolveToDebugScreen = debugHandler != null && debugHandler.WriteToDebugScreenTexture(ref cameraData);

                var colorLoadAction = RenderBufferLoadAction.DontCare;
                if (m_Destination == k_CameraTarget && !cameraData.isDefaultViewport)
                    colorLoadAction = RenderBufferLoadAction.Load;

                RenderTargetIdentifier cameraTargetID = BuiltinRenderTextureType.CameraTarget;
            #if ENABLE_VR && ENABLE_XR_MODULE
                if (cameraData.xr.enabled)
                    cameraTargetID = cameraData.xr.renderTarget;
            #endif

                if (!m_UseSwapBuffer)
                    m_ResolveToScreen = cameraData.resolveFinalTarget || m_Destination.nameID == cameraTargetID || m_HasFinalPass == true;

                if (cameraData.upscalingFilter == ImageUpscalingFilter.FSR)
                    RenderFinalPassWithFSR(cmd, ref renderingData, GetSource());
                else if (m_UseSwapBuffer && !m_ResolveToScreen)
                {
                    bool useChromaticAberration = VolumeManager.instance.stack.GetComponent<VLChromaticAberration>().IsActive();
                    var descriptor = cameraData.cameraTargetDescriptor;
                    descriptor.msaaSamples = 1;
                    descriptor.depthBufferBits = 0;
                    if (useChromaticAberration)
                    {
                        DoChromaticAberration(cmd, GetSource(), GetDestination());
                        Swap(ref renderer);
                    }
                    descriptor.width = cameraData.pixelWidth;
                    descriptor.height = cameraData.pixelHeight;
                    SetupVLDiffusion(cmd, m_Materials.uber, GetSource(), descriptor);
                    if (!m_HasFinalPass)
                    {
                        renderer.EnableSwapBufferMSAA(true);
                        destination = renderer.GetCameraColorFrontBuffer(cmd);
                    }
                    Blitter.BlitCameraTexture(cmd, GetSource(), destination, colorLoadAction, RenderBufferStoreAction.Store, m_Materials.uber, 0);
                    renderer.ConfigureCameraColorTarget(destination);
                    Swap(ref renderer);
                }
                else if (!m_UseSwapBuffer)
                {
                    var firstSource = GetSource();
                    Blitter.BlitCameraTexture(cmd, firstSource, GetDestination(), colorLoadAction, RenderBufferStoreAction.Store, m_Materials.uber, 0);
                    Blitter.BlitCameraTexture(cmd, GetDestination(), m_Destination, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, m_BlitMaterial, m_Destination.rt?.filterMode == FilterMode.Bilinear ? 1 : 0);
                }
                else if (m_ResolveToScreen)
                {
                    if (resolveToDebugScreen)
                    {
                        Blitter.BlitCameraTexture(cmd, GetSource(), debugHandler.DebugScreenColorHandle, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store, m_Materials.uber, 0);
                        renderer.ConfigureCameraTarget(debugHandler.DebugScreenColorHandle, debugHandler.DebugScreenDepthHandle);
                    }
                    else
                    {
                        RenderTargetIdentifier cameraTarget = cameraData.targetTexture != null ? new RenderTargetIdentifier(cameraData.targetTexture) : cameraTargetID;
                        RTHandleStaticHelpers.SetRTHandleStaticWrapper(cameraTarget);
                        var cameraTargetHandle = RTHandleStaticHelpers.s_RTHandleWrapper;

                        RenderingUtils.FinalBlit(cmd, ref cameraData, GetSource(), cameraTargetHandle, colorLoadAction, RenderBufferStoreAction.Store, m_Materials.uber, 0);
                        renderer.ConfigureCameraColorTarget(cameraTargetHandle);
                    }
                }
            }
        }

        private void RenderFinalPassWithFSR(CommandBuffer cmd, ref RenderingData renderingData, RTHandle sourceTex)
        {
            ref var cameraData = ref renderingData.cameraData;
            Material material = m_Materials.finalPass;
            material.shaderKeywords = null;
            PostProcessUtils.SetSourceSize(cmd, cameraData.cameraTargetDescriptor);
            SetupGrain(ref cameraData, material);
            SetupDithering(ref cameraData, material);
            if (RequireSRGBConversionBlitToBackBuffer(ref cameraData))
                material.EnableKeyword(ShaderKeywordStrings.LinearToSRGBConversion);
            bool requireHDROutput = RequireHDROutput(ref cameraData);
            if (requireHDROutput)
            {
                HDROutputUtils.Operation operations = m_EnableColorEncodingIfNeeded
                    ? HDROutputUtils.Operation.ColorEncoding : HDROutputUtils.Operation.None;
                if (!cameraData.postProcessEnabled)
                    operations |= HDROutputUtils.Operation.ColorConversion;
                SetupHDROutput(cameraData.hdrDisplayInformation, cameraData.hdrDisplayColorGamut, material, operations);
            }
            DebugHandler debugHandler = GetActiveDebugHandler(ref renderingData);
            bool resolveToDebugScreen = debugHandler != null && debugHandler.WriteToDebugScreenTexture(ref cameraData);
            var colorLoadAction = cameraData.isDefaultViewport ? RenderBufferLoadAction.DontCare : RenderBufferLoadAction.Load;
            var descriptor = cameraData.cameraTargetDescriptor;
            descriptor.msaaSamples = 1;
            descriptor.depthBufferBits = 0;
            if (requireHDROutput)
            {
                m_Materials.scalingSetup.shaderKeywords = null;
                SetupHDROutput(cameraData.hdrDisplayInformation, cameraData.hdrDisplayColorGamut, m_Materials.scalingSetup, HDROutputUtils.Operation.None);
            }
            else
            {
                descriptor.graphicsFormat = UniversalRenderPipeline.MakeUnormRenderTextureGraphicsFormat();
                m_Materials.scalingSetup.shaderKeywords = null;
            }
            m_Materials.uber.EnableKeyword(ShaderKeywordStrings.Gamma20);
            var upscaleDescriptor = descriptor;
            upscaleDescriptor.width = cameraData.pixelWidth;
            upscaleDescriptor.height = cameraData.pixelHeight;
            using (new ProfilingScope(cmd, fsrUpscalingSampler))
            {
                RenderingUtils.ReAllocateIfNeeded(ref m_ScalingSetupTarget, descriptor, FilterMode.Point, TextureWrapMode.Clamp, name: "_ScalingSetupTexture");
                Blitter.BlitCameraTexture(cmd, sourceTex, m_ScalingSetupTarget, colorLoadAction, RenderBufferStoreAction.Store, m_Materials.uber, 0);
                if (VolumeManager.instance.stack.GetComponent<VLChromaticAberration>().IsActive())
                {
                    RenderingUtils.ReAllocateIfNeeded(ref m_ChromaticAberrationTexture, descriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, name: "_ChromaticAberrationTexture");
                    DoChromaticAberration(cmd, m_ScalingSetupTarget, m_ChromaticAberrationTexture);
                    sourceTex = m_ChromaticAberrationTexture;
                }
                else
                    sourceTex = m_ScalingSetupTarget;
                m_Materials.easu.shaderKeywords = null;
                RenderingUtils.ReAllocateIfNeeded(ref m_UpscaledTarget, upscaleDescriptor, FilterMode.Point, TextureWrapMode.Clamp, name: "_UpscaledTexture");
                var inputSize = new Vector2(cameraData.cameraTargetDescriptor.width, cameraData.cameraTargetDescriptor.height);
                var outputSize = new Vector2(cameraData.pixelWidth, cameraData.pixelHeight);
                FSRUtils.SetEasuConstants(cmd, inputSize, inputSize, outputSize);
                Blitter.BlitCameraTexture(cmd, sourceTex, m_UpscaledTarget, colorLoadAction, RenderBufferStoreAction.Store, m_Materials.easu, 0);
                float sharpness = cameraData.fsrOverrideSharpness ? cameraData.fsrSharpness : FSRUtils.kDefaultSharpnessLinear;
                if (cameraData.fsrSharpness > 0f)
                {
                    material.EnableKeyword(requireHDROutput ? ShaderKeywordStrings.EasuRcasAndHDRInput : ShaderKeywordStrings.Rcas);
                    FSRUtils.SetRcasConstantsLinear(cmd, sharpness);
                }
                sourceTex = m_UpscaledTarget;
                PostProcessUtils.SetSourceSize(cmd, upscaleDescriptor);
            }
            var cameraTarget = RenderingUtils.GetCameraTargetIdentifier(ref renderingData);
            if (resolveToDebugScreen)
            {
                Blitter.BlitCameraTexture(cmd, sourceTex, debugHandler.DebugScreenColorHandle, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store, material, 0);
                cameraData.renderer.ConfigureCameraTarget(debugHandler.DebugScreenColorHandle, debugHandler.DebugScreenDepthHandle);
            }
            else
            {
                SetupVLDiffusion(cmd, material, sourceTex, upscaleDescriptor);
                RTHandleStaticHelpers.SetRTHandleStaticWrapper(cameraTarget);
                var cameraTargetHandle = RTHandleStaticHelpers.s_RTHandleWrapper;
                RenderingUtils.FinalBlit(cmd, ref cameraData, sourceTex, cameraTargetHandle, colorLoadAction, RenderBufferStoreAction.Store, material, 0);
                cameraData.renderer.ConfigureCameraTarget(cameraTargetHandle, cameraTargetHandle);
            }
        }

        internal override void RenderFinalPass(CommandBuffer cmd, ref RenderingData renderingData)
        {
            ref var cameraData = ref renderingData.cameraData;
            var material = m_Materials.finalPass;
            material.shaderKeywords = null;
            if (cameraData.postProcessEnabled && cameraData.upscalingFilter == ImageUpscalingFilter.FSR)
                return;

            PostProcessUtils.SetSourceSize(cmd, cameraData.cameraTargetDescriptor);

            SetupGrain(ref cameraData, material);
            SetupDithering(ref cameraData, material);

            if (RequireSRGBConversionBlitToBackBuffer(ref cameraData))
                material.EnableKeyword(ShaderKeywordStrings.LinearToSRGBConversion);

            HDROutputUtils.Operation hdrOperations = HDROutputUtils.Operation.None;
            bool requireHDROutput = RequireHDROutput(ref cameraData);
            if (requireHDROutput)
            {
                hdrOperations = m_EnableColorEncodingIfNeeded ? HDROutputUtils.Operation.ColorEncoding : HDROutputUtils.Operation.None;
                if (!cameraData.postProcessEnabled)
                    hdrOperations |= HDROutputUtils.Operation.ColorConversion;

                SetupHDROutput(cameraData.hdrDisplayInformation, cameraData.hdrDisplayColorGamut, material, hdrOperations);
            }

            DebugHandler debugHandler = GetActiveDebugHandler(ref renderingData);
            bool resolveToDebugScreen = debugHandler != null && debugHandler.WriteToDebugScreenTexture(ref cameraData);

            if (m_UseSwapBuffer)
                m_Source = cameraData.renderer.GetCameraColorBackBuffer(cmd);

            RTHandle sourceTex = m_Source;

            var colorLoadAction = cameraData.isDefaultViewport ? RenderBufferLoadAction.DontCare : RenderBufferLoadAction.Load;

            bool isFxaaEnabled = (cameraData.antialiasing == AntialiasingMode.FastApproximateAntialiasing);

            bool isFsrEnabled = ((cameraData.imageScalingMode == ImageScalingMode.Upscaling) && (cameraData.upscalingFilter == ImageUpscalingFilter.FSR));

            bool isTaaSharpeningEnabled = (cameraData.IsTemporalAAEnabled() && cameraData.taaSettings.contrastAdaptiveSharpening > 0.0f) && !isFsrEnabled;

            if (cameraData.imageScalingMode != ImageScalingMode.None)
            {
                bool isSetupRequired = (isFxaaEnabled || isFsrEnabled);

                var tempRtDesc = cameraData.cameraTargetDescriptor;
                tempRtDesc.msaaSamples = 1;
                tempRtDesc.depthBufferBits = 0;

                if (!requireHDROutput)
                    tempRtDesc.graphicsFormat = UniversalRenderPipeline.MakeUnormRenderTextureGraphicsFormat();

                m_Materials.scalingSetup.shaderKeywords = null;

                if (isSetupRequired)
                {
                    if (requireHDROutput)
                    {
                        SetupHDROutput(cameraData.hdrDisplayInformation, cameraData.hdrDisplayColorGamut, m_Materials.scalingSetup, HDROutputUtils.Operation.None);
                    }

                    if (isFxaaEnabled && isFsrEnabled)
                    {
                        m_Materials.scalingSetup.EnableKeyword(ShaderKeywordStrings.FxaaAndGamma20);
                    }
                    else if (isFxaaEnabled)
                    {
                        m_Materials.scalingSetup.EnableKeyword(ShaderKeywordStrings.Fxaa);
                    }
                    else if (isFsrEnabled)
                    {
                        m_Materials.scalingSetup.EnableKeyword(ShaderKeywordStrings.Gamma20);
                    }

                    RenderingUtils.ReAllocateIfNeeded(ref m_ScalingSetupTarget, tempRtDesc, FilterMode.Point, TextureWrapMode.Clamp, name: "_ScalingSetupTexture");
                    Blitter.BlitCameraTexture(cmd, m_Source, m_ScalingSetupTarget, colorLoadAction, RenderBufferStoreAction.Store, m_Materials.scalingSetup, 0);

                    sourceTex = m_ScalingSetupTarget;
                }

                switch (cameraData.imageScalingMode)
                {
                    case ImageScalingMode.Upscaling:
                    {
                        switch (cameraData.upscalingFilter)
                        {
                            case ImageUpscalingFilter.Point:
                            {
                                if(!isTaaSharpeningEnabled)
                                    material.EnableKeyword(ShaderKeywordStrings.PointSampling);
                                break;
                            }

                            case ImageUpscalingFilter.Linear:
                            {
                                break;
                            }

                            case ImageUpscalingFilter.FSR:
                            {
                                m_Materials.easu.shaderKeywords = null;

                                var upscaleRtDesc = tempRtDesc;
                                upscaleRtDesc.width = cameraData.pixelWidth;
                                upscaleRtDesc.height = cameraData.pixelHeight;

                                RenderingUtils.ReAllocateIfNeeded(ref m_UpscaledTarget, upscaleRtDesc, FilterMode.Point, TextureWrapMode.Clamp, name: "_UpscaledTexture");
                                var fsrInputSize = new Vector2(cameraData.cameraTargetDescriptor.width, cameraData.cameraTargetDescriptor.height);
                                var fsrOutputSize = new Vector2(cameraData.pixelWidth, cameraData.pixelHeight);
                                FSRUtils.SetEasuConstants(cmd, fsrInputSize, fsrInputSize, fsrOutputSize);

                                Blitter.BlitCameraTexture(cmd, sourceTex, m_UpscaledTarget, colorLoadAction, RenderBufferStoreAction.Store, m_Materials.easu, 0);

                                float sharpness = cameraData.fsrOverrideSharpness ? cameraData.fsrSharpness : FSRUtils.kDefaultSharpnessLinear;

                                if (cameraData.fsrSharpness > 0.0f)
                                {
                                    material.EnableKeyword(requireHDROutput ? ShaderKeywordStrings.EasuRcasAndHDRInput : ShaderKeywordStrings.Rcas);
                                    FSRUtils.SetRcasConstantsLinear(cmd, sharpness);
                                }

                                sourceTex = m_UpscaledTarget;
                                PostProcessUtils.SetSourceSize(cmd, upscaleRtDesc);

                                break;
                            }
                        }

                        break;
                    }

                    case ImageScalingMode.Downscaling:
                    {

                        isTaaSharpeningEnabled = false;

                        break;
                    }
                }
            }
            else if (isFxaaEnabled)
            {
                material.EnableKeyword(ShaderKeywordStrings.Fxaa);
            }

            if(isTaaSharpeningEnabled)
            {
                material.EnableKeyword(ShaderKeywordStrings.Rcas);
                FSRUtils.SetRcasConstantsLinear(cmd, cameraData.taaSettings.contrastAdaptiveSharpening);
            }

            var cameraTarget = RenderingUtils.GetCameraTargetIdentifier(ref renderingData);

            if (resolveToDebugScreen)
            {
                Blitter.BlitCameraTexture(cmd, sourceTex, debugHandler.DebugScreenColorHandle, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store, material, 0);
                cameraData.renderer.ConfigureCameraTarget(debugHandler.DebugScreenColorHandle, debugHandler.DebugScreenDepthHandle);
            }
            else
            {
                RTHandleStaticHelpers.SetRTHandleStaticWrapper(cameraTarget);
                var cameraTargetHandle = RTHandleStaticHelpers.s_RTHandleWrapper;
                RenderingUtils.FinalBlit(cmd, ref cameraData, sourceTex, cameraTargetHandle, colorLoadAction, RenderBufferStoreAction.Store, material, 0);
            }
        }

        protected override void SetupColorGrading(CommandBuffer cmd, ref RenderingData renderingData, Material material)
        {
            base.SetupColorGrading(cmd, ref renderingData, material);
            if (UseComputeGrading)
            {
                int lutSize = renderingData.postProcessingData.lutSize;
                float postExposure = Mathf.Pow(2f, m_ColorAdjustments.postExposure.value);
                material.SetVector(ShaderConstants._Lut_Params, new Vector4(1f / lutSize, lutSize - 1f, 0f, postExposure));
                material.EnableKeyword("_3D_GRADING");
            }
        }

        private bool DoVLDOF(CommandBuffer cmd, RTHandle source, RTHandle destination, ref CameraData cameraData)
        {
            var dof = VolumeManager.instance.stack.GetComponent<VLDOF>();
            if (!dof.IsActive())
                return false;
            int width = m_Descriptor.width;
            int height = m_Descriptor.height;
            GraphicsFormat format = RenderingUtils.SupportsGraphicsFormat(GraphicsFormat.R16_UNorm, FormatUsage.Blend)
                ? GraphicsFormat.R16_UNorm : GraphicsFormat.R8_UNorm;
            var descriptor = GetCompatibleDescriptor(m_Descriptor.width, m_Descriptor.height, format);
            RenderingUtils.ReAllocateIfNeeded(ref m_DOFCocTexture, descriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, name: "_DOFCocTexture");
            m_DOFMedium.ReAllocateIfNeeded(width >> 1, height >> 1, GraphicsFormat.R16G16B16A16_SFloat);
            m_DOFTemp1.ReAllocateIfNeeded(width >> 1, height >> 1, GraphicsFormat.R16G16B16A16_SFloat);
            if (dof.quality.value == Dof34QualitySetting.Bokeh)
            {
                DrawVLDOFBokeh(dof, cmd, ref cameraData, source, destination);
                return true;
            }
            return DrawVLDOFMRT(dof, cmd, ref cameraData, source, destination);
        }

        public static void SetDOFCurveProperty(CommandBuffer cmd, VLDOF dof, Camera camera, Vector3 worldCameraPosition, out bool blurBackground, out bool blurForeground)
        {
            blurBackground = dof.quality.value > Dof34QualitySetting.None;
            blurForeground = dof.quality.value > Dof34QualitySetting.OnlyBackground;
            float near = camera.nearClipPlane;
            float far = camera.farClipPlane;
            blurForeground = blurForeground && dof.focalPoint.value > near + Mathf.Epsilon;
            Vector3 focalPosition = camera.WorldToViewportPoint(worldCameraPosition + camera.transform.forward * dof.focalPoint.value);
            float focalDistance = focalPosition.z / far;
            float smoothness;
            float backgroundRange;
            float foregroundRange;
            if (dof.useFNumber.value)
            {
                float focalLength = camera.usePhysicalProperties ? camera.focalLength
                    : 12f / Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.PI / 180f);
                var depthOfField = new DepthOfFieldData(focalPosition.z, focalLength, dof.fNumber.value);
                float range = depthOfField.range;
                backgroundRange = depthOfField.backgroundRange / (far - near);
                foregroundRange = depthOfField.foregroundRange / (far - near);
                smoothness = focalDistance * Mathf.Max(range * 0.5f, 0.05f);
                blurBackground &= !depthOfField.infinityBackground;
                blurForeground &= !depthOfField.infinityForeground;
            }
            else
            {
                smoothness = focalDistance * dof.smoothness.value;
                foregroundRange = dof.focalSize.value / (far - near);
                backgroundRange = foregroundRange;
                if (foregroundRange > 0.99f)
                {
                    blurBackground = false;
                    blurForeground = false;
                }
            }
            float blurSpread = Mathf.Min(dof.maxBlurSpread.value, 0.5f) * 2f;
            if (Mathf.Approximately(0f, blurSpread))
                blurBackground = false;
            cmd.SetGlobalVector(VLDOFShaderConstants._DOFCocOffset, new Vector4(-(1f - blurSpread) * (1f - blurSpread), 0f, 0f, 0f));
            cmd.SetGlobalVector(VLDOFShaderConstants._DOFCurveParams, new Vector4(
                1f / smoothness, blurForeground ? foregroundRange : 2f, blurBackground ? backgroundRange : 2f, focalDistance));
        }

        private bool DrawVLDOFMRT(VLDOF vlDof, CommandBuffer cmd, ref CameraData cameraData, RTHandle source, RTHandle destination)
        {
            SetDOFCurveProperty(cmd, vlDof, cameraData.camera, cameraData.worldSpaceCameraPos, out bool blurBackground, out bool blurForeground);
            bool active = blurBackground | blurForeground;
            if (!active)
                return active;
            Material material = m_VLMaterials.vlDOF;
            int width = m_Descriptor.width;
            int height = m_Descriptor.height;
            material.SetFloat(VLDOFShaderConstants._ForegroundBlurExtrude, vlDof.foregroundBlurExtrude.value);
            float maxBlurSpread = Mathf.Max(vlDof.maxBlurSpread.value, 0.5f);
            CoreUtils.SetKeyword(material, "_VLDOF_FORE", blurForeground);
            cmd.SetRenderTarget(m_DOFCocTexture.nameID, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
            Blitter.BlitTexture(cmd, new Vector4(Vector2.one.x, Vector2.one.y, 0f, 0f), material, (int)VLDOFPasses.CalcCoc);
            cmd.SetGlobalTexture(ShaderConstants._FullCoCTexture, m_DOFCocTexture.nameID);
            int lowWidth = width >> 2;
            int lowHeight = height >> 2;
            m_DOFLow.ReAllocateIfNeeded(lowWidth, lowHeight, GraphicsFormat.R16G16B16A16_SFloat);
            m_DOFTemp2.ReAllocateIfNeeded(lowWidth, lowHeight, GraphicsFormat.R16G16B16A16_SFloat);
            DofMRT2Bindings.colorRenderTargets[0] = m_DOFMedium.far.nameID;
            DofMRT2Bindings.colorRenderTargets[1] = m_DOFMedium.near.nameID;
            DofMRT2Bindings.depthRenderTarget = m_DOFMedium.far.nameID;
            cmd.SetRenderTarget(DofMRT2Bindings);
            VLDOFDownsampleMRT(cmd, source, material, width >> 1, height >> 1, VLDOFPasses.DownsampleMRT);
            float aspect = (float)width / height;
            VLDOFBlurMRT(cmd, material, m_DOFMedium.far, m_DOFMedium.near, m_DOFMedium.far, m_DOFMedium.near,
                m_DOFTemp1.far, m_DOFTemp1.near, maxBlurSpread, 1f / 512f, aspect, false);
            if (blurBackground)
            {
                cmd.SetRenderTarget(m_DOFLow.far.nameID, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
                VLDOFDownsampleMRT(cmd, m_DOFMedium.far, material, lowWidth, lowHeight, VLDOFPasses.Downsample2MRT);
            }
            VLDOFBlurMRT(cmd, material, m_DOFLow.far, m_DOFMedium.near, m_DOFLow.far, m_DOFLow.near,
                m_DOFTemp2.far, m_DOFTemp2.near, maxBlurSpread, 1f / 512f, aspect, true);
            cmd.SetGlobalTexture(VLDOFShaderConstants._DOFMediumRezWorkTexture, m_DOFMedium.far.nameID);
            cmd.SetGlobalTexture(VLDOFShaderConstants._DOFBackTexture, m_DOFLow.far.nameID);
            cmd.SetGlobalTexture(VLDOFShaderConstants._DOFFrontTexture, m_DOFLow.near.nameID);
            DofMRT2Bindings.colorRenderTargets[0] = m_DOFTemp1.far.nameID;
            DofMRT2Bindings.colorRenderTargets[1] = m_DOFTemp1.near.nameID;
            DofMRT2Bindings.depthRenderTarget = m_DOFTemp1.far.nameID;
            cmd.SetRenderTarget(DofMRT2Bindings);
            Blitter.BlitTexture(cmd, new Vector4(Vector2.one.x, Vector2.one.y, 0f, 0f), material, (int)VLDOFPasses.UpsampleMRT);
            cmd.SetGlobalTexture(VLDOFShaderConstants._DOFBackTexture, m_DOFTemp1.far.nameID);
            cmd.SetGlobalTexture(VLDOFShaderConstants._DOFFrontTexture, m_DOFTemp1.near.nameID);
            cmd.SetGlobalTexture(VLDOFShaderConstants._DOFLowNearTexture, m_DOFLow.near.nameID);
            Blitter.BlitCameraTexture(cmd, source, destination, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, material, (int)VLDOFPasses.ApplyMRT);
            return active;
        }

        private void VLDOFDownsampleMRT(CommandBuffer cmd, RTHandle source, Material dofMaterial, int width, int height, VLDOFPasses pass)
        {
            cmd.SetGlobalVector(VLDOFShaderConstants._InvRenderTargetSize, new Vector4(1f / width, 1f / height, -1f / width, -1f / height));
            Blitter.BlitTexture(cmd, source, new Vector4(Vector2.one.x, Vector2.one.y, 0f, 0f), dofMaterial, (int)pass);
        }

        private void VLDOFBlurMRT(CommandBuffer cmd, Material dofMaterial, RTHandle srcBack, RTHandle srcFront, RTHandle dstBack, RTHandle dstFront, RTHandle tempBack, RTHandle tempFront, float maxBlurSpread, float oneOverBaseSize, float widthOverHeight, bool pass2)
        {
            cmd.SetGlobalVector(VLDOFShaderConstants._DOFBlurOffsets, new Vector4(0f, maxBlurSpread * oneOverBaseSize, 0f, -maxBlurSpread * oneOverBaseSize));
            cmd.SetGlobalTexture(VLDOFShaderConstants._DOFBackTexture, srcBack.nameID);
            cmd.SetGlobalTexture(VLDOFShaderConstants._DOFFrontTexture, srcFront.nameID);
            DofMRT2Bindings.colorRenderTargets[0] = tempBack.nameID;
            DofMRT2Bindings.colorRenderTargets[1] = tempFront.nameID;
            DofMRT2Bindings.depthRenderTarget = tempBack.nameID;
            cmd.SetRenderTarget(DofMRT2Bindings);
            Blitter.BlitTexture(cmd, new Vector4(Vector2.one.x, Vector2.one.y, 0f, 0f), dofMaterial,
                (int)(pass2 ? VLDOFPasses.BlurFirst2MRT : VLDOFPasses.BlurFirstMRT));
            float horizontalOffset = maxBlurSpread / widthOverHeight * oneOverBaseSize;
            cmd.SetGlobalVector(VLDOFShaderConstants._DOFBlurOffsets, new Vector4(horizontalOffset, 0f, -horizontalOffset, 0f));
            cmd.SetGlobalTexture(VLDOFShaderConstants._DOFBackTexture, tempBack.nameID);
            cmd.SetGlobalTexture(VLDOFShaderConstants._DOFFrontTexture, tempFront.nameID);
            DofMRT2Bindings.colorRenderTargets[0] = dstBack.nameID;
            DofMRT2Bindings.colorRenderTargets[1] = dstFront.nameID;
            DofMRT2Bindings.depthRenderTarget = dstBack.nameID;
            cmd.SetRenderTarget(DofMRT2Bindings);
            Blitter.BlitTexture(cmd, new Vector4(Vector2.one.x, Vector2.one.y, 0f, 0f), dofMaterial, (int)VLDOFPasses.BlurSecondMRT);
        }

        protected void PrepareBokehKernel(VLDOF vlDof, Span<Vector4> bokehKernel, float maxRadius, float rcpAspect)
        {
            const int kRings = 4;
            const int kPointsPerRing = 7;
            // Fill in sample points (concentric circles transformed to rotated N-Gon)
            int idx = 0;
            float bladeCount = vlDof.bladeCount.value;
            float curvature = 1f - vlDof.bladeCurvature.value;
            float rotation = vlDof.bladeRotation.value * Mathf.Deg2Rad;
            const float PI = Mathf.PI;
            const float TWO_PI = Mathf.PI * 2f;

            for (int ring = 1; ring < kRings; ring++)
            {
                float bias = 1f / kPointsPerRing;
                float radius = (ring + bias) / (kRings - 1f + bias);
                int points = ring * kPointsPerRing;

                for (int point = 0; point < points; point++)
                {
                    // Angle on ring
                    float phi = 2f * PI * point / points;

                    // Transform to rotated N-Gon
                    // Adapted from "CryEngine 3 Graphics Gems" [Sousa13]
                    float nt = Mathf.Cos(PI / bladeCount);
                    float dt = Mathf.Cos(phi - (TWO_PI / bladeCount) * Mathf.Floor((bladeCount * phi + Mathf.PI) / TWO_PI));
                    float r = radius * Mathf.Pow(nt / dt, curvature);
                    float u = r * Mathf.Cos(phi - rotation);
                    float v = r * Mathf.Sin(phi - rotation);

                    float uRadius = u * maxRadius;
                    float vRadius = v * maxRadius;
                    float uRadiusPowTwo = uRadius * uRadius;
                    float vRadiusPowTwo = vRadius * vRadius;
                    float kernelLength = Mathf.Sqrt((uRadiusPowTwo + vRadiusPowTwo));
                    float uRCP = uRadius * rcpAspect;

                    bokehKernel[idx] = new Vector4(uRadius, vRadius, kernelLength, uRCP);
                    idx++;
                }
            }
        }

        private void SetBokehRenderTarget(CommandBuffer cmd, RTHandle far, RTHandle near, bool blurNearField)
        {
            if (blurNearField)
            {
                DofMRT2Bindings.colorRenderTargets[0] = far.nameID;
                DofMRT2Bindings.colorRenderTargets[1] = near.nameID;
                DofMRT2Bindings.depthRenderTarget = far.nameID;
                cmd.SetRenderTarget(DofMRT2Bindings);
            }
            else
                cmd.SetRenderTarget(far.nameID, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
        }

        private bool DrawVLDOFBokeh(VLDOF vlDof, CommandBuffer cmd, ref CameraData cameraData, RTHandle source, RTHandle destination)
        {
            Material material = m_VLMaterials.vlDOF;
            Vector2 sourceSize = new Vector2(m_Descriptor.width, m_Descriptor.height);
            if (m_Descriptor.useDynamicScale)
                sourceSize *= new Vector2(ScalableBufferManager.widthScaleFactor, ScalableBufferManager.heightScaleFactor);
            int halfWidth = m_Descriptor.width / 2;
            int halfHeight = m_Descriptor.height / 2;
            float focalLength = cameraData.camera.focalLength;
            float fNumber = vlDof.fNumber.value;
            float focalPoint = vlDof.focalPoint.value;
            float maxCoC = (focalLength / 1000f) * (focalLength / fNumber) / (focalPoint - focalLength / 1000f);
            float maxRadius = vlDof.maxBlurSpread.value * 0.5f * 30f / 1080f;
            float rcpAspect = 1f / ((float)halfWidth / halfHeight);
            _bokehData._SourceSize = new Vector4(sourceSize.x, sourceSize.y, 1f / sourceSize.x, 1f / sourceSize.y);
            _bokehData._CoCParams = new Vector4(focalPoint, maxCoC, maxRadius, rcpAspect);
            float foregroundExtrude = vlDof.foregroundBlurExtrude.value;
            float nearAmount = Mathf.Clamp01(Mathf.Min(foregroundExtrude, 0.5f) * 2f);
            bool blurNearField = !Mathf.Approximately(0f, nearAmount);
            _bokehData._BokehConstants = new Vector4(1f / 540f, 1f / 270f, foregroundExtrude, nearAmount * nearAmount);
            PrepareBokehKernel(vlDof, _bokehData.BokehKernelSpan, maxRadius, rcpAspect);
            ConstantBuffer.Push(cmd, _bokehData, material, VLDOFShaderConstants.ShaderVariablesVLDOFBokeh);
            bool useFNumber = vlDof.useFNumber.value;
            if (!useFNumber)
                SetDOFCurveProperty(cmd, vlDof, cameraData.camera, cameraData.worldSpaceCameraPos, out bool blurBackground, out bool blurForeground);
            var quality = VolumeManager.instance.stack.GetComponent<VLDOFBokehQuality>().bokehQuality.value;
            CoreUtils.SetKeyword(material, "_VLDOF_FORE", blurNearField);
            CoreUtils.SetKeyword(material, "_BOKEH_LOW_QUALITY", quality == DofBokehQualitySetting.Low);
            CoreUtils.SetKeyword(material, "_BOKEH_HIGH_QUALITY", quality == DofBokehQualitySetting.High);
            cmd.SetRenderTarget(m_DOFCocTexture.nameID, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
            Blitter.BlitTexture(cmd, new Vector4(Vector2.one.x, Vector2.one.y, 0f, 0f), material,
                (int)(useFNumber ? VLDOFPasses.CalcBokehCoc : VLDOFPasses.CalcCoc));
            RTHandle cocTexture = m_DOFCocTexture;
            if (cameraData.IsTemporalAAEnabled())
            {
                cameraData.taaPersistentData.GrapCocTexture(out RTHandle previousCoc, out RTHandle nextCoc);
                cmd.SetGlobalTexture(VLDOFShaderConstants._HistoryCoCTexture, previousCoc.nameID);
                float influence = cameraData.taaSettings.resetHistoryFrames != 0 ? 1f : 0.14f;
                Vector2 jitter = TemporalAA.CalculateJitter(Time.frameCount) * cameraData.taaSettings.jitterScale;
                material.SetVector(VLDOFShaderConstants._CocReprojectionParams, new Vector4(jitter.x / sourceSize.x, jitter.y / sourceSize.y, 0f, influence));
                Blitter.BlitCameraTexture(cmd, m_DOFCocTexture, nextCoc, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, material, (int)VLDOFPasses.CocReprojection);
                cocTexture = nextCoc;
            }
            cmd.SetGlobalTexture(ShaderConstants._FullCoCTexture, cocTexture.nameID);
            SetBokehRenderTarget(cmd, m_DOFMedium.far, m_DOFMedium.near, blurNearField);
            Blitter.BlitTexture(cmd, source, new Vector4(Vector2.one.x, Vector2.one.y, 0f, 0f), material, (int)VLDOFPasses.BokehPrefilter);
            if (blurNearField)
            {
                var descriptor = new RenderTextureDescriptor(halfWidth >> 1, halfHeight >> 1, GraphicsFormat.R8_UNorm, GraphicsFormat.None);
                RenderingUtils.ReAllocateIfNeeded(ref m_DOFInflateCoc1, descriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, name: "_InflateCoc1");
                descriptor = new RenderTextureDescriptor(halfWidth >> 2, halfHeight >> 2, GraphicsFormat.R8_UNorm, GraphicsFormat.None);
                RenderingUtils.ReAllocateIfNeeded(ref m_DOFInflateCoc2, descriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, name: "_InflateCoc2");
                Blitter.BlitCameraTexture(cmd, m_DOFMedium.near, m_DOFInflateCoc1, material, (int)VLDOFPasses.InflateCocFirst);
                Blitter.BlitCameraTexture(cmd, m_DOFInflateCoc1, m_DOFInflateCoc2, material, (int)VLDOFPasses.InflateCocSecond);
                cmd.SetGlobalTexture(m_DOFInflateCoc2.name, m_DOFInflateCoc2.nameID);
            }
            cmd.SetGlobalTexture(VLDOFShaderConstants._DOFBackTexture, m_DOFMedium.far.nameID);
            cmd.SetGlobalTexture(VLDOFShaderConstants._DOFFrontTexture, m_DOFMedium.near.nameID);
            SetBokehRenderTarget(cmd, m_DOFTemp1.far, m_DOFTemp1.near, blurNearField);
            Blitter.BlitTexture(cmd, new Vector4(Vector2.one.x, Vector2.one.y, 0f, 0f), material, (int)VLDOFPasses.BokehBlurMRT);
            Blitter.BlitCameraTexture(cmd, m_DOFTemp1.far, m_DOFMedium.far, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, material, (int)VLDOFPasses.BokehFloodFill);
            cmd.SetGlobalTexture(VLDOFShaderConstants._DOFBackTexture, m_DOFMedium.far.nameID);
            cmd.SetGlobalTexture(VLDOFShaderConstants._DOFFrontTexture, m_DOFTemp1.near.nameID);
            SetBokehRenderTarget(cmd, m_DOFTemp1.far, m_DOFMedium.near, blurNearField);
            Blitter.BlitTexture(cmd, new Vector4(Vector2.one.x, Vector2.one.y, 0f, 0f), material, (int)VLDOFPasses.BokehPostBlurMRT);
            cmd.SetGlobalTexture(VLDOFShaderConstants._DOFBackTexture, m_DOFTemp1.far.nameID);
            cmd.SetGlobalTexture(VLDOFShaderConstants._DOFFrontTexture, m_DOFMedium.near.nameID);
            Blitter.BlitCameraTexture(cmd, source, destination, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, material, (int)VLDOFPasses.BokehComposite);
            return true;
        }

        public void DoJumpFloodOutline(CommandBuffer cmd, RTHandle source, RTHandle destination, CameraData cameraData)
        {
            var outline = VolumeManager.instance.stack.GetComponent<VLJumpFloodOutline>();
            Material material = m_VLMaterials.vlJumpFloodOutline;
            using (new ProfilingScope(cmd, _vlJumpFloodOutlineSampler))
            {
                int width = m_Descriptor.width;
                int height = m_Descriptor.height;
                int powerOfTwoWidth = Mathf.ClosestPowerOfTwo(width);
                int halfWidth = powerOfTwoWidth / 2;
                int halfHeight = Mathf.CeilToInt((float)(halfWidth * height) / width);
                int quarterWidth = powerOfTwoWidth / 4;
                int quarterHeight = Mathf.CeilToInt((float)(quarterWidth * height) / width);
                var descriptor = GetCompatibleDescriptor(halfWidth, halfHeight, GraphicsFormat.R8_UNorm);
                RenderingUtils.ReAllocateIfNeeded(ref m_OutlineMaskHalfRT, descriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, name: "_OutlineMaskHalf");
                descriptor = GetCompatibleDescriptor(quarterWidth, quarterHeight, GraphicsFormat.R8_UNorm);
                RenderingUtils.ReAllocateIfNeeded(ref m_OutlineMaskQuarterRT, descriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, name: "_OutlineMaskQuarter");
                descriptor = GetCompatibleDescriptor(quarterWidth, quarterHeight, GraphicsFormat.R16G16_SFloat);
                RenderingUtils.ReAllocateIfNeeded(ref m_PingBuffer, descriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, name: "_PingBuffer");
                descriptor = GetCompatibleDescriptor(quarterWidth, quarterHeight, GraphicsFormat.R16G16_SFloat);
                RenderingUtils.ReAllocateIfNeeded(ref m_PongBuffer, descriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, name: "_PongBuffer");
                Blitter.BlitCameraTexture(cmd, source, m_OutlineMaskHalfRT, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, material, 0);
                Blitter.BlitCameraTexture(cmd, m_OutlineMaskHalfRT, m_OutlineMaskQuarterRT, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, material, 1);
                Blitter.BlitCameraTexture(cmd, m_OutlineMaskQuarterRT, m_PingBuffer, material, 2);
                float outlineWidth = outline.mainOutlineAmount.value * 32f / (1920f / powerOfTwoWidth);
                for (int i = Mathf.CeilToInt(Mathf.Log(outlineWidth + 1f, 2f)) - 1; i >= 0; i--)
                {
                    float step = Mathf.Pow(2f, i) + 0.5f;
                    cmd.SetGlobalVector(VLJumpFloodOutlineShaderConstants._StepWidth, new Vector4(step, 0f, 0f, 0f));
                    Blitter.BlitCameraTexture(cmd, m_PingBuffer, m_PongBuffer, material, 3);
                    cmd.SetGlobalVector(VLJumpFloodOutlineShaderConstants._StepWidth, new Vector4(0f, step, 0f, 0f));
                    Blitter.BlitCameraTexture(cmd, m_PongBuffer, m_PingBuffer, material, 3);
                }
                float maxWidth = ~(-1 << Mathf.CeilToInt(Mathf.Log(outlineWidth + 1f, 2f)));
                cmd.SetGlobalFloat(VLJumpFloodOutlineShaderConstants._OutlineMaxWidth, maxWidth + 0.5f);
                Blitter.BlitCameraTexture(cmd, m_PingBuffer, m_OutlineMaskQuarterRT, material, 4);
                RTHandle distanceField = m_OutlineMaskQuarterRT;
                if (cameraData.IsTemporalAAEnabled())
                {
                    descriptor = GetCompatibleDescriptor(quarterWidth, quarterHeight, GraphicsFormat.R8_UNorm);
                    RenderingUtils.ReAllocateIfNeeded(ref m_DistanceFieldPingRT, descriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, name: "_DistanceFieldPing");
                    descriptor = GetCompatibleDescriptor(quarterWidth, quarterHeight, GraphicsFormat.R8_UNorm);
                    RenderingUtils.ReAllocateIfNeeded(ref m_DistanceFieldPongRT, descriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, name: "_DistanceFieldPong");
                    GrapDistanceFieldTexture(out RTHandle previousField, out RTHandle nextField);
                    cmd.SetGlobalTexture(VLJumpFloodOutlineShaderConstants._HistoryDistanceFieldTexture, previousField);
                    float influence = cameraData.taaSettings.resetHistoryFrames != 0 ? 1f : 0.14f;
                    Vector2 jitter = TemporalAA.CalculateJitter(Time.frameCount) * cameraData.taaSettings.jitterScale;
                    material.SetVector(VLJumpFloodOutlineShaderConstants._DistanceFiledReprojectionParams, new Vector4(jitter.x / quarterWidth, jitter.y / quarterHeight, 0f, influence));
                    Blitter.BlitCameraTexture(cmd, m_OutlineMaskQuarterRT, nextField, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, material, 5);
                    distanceField = nextField;
                }
                Blitter.BlitCameraTexture(cmd, distanceField, m_OutlineMaskHalfRT, material, 6);
                cmd.SetGlobalFloat(VLJumpFloodOutlineShaderConstants._MainOutlineAmount, outlineWidth / maxWidth);
                cmd.SetGlobalColor(VLJumpFloodOutlineShaderConstants._MainOutlineColor, outline.mainOutlineColor.value);
                cmd.SetGlobalFloat(VLJumpFloodOutlineShaderConstants._SubOutlineAmount, outlineWidth / maxWidth * outline.subOutlineAmount.value);
                cmd.SetGlobalColor(VLJumpFloodOutlineShaderConstants._SubOutlineColor, outline.subOutlineColor.value);
                _mainOutlineOffsetShiftUV.x = outline.mainOutlinePosition.value.x * (-1f / 1920f);
                _mainOutlineOffsetShiftUV.y = outline.mainOutlinePosition.value.y * (-1f / 1080f);
                cmd.SetGlobalVector(VLJumpFloodOutlineShaderConstants._MainOffsetShiftUV, _mainOutlineOffsetShiftUV);
                _subOutlineOffsetShiftUV.x = outline.subOutlinePosition.value.x * (-1f / 1920f);
                _subOutlineOffsetShiftUV.y = outline.subOutlinePosition.value.y * (-1f / 1080f);
                cmd.SetGlobalVector(VLJumpFloodOutlineShaderConstants._SubOffsetShiftUV, _subOutlineOffsetShiftUV);
                CoreUtils.SetKeyword(material, "USE_OUTLINE_MULTI_SHIFT", outline.mainOutlinePosition.value.sqrMagnitude > 0f && outline.subOutlinePosition.value.sqrMagnitude > 0f);
                cmd.SetGlobalFloat(VLJumpFloodOutlineShaderConstants._SmoothDelta, 0.05f);
                cmd.SetGlobalTexture(VLJumpFloodOutlineShaderConstants._DistanceFieldMap, m_OutlineMaskHalfRT);
                Blitter.BlitCameraTexture(cmd, source, destination, material, 7);
            }
        }

        public void DisposeJumpFloodOutline()
        {
            m_OutlineMaskHalfRT?.Release();
            m_OutlineMaskQuarterRT?.Release();
            m_PingBuffer?.Release();
            m_PongBuffer?.Release();
            m_DistanceFieldPingRT?.Release();
            m_DistanceFieldPongRT?.Release();
        }

        public void GrapDistanceFieldTexture(out RTHandle prevDistanceField, out RTHandle nextDistanceField)
        {
            prevDistanceField = m_distanceFieldPingPong ? m_DistanceFieldPongRT : m_DistanceFieldPingRT;
            nextDistanceField = m_distanceFieldPingPong ? m_DistanceFieldPingRT : m_DistanceFieldPongRT;
            m_distanceFieldPingPong = !m_distanceFieldPingPong;
        }

        public bool DoVLMotionBlur(CommandBuffer cmd, RTHandle source, RTHandle destination)
        {
            var motionBlur = VolumeManager.instance.stack.GetComponent<VLMotionBlur>();
            bool active = motionBlur.IsActive();
            if (!active)
                return active;
            Material material = m_VLMaterials.vlMotionBlur;
            GraphicsFormat velocityFormat = RenderingUtils.SupportsGraphicsFormat(GraphicsFormat.A2R10G10B10_UNormPack32, FormatUsage.Blend)
                ? GraphicsFormat.A2R10G10B10_UNormPack32 : GraphicsFormat.R8G8B8A8_UNorm;
            var descriptor = GetCompatibleDescriptor();
            int width = descriptor.width;
            int height = descriptor.height;
            int maxBlurRadius = (int)(height * 5f / 100f);
            int tileSize = ((maxBlurRadius - 1) / 8 + 1) * 8;
            material.SetFloat(VLMotionBlurShaderConstants.VelocityScale, motionBlur.shutterAngle.value / 360f);
            material.SetFloat(VLMotionBlurShaderConstants.MaxBlurRadius, maxBlurRadius);
            material.SetFloat(VLMotionBlurShaderConstants.RcpMaxBlurRadius, 1f / maxBlurRadius);
            descriptor.width = width;
            descriptor.height = height;
            descriptor.graphicsFormat = velocityFormat;
            RenderingUtils.ReAllocateIfNeeded(ref m_VelocityTex, descriptor, FilterMode.Point, TextureWrapMode.Repeat, name: "_VelocityTex");
            Blitter.BlitCameraTexture(cmd, source, m_VelocityTex, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, material, 0);
            descriptor.graphicsFormat = GraphicsFormat.R16G16_SFloat;
            for (int i = 0; i < m_TileRTs.Length; i++)
            {
                descriptor.width >>= 1;
                descriptor.height >>= 1;
                RenderingUtils.ReAllocateIfNeeded(ref m_TileRTs[i], descriptor, FilterMode.Point, TextureWrapMode.Repeat, name: m_TileRTs[i].name);
            }
            Blitter.BlitCameraTexture(cmd, m_VelocityTex, m_TileRTs[0], RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, material, 1);
            Blitter.BlitCameraTexture(cmd, m_TileRTs[0], m_TileRTs[1], RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, material, 2);
            Blitter.BlitCameraTexture(cmd, m_TileRTs[1], m_TileRTs[2], RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, material, 2);
            float tileMaxLoop = tileSize / 8f;
            Vector2 tileMaxOffset = Vector2.one * (tileMaxLoop - 1f) * -0.5f;
            material.SetVector(VLMotionBlurShaderConstants.TileMaxOffs, tileMaxOffset);
            material.SetFloat(VLMotionBlurShaderConstants.TileMaxLoop, (int)tileMaxLoop);
            descriptor.width = width / tileSize;
            descriptor.height = height / tileSize;
            RenderingUtils.ReAllocateIfNeeded(ref m_TileVRT, descriptor, FilterMode.Point, TextureWrapMode.Repeat, name: "_TileVRT");
            Blitter.BlitCameraTexture(cmd, m_TileRTs[2], m_TileVRT, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, material, 3);
            RenderingUtils.ReAllocateIfNeeded(ref m_NeighborMaxTex, descriptor, FilterMode.Point, TextureWrapMode.Repeat, name: "_NeighborMaxTex");
            Blitter.BlitCameraTexture(cmd, m_TileVRT, m_NeighborMaxTex, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, material, 4);
            material.SetFloat(VLMotionBlurShaderConstants.LoopCount, Mathf.Clamp(motionBlur.sampleCount.value / 2, 1, 64));
            cmd.SetGlobalTexture(m_VelocityTex.name, m_VelocityTex.nameID);
            cmd.SetGlobalTexture(m_NeighborMaxTex.name, m_NeighborMaxTex.nameID);
            Blitter.BlitCameraTexture(cmd, source, destination, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, material, 5);
            return active;
        }

        public float Exp2(float x)
        {
            return Mathf.Exp(x * 0.69315f);
        }

        private void SetupVLDiffusion(CommandBuffer cmd, Material material, RTHandle source, RenderTextureDescriptor descriptor)
        {
            var diffusion = VolumeManager.instance.stack.GetComponent<VLDiffusion>();
            Material diffusionMaterial = m_VLMaterials.vlDiffusion;
            bool active = diffusion.IsActive() && diffusionMaterial != null;
            CoreUtils.SetKeyword(material, "VL_DIFFUSION", active);
            if (active)
            {
                using (new ProfilingScope(cmd, _vlDiffusionProfilingSampler))
                {
                    float blend = diffusion.blend.value;
                    float amount = diffusion.diffusion.value;
                    float threshold = diffusion.contrastThreshold.value;
                    float inverseThreshold;
                    float contrastScale;
                    if (threshold <= 0f)
                    {
                        inverseThreshold = 0f;
                        contrastScale = 1f;
                    }
                    else if (threshold >= 1f)
                    {
                        inverseThreshold = 1f;
                        contrastScale = 0f;
                    }
                    else
                    {
                        inverseThreshold = 1f / threshold;
                        contrastScale = inverseThreshold / (inverseThreshold - 1f);
                    }
                    float power = diffusion.contrastPower.value;
                    float aspect = (float)descriptor.width / descriptor.height;
                    if (descriptor.width < descriptor.height)
                    {
                        descriptor.width = (int)(aspect * 720f);
                        descriptor.height = 720;
                    }
                    else
                    {
                        descriptor.width = 720;
                        descriptor.height = (int)(720f / aspect);
                    }
                    descriptor.msaaSamples = 1;
                    descriptor.depthBufferBits = 0;
                    RenderingUtils.ReAllocateIfNeeded(ref m_DiffusionTexture, descriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, name: "_DiffusionTexture");
                    RenderingUtils.ReAllocateIfNeeded(ref m_DiffusionTemp, descriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, name: "_DiffusionTemp");
                    cmd.SetGlobalTexture(m_DiffusionTexture.name, m_DiffusionTexture.nameID);
                    cmd.SetGlobalVector(VLShaderConstants._Diffusion_Settings, new Vector4(amount, blend, 1f / (blend + 1f), 0f));
                    cmd.SetGlobalVector(VLShaderConstants._Diffusion_Contrasts, new Vector4(threshold, inverseThreshold, contrastScale, power));
                    Vector2 offset = new Vector2(1f / descriptor.width, 1f / descriptor.height) * 0.5f;
                    cmd.SetGlobalVector(VLShaderConstants._BlurOffset, offset);
                    Blitter.BlitCameraTexture(cmd, source, m_DiffusionTexture, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, diffusionMaterial, 0);
                    Blitter.BlitCameraTexture(cmd, m_DiffusionTexture, m_DiffusionTemp, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, diffusionMaterial, 1);
                    Blitter.BlitCameraTexture(cmd, m_DiffusionTemp, m_DiffusionTexture, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, diffusionMaterial, 2);
                }
            }
        }

        private void DoVLAdditive(CommandBuffer cmd, RTHandle source, Camera camera)
        {
            var stack = VolumeManager.instance.stack;
            var bloom = stack.GetComponent<VLBloom>();
            var paraffin = stack.GetComponent<VLParaffin>();
            var starStreak = stack.GetComponent<VLStarStreak>();
            bool bloomActive = bloom.IsActive();
            bool paraffinActive = paraffin.IsActive() & paraffin.IsValidAspect(camera.aspect);
            bool starStreakActive = starStreak.IsActive();
            if (bloomActive | paraffinActive | starStreakActive)
            {
                CoreUtils.SetKeyword(m_Materials.uber, "VL_DOWNSCALE", true);
                var descriptor = GetCompatibleDescriptor();
                descriptor.width >>= 1;
                descriptor.height >>= 1;
                RenderingUtils.ReAllocateIfNeeded(ref m_DownscaleAdditiveTexture, descriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, name: "_DownscaleAdditiveTexture");
                cmd.SetGlobalTexture(m_DownscaleAdditiveTexture.name, m_DownscaleAdditiveTexture.nameID);
                if (bloomActive)
                {
                    using (new ProfilingScope(cmd, _vlBloomProfilingSampler))
                        SetupVLBloom(cmd, source, bloom, starStreak);
                }
            }
            else
                CoreUtils.SetKeyword(m_Materials.uber, "VL_DOWNSCALE", false);
            if (paraffinActive)
            {
                using (new ProfilingScope(cmd, _vlParaffinProfilingSampler))
                    SetupVLParaffin(cmd, camera.aspect, paraffin);
            }
        }

        private void SetupVLBloom(CommandBuffer cmd, RTHandle source, VLBloom vlBloom, VLStarStreak vlStarStreak)
        {
            int width = m_Descriptor.width >> 1;
            int height = m_Descriptor.height >> 1;
            int diffusion = vlBloom.diffusion.value;
            float logHeight = Mathf.Log(Mathf.Max(width, height), 2f) + Mathf.Min(diffusion, 10f) - 10f;
            int logHeightInt = Mathf.FloorToInt(logHeight);
            int mipCount = Mathf.Clamp(logHeightInt, 2, 7);
            float threshold = Mathf.GammaToLinearSpace(vlBloom.threshold.value);
            float intensity = Exp2(vlBloom.intensity.value / 10f) - 1f;
            float inverseIntensity = intensity <= 0.001f ? 1f : 1f / intensity;
            Material bloomMaterial = m_VLMaterials.vlBloom;
            cmd.SetGlobalVector(VLShaderConstants._Bloom_Settings, new Vector4(threshold, logHeight + 0.5f - logHeightInt, intensity, inverseIntensity));
            Color color = vlBloom.color.value.linear;
            float luminance = ColorUtils.Luminance(color);
            color = luminance > 0f ? color * (1f / luminance) : Color.white;
            cmd.SetGlobalColor(VLShaderConstants._Bloom_Color, color);
            var descriptor = GetCompatibleDescriptor(width, height, m_DefaultHDRFormat);
            RTHandle last = source;
            for (int i = 0; i < mipCount; i++)
            {
                RenderingUtils.ReAllocateIfNeeded(ref m_BloomMipDown[i], descriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, name: m_BloomMipDown[i].name);
                if (i > 0)
                    RenderingUtils.ReAllocateIfNeeded(ref m_BloomMipUp[i], descriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, name: m_BloomMipUp[i].name);
                Blitter.BlitCameraTexture(cmd, last, m_BloomMipDown[i], RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, bloomMaterial, i == 0 ? 0 : 1);
                last = m_BloomMipDown[i];
                descriptor.width = Mathf.Max(descriptor.width >> 1, 1);
                descriptor.height = Mathf.Max(descriptor.height >> 1, 1);
            }
            last = m_BloomMipDown[mipCount - 1];
            for (int i = mipCount - 2; i >= 0; i--)
            {
                RTHandle destination = i == 0 ? m_DownscaleAdditiveTexture : m_BloomMipUp[i];
                cmd.SetGlobalTexture(ShaderConstants._Bloom_Texture, m_BloomMipDown[i].nameID);
                Blitter.BlitCameraTexture(cmd, last, destination, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, bloomMaterial, i == 0 ? 3 : 2);
                last = destination;
            }
            if (vlStarStreak.type.value != StarStreakType.None)
            {
                RTHandle streakSource = m_BloomMipDown[0];
                cmd.SetGlobalVector(VLShaderConstants._StarStreakParam, new Vector4(Mathf.GammaToLinearSpace(vlStarStreak.threshold.value), vlStarStreak.spectrum.value, 0f, 0f));
                Blitter.BlitCameraTexture(cmd, source, streakSource, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, m_VLMaterials.vlStarStreak, 0);
                DrawStarStreak(cmd, streakSource, last, vlStarStreak);
            }
            cmd.SetGlobalTexture(ShaderConstants._Bloom_Texture, last.nameID);
        }

        private void DrawStarStreak(CommandBuffer cmd, RTHandle source, RTHandle destination, VLStarStreak vlStarStreak)
        {
            if (_starStreakParams == null)
                _starStreakParams = new Vector4[4];
            int width = m_Descriptor.width >> 2;
            int height = m_Descriptor.height >> 2;
            int iteration = vlStarStreak.iteration.value;
            float atten = vlStarStreak.atten.value;
            switch (vlStarStreak.type.value)
            {
                case StarStreakType.Two:
                {
                    float sine = Mathf.Sin(vlStarStreak.rotation.value * Mathf.Deg2Rad);
                    float cosine = Mathf.Cos(vlStarStreak.rotation.value * Mathf.Deg2Rad);
                    Color color = vlStarStreak.color.value;
                    DrawStarStreak(cmd, new Vector2(cosine / width, sine / height), color, iteration, atten, source, destination);
                    DrawStarStreak(cmd, new Vector2(-cosine / width, -sine / height), color, iteration, atten, source, destination);
                    break;
                }
                case StarStreakType.Four:
                {
                    float angle = (vlStarStreak.rotation.value + 45f) * Mathf.Deg2Rad;
                    float sine = Mathf.Sin(angle);
                    float cosine = Mathf.Cos(angle);
                    Color color = vlStarStreak.color.value;
                    DrawStarStreak(cmd, new Vector2(cosine / width, sine / height), color, iteration, atten, source, destination);
                    DrawStarStreak(cmd, new Vector2(-cosine / width, -sine / height), color, iteration, atten, source, destination);
                    DrawStarStreak(cmd, new Vector2(-sine / width, cosine / height), color, iteration, atten, source, destination);
                    DrawStarStreak(cmd, new Vector2(sine / width, -cosine / height), color, iteration, atten, source, destination);
                    break;
                }
            }
        }

        private void DrawStarStreak(CommandBuffer cmd, Vector2 dir, Color color, int iteration, float atten, RTHandle source, RTHandle destination)
        {
            float scale = 1f;
            for (int i = 0; i < iteration; i++)
            {
                for (int sample = 0; sample < 4; sample++)
                {
                    float distance = scale * sample;
                    Vector2 offset = dir * sample * scale;
                    _starStreakParams[sample] = new Vector4(offset.x, offset.y, Mathf.Pow(atten, distance), distance);
                }
                cmd.SetGlobalVectorArray(VLShaderConstants._StarStreakBlurParams, _starStreakParams);
                RTHandle input = i == 0 ? source : ((i & 1) == 0 ? m_BloomMipDown[1] : m_BloomMipUp[1]);
                if (i == iteration - 1)
                {
                    cmd.SetGlobalColor(VLShaderConstants._Bloom_Color, color);
                    Blitter.BlitCameraTexture(cmd, input, destination, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store, m_VLMaterials.vlStarStreak, 2);
                }
                else
                {
                    RTHandle output = (i & 1) == 0 ? m_BloomMipUp[1] : m_BloomMipDown[1];
                    Blitter.BlitCameraTexture(cmd, input, output, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, m_VLMaterials.vlStarStreak, 1);
                }
                scale *= 4f;
            }
        }

        private void SetupVLParaffin(CommandBuffer cmd, float aspect, VLParaffin vlParaffin)
        {
            Material material = m_VLMaterials.vlFlare;
            float baseAspect = vlParaffin.baseAspect.value;
            DrawFlare(cmd, material, vlParaffin.flare0.value, baseAspect, aspect);
            DrawFlare(cmd, material, vlParaffin.flare1.value, baseAspect, aspect);
        }

        private bool DrawFlare(CommandBuffer cmd, Material flareMaterial, FlareSetting flare, float baseAspect, float aspect)
        {
            int pass = 0;
            switch (flare.type)
            {
                case FlareType.None:
                    return false;
                case FlareType.Sphere:
                case FlareType.SphereInv:
                    if (flare.size.x < 0.001f || flare.size.y < 0.001f)
                        return false;
                    float inverseWidth = 1f / flare.size.x;
                    float centerX = -flare.center.x;
                    if (flare.fixToBaseAspect)
                    {
                        centerX /= aspect / baseAspect;
                        inverseWidth *= aspect / baseAspect;
                    }
                    cmd.SetGlobalVector(VLShaderConstants._Flare_Settings, new Vector4(centerX, -flare.center.y, inverseWidth, 1f / flare.size.y));
                    cmd.SetGlobalColor(VLShaderConstants._Flare_Color0, flare.color0.linear);
                    cmd.SetGlobalColor(VLShaderConstants._Flare_Color1, flare.color1.linear);
                    pass = flare.type == FlareType.SphereInv ? 1 : 0;
                    break;
                case FlareType.Fill:
                    cmd.SetGlobalColor(VLShaderConstants._Flare_Color0, flare.color0.linear);
                    pass = 2;
                    break;
                case FlareType.Square:
                    float aspectScale = aspect / baseAspect;
                    Vector2 size = flare.size;
                    cmd.SetGlobalVector(VLShaderConstants._Flare_Settings, new Vector4(
                        Mathf.Max(0f, flare.center.x), flare.fixToBaseAspect ? aspectScale : 1f,
                        Mathf.Max(0.1f, size.x), Mathf.Max(0.1f, size.y)));
                    cmd.SetGlobalColor(VLShaderConstants._Flare_Color0, flare.color0.linear);
                    pass = 3;
                    break;
            }
            Blitter.BlitTexture(cmd, new Vector4(Vector2.one.x, Vector2.one.y, 0f, 0f), flareMaterial, pass);
            return true;
        }

        private bool DoVLTextureBlur(CommandBuffer cmd, RTHandle source, RTHandle destination)
        {
            var blur = VolumeManager.instance.stack.GetComponent<VLTextureBlur>();
            bool active = blur.IsActive();
            if (active)
            {
                Material material = m_VLMaterials.vlTextureBlur;
                material.SetFloat(VLShaderConstants._TextureBlurIntensity, blur.intensity.value);
                material.SetTexture(VLShaderConstants._TextureBlurTexture, blur.texture.value);
                Blitter.BlitCameraTexture(cmd, source, destination, material, 0);
            }
            return active;
        }

        public void InitializeVirtualEffect()
        {
            m_AuraMipUp = new RTHandle[7];
            m_AuraMipDown = new RTHandle[7];
            for (int i = 0; i < 7; i++)
            {
                m_AuraMipUp[i] = RTHandles.Alloc("_AuraMipUp" + i);
                m_AuraMipDown[i] = RTHandles.Alloc("_AuraMipDown" + i);
            }
        }

        public void DisposeVirtualEffect()
        {
            m_AdditiveTextureToAdd?.Release();
            foreach (RTHandle handle in m_AuraMipUp)
                handle?.Release();
            foreach (RTHandle handle in m_AuraMipDown)
                handle?.Release();
        }

        public void DoVLVirtualEffectAdditiveTextureToAdd(CommandBuffer cmd, RTHandle source, Camera camera)
        {
            var stack = VolumeManager.instance.stack;
            var effect = stack.GetComponent<VLVirtualEffect>();
            var bloom = stack.GetComponent<VLBloom>();
            var paraffin = stack.GetComponent<VLParaffin>();
            var starStreak = stack.GetComponent<VLStarStreak>();
            bool bloomActive = bloom.IsActive();
            bool paraffinActive = paraffin.IsActive();
            bool validAspect = paraffin.IsValidAspect(camera.aspect);
            bool starStreakActive = starStreak.IsActive();
            if (effect.IsActiveAura() && (bloomActive | (paraffinActive & validAspect) | starStreakActive))
            {
                using (new ProfilingScope(cmd, _vlVirtualEffectSampler))
                    SetupVLVirtualEffect(cmd, source, effect);
            }
        }

        private void SetupVLVirtualEffect(CommandBuffer cmd, RTHandle source, VLVirtualEffect vlVirtualEffect)
        {
            int width = m_Descriptor.width >> 1;
            int height = m_Descriptor.height >> 1;
            int diffusion = vlVirtualEffect.auraDiffusion.value;
            float logHeight = Mathf.Log(Mathf.Max(width, height), 2f) + Mathf.Min(diffusion, 10f) - 10f;
            int logHeightInt = Mathf.FloorToInt(logHeight);
            int mipCount = Mathf.Clamp(logHeightInt, 2, 7);
            Material virtualMaterial = m_VLMaterials.vlVirtualEffect;
            Material bloomMaterial = m_VLMaterials.vlBloom;
            cmd.SetGlobalVector(VLShaderConstants._Bloom_Settings, new Vector4(0f, logHeight + 0.5f - logHeightInt, 0f, 0f));
            var descriptor = GetCompatibleDescriptor(width, height, GraphicsFormat.R8_UNorm);
            var additiveDescriptor = GetCompatibleDescriptor(width, height, GraphicsFormat.R8_UNorm);
            RenderingUtils.ReAllocateIfNeeded(ref m_AdditiveTextureToAdd, additiveDescriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, name: "_AdditiveTextureToAdd");
            RTHandle last = source;
            for (int i = 0; i < mipCount; i++)
            {
                RenderingUtils.ReAllocateIfNeeded(ref m_AuraMipDown[i], descriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, name: m_AuraMipDown[i].name);
                if (i > 0)
                {
                    RenderingUtils.ReAllocateIfNeeded(ref m_AuraMipUp[i], descriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, name: m_AuraMipUp[i].name);
                    Blitter.BlitCameraTexture(cmd, last, m_AuraMipDown[i], RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, bloomMaterial, 1);
                }
                else
                    Blitter.BlitCameraTexture(cmd, last, m_AuraMipDown[i], RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, virtualMaterial, 0);
                last = m_AuraMipDown[i];
                descriptor.width = Mathf.Max(descriptor.width >> 1, 1);
                descriptor.height = Mathf.Max(descriptor.height >> 1, 1);
            }
            if (m_AuraMipDown[0] != null)
                cmd.SetGlobalTexture(VLVirtualEffectShaderConstants._VirtualEffect_Mask, m_AuraMipDown[0]);
            last = m_AuraMipDown[mipCount - 1];
            for (int i = mipCount - 2; i >= 0; i--)
            {
                RTHandle destination = i == 0 ? m_AdditiveTextureToAdd : m_AuraMipUp[i];
                cmd.SetGlobalTexture(ShaderConstants._Bloom_Texture, m_AuraMipDown[i].nameID);
                Blitter.BlitCameraTexture(cmd, last, destination, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store,
                    i == 0 ? virtualMaterial : bloomMaterial, i == 0 ? 1 : 2);
                last = destination;
            }
        }

        public void DoVLVirtualEffectAdditiveTextureToBloom(CommandBuffer cmd, RTHandle source, RTHandle destination)
        {
            var effect = VolumeManager.instance.stack.GetComponent<VLVirtualEffect>();
            Material material = m_VLMaterials.vlVirtualEffect;
            using (new ProfilingScope(cmd, _vlVirtualEffectSampler))
            {
                if (effect.IsActiveAura())
                {
                    material.SetColor(VLVirtualEffectShaderConstants._AuraColor, effect.auraColor.value);
                    Blitter.BlitCameraTexture(cmd, source, destination, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, material, 2);
                }
            }
        }

        public void DoVLVirtualEffectNoise(CommandBuffer cmd, RTHandle source, RTHandle destination)
        {
            using (new ProfilingScope(cmd, _vlVirtualEffectSampler))
            {
                var effect = VolumeManager.instance.stack.GetComponent<VLVirtualEffect>();
                Material material = m_VLMaterials.vlVirtualEffect;
                float frequency = 0f;
                if (effect.intervalType.value == IntervalType.Infinite)
                    material.EnableKeyword("USING_FREQUENCY_INFINITE");
                else
                {
                    if (effect.intervalType.value == IntervalType.Random)
                    {
                        uint seed = math.asuint(ShaderTimeManager.CurrentTime);
                        var random = new Unity.Mathematics.Random(seed == 0u ? 1851936439u : seed);
                        frequency = random.NextFloat(0f, effect.frequency.value);
                    }
                    material.DisableKeyword("USING_FREQUENCY_INFINITE");
                }
                if (effect.noiseType.value == NoiseType.Simple)
                {
                    material.EnableKeyword("NOISE_SIMPLE");
                    float displacement = Mathf.Pow(effect.intensity.value, 3f);
                    float threshold = Mathf.Clamp01(1f - effect.intensity.value * 1.2f);
                    material.SetVector(VLVirtualEffectShaderConstants._NoiseParams, new Vector4(
                        displacement * 0.1f + 0.005f, threshold,
                        effect.intervalType.value == IntervalType.Random ? frequency : effect.frequency.value, 0f));
                }
                else
                {
                    material.DisableKeyword("NOISE_SIMPLE");
                    material.SetFloat(VLVirtualEffectShaderConstants._Granularity, effect.granularity.value);
                    material.SetFloat(VLVirtualEffectShaderConstants._RGBSplit, effect.rgbSplit.value);
                    material.SetVector(VLVirtualEffectShaderConstants._NoiseParams, new Vector4(
                        effect.intensity.value, 0f,
                        effect.intervalType.value == IntervalType.Random ? frequency : effect.frequency.value, effect.speed.value));
                }
                Blitter.BlitCameraTexture(cmd, source, destination, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, material, 3);
            }
        }

        public void DoVLVirtualEffectChromaticAberration(CommandBuffer cmd, RTHandle source, RTHandle destination)
        {
            using (new ProfilingScope(cmd, _vlVirtualEffectSampler))
            {
                var effect = VolumeManager.instance.stack.GetComponent<VLVirtualEffect>();
                Material material = m_VLMaterials.vlVirtualEffect;
                material.SetFloat(VLVirtualEffectShaderConstants._ChromaticAberrationIntensity, effect.chromaticAberrationIntensity.value);
                Blitter.BlitCameraTexture(cmd, source, destination, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, material, 4);
            }
        }

        public void DoChromaticAberration(CommandBuffer cmd, RTHandle source, RTHandle destination)
        {
            var effect = VolumeManager.instance.stack.GetComponent<VLChromaticAberration>();
            Material material = m_VLMaterials.vlChromaticAberration;
            using (new ProfilingScope(cmd, _vlChromaticAberrationSampler))
            {
                float frequency = effect.frequency.value;
                float modulation;
                if (effect.intervalType.value == IntervalType.Infinite)
                    modulation = 1f;
                else
                {
                    if (effect.intervalType.value == IntervalType.Random)
                    {
                        uint seed = math.asuint(ShaderTimeManager.CurrentTime);
                        var random = new Unity.Mathematics.Random(seed == 0u ? 1851936439u : seed);
                        frequency = random.NextFloat(0f, effect.frequency.value);
                    }
                    modulation = Mathf.Cos(frequency * ShaderTimeManager.CurrentTime) * 0.5f + 0.5f;
                }
                _offsetUV.x = modulation * effect.rgbSplit.value * (1f / 1920f);
                _offsetUV.y = modulation * effect.rgbSplit.value * (1f / 1080f);
                _redDirection = effect.redDirection.value * _offsetUV;
                _greenDirection = effect.greenDirection.value * _offsetUV;
                _blueDirection = effect.blueDirection.value * _offsetUV;
                material.SetVector(VLChromaticAberrationShaderConstants._RedDirection, _redDirection);
                material.SetVector(VLChromaticAberrationShaderConstants._GreenDirection, _greenDirection);
                material.SetVector(VLChromaticAberrationShaderConstants._BlueDirection, _blueDirection);
                Blitter.BlitCameraTexture(cmd, source, destination, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, material, 0);
            }
        }

        public void DoRGBBlockNoise(CommandBuffer cmd, RTHandle source, RTHandle destination)
        {
            var effect = VolumeManager.instance.stack.GetComponent<VLRGBBlockNoise>();
            Material material = m_VLMaterials.vlNoise;
            using (new ProfilingScope(cmd, _vlNoiseSampler))
            {
                _offsetUV.x = effect.rgbSplit.value * (1f / 1920f);
                _offsetUV.y = effect.rgbSplit.value * (1f / 1080f);
                material.SetVector(VLRGBBockNoiseShaderConstants._OffsetUV, _offsetUV);
                material.SetFloat(VLRGBBockNoiseShaderConstants._StepFrequency, effect.stepFrequency.value);
                material.SetFloat(VLRGBBockNoiseShaderConstants._TileOffsetX, effect.tileOffsetX.value);
                material.SetFloat(VLRGBBockNoiseShaderConstants._TileOffsetY, effect.tileOffsetY.value);
                material.SetFloat(VLRGBBockNoiseShaderConstants._Aspect, (float)source.rt.width / source.rt.height);
                material.SetFloat(VLRGBBockNoiseShaderConstants._Attenuation, effect.attenuation.value);
                material.SetFloat(VLRGBBockNoiseShaderConstants._Saturation, effect.saturation.value);
                Blitter.BlitCameraTexture(cmd, source, destination, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, material, 0);
            }
        }

        public void DoWaveJitterGlitch(CommandBuffer cmd, RTHandle source, RTHandle destination)
        {
            var effect = VolumeManager.instance.stack.GetComponent<VLWaveJitterGlitch>();
            Material material = m_VLMaterials.vlNoise;
            using (new ProfilingScope(cmd, _vlNoiseSampler))
            {
                float frequency = effect.frequency.value;
                float modulation;
                if (effect.intervalType.value == IntervalType.Infinite)
                    modulation = 1f;
                else
                {
                    if (effect.intervalType.value == IntervalType.Random)
                    {
                        uint seed = math.asuint(ShaderTimeManager.CurrentTime);
                        var random = new Unity.Mathematics.Random(seed == 0u ? 1851936439u : seed);
                        frequency = random.NextFloat(0f, effect.frequency.value);
                    }
                    modulation = Mathf.Cos(frequency * ShaderTimeManager.CurrentTime) * 0.5f + 0.5f;
                }
                material.SetFloat(VLWaveJitterGlitchShaderContents._Frequency, frequency);
                material.SetFloat(VLWaveJitterGlitchShaderContents._Intensity, modulation * effect.intensity.value);
                material.SetFloat(VLWaveJitterGlitchShaderContents._Granularity, effect.granularity.value);
                material.SetFloat(VLWaveJitterGlitchShaderContents._WaveSpeed, effect.waveSpeed.value);
                material.SetFloat(VLWaveJitterGlitchShaderContents._RGBSplit, effect.rgbSplit.value);
                Blitter.BlitCameraTexture(cmd, source, destination, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, material, 1);
            }
        }

    }

}
