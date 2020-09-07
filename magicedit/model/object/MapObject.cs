using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class MapObject : Object
    {
        public Position Position { get; set; }
        public bool Permeable { get; set; }     //true if characters can step on this object

        public override Object Copy()
        {
            MapObject copy = new MapObject(); ;
            CopyTo(copy);
            copy.Position = Position;
            copy.Permeable = Permeable;
            return copy;
        }

    }
}
