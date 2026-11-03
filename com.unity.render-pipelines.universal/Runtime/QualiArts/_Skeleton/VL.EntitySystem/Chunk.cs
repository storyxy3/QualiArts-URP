// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.EntitySystem {
using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.Jobs;
using VL.EntitySystem;

public abstract class Chunk : IDisposable
{


	[CompilerGenerated]
	private int _003Ccount_003Ek__BackingField;


	[CompilerGenerated]
	private int _003Ccapacity_003Ek__BackingField;


	[CompilerGenerated]
	private JobHandle _003CcurrentJobHandle_003Ek__BackingField;

	public int count
	{


		[CompilerGenerated]
		get
		{
			return default(int);
		}


		[CompilerGenerated]
		protected set
		{
		}
	}

	public int capacity
	{


		[CompilerGenerated]
		get
		{
			return default(int);
		}


		[CompilerGenerated]
		protected set
		{
		}
	}

	public JobHandle currentJobHandle
	{


		[CompilerGenerated]
		get
		{
			return default(JobHandle);
		}


		[CompilerGenerated]
		set
		{
		}
	}


	public virtual void Push()
	{
	}

	public abstract void RemoveAtSwapBack(int entityIndex);

	public abstract void SetCapacity(int newCapacity);


	public virtual void Dispose()
	{
	}

	protected void ResizeNativeArray<T>(ref TransformAccessArray array, T[] components, int capacity) where T : IEntityComponent
	{
	}

	protected void RemoveAtSwapBack<T>(ref NativeArray<T> array, int index, int count) where T : struct
	{
	}

	protected void RemoveAtSwapBack<T>(ref T[] array, int index, int count)
	{
	}


	protected Chunk()
	{
	}
}

}
