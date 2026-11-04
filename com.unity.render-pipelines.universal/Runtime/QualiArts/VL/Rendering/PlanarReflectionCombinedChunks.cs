using System;
using VL.EntitySystem;
namespace VL.Rendering
{
    public class PlanarReflectionCombinedChunks : ICombinedChunks, IDisposable, IEntityChunk<PlanarReflection, PlanarReflectionEntityChunk>
    {
        public PlanarReflectionEntityChunk entityChunk { get; private set; }
        public PlanarReflectionCachedChunk cachedChunk { get; private set; }
        public PlanarReflectionCulledChunk culledChunk { get; private set; }
        public PlanarReflectionResultChunk resultChunk { get; private set; }
        public void Create()
        {
            cachedChunk = new PlanarReflectionCachedChunk();
            culledChunk = new PlanarReflectionCulledChunk();
            resultChunk = new PlanarReflectionResultChunk();
        }
        public void CreateEntityChunk() { entityChunk = new PlanarReflectionEntityChunk(); }
        public void SetCapacity(int newCapacity)
        {
            entityChunk.SetCapacity(newCapacity);
            cachedChunk.SetCapacity(newCapacity);
            culledChunk.SetCapacity(newCapacity);
            resultChunk.SetCapacity(newCapacity);
        }
        public void Push()
        {
            entityChunk.Push();
            cachedChunk.Push();
            culledChunk.Push();
            resultChunk.Push();
        }
        public void RemoveAtSwapBack(int arrayIndex)
        {
            entityChunk.currentJobHandle.Complete();
            cachedChunk.currentJobHandle.Complete();
            culledChunk.currentJobHandle.Complete();
            resultChunk.currentJobHandle.Complete();
            entityChunk.RemoveAtSwapBack(arrayIndex);
            cachedChunk.RemoveAtSwapBack(arrayIndex);
            culledChunk.RemoveAtSwapBack(arrayIndex);
            resultChunk.RemoveAtSwapBack(arrayIndex);
        }
        public void Dispose()
        {
            entityChunk.currentJobHandle.Complete();
            cachedChunk.currentJobHandle.Complete();
            culledChunk.currentJobHandle.Complete();
            resultChunk.currentJobHandle.Complete();
            entityChunk.Dispose();
            cachedChunk.Dispose();
            culledChunk.Dispose();
            resultChunk.Dispose();
        }
    }
}
