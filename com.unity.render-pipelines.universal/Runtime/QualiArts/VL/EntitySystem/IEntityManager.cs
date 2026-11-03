using System;
namespace VL.EntitySystem
{
    public interface IEntityManager<TComponent> : IDisposable where TComponent : IEntityComponent
    {
        bool IsValidEntity(Entity entity);
        Entity CreateEntity(TComponent component);
        void UpdateEntityData(Entity entity, TComponent component);
        void DestroyEntity(Entity entity);
    }
}
