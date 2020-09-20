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

        private bool Hovered { get; set; } = false;

        private bool selected = false;
        public bool Selected
        {
            get { return selected; }
            set { selected = value; RefreshBackground(); }
        }

        public UCEVisualSelectorRow(Visual visual)
        {
            InitializeComponent();

            Visual = visual;

            iImage.Source = visual.BitmapFrame;
            lID.Content = visual.ID;
        }

        private void RefreshBackground()
        {
            if (Hovered)
                grid.Background = Brushes.DarkGray;
            else if (Selected)
                grid.Background = Brushes.PowderBlue;
            else
                grid.Background = Brushes.LightGray;
        }

        private void this_MouseEnter(object sender, MouseEventArgs e)
        {
            Hovered = true;
        }

        private void this_MouseLeave(object sender, MouseEventArgs e)
        {
            Hovered = false;
        }
    }
}
