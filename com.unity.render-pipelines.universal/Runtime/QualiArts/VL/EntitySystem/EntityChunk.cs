using Unity.Collections;
using UnityEngine.Jobs;
using UnityEngine.Rendering;
namespace VL.EntitySystem
{
    public class EntityChunk<TComponent> : Chunk where TComponent : IEntityComponent
    {
        public NativeArray<Entity> entities;
        public TComponent[] components;
        public TransformAccessArray transformAccessArray;
        public override void Push() { ++count; }
        public override void RemoveAtSwapBack(int entityIndex)
        {
            RemoveAtSwapBack(ref entities, entityIndex, count);
            RemoveAtSwapBack(ref components, entityIndex, count);
            transformAccessArray.RemoveAtSwapBack(entityIndex);
            --count;
        }
        public override void SetCapacity(int newCapacity)
        {
            ArrayExtensions.ResizeArray(ref entities, newCapacity);
            ResizeNativeArray(ref transformAccessArray, components, newCapacity);
            ArrayExtensions.ResizeArray(ref components, newCapacity);
            capacity = newCapacity;
        }
        public override void Dispose()
        {
            if (capacity == 0) return;
            entities.Dispose();
            transformAccessArray.Dispose();
            components = null;
            count = 0;
            capacity = 0;
        }
    }
}
