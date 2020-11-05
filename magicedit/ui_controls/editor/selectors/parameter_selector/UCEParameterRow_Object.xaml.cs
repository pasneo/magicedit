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
    public partial class UCEParameterRow_Object : ParameterRow
    {
        public UCEParameterRow_Object(string paramName, ObjectVariable value, Config config) : base(paramName, value)
        {
            InitializeComponent();
            
            tbParamName.Text = paramName;

            if (value.Type != VariableTypes.Object)
            {
                Scheme scheme = config.GetSchemeByName(value.Type);
                objectSelector.SchemeFilter = scheme;
            }

            objectSelector.SelectByTag((Object)value.Value);
        }

        private void objectSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Value.Value = objectSelector.SelectedTag;
        }
    }
}
