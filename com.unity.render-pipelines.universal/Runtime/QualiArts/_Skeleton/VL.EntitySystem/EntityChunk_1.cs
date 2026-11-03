// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.EntitySystem {
using Unity.Collections;
using UnityEngine.Jobs;
using VL.EntitySystem;

public class EntityChunk<TComponent> : Chunk where TComponent : IEntityComponent
{


	public NativeArray<Entity> entities;


	public TComponent[] components;


	public TransformAccessArray transformAccessArray;

	public override void Push()
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

	public EntityChunk()
	{
	}
}

}
