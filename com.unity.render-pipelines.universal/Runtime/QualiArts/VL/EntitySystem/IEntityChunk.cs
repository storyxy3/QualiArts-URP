namespace VL.EntitySystem
{
    public interface IEntityChunk<TComponent, out TEntityChunk> where TComponent : IEntityComponent where TEntityChunk : EntityChunk<TComponent>, new()
    {
        TEntityChunk entityChunk { get; }
        void CreateEntityChunk();
    }
}
