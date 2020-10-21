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
    public partial class UCEAbilityButton : UserControl
    {

        public delegate void ValueChangedDelegate(UCEAbilityButton sender);

        public event ValueChangedDelegate ValueIncreased;
        public event ValueChangedDelegate ValueDecreased;

        private int _value;
        public int Value
        {
            get { return _value; }
            set
            {
                _value = value;
                button.Content = Value.ToString();
            }
        }

        public UCEAbilityButton()
        {
            InitializeComponent();
            Value = 0;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Value++;
            ValueIncreased?.Invoke(this);
        }

        private void button_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Value == 0) return;
            Value--;
            ValueDecreased?.Invoke(this);
            e.Handled = true;
        }
    }
}
