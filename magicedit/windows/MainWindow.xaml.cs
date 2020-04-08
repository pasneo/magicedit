using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace magicedit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            try
            {
                Test();
            }
            catch(SchemeExecutionException ex)
            {
                Console.WriteLine($"Error at [{ex.CommandIndex}]: {ex.Message}");
            }

        }

        private Scheme GetLeverScheme()
        {
            Scheme leverScheme = new Scheme();
            leverScheme.CompiledScheme = new CompiledScheme();

            SchemeFunction use = new SchemeFunction();
            use.AddCommand(new CommandSetOf("ready", "cola", "true"));
            use.AddCommand(new CommandDesc("LEVER_DESC_2"));

            leverScheme.CompiledScheme.AddAction(use);

            return leverScheme;
        }

        private Scheme GetDrinkScheme()
        {
            Scheme drinkScheme = new Scheme();
            drinkScheme.CompiledScheme = new CompiledScheme();

            drinkScheme.CompiledScheme.AddVariable(new ObjectVariable(VariableTypes.Logical, "ready", false));

            SchemeFunction pickup = new SchemeFunction();
            pickup.AddCommand(new CommandJumpIfFalse("ready", 100));
            pickup.AddCommand(new CommandPrint("Picked up"));

            drinkScheme.CompiledScheme.AddAction(pickup);

            return drinkScheme;
        }

        private void GameTest()
        {
            Config config = new Config();

            //... set up character rules (eg. action points)
            config.CharacterConfig.ActionPoints = 10;

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

            MapObject drink = new MapObject();
            drink.Id = "cola";
            drink.Position = new Position(4, 3);
            drink.Variables.Add(new ObjectVariable("logical", "ready", false));   //ready of cola = false

            //... create schemes for objects
            lever.Scheme = GetLeverScheme();
            drink.Scheme = GetDrinkScheme();

            //... add everything to config

            //Create game based on config
            Game game = new Game(config);

            //Set up game with some players (eg. 2)
            game.SetupPlayers(2);

            //Start game
            game.Start();

            //do some example actions

                //move player1 2 squares north
                game.DoAction(Game.BasicActions.Movement, Game.MovementParameters.Norht);
                game.DoAction(Game.BasicActions.Movement, Game.MovementParameters.Norht);
                //'use' the lever with id 'lever'       # this makes it possible to pick up the drink
                game.SelectObject("lever");
                game.DoAction("use");
                //next player
                game.DoAction(Game.BasicActions.EndTurn);
                //move player2 2 squares east
                game.DoAction(Game.BasicActions.Movement, Game.MovementParameters.East);
                game.DoAction(Game.BasicActions.Movement, Game.MovementParameters.East);
                //'pickup' the drink with id 'cola'     # this creates a copy of cola with id 'cola (1)'
                game.SelectObject("cola");
                game.DoAction("pickup");
                //'drink' the cola with id 'cola (1)'   # after this action the action points of player2 is zero
                game.SelectObject("cola (1)");
                game.DoAction("drink");

            //... check outcomes

        }

        private void Test()
        {

            //Create function, add commands to it
            SchemeFunction function = new SchemeFunction();

            function.AddCommand(new CommandCreateVariable("number", "i"));
            function.AddCommand(new CommandSetOf("x", "object2", "30"));
            function.AddCommand(new CommandOf("x", "object2", "_0"));
            function.AddCommand(new CommandSetVariable("i", "_0"));

            function.AddCommand(new CommandPrintValue("i"));

            //Print full code of function
            function.Print();

            //Create sample object
            MapObject @object = new MapObject();

            MapObject object2 = new MapObject();
            object2.Id = "object2";
            object2.Variables.Add(new ObjectVariable("number", "x", 15));   //x of object = 15

            Config config = new Config();
            Game game = new Game(config);

            game._AddObject(object2);

            //Execute function on object
            function.Execute(@object, new Character(), game);

        }

        private void gsColSplitter_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            //tvExplorer.Width = gridExplorerContainer.ActualWidth;
        }

        private void mRun_Click(object sender, RoutedEventArgs e)
        {
            GameWindow gameWindow = new GameWindow();
            gameWindow.Show();
        }
    }
}
