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

    public delegate void SquareTypeSelectorOnSquareTypeSelectedDelegate(SquareType selectedSquareType);

    /// <summary>
    /// Interaction logic for UCVisualSelector.xaml
    /// </summary>
    public partial class UCSquareTypeSelector : UserControl
    {

        private string filter;
        public string Filter
        {
            get { return filter; }
            set
            {
                filter = value;
                RefreshList();
            }
        }

        public UCEVisualSelectorRow SelectedRow { get; private set; }
        public SquareType SelectedSquareType { get; private set; }

        // subscribe to this event to get notification when a square type had been selected
        public event SquareTypeSelectorOnSquareTypeSelectedDelegate OnSquareTypeSelected;

        public UCSquareTypeSelector()
        {
            InitializeComponent();
            RefreshList();
        }

        public void RefreshList()
        {
            spContainer.Children.Clear();
            SelectedRow = null;

            List<SquareType> squareTypes = Project.Current?.Config.Map.SquareTypes;
            if (squareTypes == null) return;

            var filteredSquareTypes = squareTypes;
            if (Filter != null && Filter.Length > 0)
            {
                string filter_locase = Filter.ToLower();
                filteredSquareTypes = squareTypes.Where(sqt => sqt.Name.ToLower().Contains(filter_locase)).ToList();
            }

            foreach(var sqt in filteredSquareTypes)
            {
                var row = new UCEVisualSelectorRow(sqt.Visual, sqt.Name, sqt);
                row.MouseDown += Row_MouseDown;
                spContainer.Children.Add(row);
            }
        }

        public void SelectBySquareType(SquareType squareType)
        {

            if (SelectedRow != null)
                SelectedRow.Selected = false;
            
            foreach(var row_obj in spContainer.Children)
            {
                var row = (UCEVisualSelectorRow)row_obj;
                if (row.Tag == squareType)
                {
                    SelectedRow = row;
                    SelectedRow.Selected = true;
                    SelectedSquareType = squareType;
                    row.BringIntoView();
                    return;
                }
            }

            SelectedRow = null;
            SelectedSquareType = null;
        }

        public void DeselectAll()
        {
            if (SelectedRow != null)
                SelectedRow.Selected = false;
            SelectedRow = null;
            SelectedSquareType = null;
        }

        private void Row_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                UCEVisualSelectorRow row = (UCEVisualSelectorRow)sender;

                if (SelectedRow != null) SelectedRow.Selected = false;
                SelectedRow = row;
                SelectedRow.Selected = true;

                SelectedSquareType = (SquareType)row.Tag;
                OnSquareTypeSelected?.Invoke(SelectedSquareType);
            }
        }
    }
}
