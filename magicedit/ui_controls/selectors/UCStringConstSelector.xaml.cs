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
    public partial class UCStringConstSelector : UserControl
    {

        public event SelectionChangedEventHandler SelectionChanged;

        public Text SelectedTag
        {
            get
            {
                ComboBoxItem cbi = (ComboBoxItem)cb.SelectedItem;
                if (cbi?.Tag == null) return null;
                return (Text)cbi.Tag;
            }
        }

        public UCStringConstSelector()
        {
            InitializeComponent();
            Refresh();
        }

        public void Refresh()
        {
            var selectedTag = SelectedTag;

            cb.Items.Clear();

            var texts = Project.Current?.Config?.StringConsts;
            if (texts == null) return;
            
            foreach(var text in texts)
            {
                ComboBoxItem cbi = new ComboBoxItem();
                string textContentCropped = text.CropContent(15);
                cbi.Content = $"{text.ID} ({textContentCropped})";
                cbi.Tag = text;
                cb.Items.Add(cbi);
            }

            SelectByTag(selectedTag);
        }

        public void SelectByTag(Text tag)
        {
            foreach (var item in cb.Items)
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
