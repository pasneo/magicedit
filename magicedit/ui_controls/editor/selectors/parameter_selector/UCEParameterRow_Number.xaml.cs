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
    public partial class UCEParameterRow_Number : ParameterRow
    {
        public UCEParameterRow_Number(string paramName, ObjectVariable value) : base(paramName, value)
        {
            InitializeComponent();

            tbParamName.Text = paramName;

            if (value.Value == null) value.Value = 0;
            iValue.NumValue = Convert.ToInt32(value.Value);
        }

        private void iValue_ValueChanged(IntegerUpDown sender)
        {
            Value.Value = iValue.NumValue;
        }
    }
}
