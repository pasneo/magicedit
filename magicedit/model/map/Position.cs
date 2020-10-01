using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{

    public class PositionJsonConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(Position);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string str = (string)value;
            return new Position(str);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string)) return ((Position)value).ToString();
            return null;
        }
    }


    [TypeConverter(typeof(PositionJsonConverter))]
    public class Position
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        //converts only from strings of (<x>,<y>) format
        public Position(string str)
        {
            var xy = str.Substring(1, str.Length - 2).Split(',');
            X = int.Parse(xy[0]);
            Y = int.Parse(xy[1]);
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

        public override string ToString()
        {
            return $"({X},{Y})";
        }

    }
}
