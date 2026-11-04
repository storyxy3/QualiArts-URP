// VL sky pass (APK 3.5.3, VL.Rendering.VLSkyPass): fullscreen skybox + cloud layers, reconstructed from the game.
// The pixel-coordinate -> view-direction matrix helpers are HDRP's (HDUtils / HDCamera), as in the game.
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace VL.Rendering
{
    public class VLSkyPass : ScriptableRenderPass
    {
        public static readonly int _MainTex = Shader.PropertyToID("_MainTex");
        public static readonly int _Tint = Shader.PropertyToID("_Tint");
        public static readonly int _Exposure = Shader.PropertyToID("_Exposure");
        public static readonly int _Rotation = Shader.PropertyToID("_Rotation");
        public static readonly int _PixelCoordToViewDirWS = Shader.PropertyToID("_PixelCoordToViewDirWS");
        public static readonly int _SkyParam = Shader.PropertyToID("_SkyParam");
        public static readonly int _CloudColor = Shader.PropertyToID("_CloudColor");

        private int _skyPass;
        private Material _skyMaterial;
        private MaterialPropertyBlock _materialPropertyBlock;
        private MaterialPropertyBlock _cloudMaterialPropertyBlock;

        public VLSkyPass(RenderPassEvent evt, Material skyMaterial, int pass = 0)
        {
            _skyMaterial = skyMaterial;
            _skyPass = pass;
            _materialPropertyBlock = new MaterialPropertyBlock();
            _cloudMaterialPropertyBlock = new MaterialPropertyBlock();
            base.profilingSampler = new ProfilingSampler(nameof(VLSkyPass));
            renderPassEvent = evt;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.camera.clearFlags != CameraClearFlags.Skybox || renderingData.cameraData.renderType != CameraRenderType.Base)
                return;

            Material skyboxMaterial;
            if (RenderSettings.skybox != null)
                skyboxMaterial = RenderSettings.skybox;
            else if (renderingData.cameraData.camera.TryGetComponent(out Skybox cameraSkybox))
                skyboxMaterial = cameraSkybox.material;
            else
                skyboxMaterial = null;
            if (skyboxMaterial == null)
                return;

            Texture texture;
            Color tint;
            float exposure;
            float rotation;
            if (skyboxMaterial.HasProperty(_MainTex))
            {
                texture = skyboxMaterial.GetTexture(_MainTex);
                tint = skyboxMaterial.GetColor(_Tint);
                exposure = skyboxMaterial.GetFloat(_Exposure);
                rotation = skyboxMaterial.GetFloat(_Rotation);
            }
            else
            {
                texture = null;
                tint = Color.black;
                exposure = 1f;
                rotation = 0f;
            }
            if (texture == null)
                texture = Texture2D.blackTexture;

            var cmd = renderingData.commandBuffer;
            using (new ProfilingScope(cmd, profilingSampler))
            {
                _materialPropertyBlock.SetTexture(_MainTex, texture);
                _materialPropertyBlock.SetColor(_Tint, tint);
                _materialPropertyBlock.SetFloat(_Exposure, exposure);
                float rotationRad = -rotation * Mathf.Deg2Rad;
                _materialPropertyBlock.SetVector(_SkyParam, new Vector4(0f, 0f, Mathf.Cos(rotationRad), Mathf.Sin(rotationRad)));
                Matrix4x4 gpuProj = renderingData.cameraData.GetGPUProjectionMatrix();
                Matrix4x4 viewMatrix = renderingData.cameraData.GetViewMatrix();
                Matrix4x4 pixelCoordToViewDir = ComputePixelCoordToWorldSpaceViewDirectionMatrix(renderingData.cameraData.camera, GetScaledScreenSize(ref renderingData.cameraData), viewMatrix, gpuProj);
                cmd.SetGlobalMatrix(_PixelCoordToViewDirWS, pixelCoordToViewDir);
                CoreUtils.DrawFullScreen(cmd, _skyMaterial, _materialPropertyBlock, _skyPass);
                DrawClouds(cmd, VolumeManager.instance.stack.GetComponent<VLCloud>());
            }
        }

        internal static Vector4 GetScaledScreenSize(ref CameraData cameraData)
        {
            float width = cameraData.pixelRect.width;
            float height = cameraData.pixelRect.height;
            float scale = cameraData.isSceneViewCamera ? 1f : cameraData.renderScale;
            width *= scale;
            height *= scale;
            if (cameraData.camera.allowDynamicResolution)
            {
                width *= ScalableBufferManager.widthScaleFactor;
                height *= ScalableBufferManager.heightScaleFactor;
            }
            return new Vector4(width, height, 1f / width, 1f / height);
        }

        internal static Matrix4x4 ComputePixelCoordToWorldSpaceViewDirectionMatrix(Camera camera, Vector4 resolution, in Matrix4x4 viewMatrix, in Matrix4x4 gpuProj)
        {
            float aspect = -gpuProj.m11 / gpuProj.m00;
            float verticalFoV = camera.GetGateFittedFieldOfView() * Mathf.Deg2Rad;
            if (!camera.usePhysicalProperties)
                verticalFoV = Mathf.Atan(-1.0f / gpuProj[5]) * 2;
            Vector2 lensShift = camera.GetGateFittedLensShift();
            return ComputePixelCoordToWorldSpaceViewDirectionMatrix(verticalFoV, lensShift, resolution, viewMatrix, false, aspect, camera.orthographic);
        }

        internal static Matrix4x4 ComputePixelCoordToWorldSpaceViewDirectionMatrix(float verticalFoV, Vector2 lensShift, Vector4 screenSize, Matrix4x4 worldToViewMatrix, bool renderToCubemap, float aspectRatio = -1, bool isOrthographic = false)
        {
            Matrix4x4 viewSpaceRasterTransform;

            if (isOrthographic)
            {
                viewSpaceRasterTransform = new Matrix4x4(
                    new Vector4(-2.0f * screenSize.z, 0.0f, 0.0f, 0.0f),
                    new Vector4(0.0f, -2.0f * screenSize.w, 0.0f, 0.0f),
                    new Vector4(1.0f, 1.0f, -1.0f, 0.0f),
                    new Vector4(0.0f, 0.0f, 0.0f, 0.0f));
            }
            else
            {
                aspectRatio = aspectRatio < 0 ? screenSize.x * screenSize.w : aspectRatio;
                float tanHalfVertFoV = Mathf.Tan(0.5f * verticalFoV);

                float m21 = (1.0f - 2.0f * lensShift.y) * tanHalfVertFoV;
                float m11 = -2.0f * screenSize.w * tanHalfVertFoV;

                float m20 = (1.0f - 2.0f * lensShift.x) * tanHalfVertFoV * aspectRatio;
                float m00 = -2.0f * screenSize.z * tanHalfVertFoV * aspectRatio;

                if (renderToCubemap)
                {
                    m11 = -m11;
                    m21 = -m21;
                }

                viewSpaceRasterTransform = new Matrix4x4(new Vector4(m00, 0.0f, 0.0f, 0.0f),
                    new Vector4(0.0f, m11, 0.0f, 0.0f),
                    new Vector4(m20, m21, -1.0f, 0.0f),
                    new Vector4(0.0f, 0.0f, 0.0f, 1.0f));
            }

            var homogeneousZero = new Vector4(0, 0, 0, 1);
            worldToViewMatrix.SetColumn(3, homogeneousZero);
            worldToViewMatrix.SetRow(2, -worldToViewMatrix.GetRow(2));
            return Matrix4x4.Transpose(worldToViewMatrix.transpose * viewSpaceRasterTransform);
        }

        private void DrawClouds(CommandBuffer cmd, VLCloud vlCloud)
        {
            if (vlCloud == null)
                return;
            if (vlCloud.cloudData.value == null)
                return;
            var cloudData = vlCloud.cloudData.value;
            if (cloudData.cloudMaterial == null)
                return;
            if (cloudData.cloudLayers.Length == 0)
                return;
            double time = ShaderTimeManager.CurrentTime;
            foreach (var layer in cloudData.cloudLayers)
            {
                if (layer.mesh == null)
                    continue;
                var matrix = Matrix4x4.TRS(new Vector3(0f, vlCloud.cloudOffsetY.value, 0f),
                    Quaternion.AngleAxis((float)(time * layer.rotateSpeed * vlCloud.multiplySpeed.value % 360.0), Vector3.up), Vector3.one);
                _cloudMaterialPropertyBlock.SetColor(_CloudColor, vlCloud.cloudEmissionColor.value);
                cmd.DrawMesh(layer.mesh, matrix, cloudData.cloudMaterial, 0, 0, _cloudMaterialPropertyBlock);
            }
        }
    }
}
