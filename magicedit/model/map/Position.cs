using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class Position
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Position operator +(Position p1, Position p2) { return new Position(p1.X + p2.X, p1.Y + p2.Y); }
        public static Position operator -(Position p1, Position p2) { return new Position(p1.X - p2.X, p1.Y - p2.Y); }

        //Manhattan distance
        public int GetDistance(Position position)
        {
            Position diff = this - position;
            return Math.Abs(diff.X) + Math.Abs(diff.Y);
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || (!this.GetType().Equals(obj.GetType()))) return false;
            Position position = (Position)obj;
            return (X == position.X) && (Y == position.Y);
        }

        public override int GetHashCode()
        {
            return (X << 2) ^ Y;
        }

    }
}
