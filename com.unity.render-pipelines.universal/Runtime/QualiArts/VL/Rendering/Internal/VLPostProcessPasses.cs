// VL post-process passes (APK 3.5.3, VL.Rendering.Internal.VLPostProcessPasses): URP 14 PostProcessPasses with an
// optional compute colour-grading pass and VLPostProcessPass, reconstructed from the game.
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.Universal.Internal;

namespace VL.Rendering.Internal
{
    internal struct VLPostProcessPasses : IDisposable
    {
        private ColorGradingLutPass m_ColorGradingLutPass;
        private ColorGradingComputePass m_ColorGradingComputePass;
        private VLPostProcessPass m_PostProcessPass;
        private VLPostProcessPass m_FinalPostProcessPass;
        internal RTHandle m_AfterPostProcessColor;
        internal RTHandle m_ColorGradingLut;
        private VLPostProcessData m_RendererPostProcessData;
        private VLPostProcessData m_CurrentPostProcessData;
        private Material m_BlitMaterial;
        private bool m_ComputeGradingAvailable;

        public ColorGradingLutPass colorGradingLutPass => m_ColorGradingLutPass;
        public ColorGradingComputePass colorGradingComputePass => m_ColorGradingComputePass;
        public VLPostProcessPass postProcessPass => m_PostProcessPass;
        public VLPostProcessPass finalPostProcessPass => m_FinalPostProcessPass;
        public RTHandle colorGradingLut => m_ColorGradingLut;
        public bool isCreated => m_CurrentPostProcessData != null;

        public VLPostProcessPasses(VLPostProcessData rendererPostProcessData, ref PostProcessParams postProcessParams)
        {
            m_ColorGradingLutPass = null;
            m_ColorGradingComputePass = null;
            m_PostProcessPass = null;
            m_FinalPostProcessPass = null;
            m_CurrentPostProcessData = null;
            m_AfterPostProcessColor = null;
            m_ColorGradingLut = null;
            m_RendererPostProcessData = rendererPostProcessData;
            m_ComputeGradingAvailable = ColorGradingComputePass.IsAvailable(rendererPostProcessData);
            m_BlitMaterial = postProcessParams.blitMaterial;
            Recreate(rendererPostProcessData, ref postProcessParams);
        }

        public void Recreate(VLPostProcessData data, ref PostProcessParams ppParams)
        {
            if (m_RendererPostProcessData)
                data = m_RendererPostProcessData;

            if (data == m_CurrentPostProcessData)
                return;

            if (m_CurrentPostProcessData != null)
            {
                m_ColorGradingLutPass?.Cleanup();
                m_PostProcessPass?.Cleanup();
                m_FinalPostProcessPass?.Cleanup();
                m_ColorGradingLutPass = null;
                m_PostProcessPass = null;
                m_FinalPostProcessPass = null;
                m_CurrentPostProcessData = null;
            }

            if (data != null)
            {
                if (m_ComputeGradingAvailable)
                    m_ColorGradingComputePass = new ColorGradingComputePass(RenderPassEvent.BeforeRendering, data);
                m_ColorGradingLutPass = new ColorGradingLutPass(RenderPassEvent.BeforeRenderingPrePasses, data);
                m_PostProcessPass = new VLPostProcessPass(RenderPassEvent.BeforeRenderingPostProcessing, data, ref ppParams, m_ComputeGradingAvailable);
                m_FinalPostProcessPass = new VLPostProcessPass(RenderPassEvent.AfterRenderingPostProcessing, data, ref ppParams, m_ComputeGradingAvailable);
                m_CurrentPostProcessData = data;
            }
        }

        public void Dispose()
        {
            m_ColorGradingLutPass?.Cleanup();
            m_PostProcessPass?.Cleanup();
            m_FinalPostProcessPass?.Cleanup();
            m_AfterPostProcessColor?.Release();
            m_ColorGradingLut?.Release();
        }

        internal void ReleaseRenderTargets()
        {
            m_AfterPostProcessColor?.Release();
            m_PostProcessPass?.Dispose();
            m_FinalPostProcessPass?.Dispose();
            m_ColorGradingLut?.Release();
        }
    }
}
