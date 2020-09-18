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
    /// Interaction logic for UCEVisualSelectorRow.xaml
    /// </summary>
    public partial class UCEVisualSelectorRow : UserControl
    {

        public Visual Visual { get; private set; }

        public UCEVisualSelectorRow(Visual visual)
        {
            InitializeComponent();

            Visual = visual;

            iImage.Source = visual.BitmapFrame;
            lID.Content = visual.ID;
        }

        private void this_MouseEnter(object sender, MouseEventArgs e)
        {
            grid.Background = Brushes.DarkGray;
        }

        private void this_MouseLeave(object sender, MouseEventArgs e)
        {
            grid.Background = Brushes.LightGray;
        }
    }
}
