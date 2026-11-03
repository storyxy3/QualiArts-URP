using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.Jobs;
namespace VL.EntitySystem
{
    public abstract class Chunk : IDisposable
    {
        public int count { get; protected set; }
        public int capacity { get; protected set; }
        public JobHandle currentJobHandle { get; set; }
        protected Chunk() { }
        public virtual void Push() { ++count; }
        public abstract void RemoveAtSwapBack(int entityIndex);
        public abstract void SetCapacity(int newCapacity);
        public virtual void Dispose() { }
        protected void ResizeNativeArray<T>(ref TransformAccessArray array, T[] components, int capacity) where T : IEntityComponent
        {
            var newArray = new TransformAccessArray(capacity);
            if (array.isCreated)
            {
                for (int i = 0; i < array.length; ++i)
                    newArray.Add(components[i].transform);
                array.Dispose();
            }
            array = newArray;
        }
        protected void RemoveAtSwapBack<T>(ref Unity.Collections.NativeArray<T> array, int index, int count) where T : struct
        {
            array[index] = array[count - 1];
        }
        protected void RemoveAtSwapBack<T>(ref T[] array, int index, int count)
        {
            array[index] = array[count - 1];
        }
    }
}
