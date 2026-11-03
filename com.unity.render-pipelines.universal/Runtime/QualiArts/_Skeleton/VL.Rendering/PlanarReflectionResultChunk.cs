// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering {
using Unity.Collections;
using Unity.Mathematics;
using VL.EntitySystem;

public class PlanarReflectionResultChunk : Chunk
{


	public NativeArray<float4x4> maskMeshMatrices;


	public NativeArray<float4x4> worldToTangentMatrices;


	public NativeArray<float4> clipPlanes;


	public NativeArray<float4x4> reflectionMatrices;


	public NativeArray<float4x4> reflectionViewMatrices;


	public NativeArray<float3> reflectionCameraPositions;


	public PlanarReflectionResultChunk()
	{
	}


	public override void RemoveAtSwapBack(int entityIndex)
	{
	}


	public override void SetCapacity(int newCapacity)
	{
	}


	public override void Dispose()
	{
	}
}

}
