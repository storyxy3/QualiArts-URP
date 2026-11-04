using System;
using UnityEngine.Rendering;
namespace VL.Rendering
{
    [Serializable]
    public class SubWorldVolumeParameter : VolumeParameter<SubWorldVolume>
    {
        public SubWorldVolumeParameter(SubWorldVolume value) : base(value, false) { }
    }
}
