using System;
using UnityEngine.Rendering;

namespace VL.Rendering
{

[Serializable]
public class CloudDataParameter : VolumeParameter<CloudData>
{
	public CloudDataParameter(CloudData value) : base(value, false)
	{
	}
}

}
