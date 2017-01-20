using System;
using System.Runtime.CompilerServices;

namespace Composition.Classes
{
    public class Vector3
    {
        //координаты вектора
        public float X, Y, Z;
        //длина вектора
        public float D;
        public Vector3(float inX, float inY, float inZ)
        {
            X = inX;
            Y = inY;
            Z = inZ;
            D = 0;
            D = setDistance(X, Y, Z);
        }
        public Vector3(Coordinate start, Coordinate end)
        {
            X = end.X - start.X;
            Y = end.Y - start.Y;
            Z = end.Z - start.Z;
            D = 0;
            D = setDistance(X, Y, Z);
        }

        public Vector3(Coordinate start, float endX, float endY, float endZ)
        {
            X = endX - start.X;
            Y = endY - start.Y;
            Z = endZ - start.Z;
            D = 0;
            D = setDistance(X, Y, Z);
        }

        public Vector3(float startX, float startY, float startZ, Coordinate end)
        {
            X = end.X - startX;
            Y = end.Y - startY;
            Z = end.Z - startZ;
            D = 0;
            D = setDistance(X, Y, Z);
        }

        public Vector3(float startX, float startY, float startZ, float endX, float endY, float endZ)
        {
            X = endX - startX;
            Y = endY - startY;
            Z = endZ - startZ;
            D = 0;
            D = setDistance(X, Y, Z);
        }

        private float setDistance(float x, float y, float z)
        {
            return (float)Math.Sqrt(x * x + y * y + z * z);
        }

        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        public Vector3 GetNormal()
        {
            return new Vector3(X / D, Y / D, Z / D);
        }

        //скалярное произведение
        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        public static float DotProduct(Vector3 a, Vector3 b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

        //векторное произведение
        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        public static Vector3 CrossProduct(Vector3 a, Vector3 b)
        {
            return new Vector3(
                a.Y * b.Z - a.Z * b.Y,
                a.Z * b.X - b.Z * a.X,
                a.X * b.Y - b.X * a.Y
                );
        }

        //разность между векторами
        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Subtract(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }


        public static float MixedProduct(Vector3 a, Vector3 b, Vector3 c)
        {
            return DotProduct(CrossProduct(a,b),c);
        }
    }
}
