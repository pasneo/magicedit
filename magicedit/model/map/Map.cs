using Newtonsoft.Json;
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

        public Scheme Scheme { get; set; } //scheme for map, that contains actions used by Squares (see SquareType)

        public List<SquareType> SquareTypes = new List<SquareType>();
        private List<Square> Squares = new List<Square>();

        private List<Position> SpawnerPositions = new List<Position>();

        //References to the objects on the map (these are not stored here)
        [JsonIgnore]
        public List<MapObject> Objects = new List<MapObject>();

        /* *** */

        public Map() { }

        public Map(int width, int height)
        {
            Width = width;
            Height = height;
        }
        
        public SquareType GetSquareTypeByName(string name)
        {
            return SquareTypes.Where(sqt => sqt.Name == name).FirstOrDefault();
        }

        public void AddSquare(Square square)
        {
            Squares.Add(square);
        }

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

        //Collects every MapObject from the given list, and stores them in Objects list
        public void RecollectMapObjects(List<Object> objects)
        {
            Objects.Clear();
            foreach(Object obj in objects)
            {
                if (obj is MapObject) Objects.Add((MapObject)obj);
            }
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

        public SquareType GetSquareTypeAt(Position position)
        {
            foreach(Square square in Squares)
            {
                if (square.Position.Equals(position)) return square.Type;
            }
            return null;
        }

        public void CallSquareMethod(SquareType squareType, Character actor, Game game)
        {
            if (Scheme == null || squareType.ActionName == null || squareType.ActionName == "") return;

            SchemeFunction action = Scheme.GetFunctionByName(squareType.ActionName);

            if (action == null) throw new GameException($"No such action in map: {squareType.ActionName}");

            action.Execute(new Object(), actor, game);
        }




        public void _AddObject(MapObject @object) { Objects.Add(@object); }

    }
}
