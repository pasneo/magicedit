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
    /// Interaction logic for UCENumericButton.xaml
    /// </summary>
    public partial class UCENumericButton : UserControl
    {

        public delegate void ValueChangedDelegate(UCENumericButton sender);

        public event ValueChangedDelegate ValueChanged;

        private int _value;
        public int Value
        {
            get { return _value; }
            set
            {
                _value = value;
                if (Value < 0) button.Background = Brushes.Red;
                else if (Value > 0) button.Background = Brushes.LightGreen;
                else button.Background = Brushes.LightGray;
                button.Content = (Value > 0) ? "+" + Value.ToString() : Value.ToString();
            }
        }

        public UCENumericButton()
        {
            InitializeComponent();
            Value = 0;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Value++;
            ValueChanged?.Invoke(this);
        }

        private void button_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Value--;
            ValueChanged?.Invoke(this);
            e.Handled = true;
        }
    }
}
