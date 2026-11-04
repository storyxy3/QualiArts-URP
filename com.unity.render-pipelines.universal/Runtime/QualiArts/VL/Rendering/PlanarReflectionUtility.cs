using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
namespace VL.Rendering
{
    public class PlanarReflectionUtility
    {
        public static class PropertyId
        {
            public static readonly int _PlanarReflection_CameraPos = Shader.PropertyToID("_PlanarReflection_CameraPos");
            public static readonly int _PlanarReflection_MatrixV = Shader.PropertyToID("_PlanarReflection_MatrixV");
            public static readonly int _PlanarReflection_MatrixP = Shader.PropertyToID("_PlanarReflection_MatrixP");
            public static readonly int _PlanarReflectionPlane = Shader.PropertyToID("_PlanarReflectionPlane");
            public static readonly int _PlanarReflectionMatrix = Shader.PropertyToID("_PlanarReflectionMatrix");
        }
        public static readonly Matrix4x4[] OffsetMatrices = {
            Matrix4x4.Translate(new Vector3(0, -0.025f, 0)),
            Matrix4x4.Translate(new Vector3(0, 0.025f, 0))
        };
        public static void RenderPlanarReflection(CommandBuffer cmd, ScriptableRenderContext context, ref RenderingData renderingData,
            PlanarReflectionEntityManager entityManager, int index, Material material, int stencilPass, int maskPass, int resetPass, ref RenderStateBlock colorRenderState)
        {
            if (entityManager == null || entityManager.combinedChunks == null) return;
            var chunks = entityManager.combinedChunks;
            int visibleIndex = chunks.culledChunk.visibleIndices[index];
            if (visibleIndex < 0) return;
            var resultChunk = chunks.resultChunk;
            var cachedChunk = chunks.cachedChunk;
            var camera = renderingData.cameraData.camera;
            Matrix4x4 maskMatrix = resultChunk.maskMeshMatrices.ReinterpretLoad<Matrix4x4>(index);
            Mesh mesh = cachedChunk.meshes[visibleIndex];
            Vector4 reflectionPlane = cachedChunk.reflectionPlanes.ReinterpretLoad<Vector4>(visibleIndex);
            Vector4 clipPlane = resultChunk.clipPlanes.ReinterpretLoad<Vector4>(index);
            Matrix4x4 projectionMatrix = camera.CalculateObliqueMatrix(clipPlane);
            Matrix4x4 viewMatrix = resultChunk.reflectionViewMatrices.ReinterpretLoad<Matrix4x4>(index);
            cmd.SetGlobalVector(PropertyId._PlanarReflectionPlane, reflectionPlane);
            cmd.DrawMesh(mesh, maskMatrix, material, 0, stencilPass);
            cmd.DrawMesh(mesh, maskMatrix, material, 0, maskPass);
            cmd.SetGlobalVector(PropertyId._PlanarReflection_CameraPos, renderingData.cameraData.worldSpaceCameraPos);
            cmd.SetGlobalMatrix(PropertyId._PlanarReflection_MatrixV, renderingData.cameraData.GetViewMatrix());
            cmd.SetGlobalMatrix(PropertyId._PlanarReflection_MatrixP, renderingData.cameraData.GetProjectionMatrix());
            cmd.SetInvertCulling(!GL.invertCulling);
            Vector3 reflectionCameraPosition = resultChunk.reflectionCameraPositions.ReinterpretLoad<Vector3>(index);
            Matrix4x4 reflectionMatrix = resultChunk.reflectionMatrices.ReinterpretLoad<Matrix4x4>(index);
            cmd.SetGlobalVector(ShaderPropertyId.worldSpaceCameraPos, reflectionCameraPosition);
            cmd.SetGlobalMatrix(PropertyId._PlanarReflectionMatrix, reflectionMatrix);
            cmd.SetGlobalVector(PropertyId._PlanarReflectionPlane, reflectionPlane);
            cmd.SetViewProjectionMatrices(viewMatrix, projectionMatrix);
            camera.TryGetCullingParameters(out var cullingParameters);
            CameraUtility.CalcCullingPlane(ref cullingParameters, projectionMatrix * viewMatrix);
            cullingParameters.maximumVisibleLights = 0;
            cullingParameters.shadowDistance = 0;
            cullingParameters.cullingOptions = CullingOptions.DisablePerObjectCulling;
            cullingParameters.cullingMask = (uint)cachedChunk.filteringSettings[visibleIndex].layerMask;
            cullingParameters.reflectionProbeSortingCriteria = ReflectionProbeSortingCriteria.None;
            var cullingResults = context.Cull(ref cullingParameters);
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            var filteringSettings = cachedChunk.filteringSettings[visibleIndex];
            var depthDrawingSettings = new DrawingSettings(RenderingUtility.ShaderTagIds.DepthOnly,
                new SortingSettings(camera) { criteria = SortingCriteria.None }) {
                perObjectData = PerObjectData.None,
                enableDynamicBatching = renderingData.supportsDynamicBatching,
                enableInstancing = camera.cameraType != CameraType.Preview
            };
            var depthFilteringSettings = new FilteringSettings(filteringSettings.renderQueueRange, filteringSettings.layerMask, 0x10000);
            var depthRenderState = colorRenderState;
            var stencilState = depthRenderState.stencilState;
            stencilState.writeMask = 0;
            depthRenderState.stencilState = stencilState;
            context.DrawRenderers(cullingResults, ref depthDrawingSettings, ref depthFilteringSettings, ref depthRenderState);
            var colorDrawingSettings = new DrawingSettings(RenderingUtility.ShaderTagIds.UniversalForwardPerformance,
                new SortingSettings(camera) { criteria = SortingCriteria.OptimizeStateChanges | SortingCriteria.RenderQueue }) {
                perObjectData = renderingData.perObjectData,
                mainLightIndex = renderingData.lightData.mainLightIndex,
                enableDynamicBatching = renderingData.supportsDynamicBatching,
                enableInstancing = camera.cameraType != CameraType.Preview
            };
            filteringSettings.renderingLayerMask |= 0x20000;
            context.DrawRenderers(cullingResults, ref colorDrawingSettings, ref filteringSettings, ref colorRenderState);
            var shaderTags = cachedChunk.additionalShaderTagIdLists[visibleIndex];
            var shaderTagSpan = shaderTags.AsReadOnlySpan();
            if (shaderTagSpan.Length > 0)
            {
                var additionalDrawingSettings = new DrawingSettings(shaderTagSpan[0],
                    new SortingSettings(camera) { criteria = SortingCriteria.OptimizeStateChanges }) {
                    perObjectData = renderingData.perObjectData,
                    mainLightIndex = renderingData.lightData.mainLightIndex,
                    enableDynamicBatching = renderingData.supportsDynamicBatching,
                    enableInstancing = camera.cameraType != CameraType.Preview
                };
                for (int i = 1; i < shaderTagSpan.Length; ++i)
                    additionalDrawingSettings.SetShaderPassName(i, shaderTagSpan[i]);
                context.DrawRenderers(cullingResults, ref additionalDrawingSettings, ref filteringSettings, ref colorRenderState);
            }
            cmd.SetInvertCulling(GL.invertCulling);
            cmd.SetViewProjectionMatrices(renderingData.cameraData.GetViewMatrix(), renderingData.cameraData.GetProjectionMatrix());
            cmd.SetGlobalVector(ShaderPropertyId.worldSpaceCameraPos, renderingData.cameraData.worldSpaceCameraPos);
            cmd.DrawMesh(mesh, maskMatrix, material, 0, resetPass);
        }
    }
}
