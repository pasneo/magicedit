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
    /// Interaction logic for UCParameterSelector.xaml
    /// </summary>
    public partial class UCParameterSelector : UserControl
    {

        private Object _object;
        public Object Object
        {
            get { return _object; }
            set
            {
                _object = value;
                Refresh();
            }
        }

        public UCParameterSelector()
        {
            InitializeComponent();
        }

        public void Refresh()
        {
            spParams.Children.Clear();

            Scheme scheme = Object?.Scheme;

            if (scheme?.CompiledScheme == null) return;

            foreach(var param in scheme.CompiledScheme.Parameters)
            {
                var value = Object.GetParameterByName(param.Name);

                if (value == null || value.Type != param.Type)
                {
                    // if parameter's type changed, we delete the old and create a new
                    if (value != null && value.Type != param.Type)
                    {
                        Object.Parameters.RemoveAll(p => p.Name == param.Name);
                    }

                    value = new ObjectVariable(param.Type, param.Name, null);
                    Object.Parameters.Add(value);
                }

                var paramRow = ParameterRowFactory.Create(param, value, Project.Current.Config);
                paramRow.Margin = new Thickness(5,5,5,5);
                spParams.Children.Add(paramRow);
            }
        }

    }
}
