// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.EntitySystem {
using VL.EntitySystem;

public interface IEntityChunk<TComponent, TChunkKey, TEntityChunk> where TComponent : IEntityComponent<TChunkKey> where TEntityChunk : EntityChunk<TComponent, TChunkKey>, new()
{

	TEntityChunk entityChunk
	{

		get;
	}

	void CreateEntityChunk(TChunkKey chunkKey);
}

}
