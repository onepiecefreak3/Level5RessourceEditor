using System;

namespace Leve5RessourceEditor.Level5.Models
{
    public class PointMapping : ICloneable, IEquatable<PointMapping>
    {
        public float x;
        public float y;
        public float z;
        public float u;
        public float v;

        public object Clone()
        {
            return new PointMapping
            {
                x = x,
                y = y,
                z = z,
                u = u,
                v = v
            };
        }

        public override bool Equals(object obj)
        {
            if (!(obj is PointMapping mapping))
                return base.Equals(obj);

            return Equals(mapping);
        }

        public bool Equals(PointMapping other)
        {
            if (ReferenceEquals(null, other))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z) && u.Equals(other.u) && v.Equals(other.v);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = x.GetHashCode();
                hashCode = (hashCode * 397) ^ y.GetHashCode();
                hashCode = (hashCode * 397) ^ z.GetHashCode();
                hashCode = (hashCode * 397) ^ u.GetHashCode();
                hashCode = (hashCode * 397) ^ v.GetHashCode();
                return hashCode;
            }
        }
    }
}
