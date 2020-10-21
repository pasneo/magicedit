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
    /// Interaction logic for UCEClassSelector.xaml
    /// </summary>
    public partial class UCEClassSelector : UserControl
    {

        private ObjectVariable ClassVariable { get; set; }

        public UCEClassSelector(ClassList classList, ObjectVariable classVariable)
        {
            InitializeComponent();
            ClassVariable = classVariable;

            if (classList.ShownName != null)
                tbClasslistName.Text = $"{classList.ShownName.Content} ({classList.Name})";
            else
                tbClasslistName.Text = classList.Name;

            BuildClassSelector(classList);
        }

        private void BuildClassSelector(ClassList classList)
        {
            foreach(Class c in classList.Classes)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Tag = c;

                if (c.ShownName != null) item.Content = $"{c.ShownName.Content} ({c.Name})";
                else item.Content = c.Name;

                cbClass.Items.Add(item);

                if (ClassVariable.Value == c) cbClass.SelectedItem = item;
            }
        }

        private void cbClass_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem item = (ComboBoxItem)cbClass.SelectedItem;
            ClassVariable.Value = (Class)item.Tag;
        }
    }
}
