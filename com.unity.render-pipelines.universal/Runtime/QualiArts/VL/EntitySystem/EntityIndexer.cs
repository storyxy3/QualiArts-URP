using System.Collections.Generic;
namespace VL.EntitySystem
{
    public class EntityIndexer
    {
        public struct EntityItem
        {
            public int chunkIndex;
            public int arrayIndex;
            public int version;
        }
        private List<EntityItem> _entities = new List<EntityItem>();
        private Queue<int> _freeIndices = new Queue<int>();
        public bool IsValid(Entity entity)
        {
            return entity.index < _entities.Count && _entities[entity.index].version == entity.version;
        }
        public Entity CreateEntity(int arrayIndex, int chunkIndex = 0)
        {
            if (_freeIndices.Count != 0)
            {
                int index = _freeIndices.Dequeue();
                var item = _entities[index];
                item.arrayIndex = arrayIndex;
                item.chunkIndex = chunkIndex;
                ++item.version;
                _entities[index] = item;
                return new Entity { index = index, version = item.version };
            }
            int newIndex = _entities.Count;
            _entities.Add(new EntityItem { chunkIndex = chunkIndex, arrayIndex = arrayIndex, version = 1 });
            return new Entity { index = newIndex, version = 1 };
        }
        public void DestroyEntity(Entity entity)
        {
            _freeIndices.Enqueue(entity.index);
            var item = _entities[entity.index];
            ++item.version;
            _entities[entity.index] = item;
        }
        public EntityItem GetItem(Entity entity) { return _entities[entity.index]; }
        public void UpdateIndex(Entity entity, int newArrayIndex)
        {
            var item = _entities[entity.index];
            item.arrayIndex = newArrayIndex;
            item.version = entity.version;
            _entities[entity.index] = item;
        }
        public void RemapChunkIndices(List<int> remaper)
        {
            for (int i = 0; i < _entities.Count; ++i)
            {
                int newIndex = remaper[_entities[i].chunkIndex];
                var item = _entities[i];
                item.chunkIndex = newIndex;
                _entities[i] = item;
            }
        }
        public void Clear()
        {
            _entities.Clear();
            _freeIndices.Clear();
        }
    }
}
