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
    /// Interaction logic for UCEParameterRow_Number.xaml
    /// </summary>
    public partial class UCEParameterRow_Logical : ParameterRow
    {
        public UCEParameterRow_Logical(string paramName, ObjectVariable value) : base(paramName, value)
        {
            InitializeComponent();

            tbParamName.Text = paramName;

            if (value.Value == null) value.Value = false;
            cb.IsChecked = (bool)value.Value;
        }

        private void cb_Checked(object sender, RoutedEventArgs e)
        {
            Value.Value = cb.IsChecked == true;
        }
    }
}
