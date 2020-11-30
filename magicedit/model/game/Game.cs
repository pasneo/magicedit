using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class Game
    {

        public delegate void OnReportDelegate(Text report);
        public event OnReportDelegate OnReport;

        public delegate void OnNextPlayerDelegate();
        public event OnNextPlayerDelegate OnNextPlayer;

        //Actions that characters can do without a target object
        public class BasicActions
        {
            public static string Movement = "_movement";
            public static string EndTurn = "_end turn";
        }

        public class MovementParameters
        {
            public static string Norht = "_north";
            public static string East = "_east";
            public static string South = "_south";
            public static string West = "_west";
        }


        public Config Config { get; private set; }

        //A copy of the map in Config
        private Map Map;
        
        private List<Player> Players;

        //A list of all the objects currently existing in the game
        private List<Object> Objects = new List<Object>();

        public int Round { get; private set; }
        public int CurrentPlayerNo { get; private set; }
        public Player CurrentPlayer { get; private set; }
        public Object SelectedObject { get; private set; }

        /* *** */

        public Game(Config config)
        {
            Config = config;
            Map = config.Map;
            
            //Collect all objects into the list Objects, construct and init objects
            Objects = Config.CopyObjects(null); //use 'null' so each initial object is copied with their original Id

            foreach(Object obj in Objects)
            {
                obj.Create(this);
            }

            //collect special schemes
            if (Map.Scheme == null)
                Map.Scheme = Config.GetSchemeByName(SpecialSchemes.Map);

            Map?.Scheme?.Compile(config);
            Map?.RecollectMapObjects(Objects);
            Map?.CreateSchemeObject(this);
        }

        private void InitializeCharacter(Character character, int spawnNo)
        {
            if (character.Scheme == null)
            {
                Scheme characterScheme = Config.GetSchemeByName(SpecialSchemes.Character);
                if (characterScheme == null)
                {
                    characterScheme = new Scheme(SpecialSchemes.Character);
                    characterScheme.Compile(Config);
                }
                character.Scheme = characterScheme;
            }

            character.CreateInventorySlots(Config);      //generate variables for inventory slots
            if (spawnNo >= 0)
                character.Position = Map.GetSpawnerByNo(spawnNo);
            character.EvaluateClassItemSpellModifiers(this);    //add items & spells to character provided by its class modifiers

            Objects.Add(character);
            Map.Objects.Add(character);
        }

        //Sets up a game with empty characters (used mainly for test purposes)
        public void SetupPlayers(int numberOfPlayers)
        {

            if (numberOfPlayers > Map.GetSpawnerCount()) throw new GameException("Number of players exceeds number of spawn points");

            Players = new List<Player>();

            //characters are placed on spawn points 0-n
            for(int i=0; i<numberOfPlayers; ++i)
            {
                Player player = new Player();
                InitializeCharacter(player.Character, i);
                Players.Add(player);
            }

        }

        public void SetupPlayers(List<Character> characters)
        {

            int numberOfPlayers = characters.Count;

            if (numberOfPlayers > Map.GetSpawnerCount()) throw new GameException("Number of players exceeds number of spawn points");

            Players = new List<Player>();

            //characters are placed on spawn points 0-n
            for (int i = 0; i < numberOfPlayers; ++i)
            {
                Player player = new Player(characters[i]);
                InitializeCharacter(player.Character, i);
                Players.Add(player);
            }

        }

        public void SetupPlayers(List<Player> players)
        {
            int numberOfPlayers = players.Count;

            if (numberOfPlayers > Map.GetSpawnerCount()) throw new GameException("Number of players exceeds number of spawn points");

            Players = players;

            //characters are placed on spawn points 0-n
            int i = 0;
            foreach(Player player in Players)
            {
                InitializeCharacter(player.Character, i);
                ++i;
            }
        }

        public void Start()
        {

            Log.Write("Starting game");

            Round = 0;

            if (Players.Count == 0) return;

            CurrentPlayerNo = 0;
            CurrentPlayer = Players[CurrentPlayerNo];
            CurrentPlayer.AvailableActionPoints = Config.CharacterConfig.ActionPoints;
        }

        public Object GetObjectById(string objectId)
        {
            foreach(Object @object in Objects)
            {
                if (@object.Id == objectId) return @object;
            }
            return null;
        }

        //The current player can select the object with which they want to do something
        public void SelectObject(string objectId)
        {

            Log.Write($"Player [{CurrentPlayerNo}]: select object '{objectId}'");

            if (objectId == null || objectId == "_none")
            {
                SelectedObject = null;
                return;
            }

            SelectedObject = GetObjectById(objectId);

            if (SelectedObject == null)
            {
                throw new GameException("The given id is invalid");
            }

            //Check if character can reach the object to be selected (eg. owns it, or close enough on map)
            if (!CurrentPlayer.Character.CanReachObject(this, SelectedObject))
                throw new GameException("Character cannot reach selected object");

        }

        //After an object is selected, the current player can do some action with it (or they can do basic actions)
        public void DoAction(string actionName, params string[] actionParameters)
        {

            if (SelectedObject != null)
                Log.Write($"Player [{CurrentPlayerNo}]: do '{actionName}' with object '{SelectedObject.Id}'");
            else
                Log.Write($"Player [{CurrentPlayerNo}]: do '{actionName}({string.Join(", ",actionParameters)})'");



            if (actionName == BasicActions.Movement)
            {
                
                Character character = CurrentPlayer.Character;

                Position newPosition = new Position(character.Position.X, character.Position.Y);

                if (actionParameters[0] == MovementParameters.Norht) newPosition.Y -= 1;
                else if (actionParameters[0] == MovementParameters.East) newPosition.X += 1;
                else if (actionParameters[0] == MovementParameters.South) newPosition.Y += 1;
                else if (actionParameters[0] == MovementParameters.West) newPosition.X -= 1;

                // check if new position is within map borders
                if (!Map.IsPositionWithin(newPosition))
                    throw new GameException("Invalid movement");

                // if no special square at target position, or it has no associated action, the movement cost is the basic movement cost
                int MovementCost = Config.CharacterConfig.MovementActionPoints;

                // check square restrictions at new position
                SquareType squareType = Map.GetSquareTypeAt(newPosition);
                if (squareType != null)
                {
                    if (!squareType.AllowsCharacter(character)) throw new GameException("This character is not allowed to step on this square");
                    MovementCost = Map.GetMovementCost(squareType, Config);
                }

                if (MovementCost > CurrentPlayer.AvailableActionPoints)
                    throw new GameException("Too few action points for movement");

                character.Position = newPosition;
                CurrentPlayer.AvailableActionPoints -= Config.CharacterConfig.MovementActionPoints;

                // if character stepped on a square, we may have to call an action
                Map.ExecuteSquareAction(squareType, character, this);

            }
            else if (actionName == BasicActions.EndTurn)
            {
                NextPlayer();
            }
            else if (SelectedObject != null)
            {
                //Get action points to be removed on a successful action
                int actionPointsToRemove = SelectedObject.Scheme.GetFunctionByName(actionName).ActionPoints;
                
                if (actionPointsToRemove > CurrentPlayer.AvailableActionPoints)
                    throw new GameException("Too few action points");

                actionPointsToRemove = SelectedObject.ExecuteAction(actionName, CurrentPlayer.Character, this);

                CurrentPlayer.AvailableActionPoints -= actionPointsToRemove;
            }
            else
            {
                throw new GameException("Non-basic actions can only be applied to objects");
            }

        }

        private void NextPlayer()
        {

            Log.Write($"Next player's turn");

            CurrentPlayerNo++;

            if (CurrentPlayerNo >= Players.Count)
            {
                NextRound();
                CurrentPlayerNo = 0;
            }

            CurrentPlayer = Players[CurrentPlayerNo];
            CurrentPlayer.AvailableActionPoints = Config.CharacterConfig.ActionPoints;

            Log.Write($"Next player: {CurrentPlayerNo}");

            OnNextPlayer?.Invoke();
        }

        private void NextRound()
        {
            Round++;

            Log.Write($"Next round: {Round}");
        }

        public void Report(Text report)
        {
            Console.WriteLine(report.Content);
            OnReport?.Invoke(report);
        }

        public Map GetMap()
        {
            return Map;
        }

        public void _AddObject(Object @object)
        {
            Objects.Add(@object);
        }

    }
}
