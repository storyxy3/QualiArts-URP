// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.EntitySystem {
using VL.EntitySystem;

public interface IEntityComponent<TChunkKey> : IEntityComponent
{

	TChunkKey oldChunkKey
	{

		set;
	}

	TChunkKey chunkKey
	{

		get;
	}

	bool isChunkChanged
	{

		get;
	}
}

}
