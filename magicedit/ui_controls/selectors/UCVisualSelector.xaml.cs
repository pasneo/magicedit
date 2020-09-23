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

    public delegate void VisualSelectorOnVisualSelectedDelegate(Visual selectedVisual);

    /// <summary>
    /// Interaction logic for UCVisualSelector.xaml
    /// </summary>
    public partial class UCVisualSelector : UserControl
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

        public Visual SelectedVisual { get; set; }

        // subscribe to this event to get notification when a visual had been selected
        public event VisualSelectorOnVisualSelectedDelegate OnVisualSelected;

        public UCVisualSelector()
        {
            InitializeComponent();
            RefreshList();
        }

        public void RefreshList()
        {
            spContainer.Children.Clear();

            List<Visual> visuals = Project.Current?.Config.Visuals;
            if (visuals == null) return;

            var filteredVisuals = visuals;
            if (Filter != null && Filter.Length > 0)
            {
                string filter_locase = Filter.ToLower();
                filteredVisuals = visuals.Where(visual => visual.ID.ToLower().Contains(filter_locase)).ToList();
            }

            foreach(var visual in filteredVisuals)
            {
                var row = new UCEVisualSelectorRow(visual);
                row.MouseDown += Row_MouseDown;
                spContainer.Children.Add(row);
            }
        }

        private void Row_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                UCEVisualSelectorRow row = (UCEVisualSelectorRow)sender;
                SelectedVisual = (Visual)row.Tag;
                OnVisualSelected?.Invoke(SelectedVisual);
            }
        }
    }
}
