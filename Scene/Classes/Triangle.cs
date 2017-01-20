using System.Drawing;

namespace Composition.Classes
{
    
    public class Triangle
    {
        //вершины треугольника
        public Coordinate A, B, C;
        //цвет трекгольника
        public int r, g, b;
        //нормаль треугольника
        public Vector3 Normal;

        public Triangle(Coordinate a, Coordinate b, Coordinate c)
        {
            A = a;
            B = b;
            C = c;
            calcNormal();
        }

        public void SetColor(int r, int g, int b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }

        public Color GetColor()
        {
            return Color.FromArgb(r, g, b);
        }

        private void calcNormal()
        {
            Normal = Vector3.CrossProduct(
                new Vector3(B, A).GetNormal(),
                new Vector3(C, A).GetNormal()
            );
        }
    }
}
