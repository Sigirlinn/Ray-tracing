using System;
using System.Runtime.CompilerServices;

namespace Composition.Classes
{
    
    public class MathRender
    {
        private const float pi = (float)Math.PI;
        public MathRender() { }

        // проверка на принадлежность точки треугольнику
        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        public static bool PointInTriangle(Coordinate point, Coordinate a, Coordinate b, Coordinate c)
        {
            const float eps = 0.000001f;

            float v0X = c.X - a.X;
            float v0Y = c.Y - a.Y;
            float v0Z = c.Z - a.Z;

            float v1X = b.X - a.X;
            float v1Y = b.Y - a.Y;
            float v1Z = b.Z - a.Z;

            float v2X = point.X - a.X;
            float v2Y = point.Y - a.Y;
            float v2Z = point.Z - a.Z;

            float dot00 = v0X * v0X + v0Y * v0Y + v0Z * v0Z;
            float dot01 = v0X * v1X + v0Y * v1Y + v0Z * v1Z;
            float dot02 = v0X * v2X + v0Y * v2Y + v0Z * v2Z;
            float dot11 = v1X * v1X + v1Y * v1Y + v1Z * v1Z;
            float dot12 = v1X * v2X + v1Y * v2Y + v1Z * v2Z;

            float invDenom = 1.0f / (dot00 * dot11 - dot01 * dot01);
            float u = (dot11 * dot02 - dot01 * dot12) * invDenom;
            if (u + eps < 0) return false;
            float v = (dot00 * dot12 - dot01 * dot02) * invDenom;
            return ((v + eps >= 0) && (u + v - eps < 1));
        }

        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        public static bool PointInTriangle(float px, float py, float pz, Coordinate a, Coordinate b, Coordinate c)
        {
            const float eps = 0.000001f;

            float v0X = c.X - a.X;
            float v0Y = c.Y - a.Y;
            float v0Z = c.Z - a.Z;

            float v1X = b.X - a.X;
            float v1Y = b.Y - a.Y;
            float v1Z = b.Z - a.Z;

            float v2X = px - a.X;
            float v2Y = py - a.Y;
            float v2Z = pz - a.Z;

            float dot00 = v0X * v0X + v0Y * v0Y + v0Z * v0Z;
            float dot01 = v0X * v1X + v0Y * v1Y + v0Z * v1Z;
            float dot02 = v0X * v2X + v0Y * v2Y + v0Z * v2Z;
            float dot11 = v1X * v1X + v1Y * v1Y + v1Z * v1Z;
            float dot12 = v1X * v2X + v1Y * v2Y + v1Z * v2Z;

            float invDenom = 1.0f / (dot00 * dot11 - dot01 * dot01);
            float u = (dot11 * dot02 - dot01 * dot12) * invDenom;
            if (u + eps < 0) return false;
            float v = (dot00 * dot12 - dot01 * dot02) * invDenom;
            return ((v + eps >= 0) && (u + v - eps < 1));
        }
        
        //метод возвращает косинус между векторами (наблюдения и лучом света)
        private static float GetCos(ref Vector3 a, ref Vector3 b)
        {
            return (a.X * b.X + a.Y * b.Y + a.Z * b.Z) / (a.D * b.D);
        }
        
        //метод возвращвет яркость от заданного источника света в заданной точке
        public static float GetLightPoint(Coordinate point, ref Lamp light, ref Vector3 normal)
        {
            Vector3 planeLamp = new Vector3(point, light.XYZ);
            float cos = GetCos(ref normal, ref planeLamp);
            //поток источника, сила
            float f = light.ThreadLight;					        
            //нормируем, так как источник точечный и светит во все стороны
            float i = f / (4 * pi);	                                
            //считаем значение освещенности
            float e = i * cos / (planeLamp.D * planeLamp.D);
            return e / pi;
        }
        
        public static float GetLightPoint(float px, float py, float pz, ref Lamp light, ref Vector3 normal, int a)
        {
            Vector3 planeLamp = new Vector3(px, py, pz, light.XYZ);
            float cos = GetCos(ref normal, ref planeLamp);
            //поток источника, сила
            float f = light.ThreadLight;
            //нормируем, так как источник точечный и светит во все стороны
            float i = f / (4 * pi);
            //считаем значение освещенности
            float e = i * cos / (planeLamp.D * planeLamp.D);
            return e / pi;
        }
        
        public static float GetLightPoint(float px, float py, float pz, float threadLight, Coordinate lightXYZ, ref Vector3 normal)
        {
            Vector3 planeLamp = new Vector3(px, py, pz, lightXYZ);
            float cos = GetCos(ref normal, ref planeLamp);
            //поток источника, сила
            //нормируем, так как источник точечный и светит во все стороны
            float i = threadLight / (4 * pi);
            //считаем значение освещенности
            float e = i * cos / (planeLamp.D * planeLamp.D);
            return e / pi;
        }

        public static float getLightInPoint(Coordinate point, ref Triangle[] trian, ref Lamp light, int id)
        {
            //луч из светильника в точку яркость которой считаем
            Vector3 fromLampInPixel = new Vector3(light.XYZ, point);

            float xP, yP, zP;
            float objDistance = 0;
            //это не длина вектора, это длина вектора, которая требуется, чтобы пересечь плоскость (не треугольник)
            //но его содержащая

            for (int k = 0; k < trian.Length; k++)
            {
                if (k != id)
                {
                    //смотрим пересекает ли вектор другие плоскости
                    objDistance = intersection(ref fromLampInPixel,  light.XYZ, ref trian[k].Normal,  trian[k].A);

                    //проверяем на пересечение, на то чтобы это не была таже самая плоскость, 
                    //и на то чтобы k-я плоскость была к нам ближе чем изначальная 
                    if (objDistance > 0)
                    {
                        //считаем точку пересечения луча и к-й плоскости
                        xP = light.XYZ.X + fromLampInPixel.X * objDistance;
                        yP = light.XYZ.Y + fromLampInPixel.Y * objDistance;
                        zP = light.XYZ.Z + fromLampInPixel.Z * objDistance;
                        //проверяем, находится ли точка в треугольнике, из которых состоит сцена, или
                        //она где-то в пространстве, и тогда нам она не нужна
                        if (PointInTriangle(xP, yP, zP,  trian[k].A, trian[k].B, trian[k].C))
                        {
                            //если точка находится в треугольнике, значит луч пересекает ещё какой-то
                            //треугольник из сцены кроме изначального
                            Vector3 fromLampInKPixel = new Vector3(light.XYZ, xP, yP, zP);
                            //смотрим чтобы длина изначального луча была больше полученного,
                            //это значит что k-й треугольник находится ближе к источнику света
                            float dot = Vector3.DotProduct(fromLampInKPixel, fromLampInPixel);
                            if (fromLampInPixel.D > fromLampInKPixel.D && dot > 0)
                            {
                                return 0;
                            }
                        }
                    }
                }
            }
            //ничего не было найденно, значит просто считаем яркость
            float kf = GetLightPoint(point, ref light, ref trian[id].Normal);
            return kf;

        }

        public static float getLightInPoint(float px, float py, float pz, ref Triangle[] trian, ref Lamp light, int id)
        {
            //луч из светильника в точку яркость которой считаем
            Vector3 fromLampInPixel = new Vector3(light.XYZ, px, py, pz);

            float xP, yP, zP;
            float objDistance = 0;
            //это не длина вектора, это длина вектора, которая требуется, чтобы пересечь плоскость (не треугольник)
            //но его содержащая

            for (int k = 0; k < trian.Length; k++)
            {
                if (k != id)
                {
                    //смотрим пересекает ли вектор другие плоскости
                    objDistance = intersection(ref fromLampInPixel, light.XYZ, ref trian[k].Normal, trian[k].A);

                    //проверяем на пересечение, на то чтобы это не была таже самая плоскость, 
                    //и на то чтобы k-я плоскость была к нам ближе чем изначальная 
                    if (objDistance > 0)
                    {
                        //считаем точку пересечения луча и к-й плоскости
                        xP = light.XYZ.X + fromLampInPixel.X * objDistance;
                        yP = light.XYZ.Y + fromLampInPixel.Y * objDistance;
                        zP = light.XYZ.Z + fromLampInPixel.Z * objDistance;
                        //проверяем, находится ли точка в треугольнике, из которых состоит сцена, или
                        //она где-то в пространстве, и тогда нам она не нужна
                        if (PointInTriangle(xP, yP, zP, trian[k].A, trian[k].B, trian[k].C))
                        {
                            //если точка находится в треугольнике, значит луч пересекает ещё какой-то
                            //треугольник из сцены кроме изначального
                            Vector3 fromLampInKPixel = new Vector3(light.XYZ, xP, yP, zP);
                            //смотрим чтобы длина изначального луча была больше полученного,
                            //это значит что k-й треугольник находится ближе к источнику света
                            float dot = Vector3.DotProduct(fromLampInKPixel, fromLampInPixel);
                            if (fromLampInPixel.D > fromLampInKPixel.D && dot > 0)
                            {
                                return 0;
                            }
                        }
                    }
                }
            }
            //ничего не было найденно, значит просто считаем яркость
            float kf = GetLightPoint(px, py, pz, light.ThreadLight, light.XYZ,  ref trian[id].Normal);
            return kf;

        }

        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        public static float intersection(ref Vector3 ray, Coordinate fromRay, ref Vector3 normal, Coordinate anyPoint)
        {
            float rez = -(normal.X * (fromRay.X - anyPoint.X) +
                     normal.Y * (fromRay.Y - anyPoint.Y) +
                     normal.Z * (fromRay.Z - anyPoint.Z)) /
                    (normal.X * ray.X +
                     normal.Y * ray.Y +
                     normal.Z * ray.Z);
            return rez;
        }

        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        public static float intersection(ref Vector3 ray, float fromRayX, float fromRayY, float fromRayZ, ref Vector3 normal, Coordinate anyPoint)
        {
            float rez = -(normal.X * (fromRayX - anyPoint.X) +
                     normal.Y * (fromRayY - anyPoint.Y) +
                     normal.Z * (fromRayZ - anyPoint.Z)) /
                    (normal.X * ray.X +
                     normal.Y * ray.Y +
                     normal.Z * ray.Z);
            return rez;
        }
    }
}
