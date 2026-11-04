using System;
using UnityEngine;
using UnityEngine.Rendering;
namespace VL.Rendering
{
    [Serializable]
    public class FlareParameter : VolumeParameter<FlareSetting>
    {
        public FlareParameter() : base(default, false) { }
        public override void Interp(FlareSetting from, FlareSetting to, float t)
        {
            m_Value.type = t > 0f ? to.type : from.type;
            m_Value.fixToBaseAspect = t > 0f ? to.fixToBaseAspect : from.fixToBaseAspect;
            m_Value.center = Vector2.Lerp(from.center, to.center, t);
            m_Value.size = Vector2.Lerp(from.size, to.size, t);
            m_Value.color0 = Color.Lerp(from.color0, to.color0, t);
            m_Value.color1 = Color.Lerp(from.color1, to.color1, t);
        }
    }
}
