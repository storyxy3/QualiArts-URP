using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace VL.Rendering
{

[Serializable]
[VolumeComponentMenu("VL/Global LightProbe")]
public class VLGlobalLightProbe : VolumeComponent
{
	[Tooltip("LightingPassが実装されていないLitピクセルのLightProbeポイント")]
	public Vector3Parameter pivot = new Vector3Parameter(Vector3.zero, false);

	public VLGlobalLightProbe()
	{
	}
}

}
