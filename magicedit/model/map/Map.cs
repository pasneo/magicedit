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
        private List<MapObject> Objects = new List<MapObject>();

        /* *** */
        
        public void AddSpawner(Position position)
        {
            SpawnerPositions.Add(position);
        }

        public int GetSpawnerCount() { return SpawnerPositions.Count; }
        public Position GetSpawnerByNo(int no) { return SpawnerPositions[no]; }

        public bool IsPositionWithin(Position position)
        {
            return (position.X >= 0) && (position.X < Width) &&
                   (position.Y >= 0) && (position.Y < Height);
        }

        //If the given object is on this map, it is returned as MapObject (else null is returned)
        public MapObject GetMapObject(Object @object)
        {
            foreach(MapObject obj in Objects)
            {
                if (obj == @object) return obj;
            }
            return null;
        }

        public void _AddObject(MapObject @object) { Objects.Add(@object); }

    }
}
