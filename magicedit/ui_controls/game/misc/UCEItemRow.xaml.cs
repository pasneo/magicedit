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
    /// Interaction logic for UCEItemRow.xaml
    /// </summary>
    public partial class UCEItemRow : UserControl
    {

        public delegate void OnSelectedDelegate(UCEItemRow itemRow);

        public event OnSelectedDelegate OnSelected;
        public event OnSelectedDelegate OnMoveClicked;

        private bool _selected;
        public bool Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                RefreshBackground();
            }
        }

        private bool _hover;
        public bool Hover
        {
            get { return _hover; }
            set
            {
                _hover = value;
                RefreshBackground();
            }
        }

        public Object Item { get; set; }

        public UCEItemRow()
        {
            InitializeComponent();
        }

        public UCEItemRow(Object item)
        {
            Item = item;

            InitializeComponent();

            tbItemName.Text = item?.Name;
            img.Source = DefaultResources.GetVisualImageOrDefault(item.Visual);
        }

        private void RefreshBackground()
        {
            if (Hover)
            {
                grid.Background = Brushes.DarkOrange;
            }
            else if (Selected)
            {
                grid.Background = Brushes.Orange;
            }
            else
            {
                grid.Background = Brushes.LightGray;
            }
        }

        private void hyMove_Click(object sender, RoutedEventArgs e)
        {
            OnMoveClicked?.Invoke(this);
        }

        private void grid_MouseEnter(object sender, MouseEventArgs e)
        {
            Hover = true;
        }

        private void grid_MouseLeave(object sender, MouseEventArgs e)
        {
            Hover = false;
        }

        private void grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Selected = !Selected;
            OnSelected?.Invoke(this);
        }
    }
}
