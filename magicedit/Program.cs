using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    class Program
    {

        static void Main(string[] args)
        {

            Program program = new Program();
            program.Test();

            try
            {
                //program = new Program();
                //program.Test();
            }
            catch (GameException ex)
            {
                Console.WriteLine($"Game error: {ex.Message}");
            }
            catch (SchemeExecutionException ex)
            {
                Console.WriteLine($"Error at [{ex.CommandIndex}]: {ex.Message}");
            }

            Console.ReadKey();

        }

        private Scheme GetLeverScheme()
        {
            Scheme leverScheme = new Scheme();
            leverScheme.CompiledScheme = new CompiledScheme();

            SchemeFunction use = new SchemeFunction();
            use.Name = "use";
            use.AddCommand(new CommandSetOf("ready", "box", "true"));
            use.AddCommand(new CommandDesc("LEVER_DESC_2"));

            leverScheme.CompiledScheme.AddAction(use);

            return leverScheme;
        }

        private Scheme GetBoxScheme()
        {
            Scheme boxScheme = new Scheme();
            boxScheme.CompiledScheme = new CompiledScheme();

            boxScheme.CompiledScheme.AddVariable(new ObjectVariable(VariableTypes.Logical, "ready", false));

            SchemeFunction pickup = new SchemeFunction();
            pickup.Name = "pickup";
            pickup.AddCommand(new CommandJumpIfFalse("ready", 3));
            pickup.AddCommand(new CommandPrint("Picked up"));
            pickup.AddCommand(new CommandJump(4));
            pickup.AddCommand(new CommandPrint("Box is stuck"));

            boxScheme.CompiledScheme.AddAction(pickup);

            return boxScheme;
        }

        private void GameTest()
        {
            Config config = new Config();

            //... set up character rules (eg. action points)
            config.CharacterConfig.ActionPoints = 10;
            config.CharacterConfig.MovementActionPoints = 1;

            //... create string consts
            config.AddStringConst("LEVER_DESC", new Text("This is a lever"));
            config.AddStringConst("LEVER_DESC_2", new Text("Someone used this lever"));

            //... create map, spawners and squares
            Map map = new Map();
            map.Width = 5;
            map.Height = 5;

            map.AddSpawner(new Position(2, 4));
            map.AddSpawner(new Position(0, 3));

            //... create objects (with known IDs)
            MapObject lever = new MapObject();
            lever.Id = "lever";
            lever.Position = new Position(2, 1);
            lever.Description = config.GetStringConstByName("LEVER_DESC");

            MapObject box = new MapObject();
            box.Id = "box";
            box.Position = new Position(4, 3);
            box.Variables.Add(new ObjectVariable("logical", "ready", false));   //ready of box = false

            map._AddObject(lever);
            map._AddObject(box);

            //... create schemes for objects
            lever.Scheme = GetLeverScheme();
            box.Scheme = GetBoxScheme();

            //... add everything to config
            config.Map = map;

            //Create game based on config
            Game game = new Game(config);
            game._AddObject(lever);
            game._AddObject(box);

            //Set up game with some players (eg. 2)
            game.SetupPlayers(2);

            //Start game
            game.Start();

            //do some example actions

            Console.WriteLine($"Lever desc: {lever.Description.Content}");

            //move player1 2 squares north
            game.DoAction(Game.BasicActions.Movement, Game.MovementParameters.Norht);
            game.DoAction(Game.BasicActions.Movement, Game.MovementParameters.Norht);
            //'use' the lever with id 'lever'       # this makes it possible to pick up the box
            game.SelectObject("lever");
            //game.DoAction("use");

            Console.WriteLine($"Lever desc: {lever.Description.Content}");

            //next player
            game.DoAction(Game.BasicActions.EndTurn);

            //move player2 2 squares east
            game.SelectObject("_none");
            game.DoAction(Game.BasicActions.Movement, Game.MovementParameters.East);
            game.DoAction(Game.BasicActions.Movement, Game.MovementParameters.East);
            game.DoAction(Game.BasicActions.Movement, Game.MovementParameters.East);
            //'pickup' the box with id 'box'
            game.SelectObject("box");
            game.DoAction("pickup");

            //... check outcomes

        }

        private void Test()
        {

            SchemeFunction f = new SchemeFunction();
            f.AddCommand(new CommandAddAction("grab"));
            f.AddCommand(new CommandAddAction("examine"));

            f.AddCommand(new CommandRemoveAction("grab"));
            f.AddCommand(new CommandAddAction("drop"));

            MapObject @object = new MapObject();
            @object.Name = "some_object";
            @object.Variables.Add(new ObjectVariable("logical", "isDwarf", false));
            @object.Variables.Add(new ObjectVariable("logical", "isElf", false));

            @object.Scheme = new Scheme("some_scheme");
            @object.Scheme.CompiledScheme = new CompiledScheme();
            @object.Scheme.CompiledScheme.AddAction(new SchemeFunction("grab"));
            @object.Scheme.CompiledScheme.AddAction(new SchemeFunction("examine"));
            @object.Scheme.CompiledScheme.AddAction(new SchemeFunction("drop"));

            Config config = new Config();
            config.AddScheme(@object.Scheme);

            Game game = new Game(config);
            game._AddObject(@object);

            //Execute function on object
            f.Execute(@object, new Character(), game);

        }

    }
}
