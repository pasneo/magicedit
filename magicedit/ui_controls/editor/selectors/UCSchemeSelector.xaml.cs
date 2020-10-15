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
    /// Interaction logic for UCSchemeSelector.xaml
    /// </summary>
    public partial class UCSchemeSelector : UserControl
    {

        public event SelectionChangedEventHandler SelectionChanged;

        public Scheme SelectedTag
        {
            get
            {
                ComboBoxItem cbi = (ComboBoxItem)cb.SelectedItem;
                if (cbi?.Tag == null) return null;
                return (Scheme)cbi.Tag;
            }
        }

        public UCSchemeSelector()
        {
            InitializeComponent();
            Refresh();
        }

        public void Refresh()
        {
            var selectedTag = SelectedTag;

            cb.Items.Clear();

            var schemes = Project.Current?.Config?.Schemes;
            if (schemes == null) return;
            
            foreach(var scheme in schemes)
            {
                ComboBoxItem cbi = new ComboBoxItem();
                cbi.Content = scheme.Name;
                cbi.Tag = scheme;
                cb.Items.Add(cbi);
            }

            SelectByTag(selectedTag);
        }

        public void SelectByTag(Scheme tag)
        {
            foreach(var item in cb.Items)
            {
                ComboBoxItem cbi = (ComboBoxItem)item;
                if (cbi.Tag == tag)
                {
                    cb.SelectedItem = item;
                    return;
                }
            }
            cb.SelectedItem = null;
        }

        private void cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectionChanged?.Invoke(this, e);
        }
    }
}
