namespace Composition.Classes
{
    public class Lamp
    {
        public Coordinate XYZ;
        public float ThreadLight;
        public Lamp() { }
        public Lamp(float x, float y, float z, float threadLight)
        {
            XYZ.X = x;
            XYZ.Y = y;
            XYZ.Z = z;
            ThreadLight = threadLight;
        }
    }
}
