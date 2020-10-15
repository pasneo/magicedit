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
    /// Interaction logic for IntegerUpDown.xaml
    /// </summary>
    public partial class IntegerUpDown : UserControl
    {

        public delegate void IntegerUpDownValueChangedDelegate(IntegerUpDown sender);

        private int _minValue = int.MinValue;
        public int MinValue {
            get { return _minValue; }
            set
            {
                _minValue = value;
                if (NumValue < MinValue) NumValue = MinValue;
            }
        }

        private int _maxValue = int.MaxValue;
        public int MaxValue
        {
            get { return _maxValue; }
            set
            {
                _maxValue = value;
                if (NumValue > MaxValue) NumValue = MaxValue;
            }
        }

        public event IntegerUpDownValueChangedDelegate ValueChanged;

        private int _numValue = 0;

        public int NumValue
        {
            get { return _numValue; }
            set
            {
                txtNum.Background = Brushes.LightGreen;
                _numValue = value;
                if (_numValue < MinValue) _numValue = MinValue;
                if (_numValue > MaxValue) _numValue = MaxValue;
                txtNum.Text = _numValue.ToString();
            }
        }

        public IntegerUpDown()
        {
            InitializeComponent();
            txtNum.Text = _numValue.ToString();
        }

        private void cmdUp_Click(object sender, RoutedEventArgs e)
        {
            int lastValue = NumValue;
            NumValue++;
            if (lastValue != NumValue) ValueChanged?.Invoke(this);
        }

        private void cmdDown_Click(object sender, RoutedEventArgs e)
        {
            int lastValue = NumValue;
            NumValue--;
            if (lastValue != NumValue) ValueChanged?.Invoke(this);
        }

        private void txtNum_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtNum == null)
            {
                return;
            }

            txtNum.Text = new string(txtNum.Text.Where(c => char.IsDigit(c)).ToArray());

            int lastValue = NumValue;
            int value = 0;

            if (int.TryParse(txtNum.Text, out value))
            {
                if (value < MinValue || value > MaxValue)
                    txtNum.Background = Brushes.PaleVioletRed;
                else
                {
                    NumValue = value;
                    txtNum.Background = Brushes.LightGreen;
                    if (NumValue != lastValue) ValueChanged?.Invoke(this);
                }
            }
        }

    }
}
