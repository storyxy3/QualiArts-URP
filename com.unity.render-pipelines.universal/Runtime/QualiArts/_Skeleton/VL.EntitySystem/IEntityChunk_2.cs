// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.EntitySystem {
using VL.EntitySystem;

public interface IEntityChunk<TComponent, out TEntityChunk> where TComponent : IEntityComponent where TEntityChunk : EntityChunk<TComponent>, new()
{

	TEntityChunk entityChunk
	{

		get;
	}

	void CreateEntityChunk();
}

}
