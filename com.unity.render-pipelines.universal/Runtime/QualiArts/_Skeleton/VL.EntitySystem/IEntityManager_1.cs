// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.EntitySystem {
using System;
using VL.EntitySystem;

public interface IEntityManager<TComponent> : IDisposable where TComponent : IEntityComponent
{

	bool IsValidEntity(Entity entity);

	Entity CreateEntity(TComponent component);

	void UpdateEntityData(Entity entity, TComponent component);

	void DestroyEntity(Entity entity);
}

}
