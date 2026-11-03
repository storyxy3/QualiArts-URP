using UnityEngine;
namespace VL.EntitySystem
{
    public interface IEntityComponent
    {
        Transform transform { get; }
        Entity entity { get; set; }
    }
    public interface IEntityComponent<TChunkKey> : IEntityComponent
    {
        TChunkKey oldChunkKey { set; }
        TChunkKey chunkKey { get; }
        bool isChunkChanged { get; }
    }
}
