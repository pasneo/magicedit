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
    public partial class UCObjectSelector : UserControl
    {

        public event SelectionChangedEventHandler SelectionChanged;

        private ObjectTypeTags? _typeTag;
        public ObjectTypeTags? TypeTag {
            get { return _typeTag; }
            set
            {
                _typeTag = value;
                Refresh();
            }
        }

        private Scheme _schemeFilter;
        public Scheme SchemeFilter
        {
            get { return _schemeFilter; }
            set
            {
                _schemeFilter = value;
                TypeTag = null;
            }
        }

        public Object SelectedTag
        {
            get
            {
                ComboBoxItem cbi = (ComboBoxItem)cb.SelectedItem;
                if (cbi?.Tag == null) return null;
                return (Object)cbi.Tag;
            }
        }

        public UCObjectSelector()
        {
            InitializeComponent();
            Refresh();
        }

        public void Refresh()
        {
            var selectedTag = SelectedTag;

            cb.Items.Clear();

            var objects = Project.Current?.Config?.Objects;
            if (objects == null) return;
            
            foreach(var obj in objects)
            {
                if ((TypeTag != null && obj.TypeTag == TypeTag) ||
                    (TypeTag == null && (SchemeFilter == null ||
                        (obj.Scheme != null &&
                            (obj.Scheme == SchemeFilter || obj.Scheme.HasAncestor(SchemeFilter))))))
                {
                    ComboBoxItem cbi = new ComboBoxItem();
                    cbi.Content = obj.Name;
                    cbi.Tag = obj;
                    cb.Items.Add(cbi);
                }
            }

            SelectByTag(selectedTag);
        }

        public void SelectByTag(Object tag)
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
