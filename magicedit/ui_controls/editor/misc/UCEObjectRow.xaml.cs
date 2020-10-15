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
    /// Interaction logic for UCEObjectRow.xaml
    /// </summary>
    public partial class UCEObjectRow : UserControl
    {

        public delegate void CheckChangedDelegate(UCEObjectRow row);

        public event CheckChangedDelegate CheckChanged;

        private bool _checked;
        public bool Checked
        {
            get { return _checked; }
            set
            {
                _checked = value;
                cb.IsChecked = Checked;
                RefreshBackground();
            }
        }

        private bool _hovered = false;
        public bool Hovered
        {
            get { return _hovered; }
            set { _hovered = value; RefreshBackground(); }
        }

        public UCEObjectRow(Object obj, bool selected = false)
        {
            InitializeComponent();

            Tag = obj;
            Checked = selected;
            
            iImage.Source = DefaultResources.GetVisualImageOrDefault(obj.Visual);
            lID.Text = obj.Id;
        }

        private void RefreshBackground()
        {
            if (Hovered)
                grid.Background = Brushes.DarkGray;
            else if (Checked)
                grid.Background = Brushes.PowderBlue;
            else
                grid.Background = Brushes.LightGray;
        }

        private void this_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Checked = !Checked;
            CheckChanged?.Invoke(this);
            e.Handled = true;
        }

        private void this_MouseEnter(object sender, MouseEventArgs e)
        {
            Hovered = true;
        }

        private void this_MouseLeave(object sender, MouseEventArgs e)
        {
            Hovered = false;
        }

        private void cb_CheckChanged(object sender, RoutedEventArgs e)
        {
            Checked = cb.IsChecked.HasValue ? cb.IsChecked.Value : false;
            CheckChanged?.Invoke(this);
            e.Handled = true;
        }
    }
}
