using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace VL.Rendering
{

[Serializable]
[VolumeComponentMenu("VL/Cloud")]
public class VLCloud : VolumeComponent
{
	public CloudDataParameter cloudData = new CloudDataParameter(null);

	public FloatParameter multiplySpeed = new FloatParameter(1.0f, false);

	public FloatParameter cloudOffsetY = new FloatParameter(0.0f, false);

	public ColorParameter cloudEmissionColor = new ColorParameter(new Color(1.0f, 1.0f, 1.0f, 1.0f), true, true, true, false);

	public VLCloud()
	{
	}
}

}
