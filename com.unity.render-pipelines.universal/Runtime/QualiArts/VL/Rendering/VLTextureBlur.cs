using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace VL.Rendering
{

[Serializable]
[VolumeComponentMenu("VL/VL Texture Blur")]
public class VLTextureBlur : VolumeComponent
{
	public TextureParameter texture = new TextureParameter(null, false);

	public ClampedFloatParameter intensity = new ClampedFloatParameter(0.0f, 0.0f, 0.1f, false);

	public bool IsActive()
	{
		return texture.value != null && intensity.value > 0f;
	}

	public VLTextureBlur()
	{
	}
}

}
