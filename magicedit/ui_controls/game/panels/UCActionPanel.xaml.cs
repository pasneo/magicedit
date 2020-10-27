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
    /// Interaction logic for UCActionPanel.xaml
    /// </summary>
    public partial class UCActionPanel : UserControl
    {

        public delegate void ActionExecutedDelegate();

        public event ActionExecutedDelegate ActionExecuted;

        public Game Game { get; set; }
        
        public Object SelectedObject { get; set; }
        public Position SelectedPosition { get; set; }

        public UCActionPanel()
        {
            InitializeComponent();
        }

        public void Refresh()
        {
            list.Items.Clear();
            list.IsEnabled = true;

            int actionPoints = Game.CurrentPlayer.AvailableActionPoints;

            if (SelectedPosition != null)
            {
                if (Game.CurrentPlayer.Character.CanStepToPosition(SelectedPosition, Game))
                {
                    int movementCost = Game.GetMap().GetMovementCost(SelectedPosition, Game.Config);
                    
                    var actionPlan = new MoveActionPlan(Game, Game.CurrentPlayer.Character, SelectedPosition);
                    UCEActionRow row = new UCEActionRow("Move", movementCost, movementCost <= actionPoints, actionPlan);
                    AddActionRow(row);
                }
            }

            if (SelectedObject != null && SelectedObject.AvailableActions.Count > 0)
            {
                var actions = SelectedObject.AvailableActions;

                foreach (var action in actions)
                {
                    int actionCost = SelectedObject.Scheme.GetFunctionByName(action).ActionPoints;
                    
                    var actionPlan = new ObjectActionPlan(Game, Game.CurrentPlayer.Character, SelectedObject, action);
                    UCEActionRow row = new UCEActionRow(action, actionCost, actionCost <= actionPoints, actionPlan);
                    AddActionRow(row);
                }

            }

            if (list.Items.Count == 0)
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = "No other actions available";
                item.IsEnabled = false;
                list.Items.Add(item);
            }
            
            var endTurnActionPlan = new EndTurnActionPlan(Game, Game.CurrentPlayer.Character);
            UCEActionRow endTurnRow = new UCEActionRow("End turn", 0, true, endTurnActionPlan);
            AddActionRow(endTurnRow);

        }

        private ListBoxItem AddActionRow(UCEActionRow row)
        {
            ListBoxItem item = new ListBoxItem();
            item.Content = row;
            list.Items.Add(item);
            item.PreviewMouseLeftButtonDown += Item_MouseLeftButtonDown;
            return item;
        }

        private void Item_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            ListBoxItem item = (ListBoxItem)sender;

            UCEActionRow row = (UCEActionRow)item.Content;
            list.SelectedItem = null;
            row.Execute();

            ActionExecuted?.Invoke();
        }

        private void list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
    }
}
