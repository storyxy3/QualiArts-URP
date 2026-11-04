using System;
using UnityEngine.Rendering;

namespace VL.Rendering
{

[Serializable]
public sealed class TonemappingModeParameter : VolumeParameter<VLTonemappingMode>
{
	public TonemappingModeParameter(VLTonemappingMode value, bool overrideState = false) : base(value, overrideState)
	{
	}
}

}
