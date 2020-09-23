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

        //public object Tag { get; set; }

        private bool hovered = false;
        public bool Hovered
        {
            get { return hovered; }
            set { hovered = value; RefreshBackground(); }
        }

        private bool selected = false;
        public bool Selected
        {
            get { return selected; }
            set { selected = value; RefreshBackground(); }
        }

        public UCEVisualSelectorRow(Visual visual)
        {
            Initialize(visual, visual.ID, visual);
        }

        public UCEVisualSelectorRow(Visual visual, string id, object tag = null)
        {
            Initialize(visual, id, tag);
        }

        private void Initialize(Visual visual, string id, object tag = null)
        {
            InitializeComponent();

            Tag = tag;

            iImage.Source = DefaultResources.GetVisualImageOrDefault(visual);
            lID.Text = id;
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
