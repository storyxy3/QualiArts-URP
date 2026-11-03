// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering {
using System;
using UnityEngine;
using UnityEngine.Rendering;
using VL.Rendering;

[Serializable]

[VolumeComponentMenu("VL/Mirror Plane")]
public class VLMirrorVolumeComponent : VolumeComponent
{


	private static readonly Plane[] Planes;


	[Tooltip("MirrorPlaneを使用")]
	public MirrorMeshParameter mesh;


	[Tooltip("基準座標")]
	public NoInterpVector3Parameter position;


	[Tooltip("回転量")]
	public MirrorQuaternionParameter rotation;


	[Tooltip("スケール")]
	public NoInterpVector3Parameter scale;


	[Tooltip("法線軸の向き")]
	public NoInterpVector3Parameter normal;


	[Tooltip("Smoothを使用する場合はtrue, 普通の鏡はfalse")]
	public BoolParameter useGlossy;


	[Tooltip("Bumpmapスケール")]
	public ClampedFloatParameter bumpScale;


	[Tooltip("Bumpmap制限")]
	public ClampedFloatParameter bumpLimit;


	[Tooltip("Bounds")]
	public MirrorBoundsParameter bounds;

	public Vector4 plane
	{


		get
		{
			return default(Vector4);
		}
	}


	public bool IsActive(Camera c)
	{
		return default(bool);
	}


	public VLMirrorVolumeComponent()
	{
	}
}

}
