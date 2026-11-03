using System;
namespace VL.EntitySystem
{
    public interface ICombinedChunks : IDisposable
    {
        void Create();
        void SetCapacity(int newCapacity);
        void Push();
        void RemoveAtSwapBack(int arrayIndex);
    }
}
