// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.EntitySystem {
using System;
using UnityEngine;
using VL.EntitySystem;

public class SharedEntityManager<TManager, TComponent> : IDisposable where TManager : class, IEntityManager<TComponent>, new() where TComponent : MonoBehaviour, IEntityComponent
{


	private TManager _entityManager;


	private int _referenceCounter;

	public TManager Get()
	{
		return null;
	}

	public void Release(ref TManager manager)
	{
	}

	public void Dispose()
	{
	}

	private void OnAdd(TComponent component)
	{
	}

	private void OnRemove(TComponent component)
	{
	}

	private void OnPropertyChange(TComponent component)
	{
	}

	public SharedEntityManager()
	{
	}
}

}
