using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace Snake
{
    class Food
    {
        public int X;
        public int Y;
        public Ellipse Fruit;

        public Food(int x, int y, Ellipse fruit)
        {
            X = x;
            Y = y;
            Fruit = fruit;
        }
    }
}
