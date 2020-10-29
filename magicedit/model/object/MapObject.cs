using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class MapObject : Object
    {
        public override string ObjectType => "MapObject";

        public Position Position { get; set; }
        public bool Permeable { get; set; }     //true if characters can step on this object

        /* *** */

        public MapObject() { }
        public MapObject(string id, string name) : base(id, name) { }

        public override Object Copy(Game game)
        {
            MapObject copy = new MapObject(); ;
            CopyTo(copy, game);
            copy.Position = Position;
            copy.Permeable = Permeable;
            return copy;
        }

    }
}
