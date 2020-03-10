using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class Map
    {

        public int Width { get; set; }
        public int Height { get; set; }

        private List<Square> Squares;

        private List<MapObject> Objects;

    }
}
