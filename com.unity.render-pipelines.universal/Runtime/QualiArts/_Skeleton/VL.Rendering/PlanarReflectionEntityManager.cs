// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering {
using UnityEngine;
using VL.EntitySystem;
using VL.Rendering;

public class PlanarReflectionEntityManager : SingleEntityManager<PlanarReflection, PlanarReflectionCombinedChunks, PlanarReflectionEntityChunk>
{


	private Mesh _boxMesh;


	protected override void UpdateChunk(ref PlanarReflectionCombinedChunks chunks, int arrayIndex, PlanarReflection component)
	{
	}


	public override void Dispose()
	{
	}


	private static Mesh CreateBoxMesh()
	{
		return null;
	}


	public PlanarReflectionEntityManager()
	{
	}
}

}
