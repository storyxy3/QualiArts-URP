// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.EntitySystem {
using System;
using System.Collections.Generic;
using UnityEngine.Rendering;
using VL.EntitySystem;

public class EntityManager<TComponent, TChunkKey, TCombinedChunks, TEntityChunk> : IEntityManager<TComponent>, IDisposable where TComponent : IEntityComponent<TChunkKey> where TCombinedChunks : ICombinedChunks, IEntityChunk<TComponent, TChunkKey, TEntityChunk>, new() where TEntityChunk : EntityChunk<TComponent, TChunkKey>, new()
{

	public struct TempChunks
	{


		public TCombinedChunks combinedChunks;


		public int previousChunkIndex;


		public bool valid;
	}


	public readonly List<TCombinedChunks> combinedChunksList;


	public int chunkCount;


	private readonly ProfilingSampler _createEntitySampler;


	private readonly ProfilingSampler _resizeChunksSampler;


	private readonly ProfilingSampler _sortChunksSampler;


	private readonly EntityIndexer _entityIndexer;


	private readonly Dictionary<TChunkKey, int> _objectToChunkIndex;


	private readonly List<TempChunks> _tempChunks;


	private readonly List<int> _combinedChunkRemap;


	private Comparison<TempChunks> _sortCache;

	protected virtual TChunkKey defaultChunkKey
	{

		get
		{
			return default(TChunkKey);
		}
	}

	public EntityManager()
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

	private int CreateChunkIndex(TChunkKey chunkKey)
	{
		return default(int);
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

	protected virtual int ChunkSort(TempChunks a, TempChunks b)
	{
		return default(int);
	}

	public void Update()
	{
	}

	public virtual void Dispose()
	{
	}
}

}
