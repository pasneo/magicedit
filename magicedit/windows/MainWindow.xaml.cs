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

            Test();

        }

        private void Test()
        {
            SchemeFunction function = new SchemeFunction();

            function.AddCommand(new CommandCreateVariable("number", "a"));
            function.AddCommand(new CommandSetVariable("a", "5"));
            function.AddCommand(new CommandPrintValue("a"));

            function.AddCommand(new CommandSetOf("x", "me", "30"));

            function.AddCommand(new CommandOf("x", "me", "a"));
            function.AddCommand(new CommandPrintValue("a"));

            function.Print();

            //Create sample object
            MapObject @object = new MapObject();
            @object.Variables.Add(new ObjectVariable("number", "x", 15));

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
