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
    /// Interaction logic for UCStringConstManager.xaml
    /// </summary>
    public partial class UCObjectSchemeManager : MainUserControl
    {
        

        public UCObjectSchemeManager()
        {
            InitializeComponent();
        }

        public override void Open()
        {
            RefreshList();
        }

        public void RefreshList()
        {
            list.Items.Clear();

            var schemes = Project.Current?.Config.Schemes;
            if (schemes == null) return;

            foreach (var scheme in schemes)
            {
                AddListBoxItem(scheme);
            }

        }

        private ListBoxItem AddListBoxItem(Scheme scheme)
        {

            ListBoxItem listBoxItem = new ListBoxItem();
            listBoxItem.Tag = scheme;

            listBoxItem.Content = scheme.Name;

            list.Items.Add(listBoxItem);

            return listBoxItem;
        }

        private void ClearInfo()
        {
            tbID.Text = "";
            tbCode.Text = "";
            tbID.IsEnabled = false;
            tbCode.IsEnabled = false;
            bDelete.IsEnabled = false;
        }

        private void RefreshInfo()
        {
            if (list.SelectedItem != null)
            {
                tbID.IsEnabled = true;
                tbCode.IsEnabled = true;
                bDelete.IsEnabled = true;

                Scheme scheme = (Scheme)((ListBoxItem)list.SelectedItem).Tag;

                tbID.Text = scheme.Name;
                tbCode.Text = scheme.Code;
            }
            else ClearInfo();
        }

        private void listTexts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshInfo();
        }

        private void bAdd_Click(object sender, RoutedEventArgs e)
        {
            string newId = IdGenerator.Generate("scheme");
            while (Project.Current.Config.GetSchemeByName(newId) != null) newId = IdGenerator.Generate("scheme");

            Scheme scheme = new Scheme(newId);
            Project.Current.Config.AddScheme(scheme);

            var item = AddListBoxItem(scheme);

            list.SelectedItem = item;
        }

        private void tbID_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (list.SelectedItem == null || ((ListBoxItem)list.SelectedItem).Tag == null) return;
            var item = ((ListBoxItem)list.SelectedItem);
            Scheme scheme = (Scheme)item.Tag;
            scheme.Name = tbID.Text;
            item.Content = tbID.Text;
        }

        private void tbCode_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (list.SelectedItem == null || ((ListBoxItem)list.SelectedItem).Tag == null) return;
            var item = ((ListBoxItem)list.SelectedItem);
            Scheme scheme = (Scheme)item.Tag;
            scheme.Code = tbCode.Text;
        }

        private void bDelete_Click(object sender, RoutedEventArgs e)
        {
            ListBoxItem item = (ListBoxItem)list.SelectedItem;
            Scheme scheme = (Scheme)item.Tag;

            Project.Current.Config.Schemes.Remove(scheme);
            list.SelectedItem = null;
            list.Items.Remove(item);
        }
    }
}
