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
using System.Windows.Shapes;

namespace magicedit
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        public Game Game { get; set; }
        
        public GameWindow(Game game)
        {
            Game = game;

            InitializeComponent();

            map.Game = Game;
            map.OnMapPositionSelectionChanged += Map_OnMapPositionSelectionChanged;

            actionPanel.Game = Game;
        }

        private void Map_OnMapPositionSelectionChanged(UCMapEditor mapEditor)
        {
            var selectedObjects = map.GetSelectedMapObjects();

            var selectedObject = selectedObjects.FirstOrDefault();

            tbDesc.Text = "";
            actionPanel.SelectedObject = selectedObject;

            if (selectedObject != null)
            {
                string desc = selectedObject.Description?.Content;
                if (desc != null) tbDesc.Text = desc;
            }
        }
    }
}
