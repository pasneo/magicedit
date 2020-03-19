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

            function.AddCommand(new CommandCreateVariable("number", "b"));
            function.AddCommand(new CommandSetVariable("b", "a"));
            function.AddCommand(new CommandPrintValue("b"));

            function.AddCommand(new CommandAdd("a", "b", "a"));
            function.AddCommand(new CommandPrintValue("a"));

            function.Execute(new MapObject(), new Character());

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
