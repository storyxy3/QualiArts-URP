using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
namespace VL.Rendering
{
    public static partial class RenderingUtility
    {
        public static class ShaderTagIds
        {
            public static ShaderTagId UniversalForwardPerformance = new ShaderTagId("UniversalForwardPerformance");
            public static ShaderTagId DepthOnly = new ShaderTagId("DepthOnly");
        }
        private static bool _supportedDeviceInitialized = false;
        private static bool _isSupportedDevice = true;
        public static bool IsSupportedDevice()
        {
            if (_supportedDeviceInitialized) return _isSupportedDevice;
            _supportedDeviceInitialized = true;
            _isSupportedDevice = true;
            return true;
        }
        public static void SetGlobalMipBias(CommandBuffer cmd, ref CameraData cameraData, float scale)
        {
            float width = cameraData.cameraTargetDescriptor.width * scale;
            int pixelWidth = cameraData.camera.pixelWidth;
            if (cameraData.camera.allowDynamicResolution) width *= ScalableBufferManager.widthScaleFactor;
            float mipBias = Math.Min(-(float)Math.Log(pixelWidth / width, 2.0), 0.0f);
            mipBias = Math.Min(mipBias, Math.Min(cameraData.taaSettings.mipBias, 0.0f));
            cmd.SetGlobalVector(ShaderPropertyId.globalMipBias, new Vector4(mipBias, Mathf.Pow(2.0f, mipBias), 0.0f, 0.0f));
        }
        public static MotionVectorsPersistentData GetMotionVectorData(ref CameraData cameraData)
        {
            return cameraData.camera.TryGetComponent<UniversalAdditionalCameraData>(out var data) ? data.motionVectorsPersistentData : null;
        }
        public static void SetupMotionVectorData(CommandBuffer cmd, ref CameraData cameraData)
        {
            var data = GetMotionVectorData(ref cameraData);
            if (data == null) return;
            int passId = data.GetXRMultiPassId(ref cameraData);
#if ENABLE_VR && ENABLE_XR_MODULE
            if (cameraData.xr.enabled && cameraData.xr.singlePassEnabled)
            {
                cmd.SetGlobalMatrixArray("_PrevViewProjMatrixStereo", data.previousViewProjectionStereo);
                cmd.SetGlobalMatrixArray("_NonJitteredViewProjMatrixStereo", data.viewProjectionStereo);
                cmd.SetGlobalVectorArray(VLMotionVectorRenderPass.kPreviousWorldCameraPositionStereo, data.previousWorldSpaceCameraPosStereo);
            }
            else
#endif
            {
                cmd.SetGlobalMatrix("_PrevViewProjMatrix", data.previousViewProjectionStereo[passId]);
                cmd.SetGlobalMatrix("_NonJitteredViewProjMatrix", data.viewProjectionStereo[passId]);
                cmd.SetGlobalVector(VLMotionVectorRenderPass.kPreviousWorldCameraPosition, data.previousWorldSpaceCameraPos);
            }
        }
    }
}
