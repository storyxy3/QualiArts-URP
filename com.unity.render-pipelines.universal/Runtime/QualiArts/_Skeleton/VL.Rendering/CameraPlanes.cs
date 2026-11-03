// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering {
using System;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

[Serializable]

public struct CameraPlanes
{


	public float4 p0;


	public float4 p1;


	public float4 p2;


	public float4 p3;


	public float4 p4;


	public float4 p5;

	public unsafe ref float4 this[int index]
	{


		get
		{
			return ref *(float4*)null;
		}
	}

	[MethodImpl(256)]


	public CameraPlanes(in Matrix4x4 vpMatrix) : this()
	{
	}


	public bool TestAABB(Bounds bounds)
	{
		return default(bool);
	}


	public bool TestBoundingSphere(float4 sphere)
	{
		return default(bool);
	}

	[MethodImpl(256)]


	private static float GetDistanceToPoint(float4 plane, float3 point)
	{
		return default(float);
	}
}

}
