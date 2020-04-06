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

        private void GameTest()
        {
            Config config = new Config();

            //... create string consts
            //... create map and squares
            //... create objects (with known IDs)
            //... create schemes for objects

            //Create game based on config
            Game game = new Game(config);

            //Set up game with some players (eg. one)
            game.SetupPlayers(1);

            //Start game
            game.Start();

            //... do some example actions

        }

        private void Test()
        {

            //Create function, add commands to it
            SchemeFunction function = new SchemeFunction();

            function.AddCommand(new CommandCreateVariable("number", "i"));
            function.AddCommand(new CommandCreateVariable("logical", "l"));
            function.AddCommand(new CommandSetVariable("i", "0"));
            
            function.AddCommand(new CommandPrintValue("i"));

            function.AddCommand(new CommandAdd("i", "1", "i"));

            function.AddCommand(new CommandLower("10", "i", "l"));
            function.AddCommand(new CommandJumpIfFalse("l", 3));

            //Print full code of function
            function.Print();

            //Create sample object
            MapObject @object = new MapObject();
            @object.Variables.Add(new ObjectVariable("number", "x", 15));   //x of object = 15

            //Execute function on object
            function.Execute(@object, new Character());

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
