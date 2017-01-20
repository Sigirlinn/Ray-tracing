using System;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace Composition.Classes
{
    public class Scene
    {
        public float CountProc;
        public event EventHandler Tik;
        public int CountLight;
        public float Power;
        //камера
        private Camera camera;
        //треугольники
        private Triangle[] triangle;
        //источники света
        private Lamp[] light;
        private Lamp[] oldLight;
        //константы для парсера
        private const string LIGHT_LABEL = "light";
        private const string TRIANGLE_LABEL = "triangle";
        private const string CAMERA_LABEL = "camera";
        private const string COMMENT_LABEL = "#";
        //флажок на стекло
        private bool flagDirectLight = false;
        private string fileName;
        private int width;
        private int height;
        private float delProc;

        public Scene(int width, int height, string path)
        {
            this.width = width;
            this.height = height;
            fileName = Path.GetFileNameWithoutExtension(path);
            loadObjectScene(path);
            CountLight = 100;
            Power = 1500;
        }

        public string GetFileName()
        {
            return fileName;
        }

        public void SetDirectLight(bool value)
        {
            flagDirectLight = value;
        }

        public void SetCamera(float eyeX, float eyeY, float eyeZ, float beginX, float beginY, float beginZ, int width, int height, float near)
        {
            camera = new Camera(eyeX, eyeY, eyeZ, beginX, beginY, beginZ, width, height, near);
        }

        public Camera GetCamera()
        {
            return camera;
        }
        
        private void loadObjectScene(string path)
        {
            if (path == "") return;
            //открываем файл
            StreamReader streamReader = new StreamReader(path);
            //считываем содержимое
            string fileContent = streamReader.ReadToEnd();
            //закрываем
            streamReader.Close();
            //разбиваем полученные данные на строки по знаку переноса и отбрасываем комментарии
            string[] lines = fileContent.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                .Where(x => !x.StartsWith(COMMENT_LABEL)).ToArray();
            int k = 0;
            string newLine = "";
            //просматриваем получившийся массив строк
            while (k < lines.Length)
            {
                newLine = lines[k];
                k++;
                if (newLine.StartsWith(LIGHT_LABEL))
                {
                    k = readAndCreateLamp(newLine, lines, k);
                }
                else if (newLine.StartsWith(TRIANGLE_LABEL))
                {
                    k = readAndCreateTriangle(newLine, lines, k);
                }
                else if (newLine.StartsWith(CAMERA_LABEL))
                {
                    k = readAndCreateCamera(newLine, lines, k);
                }

            }
        }

        private int readAndCreateLamp(string newLine, string []lines, int k) {
            string[] splitLine = newLine.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            //количество источников света
            int countLight = Convert.ToInt32(splitLine[1]);
            light = new Lamp[countLight];
            oldLight = new Lamp[countLight];
            for (int i = 0; i < countLight; i++)
            {
                if (k < lines.Length)
                {
                    newLine = lines[k];
                    k++;
                    splitLine = newLine.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    //координаты точечных источников
                    float x = Convert.ToSingle(splitLine[0]);
                    float y = Convert.ToSingle(splitLine[1]);
                    float z = Convert.ToSingle(splitLine[2]);
                    //поток света
                    float thread = Convert.ToSingle(splitLine[3]);
                    light[i] = new Lamp(x, y, z, thread);
                    oldLight[i] = new Lamp(x, y, z, thread);
                }
            }
            return k;
        }

        private int readAndCreateTriangle(string newLine, string []lines, int k)
        {
            string[] splitLine = newLine.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            //количество треугольников
            int counTriangle = Convert.ToInt32(splitLine[1]);
            triangle = new Triangle[counTriangle];

            Coordinate[] coor = new Coordinate[3];
            for (int i = 0; i < counTriangle; i++)
            {
                newLine = lines[k];
                k++;
                splitLine = newLine.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                //цвет треугольника 
                int r = Convert.ToInt32(splitLine[0]);
                int g = Convert.ToInt32(splitLine[1]);
                int b = Convert.ToInt32(splitLine[2]);
                if (k < lines.Length)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        newLine = lines[k];
                        k++;
                        splitLine = newLine.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        //координаты вершины треугольника
                        float x = Convert.ToSingle(splitLine[0]);
                        float y = Convert.ToSingle(splitLine[1]);
                        float z = Convert.ToSingle(splitLine[2]);
                        coor[j] = new Coordinate(x, y, z);
                    }
                }
                triangle[i] = new Triangle(coor[0], coor[1], coor[2]);
                triangle[i].SetColor(r, g, b);
            }
            return k;

        }

        private int readAndCreateCamera(string newLine, string []lines, int k)
        {
            newLine = lines[k];
            k++;
            string[] splitLine = newLine.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            //координаты камеры
            float eyeX = Convert.ToSingle(splitLine[0]);
            float eyeY = Convert.ToSingle(splitLine[1]);
            float eyeZ = Convert.ToSingle(splitLine[2]);

            newLine = lines[k];
            k++;
            splitLine = newLine.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            //куда направлен взгляд
            float endX = Convert.ToSingle(splitLine[0]);
            float endY = Convert.ToSingle(splitLine[1]);
            float endZ = Convert.ToSingle(splitLine[2]);

            newLine = lines[k];
            k++;
            splitLine = newLine.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            //расстояние между камерой и экраном
            float near = Convert.ToSingle(splitLine[0]);

            camera = new Camera(eyeX, eyeY, eyeZ, endX, endY, endZ, width, height, near);
            return k;
        }

        public Bitmap Render()
        {
            //создаем bitmap
            Bitmap bmp = new Bitmap(camera.Width, camera.Height, PixelFormat.Format24bppRgb);

            // Lock the bitmap's bits.  
            Rectangle rect = new Rectangle(0, 0, camera.Width, camera.Height);
            BitmapData bmpData =
                bmp.LockBits(rect, ImageLockMode.ReadWrite,
                bmp.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int bytes = bmpData.Stride * (bmp.Height); 
            byte[] rgbValues = getImageBit(bytes, bmpData.Stride);

            // Copy the RGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);

            // Unlock the bits.
            bmp.UnlockBits(bmpData);
            return bmp;
        }

        private byte[] getImageBit(int size, int stribe)
        {
            //массив байтов под картинку рендера
            CountProc = 0;
            byte[] rgbValues = new byte[size];
            int xcount = 4, ycount = 2;
            int idW = camera.Width / xcount;
            int idH = camera.Height / ycount;
            int threadCount = 8;
            int xmax, xmin, ymax, ymin;
            delProc = 100.0f / (threadCount * idH);
            Parallel.For(0, threadCount, i =>
            {
                /*int y = i / ycount;
                int x = i % xcount;
                xmin = x * idW;
                xmax = xmin + idW;
                ymin = y * idH;
                ymax = ymin + idH;
                getSceneBrightnessParal(xmin, xmax, ymin, ymax, ref rgbValues, stribe);*/
                switch (i) {
                    case 0:
                        {
                            getSceneBrightnessParal(0, idW, 0, idH, ref rgbValues, stribe);
                            break;
                        }
                    case 1:
                        {
                            getSceneBrightnessParal(idW,  idW * 2, 0, idH, ref rgbValues, stribe);
                            break;
                        }
                    case 2:
                        {
                            getSceneBrightnessParal(idW * 2, idW * 3, 0, idH, ref rgbValues, stribe);
                            break;
                        }
                    case 3:
                        {
                            getSceneBrightnessParal(idW * 3, idW * 4, 0, idH, ref rgbValues, stribe);
                            break;
                        }
                    case 4:
                        {
                            getSceneBrightnessParal(0, idW, idH, idH *2, ref rgbValues, stribe);
                            break;
                        }
                    case 5:
                        {
                            getSceneBrightnessParal(idW, idW * 2, idH, idH * 2, ref rgbValues, stribe);
                            break;
                        }
                    case 6:
                        {
                            getSceneBrightnessParal(idW * 2, idW * 3, idH, idH * 2, ref rgbValues, stribe);
                            break;
                        }
                    case 7:
                        {
                            getSceneBrightnessParal(idW * 3, idW * 4, idH, idH * 2, ref rgbValues, stribe);
                            break;
                        }

                }

            });
            return rgbValues;
        }

        private void getSceneBrightnessParal(int xmin, int xmax, int ymin, int ymax, ref byte[] rgbValues, int stribe)
        {

            float ratio = 1.0f * camera.Width / camera.Height;
            float dx = camera.Width / 2;
            float dy = camera.Height / 2 * ratio;

            //считаем 3д координаты центра камеры
            Coordinate center;
            center.X = camera.EyeX + (camera.Eye.X / camera.Eye.D * camera.Near);
            center.Y = camera.EyeY + (camera.Eye.Y / camera.Eye.D * camera.Near);
            center.Z = camera.EyeZ + (camera.Eye.Z / camera.Eye.D * camera.Near);

            //считаем вектора для откладывания точек по ширине и высоте экрана
            Vector3 right = Vector3.CrossProduct(camera.Up, camera.Eye).GetNormal();
            Vector3 top = Vector3.CrossProduct(camera.Eye, right).GetNormal();


            Color pixColor = Color.Black;
            float kf;
            Vector3 vx, vy;
            float pixX, pixY, pixZ;
            float log, R, G, B;
            for (int i = ymin; i < ymax; i++)
            {
                for (int j = xmin; j < xmax; j++)
                {
                    vx = new Vector3(
                       right.X * (-camera.Width / 2 + j),
                       right.Y * (-camera.Width / 2 + j),
                       right.Z * (-camera.Width / 2 + j));
                    vy = new Vector3(
                        top.X * (camera.Height / 2 - i),
                        top.Y * (camera.Height / 2 - i),
                        top.Z * (camera.Height / 2 - i));
                    //считаем координаты пикселя
                    pixX = (vx.X) / dx + (vy.X) / dy + center.X;
                    pixY = (vx.Y) / dx + (vy.Y) / dy + center.Y;
                    pixZ = vx.Z / dx + vy.Z / dy + center.Z;

                    kf = 0;
                    Vector3 fromEyeInPixel = new Vector3(
                        pixX - camera.EyeX,
                        pixY - camera.EyeY,
                        pixZ - camera.EyeZ);
                    float xP, yP, zP;
                    float[] objDistance = new float[triangle.Length];
                    int idPlane = -1;

                    for (int k = 0; k < triangle.Length; k++)
                    {
                        objDistance[k] = MathRender.intersection(ref fromEyeInPixel, camera.EyeX, camera.EyeY, camera.EyeZ, ref triangle[k].Normal, triangle[k].A);

                        if (objDistance[k] > 0)
                        {
                            xP = camera.EyeX + fromEyeInPixel.X * objDistance[k];
                            yP = camera.EyeY + fromEyeInPixel.Y * objDistance[k];
                            zP = camera.EyeZ + fromEyeInPixel.Z * objDistance[k];
                            if (MathRender.PointInTriangle(xP, yP, zP, triangle[k].A, triangle[k].B, triangle[k].C))
                            {

                                if ((idPlane == -1) || (objDistance[k] < objDistance[idPlane]))
                                {
                                    idPlane = k;
                                }
                            }
                        }
                    }


                    if ((idPlane != -1) && (objDistance[idPlane] > 0))
                    {
                        xP = camera.EyeX + fromEyeInPixel.X * objDistance[idPlane];
                        yP = camera.EyeY + fromEyeInPixel.Y * objDistance[idPlane];
                        zP = camera.EyeZ + fromEyeInPixel.Z * objDistance[idPlane];
                        if (flagDirectLight)
                        {
                            light = CreateLight(light, CountLight, Power);
                        }
                        else
                        {
                            light = oldLight;
                        }
                        float LdotN, LdotE;
                        for (int g = 0; g < light.Length; g++)
                        {
                            //проверка на то чтобы светильник и глаз находились по одну сторону треугольника 

                            LdotN = Vector3.DotProduct(
                                //вектор нормали поверхноти
                                triangle[idPlane].Normal,
                                //вектор от точки на поверхности до источника света
                                new Vector3(xP, yP, zP, light[g].XYZ));
                            LdotE = Vector3.DotProduct(
                                //вектор от пикселя камеры до точки на поверхности
                                new Vector3(xP, yP, zP, pixX, pixY, pixZ),
                                //вектор от точки на поверхности до источника света
                                triangle[idPlane].Normal);
                            if (LdotN >= 0 && LdotE >= 0)
                            {
                                float rez = MathRender.getLightInPoint(xP, yP, zP, ref triangle, ref light[g], idPlane);
                                kf += rez;
                            }

                        }
                        pixColor = triangle[idPlane].GetColor();
                    }
                    log = kf * 11000;
                    R = pixColor.R / 255f * log;
                    G = pixColor.G / 255f * log;
                    B = pixColor.B / 255f * log;
                    int id = i * stribe + j * 3;
                    if (B > 255) B = 255;
                    rgbValues[id] = (byte)B;
                    if (G > 255) G = 255;
                    rgbValues[id + 1] = (byte)G;
                    if (R > 255) R = 255;
                    rgbValues[id + 2] = (byte)R;
                }
                CountProc += delProc;
                Tik((object)CountProc, new EventArgs());
            }
        }

        private Lamp[] CreateLight(Lamp []light, int count, float power)
        {
            
            int xmax = 5, xmin = -5, zmin = 95, zmax = 105, x, z;
            //int xmax = 10, xmin = -10, zmin = 90, zmax = 110, x, z;
            float ymax = 99.5f;
            float threadLight = power / count;
            Random r = new Random();
            light = new Lamp[count];
            for(int i = 0; i < 10; i++)
            {
                for(int j = 0; j < 10; j++)
                {
                    light[i * 10 + j] = new Lamp();
                    light[i * 10 + j].ThreadLight = threadLight;
                    light[i * 10 + j].XYZ = new Coordinate(i + xmin, ymax, j + zmin);
                }
            }
            /*for(int i = 0; i < count; i++)
            {

                
                x = r.Next(xmin * 100, xmax * 100) / 100;
                z = r.Next(zmin * 100, zmax * 100) / 100;
            }
            */
            return light;
        }
    }
}

