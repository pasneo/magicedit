using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class Square
    {
        public SquareType Type { get; set; }
        public Position Position { get; set; }

        public Square() { }

        public Square(SquareType type, Position position)
        {
            Type = type;
            Position = position;
        }

    }
}
