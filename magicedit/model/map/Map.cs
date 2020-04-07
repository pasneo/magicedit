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

        private List<Position> SpawnerPositions = new List<Position>();

        //References to the objects on the map (these are not stored here)
        private List<MapObject> Objects;

        /* *** */
        
        public void AddSpawner(Position position)
        {
            SpawnerPositions.Add(position);
        }

    }
}
