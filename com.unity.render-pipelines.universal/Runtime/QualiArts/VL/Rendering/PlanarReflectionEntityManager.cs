using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using VL.EntitySystem;
namespace VL.Rendering
{
    public class PlanarReflectionEntityManager : SingleEntityManager<PlanarReflection, PlanarReflectionCombinedChunks, PlanarReflectionEntityChunk>
    {
        private Mesh _boxMesh;
        protected override void UpdateChunk(ref PlanarReflectionCombinedChunks chunks, int arrayIndex, PlanarReflection component)
        {
            var cachedChunk = chunks.cachedChunk;
            cachedChunk.additionalShaderTagIdLists[arrayIndex] = component.shaderTagIdList;
            cachedChunk.filteringSettings[arrayIndex] = new FilteringSettings(new RenderQueueRange(component.minimumRenderQueue, component.maximumRenderQueue), component.layerMask, component.renderingLayerMask);
            cachedChunk.fullResolutions[arrayIndex] = component.fullResolution;
            if (component.maskType == PlanarReflection.MaskType.Mesh && component.mesh != null)
            {
                cachedChunk.meshes[arrayIndex] = component.mesh;
                cachedChunk.localBounds[arrayIndex] = component.mesh.bounds;
                cachedChunk.sizes[arrayIndex] = new float3(1);
                return;
            }
            if (_boxMesh == null) _boxMesh = CreateBoxMesh();
            cachedChunk.meshes[arrayIndex] = _boxMesh;
            Vector3 size = new Vector3(component.size.x, 0.05f, component.size.y);
            cachedChunk.localBounds[arrayIndex] = new Bounds(Vector3.zero, size);
            cachedChunk.sizes[arrayIndex] = size;
        }
        public override void Dispose()
        {
            base.Dispose();
            if (_boxMesh != null) CoreUtils.Destroy(_boxMesh);
        }
        private static Mesh CreateBoxMesh()
        {
            Vector3[] positions = {
                new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0.5f, -0.5f, -0.5f),
                new Vector3(0.5f, -0.5f, 0.5f), new Vector3(-0.5f, -0.5f, 0.5f),
                new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(0.5f, 0.5f, -0.5f),
                new Vector3(0.5f, 0.5f, 0.5f), new Vector3(-0.5f, 0.5f, 0.5f)
            };
            int[] indices = { 0, 1, 2, 0, 2, 3, 1, 6, 2, 1, 5, 6, 3, 4, 0, 3, 7, 4, 0, 5, 1, 0, 4, 5, 2, 6, 7, 2, 7, 3, 5, 4, 7, 5, 7, 6 };
            var mesh = new Mesh();
            mesh.hideFlags = HideFlags.DontSave;
            mesh.indexFormat = IndexFormat.UInt16;
            mesh.vertices = positions;
            mesh.triangles = indices;
            mesh.Optimize();
            mesh.UploadMeshData(true);
            return mesh;
        }
    }
}
