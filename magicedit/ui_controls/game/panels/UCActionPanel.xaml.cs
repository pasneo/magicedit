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

        public Game Game { get; set; }

        private Object selectedObject;
        public Object SelectedObject
        {
            get { return selectedObject; }
            set
            {
                selectedObject = value;
                Refresh();
            }
        }

        public UCActionPanel()
        {
            InitializeComponent();
        }

        private void Refresh()
        {
            list.Items.Clear();

            if (SelectedObject == null)
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = "No actions available";
                list.Items.Add(item);
                list.IsEnabled = false;
                return;
            }

            list.IsEnabled = true;

            var actions = SelectedObject.AvailableActions;

            int actionPoints = Game.CurrentPlayer.AvailableActionPoints;

            foreach(var action in actions)
            {
                int actionCost = SelectedObject.Scheme.GetFunctionByName(action).ActionPoints;

                ListBoxItem item = new ListBoxItem();
                UCEActionRow row = new UCEActionRow(action, actionCost, actionCost <= actionPoints);
                
                item.Content = row;

                list.Items.Add(item);
            }

        }

    }
}
