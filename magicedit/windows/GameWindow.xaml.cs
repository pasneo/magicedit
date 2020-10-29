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

        public Object SelectedObject { get; private set; }

        private bool ReportArrived = false;
        
        public GameWindow(Game game)
        {
            Game = game;

            Game.OnReport += Game_OnReport;
            Game.OnNextPlayer += Game_OnNextPlayer;

            InitializeComponent();

            map.Game = Game;
            map.OnMapPositionSelectionChanged += Map_OnMapPositionSelectionChanged;

            actionPanel.Game = Game;
            actionPanel.ActionExecuted += ActionPanel_ActionExecuted;

            inventoryPanel.Character = Game.CurrentPlayer.Character;
            inventoryPanel.OnItemSelected += InventoryPanel_OnItemSelected;
        }

        private void InventoryPanel_OnItemSelected(UCEItemRow itemRow)
        {
            map.DeselectAll();
            if (itemRow != null && itemRow.Selected)
                SelectedObject = itemRow.Item;
            else
                SelectedObject = null;
            Refresh();
        }

        private void Game_OnNextPlayer()
        {
            tbReport.Text = "";
            inventoryPanel.Character = Game.CurrentPlayer.Character;
        }

        private void Game_OnReport(Text report)
        {
            tbReport.Text = report.Content;
            ReportArrived = true;
        }

        private void ActionPanel_ActionExecuted()
        {
            map.DeselectAll();
            //Refresh();

            inventoryPanel.Refresh();

            if (!ReportArrived) tbReport.Text = "";
            ReportArrived = false;
        }

        private void Refresh()
        {
            var selectedPosition = map.SelectedPositions.FirstOrDefault();

            if (SelectedObject != null && !Game.CurrentPlayer.Character.CanReachObject(Game, SelectedObject))
            {
                SelectedObject = null;
                tbDesc.Text = "You are too far away from this item to examine it.";
            }
            else
            {
                tbDesc.Text = "";
                if (SelectedObject != null)
                {
                    string desc = SelectedObject.Description?.Content;
                    if (desc != null) tbDesc.Text = desc;
                }
            }

            actionPanel.SelectedObject = SelectedObject;
            actionPanel.SelectedPosition = selectedPosition;
            actionPanel.Refresh();

            txActionPoints.Text = Game.CurrentPlayer.AvailableActionPoints.ToString();

        }

        private void Map_OnMapPositionSelectionChanged(UCMapEditor mapEditor)
        {
            var selectedObjects = map.GetSelectedMapObjects();
            SelectedObject = selectedObjects.FirstOrDefault();

            Refresh();
        }
    }
}
