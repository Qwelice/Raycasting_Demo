using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raycasting_Demo
{
    /// <summary>
    /// Класс для хранения координат X, Y
    /// </summary>
    class MyVector
    {
        public double X { get; set; }
        public double Y { get; set; }

        public MyVector()
        {
            X = 0.0;
            Y = 0.0;
        }

        public MyVector(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}
