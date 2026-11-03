// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering {
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using VL.EntitySystem;
using VL.Rendering;

public class PlanarReflectionCachedChunk : Chunk
{


	public Mesh[] meshes;


	public NativeArray<PlanarReflection.ShaderTagIdList> additionalShaderTagIdLists;


	public NativeArray<FilteringSettings> filteringSettings;


	public NativeArray<float3> positions;


	public NativeArray<quaternion> rotations;


	public NativeArray<float3> sizes;


	public NativeArray<float4x4> localToWorldMatrices;


	public NativeArray<Bounds> localBounds;


	public NativeArray<Bounds> bounds;


	public NativeArray<float4> reflectionPlanes;


	public NativeArray<float4x4> worldToTangentMatrices;


	public NativeArray<bool> fullResolutions;


	public override void RemoveAtSwapBack(int entityIndex)
	{
	}


	public override void SetCapacity(int newCapacity)
	{
	}


	public override void Dispose()
	{
	}


	public PlanarReflectionCachedChunk()
	{
	}
}

}
