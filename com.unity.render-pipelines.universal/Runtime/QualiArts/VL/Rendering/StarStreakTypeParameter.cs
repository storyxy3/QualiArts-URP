using System;
using UnityEngine.Rendering;
namespace VL.Rendering
{
    [Serializable]
    public class StarStreakTypeParameter : VolumeParameter<StarStreakType>
    {
        public StarStreakTypeParameter() : base(StarStreakType.None, false) { }
        protected bool Equals(StarStreakTypeParameter other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return value == other.value;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((StarStreakTypeParameter)obj);
        }
        public override int GetHashCode() => (int)value;
        public static bool operator ==(StarStreakTypeParameter lhs, StarStreakType rhs) => !ReferenceEquals(lhs, null) && lhs.value == rhs;
        public static bool operator !=(StarStreakTypeParameter lhs, StarStreakType rhs) => ReferenceEquals(lhs, null) || lhs.value != rhs;
    }
}
