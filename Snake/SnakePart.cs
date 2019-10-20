using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace Snake
{
    class SnakePart
    {
        public Rectangle Rect { get; set; }
        public Direction Direct { get; set; }

        public SnakePart(Rectangle rect, Direction direct)
        {
            Rect = rect;
            Direct = direct;
        }

    }
}
