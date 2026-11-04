using System;
using UnityEngine.Rendering;
namespace VL.Rendering
{
    [Serializable]
    [VolumeComponentMenu("VL/Planar Reflection")]
    public class PlanarReflectionVolumeComponent : VolumeComponent
    {
        public BoolParameter enable = new BoolParameter(true);
    }
}
