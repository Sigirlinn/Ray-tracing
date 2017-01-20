using System.Drawing;
using System.Collections.Generic;

namespace Composition.Classes
{
    public class Plane
    {
        public Coordinate[][] VertexArea;
        public List<Coordinate[]> FreePoint = new List<Coordinate[]>();
        public Coordinate[] Vertex;
        public int CountW;
        public int CountH;
        public float PixW;
        public float PixH;
        public Vector3 Normal;
        public int r = 0, g = 53, b = 179;

        public Plane() { }

        public Plane(Coordinate []coor, int xCount, int yCount)
        {
            this.Vertex = new Coordinate[4];
            this.Vertex[0] = coor[0];
            this.Vertex[1] = coor[1];
            this.Vertex[2] = coor[2];
            this.Vertex[3] = coor[3];
            calcNormal();
            Area(xCount, yCount);
            del();
            
        }

        private void calcNormal(){
            Normal = Vector3.CrossProduct(
                new Vector3(Vertex[0], Vertex[1]).GetNormal(),
                new Vector3(Vertex[0], Vertex[3]).GetNormal()
            );
        }

        public void Area(int xCount, int yCount)
        {

            Vector3 right = new Vector3(Vertex[0], Vertex[1]);
            Vector3 top = new Vector3(Vertex[0], Vertex[3]);
            CountH = yCount;
            CountW = xCount;
            PixH = top.D / CountH;
            PixW = right.D / CountW;

            //Random random = new Random();
            VertexArea = new Coordinate[CountH][];
            for (int i = 0; i < CountH; i++)
            {
                VertexArea[i] = new Coordinate[CountW];
                for (int j = 0; j < CountW; j++)
                {
                    Vector3 x, y;
                    x = new Vector3(Vertex[0].X + right.GetNormal().X * (j * PixW),
                        Vertex[0].Y + right.GetNormal().Y * (j * PixW),
                        Vertex[0].Z + right.GetNormal().Z * (j * PixW));
                    y = new Vector3(x.X + top.GetNormal().X * (i * PixH),
                        x.Y + top.GetNormal().Y * (i * PixH),
                        x.Z + top.GetNormal().Z * (i * PixH));
                    VertexArea[i][j] = new Coordinate(y.X, y.Y, y.Z);
                }
            }

        }
        public void SetColor(Color color)
        {
            r = color.R;
            g = color.G;
            b = color.B;
        }


        public void del()
        {
            Coordinate[] rez = new Coordinate[3];
            rez[0] = Vertex[0];
            rez[1] = Vertex[1];
            rez[2] = Vertex[2];
            FreePoint.Add(rez);
            rez = new Coordinate[3];
            rez[0] = Vertex[2];
            rez[1] = Vertex[3];
            rez[2] = Vertex[0];
            FreePoint.Add(rez);
            /*for (int i = 0; i < CountH - 1; i++)
            {
                for (int j = 0; j < CountW - 1; j++)
                {
                    Coordinate[] rez = new Coordinate[3];
                    rez[0] = VertexArea[i][j];
                    rez[1] = VertexArea[i+ 1][j];
                    rez[2] = VertexArea[i][j + 1];
                    FreePoint.Add(rez);
                    rez = new Coordinate[3];
                    rez[0] = VertexArea[i + 1][j + 1];
                    rez[1] = VertexArea[i + 1][j];
                    rez[2] = VertexArea[i][j + 1];
                    FreePoint.Add(rez);
                    
                }
            }*/
            /*Vertex[1].X -= PixW;
            Vertex[2].X -= PixW;
            Vertex[2].Y += PixH;
            Vertex[3].Y += PixH;*/
        }
    }
}
