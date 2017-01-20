namespace Composition.Classes
{
    public class Camera
    {
        //вектор камеры, направление взгляда
        public Vector3 Eye;
        //вектор вверх
        public Vector3 Up;
        //ширина экрана
        public int Width;
        //высота экрана
        public int Height;
        //позиция камеры
        public float EyeX, EyeY, EyeZ;
        //точка куда смотрит камера
        public float EndX, EndY, EndZ;
        //расстойние между позицией камеры и экраном
        public float Near;

        /// <summary>
        /// Задаем камеру
        /// </summary>
        /// <param name="eyeX">координата глаза по оси ОХ</param>
        /// <param name="eyeY">координата глаза по оси ОY</param>
        /// <param name="eyeZ">координата глаза по оси ОZ</param>
        /// <param name="endX">координата точки, в которую направленна камера по оси ОХ</param>
        /// <param name="endY">координата точки, в которую направленна камера по оси ОY</param>
        /// <param name="endZ">координата точки, в которую направленна камера по оси ОZ</param>
        /// <param name="width">ширина viewportа</param>
        /// <param name="height">высота viewportа</param>
        /// <param name="near">расстояние от глаза до середины viewportа</param>
        public Camera(float eyeX, float eyeY, float eyeZ, float endX, float endY, float endZ, int width, int height, float near)
        {
            Eye = new Vector3(endX - eyeX, endY - eyeY, endZ - eyeZ);

            EyeX = eyeX;
            EyeY = eyeY;
            EyeZ = eyeZ;

            EndX = endX;
            EndY = endY;
            EndZ = endZ;

            Up = new Vector3(0, 1, 0);

            Width = width;
            Height = height;
            Near = near;
        }
    }
}
