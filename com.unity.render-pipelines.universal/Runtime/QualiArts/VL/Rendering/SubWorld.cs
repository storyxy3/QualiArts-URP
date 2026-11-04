using System;
using UnityEngine.Rendering;
namespace VL.Rendering
{
    [Serializable]
    public class SubWorld : VolumeComponent
    {
        public SubWorldVolumeParameter target = new SubWorldVolumeParameter(null);
    }
}
