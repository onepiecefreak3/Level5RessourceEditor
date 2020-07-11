namespace Leve5RessourceEditor.Level5.Models
{
    public class PointMapping
    {
        public float x;
        public float y;
        public float z;
        public float u;
        public float v;

        public override bool Equals(object obj)
        {
            if (!(obj is PointMapping mapping))
                return base.Equals(obj);

            return x == mapping.x &&
                y == mapping.y &&
                z == mapping.z &&
                u == mapping.u &&
                v == mapping.v;
        }
    }
}
