using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace VL.EntitySystem
{
    public class EntityManager<TComponent, TChunkKey, TCombinedChunks, TEntityChunk> : IEntityManager<TComponent>, IDisposable
        where TComponent : IEntityComponent<TChunkKey>
        where TCombinedChunks : ICombinedChunks, IEntityChunk<TComponent, TChunkKey, TEntityChunk>, new()
        where TEntityChunk : EntityChunk<TComponent, TChunkKey>, new()
    {
        public struct TempChunks
        {
            public TCombinedChunks combinedChunks;
            public int previousChunkIndex;
            public bool valid;
        }

        public readonly List<TCombinedChunks> combinedChunksList = new List<TCombinedChunks>();
        public int chunkCount;
        private readonly ProfilingSampler _createEntitySampler;
        private readonly ProfilingSampler _resizeChunksSampler;
        private readonly ProfilingSampler _sortChunksSampler;
        private readonly EntityIndexer _entityIndexer = new EntityIndexer();
        private readonly Dictionary<TChunkKey, int> _objectToChunkIndex = new Dictionary<TChunkKey, int>();
        private readonly List<TempChunks> _tempChunks = new List<TempChunks>();
        private readonly List<int> _combinedChunkRemap = new List<int>();
        private Comparison<TempChunks> _sortCache;

        protected virtual TChunkKey defaultChunkKey => default;

        public EntityManager()
        {
            _createEntitySampler = new ProfilingSampler("EntityManager.CreateEntity");
            _resizeChunksSampler = new ProfilingSampler("EntityManager.ResizeChunks");
            _sortChunksSampler = new ProfilingSampler("EntityManager.SortChunks");
            _sortCache = ChunkSort;
        }

        public bool IsValidEntity(Entity entity)
        {
            return _entityIndexer.IsValid(entity);
        }

        public Entity CreateEntity(TComponent component)
        {
            TChunkKey chunkKey = component.chunkKey;
            if (chunkKey == null)
                chunkKey = defaultChunkKey;
            using (new ProfilingScope(null, _createEntitySampler))
            {
                int chunkIndex = CreateChunkIndex(chunkKey);
                var entityChunk = combinedChunksList[chunkIndex].entityChunk;
                int arrayIndex = entityChunk.count;
                Entity entity = _entityIndexer.CreateEntity(arrayIndex, chunkIndex);
                if (entityChunk.capacity == entityChunk.count)
                {
                    using (new ProfilingScope(null, _resizeChunksSampler))
                        combinedChunksList[chunkIndex].SetCapacity(Mathf.Max(8, entityChunk.capacity * 2));
                }
                combinedChunksList[chunkIndex].Push();
                entityChunk.components[arrayIndex] = component;
                entityChunk.entities[arrayIndex] = entity;
                entityChunk.transformAccessArray.Add(component.transform);
                UpdateEntityData(entity, component);
                return entity;
            }
        }

        private int CreateChunkIndex(TChunkKey chunkKey)
        {
            if (_objectToChunkIndex.TryGetValue(chunkKey, out int chunkIndex))
                return chunkIndex;
            var chunks = new TCombinedChunks();
            chunks.Create();
            chunks.CreateEntityChunk(chunkKey);
            combinedChunksList.Add(chunks);
            _tempChunks.Add(default);
            _combinedChunkRemap.Add(0);
            _objectToChunkIndex.Add(chunkKey, chunkCount);
            return chunkCount++;
        }

        protected virtual void UpdateChunk(ref TCombinedChunks chunks, int arrayIndex, TComponent component)
        {
        }

        public void UpdateAllEntityData()
        {
            for (int i = 0; i < chunkCount; i++)
            {
                var chunks = _tempChunks[i].combinedChunks;
                for (int j = 0; j < chunks.entityChunk.count; j++)
                    UpdateEntityData(chunks.entityChunk.entities[j], chunks.entityChunk.components[j]);
            }
        }

        public void UpdateEntityData(Entity entity, TComponent component)
        {
            var item = _entityIndexer.GetItem(entity);
            var chunks = combinedChunksList[item.chunkIndex];
            UpdateChunk(ref chunks, item.arrayIndex, component);
        }

        public void DestroyEntity(Entity entity)
        {
            if (!_entityIndexer.IsValid(entity))
                return;
            var item = _entityIndexer.GetItem(entity);
            _entityIndexer.DestroyEntity(entity);
            var chunks = combinedChunksList[item.chunkIndex];
            var entityChunk = chunks.entityChunk;
            int lastIndex = entityChunk.count - 1;
            if (lastIndex != item.arrayIndex)
                _entityIndexer.UpdateIndex(entityChunk.entities[lastIndex], item.arrayIndex);
            chunks.RemoveAtSwapBack(item.arrayIndex);
        }

        protected virtual int ChunkSort(TempChunks a, TempChunks b)
        {
            if (a.valid && !b.valid)
                return -1;
            if (!a.valid && b.valid)
                return 1;
            return 0;
        }

        public void Update()
        {
            using (new ProfilingScope(null, _sortChunksSampler))
            {
                for (int i = 0; i < chunkCount; i++)
                {
                    _ = combinedChunksList[i].entityChunk.chunkKey;
                    _tempChunks[i] = new TempChunks
                    {
                        combinedChunks = combinedChunksList[i],
                        previousChunkIndex = i,
                        valid = combinedChunksList[i].entityChunk.count != 0
                    };
                }
                _tempChunks.Sort(_sortCache);
                bool changed = false;
                for (int i = 0; i < chunkCount; i++)
                {
                    if (i != _tempChunks[i].previousChunkIndex || !_tempChunks[i].valid)
                    {
                        changed = true;
                        break;
                    }
                }
                if (!changed)
                    return;
                _objectToChunkIndex.Clear();
                int validCount = 0;
                for (int i = 0; i < chunkCount; i++)
                {
                    var temp = _tempChunks[i];
                    if (temp.valid)
                    {
                        combinedChunksList[i] = temp.combinedChunks;
                        if (!_objectToChunkIndex.ContainsKey(combinedChunksList[i].entityChunk.chunkKey))
                            _objectToChunkIndex.Add(combinedChunksList[i].entityChunk.chunkKey, i);
                        _combinedChunkRemap[temp.previousChunkIndex] = i;
                        validCount++;
                    }
                    else
                        temp.combinedChunks.Dispose();
                }
                if (chunkCount > validCount)
                {
                    combinedChunksList.RemoveRange(validCount, chunkCount - validCount);
                    _tempChunks.RemoveRange(validCount, chunkCount - validCount);
                    chunkCount = validCount;
                }
                _entityIndexer.RemapChunkIndices(_combinedChunkRemap);
            }
        }

        public virtual void Dispose()
        {
            foreach (var chunks in combinedChunksList)
                chunks.Dispose();
            _entityIndexer.Clear();
            _objectToChunkIndex.Clear();
            combinedChunksList.Clear();
            _tempChunks.Clear();
            chunkCount = 0;
        }
    }
}
