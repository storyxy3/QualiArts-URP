// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering {
using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using VL.Rendering;

[Serializable]

[VolumeComponentMenu("VL/ChromaticAberration")]
public class VLChromaticAberration : VolumeComponent, IPostProcessComponent
{


	public Vector2Parameter redDirection;


	public Vector2Parameter greenDirection;


	public Vector2Parameter blueDirection;


	public IntervalTypeParameter intervalType;


	public ClampedFloatParameter frequency;


	public ClampedFloatParameter rgbSplit;


	public bool IsActive()
	{
		return default(bool);
	}


	public bool IsTileCompatible()
	{
		return default(bool);
	}


	public VLChromaticAberration()
	{
	}
}

}
