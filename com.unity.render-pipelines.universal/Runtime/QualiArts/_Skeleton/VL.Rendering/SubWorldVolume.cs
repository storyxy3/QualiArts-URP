// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering {
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using VL.Rendering;

[ExecuteAlways]
public class SubWorldVolume : Volume
{


	private static readonly Plane[] Planes;


	public SubWorldMode mode;


	public Mesh mesh;


	public LayerMask layerMask;


	public int minimumRenderQueue;


	public int maximumRenderQueue;


	public uint renderingLayerMask;

	[NonSerialized]


	private Vector3 _position;

	[NonSerialized]


	private Bounds _bounds;

	[NonSerialized]


	private FilteringSettings _filteringSettings;

	[NonSerialized]


	private SubWorld _subWorld;

	public unsafe ref FilteringSettings filteringSettings
	{


		get
		{
			return ref *(FilteringSettings*)null;
		}
	}

	public bool maskMode
	{


		get
		{
			return default(bool);
		}
	}

	public Vector3 position
	{


		get
		{
			return default(Vector3);
		}
	}


	private void OnDestroy()
	{
	}


	private void LateUpdate()
	{
	}


	public void UpdateParameters()
	{
	}


	private void UpdateMeshParameter()
	{
	}


	private void UpdateTextureParameter()
	{
	}


	public bool IsActive(Camera c, Vector3 cameraPosition)
	{
		return default(bool);
	}


	public Rect GetViewportRect(ref CameraData cameraData)
	{
		return default(Rect);
	}


	private void OnValidate()
	{
	}


	public SubWorldVolume()
	{
	}
}

}
