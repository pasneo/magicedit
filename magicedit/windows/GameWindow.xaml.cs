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
            actionPanel.ActionExecuted += ActionPanel_ActionExecuted;
        }

        private void ActionPanel_ActionExecuted()
        {
            map.DeselectAll();
            Refresh();
        }

        private void Refresh()
        {
            var selectedObjects = map.GetSelectedMapObjects();

            var selectedObject = selectedObjects.FirstOrDefault();
            var selectedPosition = map.SelectedPositions.FirstOrDefault();

            if (!Game.CurrentPlayer.Character.CanReachObject(Game, selectedObject)) selectedObject = null;

            actionPanel.SelectedObject = selectedObject;
            actionPanel.SelectedPosition = selectedPosition;
            actionPanel.Refresh();

            txActionPoints.Text = Game.CurrentPlayer.AvailableActionPoints.ToString();

            tbDesc.Text = "";
            if (selectedObject != null)
            {
                string desc = selectedObject.Description?.Content;
                if (desc != null) tbDesc.Text = desc;
            }
        }

        private void Map_OnMapPositionSelectionChanged(UCMapEditor mapEditor)
        {
            Refresh();
        }
    }
}
