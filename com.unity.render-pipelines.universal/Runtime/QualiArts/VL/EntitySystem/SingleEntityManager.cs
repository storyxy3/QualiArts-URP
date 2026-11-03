using System;
using UnityEngine;
using UnityEngine.Rendering;
namespace VL.EntitySystem
{
    public class SingleEntityManager<TComponent, TCombinedChunks, TEntityChunk> : IEntityManager<TComponent>, IDisposable
        where TComponent : IEntityComponent
        where TCombinedChunks : ICombinedChunks, IEntityChunk<TComponent, TEntityChunk>, new()
        where TEntityChunk : EntityChunk<TComponent>, new()
    {
        public TCombinedChunks combinedChunks;
        private readonly ProfilingSampler _createEntitySampler;
        private readonly ProfilingSampler _resizeChunksSampler;
        private readonly EntityIndexer _entityIndexer = new EntityIndexer();
        protected SingleEntityManager()
        {
            _createEntitySampler = new ProfilingSampler("EntityManager.CreateEntity");
            _resizeChunksSampler = new ProfilingSampler("EntityManager.ResizeChunks");
        }
        public bool IsValidEntity(Entity entity) { return _entityIndexer.IsValid(entity); }
        public Entity CreateEntity(TComponent component)
        {
            using (new ProfilingScope(null, _createEntitySampler))
            {
                if (combinedChunks == null)
                {
                    combinedChunks = new TCombinedChunks();
                    combinedChunks.Create();
                    combinedChunks.CreateEntityChunk();
                }
                var entityChunk = combinedChunks.entityChunk;
                int arrayIndex = entityChunk.count;
                var entity = _entityIndexer.CreateEntity(arrayIndex);
                if (entityChunk.capacity == entityChunk.count)
                {
                    using (new ProfilingScope(null, _resizeChunksSampler))
                        combinedChunks.SetCapacity(Mathf.Max(8, entityChunk.capacity * 2));
                }
                combinedChunks.Push();
                entityChunk.components[arrayIndex] = component;
                entityChunk.entities[arrayIndex] = entity;
                entityChunk.transformAccessArray.Add(component.transform);
                UpdateEntityData(entity, component);
                return entity;
            }
        }
        protected virtual void UpdateChunk(ref TCombinedChunks chunks, int arrayIndex, TComponent component) { }
        public void UpdateAllEntityData()
        {
            if (combinedChunks == null) return;
            for (int i = 0; i < combinedChunks.entityChunk.count; ++i)
                UpdateEntityData(combinedChunks.entityChunk.entities[i], combinedChunks.entityChunk.components[i]);
        }
        public void UpdateEntityData(Entity entity, TComponent component)
        {
            var item = _entityIndexer.GetItem(entity);
            UpdateChunk(ref combinedChunks, item.arrayIndex, component);
        }
        public void DestroyEntity(Entity entity)
        {
            if (!_entityIndexer.IsValid(entity)) return;
            var item = _entityIndexer.GetItem(entity);
            _entityIndexer.DestroyEntity(entity);
            var entityChunk = combinedChunks.entityChunk;
            int lastIndex = entityChunk.count - 1;
            if (lastIndex != item.arrayIndex)
                _entityIndexer.UpdateIndex(entityChunk.entities[lastIndex], item.arrayIndex);
            combinedChunks.RemoveAtSwapBack(item.arrayIndex);
        }
        public void Update()
        {
            if (combinedChunks != null && combinedChunks.entityChunk.count == 0)
            {
                combinedChunks.Dispose();
                combinedChunks = default;
            }
        }
        public virtual void Dispose()
        {
            if (combinedChunks != null)
            {
                combinedChunks.Dispose();
                combinedChunks = default;
            }
            _entityIndexer.Clear();
        }
    }
}
