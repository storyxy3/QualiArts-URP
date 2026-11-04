using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
namespace VL.Rendering
{
    public sealed class VLMotionVectorRenderPass : MotionVectorRenderPass
    {
        internal static readonly int kPreviousWorldCameraPosition = Shader.PropertyToID("_PrevWorldSpaceCameraPos");
        internal static readonly int kPreviousWorldCameraPositionStereo = Shader.PropertyToID("_PrevWorldSpaceCameraPosStereo");
        public static readonly ShaderTagId motionVectorTagId = new ShaderTagId("MotionVectors");
        public static readonly ShaderTagId outlineMotionVectorTagId = new ShaderTagId("OutlineMotionVectors");
        internal VLMotionVectorRenderPass(RenderPassEvent evt, Material cameraMaterial, Material objectMaterial, LayerMask opaqueLayerMask)
            : base(evt, cameraMaterial, objectMaterial, opaqueLayerMask)
        {
            profilingSampler = new ProfilingSampler("VLMotionVectorRenderPass");
        }
private static void ExecutePass(ScriptableRenderContext context, PassData passData, ref RenderingData renderingData)
        {
            var cameraMaterial = passData.cameraMaterial;
            var objectMaterial = passData.objectMaterial;

            if (cameraMaterial == null || objectMaterial == null)
                return;

            // Get data
            ref var cameraData = ref renderingData.cameraData;
            Camera camera = cameraData.camera;
            MotionVectorsPersistentData motionData = RenderingUtility.GetMotionVectorData(ref cameraData);

            if (motionData == null)
                return;

            // Never draw in Preview
            if (camera.cameraType == CameraType.Preview)
                return;

            // Profiling command
            var cmd = renderingData.commandBuffer;
            using (new ProfilingScope(cmd, ProfilingSampler.Get(URPProfileId.MotionVectors)))
            {
                int passID = motionData.GetXRMultiPassId(ref cameraData);

                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
#if ENABLE_VR && ENABLE_XR_MODULE
                if (cameraData.xr.enabled && cameraData.xr.singlePassEnabled)
                {
                    cmd.SetGlobalMatrixArray("_PrevViewProjMatrixStereo", motionData.previousViewProjectionStereo);
                    cmd.SetGlobalMatrixArray("_NonJitteredViewProjMatrixStereo", motionData.viewProjectionStereo);
                }
                else
#endif
                {
                    // TODO: These should be part of URP main matrix set. For now, we set them here for motion vector rendering.
                    cmd.SetGlobalMatrix("_PrevViewProjMatrix", motionData.previousViewProjectionStereo[passID]);
                    cmd.SetGlobalMatrix("_NonJitteredViewProjMatrix", motionData.viewProjectionStereo[passID]);
                }

                // These flags are still required in SRP or the engine won't compute previous model matrices...
                // If the flag hasn't been set yet on this camera, motion vectors will skip a frame.
                camera.depthTextureMode |= DepthTextureMode.MotionVectors | DepthTextureMode.Depth;

                // TODO: add option to only draw either one?
                DrawCameraMotionVectors(context, cmd, ref renderingData, camera, cameraMaterial);
                DrawObjectMotionVectors(context, ref renderingData, camera, objectMaterial, cmd, ref passData.filteringSettings);
                RenderingBridge.Execute(RenderingBridge.EventType.MotionVector, context, ref renderingData);
            }
        }
public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            m_PassData.cameraMaterial = m_CameraMaterial;
            m_PassData.objectMaterial = m_ObjectMaterial;
            m_PassData.filteringSettings = m_FilteringSettings;

            ExecutePass(context, m_PassData, ref renderingData);
        }
private static void DrawObjectMotionVectors(ScriptableRenderContext context, ref RenderingData renderingData, Camera camera, Material objectMaterial, CommandBuffer cmd, ref FilteringSettings filteringSettings)
        {
#if ENABLE_VR && ENABLE_XR_MODULE
            bool foveatedRendering = renderingData.cameraData.xr.supportsFoveatedRendering;
            if (foveatedRendering)
            {
                // This is a geometry pass, enable foveated rendering (we need to disable it after)
                cmd.SetFoveatedRenderingMode(FoveatedRenderingMode.Enabled);
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
            }
#endif

            var drawingSettings = GetDrawingSettings(ref renderingData, objectMaterial);
            var renderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);

            filteringSettings.renderQueueRange = new RenderQueueRange(0, 2200);
            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings, ref renderStateBlock);
            filteringSettings.renderQueueRange = new RenderQueueRange(2301, 2500);
            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings, ref renderStateBlock);
            var outline = new DrawingSettings(outlineMotionVectorTagId, new SortingSettings(camera) { criteria = SortingCriteria.OptimizeStateChanges })
            {
                perObjectData = PerObjectData.MotionVectors,
                enableDynamicBatching = renderingData.supportsDynamicBatching,
                enableInstancing = true
            };
            var outlineFiltering = new FilteringSettings(RenderQueueRange.opaque, camera.cullingMask);
            context.DrawRenderers(renderingData.cullResults, ref outline, ref outlineFiltering, ref renderStateBlock);

#if ENABLE_VR && ENABLE_XR_MODULE
            if (foveatedRendering)
            {
                cmd.SetFoveatedRenderingMode(FoveatedRenderingMode.Disabled);
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
            }
#endif
        }
    }
}
