﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class Map
    {

        private int width = 10;
        public int Width
        {
            get { return width; }
            set { width = value; SizeChanged(); }
        }

        private int height = 10;
        public int Height
        {
            get { return height; }
            set { height = value; SizeChanged(); }
        }

        public Object SchemeObject { get; set; }   //an object for tracking states of map when executing actions from map's scheme
        public Scheme Scheme { get; set; } //scheme for map, that contains actions used by Squares (see SquareType)

        public List<SquareType> SquareTypes = new List<SquareType>();
        //private List<Square> Squares = new List<Square>();
        public Dictionary<Position, SquareType> Squares = new Dictionary<Position, SquareType>();

        public List<Position> SpawnerPositions = new List<Position>();

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

        public void RemoveSquareTypeAt(Position position)
        {
            Squares.Remove(position);
        }

        public void SetSquareTypeAt(Position position, SquareType squareType)
        {
            Squares[position] = squareType;
        }

        public void SetOrRemoveSquareTypeAt(Position position, SquareType squareType)
        {
            if (GetSquareTypeAt(position) == squareType) Squares.Remove(position);
            else Squares[position] = squareType;
        }
        
        public SquareType GetSquareTypeAt(Position position)
        {
            if (Squares.ContainsKey(position)) return Squares[position];
            return null;
        }

        // removes squaretype and all its references/occurences
        public void RemoveSquareType(SquareType squareType)
        {
            SquareTypes.Remove(squareType);

            List<Position> keysToRemove = new List<Position>();

            foreach(var square in Squares)
            {
                if (square.Value == squareType)
                {
                    keysToRemove.Add(square.Key);
                }
            }

            keysToRemove.ForEach(pos => Squares.Remove(pos));
        }

        public void AddSpawner(Position position)
        {
            SpawnerPositions.Add(position);
        }

        public int GetSpawnerCount() { return SpawnerPositions.Count; }
        public Position GetSpawnerByNo(int no) { return SpawnerPositions[no]; }

        public bool HasSpawnerAt(Position position)
        {
            foreach(var spawner in SpawnerPositions)
            {
                if (spawner.Equals(position)) return true;
            }
            return false;
        }

        public void RemoveSpawnerAt(Position position)
        {
            SpawnerPositions.RemoveAll(spawner => spawner.Equals(position));
        }

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

        public bool IsPositionPermeable(Position position)
        {
            foreach (MapObject obj in Objects)
            {
                if (obj.Position != null && !obj.IsPermeable && obj.Position.Equals(position)) return false;
            }
            return true;
        }

        public void CreateSchemeObject(Game game)
        {
            SchemeObject = new Object();
            SchemeObject.Scheme = Scheme;
            SchemeObject.Create(game);
        }

        public void ExecuteSquareAction(SquareType squareType, Character actor, Game game)
        {
            if (squareType == null ||Scheme == null || squareType.ActionName == null || squareType.ActionName == "") return;

            SchemeFunction action = Scheme.GetFunctionByName(squareType.ActionName);

            if (action == null) throw new GameException($"No such action in map: {squareType.ActionName}");
            
            action.Execute(SchemeObject, actor, game);
        }

        public int GetMovementCost(SquareType squareType, Config config)
        {
            if (Scheme == null || squareType.ActionName == null || squareType.ActionName == "") return config.CharacterConfig.MovementActionPoints;

            SchemeFunction action = Scheme.GetFunctionByName(squareType.ActionName);

            if (action == null) return config.CharacterConfig.MovementActionPoints;

            return action.ActionPoints;
        }

        public int GetMovementCost(Position position, Config config)
        {
            SquareType squareType = GetSquareTypeAt(position);
            if (squareType != null) return GetMovementCost(squareType, config);
            return config.CharacterConfig.MovementActionPoints;
        }

        private void SizeChanged()
        {
            //check squares
            Squares = Squares.Where(p => IsPositionWithin(p.Key)).ToDictionary(p => p.Key, p => p.Value);
            //check spawners
            SpawnerPositions.RemoveAll(sp => !IsPositionWithin(sp));
        }

        public void RemoveObject(MapObject @object)
        {
            Objects.Remove(@object);
        }

        public void _AddObject(MapObject @object) { Objects.Add(@object); }

    }
}
