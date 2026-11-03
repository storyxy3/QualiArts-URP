// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.EntitySystem {
using System.Runtime.CompilerServices;
using VL.EntitySystem;

public static class EntityComponentEvent<TComponent>
{

	public delegate void EntityComponentAction(TComponent component);


	[CompilerGenerated]
	private static EntityComponentAction m_onAdd;


	[CompilerGenerated]
	private static EntityComponentAction m_onRemove;


	[CompilerGenerated]
	private static EntityComponentAction m_onPropertyChange;

	public static event EntityComponentAction onAdd
	{

		[CompilerGenerated]
		add
		{
		}

		[CompilerGenerated]
		remove
		{
		}
	}

	public static event EntityComponentAction onRemove
	{

		[CompilerGenerated]
		add
		{
		}

		[CompilerGenerated]
		remove
		{
		}
	}

	public static event EntityComponentAction onPropertyChange
	{

		[CompilerGenerated]
		add
		{
		}

		[CompilerGenerated]
		remove
		{
		}
	}

	public static void InvokeAdd(IEntityComponent component)
	{
	}

	public static void InvokeRemove(IEntityComponent component)
	{
	}

	public static void InvokePropertyChange(IEntityComponent component)
	{
	}

	public static void InvokeAdd<T>(IEntityComponent<T> component)
	{
	}

	public static void InvokeRemove<T>(IEntityComponent<T> component)
	{
	}

	public static void InvokePropertyChange<T>(IEntityComponent<T> component)
	{
	}
}

}
