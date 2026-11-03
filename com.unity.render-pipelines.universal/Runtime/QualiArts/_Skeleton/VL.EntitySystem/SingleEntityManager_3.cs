// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.EntitySystem {
using System;
using UnityEngine.Rendering;
using VL.EntitySystem;

public class SingleEntityManager<TComponent, TCombinedChunks, TEntityChunk> : IEntityManager<TComponent>, IDisposable where TComponent : IEntityComponent where TCombinedChunks : ICombinedChunks, IEntityChunk<TComponent, TEntityChunk>, new() where TEntityChunk : EntityChunk<TComponent>, new()
{


	public TCombinedChunks combinedChunks;


	private readonly ProfilingSampler _createEntitySampler;


	private readonly ProfilingSampler _resizeChunksSampler;


	private readonly EntityIndexer _entityIndexer;

	protected SingleEntityManager()
	{
	}

	public bool IsValidEntity(Entity entity)
	{
		return default(bool);
	}

	public Entity CreateEntity(TComponent component)
	{
		return default(Entity);
	}

	protected virtual void UpdateChunk(ref TCombinedChunks chunks, int arrayIndex, TComponent component)
	{
	}

	public void UpdateAllEntityData()
	{
	}

	public void UpdateEntityData(Entity entity, TComponent component)
	{
	}

	public void DestroyEntity(Entity entity)
	{
	}

	public void Update()
	{
	}

	public virtual void Dispose()
	{
	}
}

}
