// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.EntitySystem {
using System.Collections.Generic;
using VL.EntitySystem;

public class EntityIndexer
{

	public struct EntityItem
	{


		public int chunkIndex;


		public int arrayIndex;


		public int version;
	}


	private List<EntityItem> _entities;


	private Queue<int> _freeIndices;


	public bool IsValid(Entity entity)
	{
		return default(bool);
	}


	public Entity CreateEntity(int arrayIndex, int chunkIndex = 0)
	{
		return default(Entity);
	}


	public void DestroyEntity(Entity entity)
	{
	}


	public EntityItem GetItem(Entity entity)
	{
		return default(EntityItem);
	}


	public void UpdateIndex(Entity entity, int newArrayIndex)
	{
	}


	public void RemapChunkIndices(List<int> remaper)
	{
	}


	public void Clear()
	{
	}


	public EntityIndexer()
	{
	}
}

}
