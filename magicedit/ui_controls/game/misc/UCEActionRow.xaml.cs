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
    /// Interaction logic for UCEActionRow.xaml
    /// </summary>
    public partial class UCEActionRow : UserControl
    {

        public UCEActionRow()
        {
            InitializeComponent();
        }

        public UCEActionRow(string action, int actionPoint, bool available)
        {
            InitializeComponent();

            tbAction.Text = action;
            tbActionPoint.Text = actionPoint.ToString();

            if (!available)
            {
                tbAction.Foreground = tbActionPoint.Foreground = Brushes.LightGray;
            }
        }
    }
}
