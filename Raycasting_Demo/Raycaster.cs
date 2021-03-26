using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raycasting_Demo
{
    /// <summary>
    /// Класс предоставляющий средства для создания псевдо 3D
    /// </summary>
    class Raycaster
    {
        // Массив для схематичного представления карты
        int[,] map;

        public Raycaster()
        {
            // Заполнение карты объектами, где 1 - препятствие,
            // 0 - свободное пространство
            map = new int[,]
            {
                {1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                {1, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                {1, 0, 1, 1, 0, 0, 0, 2, 0, 1 },
                {1, 0, 1, 1, 0, 0, 0, 0, 0, 1 },
                {1, 0, 0, 0, 0, 0, 0, 1, 0, 1 },
                {1, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                {1, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                {1, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                {1, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                {1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }
            };
        }
        /// <summary>
        /// Метод создающий кадр псевдо 3D от первого лица
        /// </summary>
        /// <param name="width"> ширина полотна, на котором будет нарисован кадр </param>
        /// <param name="height"> высота полотна, на котором будет нарисован кадр </param>
        /// <returns> готовое полотно с нарисованным кадром </returns>
        public Bitmap NewFrame(int width, int height)
        {
            // объявляем и инициализируем полотно 
            var frame = new Bitmap(width, height);
            // используем графический контекст полотна 
            using(var g = Graphics.FromImage(frame))
            {
                /* закаршиваем полотно полностью чёрным цветом
                 в дальнейшем данный цвет послужит фоном*/
                g.Clear(Color.Black);

                /* записываем в вектор координаты игрока на схематичной карте
                координаты располагаем в пределах индексов двумерного массива карты,
                чтобы не выйти за его пределы */
                MyVector playerPos = new MyVector(7, 5);

                // вектор направления взгляда игрока
                MyVector playerDir = new MyVector(-1, 0.33);

                // вектор видимой области карты (то, что должен видеть игрок)
                MyVector cameraPlane = new MyVector(0.33, 1);

                // пробегаемся по ширине полотна
                for (int i = 0; i < width; i++)
                {
                    // координата Х на плоскости камеры
                    double cameraX = 2 * i / (width * 1.0) - 1;

                    /* направление луча, которым мы будем проверять, 
                    есть ли впереди препятствие */
                    MyVector rayDir = new MyVector(playerDir.X + cameraPlane.X * cameraX,
                       playerDir.Y + cameraPlane.Y * cameraX);
                    
                    // координаты клетки, на которой мы находимся
                    int mapX = (int)playerPos.X;
                    int mapY = (int)playerPos.Y;

                    /* расстояние, которое луч должен пройти до первой стороны Х
                     и первой стороны Y */
                    double sideDistX;
                    double sideDistY;

                    /* расстояние, которое луч должен пройти от первой стороны Х 
                     до следующей стороны Х
                     и от первой стороны Y до следующей стороны Y */
                    double deltaDistX = Math.Abs(1 / rayDir.X);
                    double deltaDistY = Math.Abs(1 / rayDir.Y);

                    // переменная для вычисления длины луча
                    double perpWallDist;

                    /* переменные, принимающие значение 1 или -1
                     для осуществления шага луча по клеткам карты*/
                    int stepX;
                    int stepY;
                    
                    // попали в препятствие или не попали
                    bool hit = false;

                    /* информация попадания по препятствию:
                    если попадание произошло по стороне Х, то примет значение 1,
                    иначе 0*/
                    int side = 0;

                    /* вычисляем направление шага по Х и Y, 
                     а также расстояния до первых стороны Х и стороны Y */
                    if (rayDir.X < 0)
                    {
                        stepX = -1;
                        sideDistX = (playerPos.X - mapX) * deltaDistX;
                    }
                    else
                    {
                        stepX = 1;
                        sideDistX = (mapX + 1.0 - playerPos.X) * deltaDistX;
                    }
                    if (rayDir.Y < 0)
                    {
                        stepY = -1;
                        sideDistY = (playerPos.Y - mapY) * deltaDistY;
                    }
                    else
                    {
                        stepY = 1;
                        sideDistY = (mapY + 1.0 - playerPos.Y) * deltaDistY;
                    }
                    
                    /* цикл пробрасывания луча по определённым клеткам карты
                     до попадания по препятствию */
                    while (!hit)
                    {
                        if (sideDistX < sideDistY)
                        {
                            sideDistX += deltaDistX;
                            mapX += stepX;
                            side = 0;
                        }
                        else
                        {
                            sideDistY += deltaDistY;
                            mapY += stepY;
                            side = 1;
                        }

                        if (map[mapX, mapY] > 0) hit = true;
                    }

                    // вычисление расстояния от стены до плоскости камеры 
                    if (side == 0) perpWallDist = (mapX - playerPos.X + (1 - stepX) / 2)
                            / rayDir.X;
                    else perpWallDist = (mapY - playerPos.Y + (1 - stepY) / 2)
                            / rayDir.Y;

                    /* вычисление высоты рисуемой части стены, 
                    чтобы придать ощущение расстояния */
                    int lineHeight = (int)(height / perpWallDist);

                    /* вычисление координат Y, чтобы рисуемая линия стены 
                    была правильной высоты */
                    int drawStart = -lineHeight / 2 + height / 2;
                    if (drawStart < 0) drawStart = 0;
                    int drawEnd = lineHeight / 2 + height / 2;
                    if (drawEnd >= height) drawEnd = height - 1;

                    // цвет препятствий 
                    Color color = Color.Red;
                    // относительно координат mapX и mapY определяем цвет стенки
                    switch (map[mapX, mapY])
                    {
                        case 1:
                            color = Color.Red;
                            // если попадание по стороне Y, то затемняем
                            if (side == 1) color = Color.FromArgb(color.R / 2, 
                                color.G, color.B);
                            break;
                        case 2:
                            color = Color.Green;
                            if (side == 1) color = Color.FromArgb(color.R, 
                                color.G / 2, color.B);
                            break;
                        default:
                            break;
                    }

                    // рисуем линию стенки
                    g.DrawLine(new Pen(color), i, drawStart, i, drawEnd);
                }
            }
            // возвращаем готовое полотно
            return frame;
        }
    }
}
