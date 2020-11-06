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
    /// Interaction logic for UCItemCategoryManager.xaml
    /// </summary>
    public partial class UCItemCategoryManager : MainUserControl
    {
        public UCItemCategoryManager()
        {
            InitializeComponent();
        }

        public override void Open(EditorErrorDescriptor eed)
        {
            RefreshList();
        }

        public void RefreshList()
        {
            list.Items.Clear();

            List<ItemSpellCategory> categories = Project.Current?.Config.ItemConfig.Categories;
            if (categories == null) return;

            foreach (ItemSpellCategory cat in categories)
            {
                AddListBoxItem(cat);
            }

        }

        private ListBoxItem AddListBoxItem(ItemSpellCategory cat)
        {
            ListBoxItem listBoxItem = new ListBoxItem();
            listBoxItem.Tag = cat;

            listBoxItem.Content = cat.Name;

            list.Items.Add(listBoxItem);

            return listBoxItem;
        }

        private void ClearInfo()
        {
            tbName.Text = tbCode.Text = "";
            tbName.IsEnabled = tbCode.IsEnabled = false;
            bDelete.IsEnabled = false;
        }

        private void RefreshInfo()
        {
            if (list.SelectedItem != null)
            {
                tbName.IsEnabled = tbCode.IsEnabled = true;
                bDelete.IsEnabled = true;

                ItemSpellCategory cat = (ItemSpellCategory)((ListBoxItem)list.SelectedItem).Tag;

                tbName.Text = cat.Name;
                tbCode.Text = (cat.Scheme?.Code);
            }
            else ClearInfo();
        }

        private void list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshInfo();
        }

        private void bAdd_Click(object sender, RoutedEventArgs e)
        {
            string newCatName = IdGenerator.Generate("itemcat");
            while (Project.Current.Config.ItemConfig.Categories.Where(cat => cat.Name.Equals(newCatName)).Count() > 0) newCatName = IdGenerator.Generate("itemcat");

            ItemSpellCategory newCat = new ItemSpellCategory(newCatName);

            Project.Current.Config.ItemConfig.Categories.Add(newCat);
            ListBoxItem item = AddListBoxItem(newCat);
            list.SelectedItem = item;
        }

        private void bDelete_Click(object sender, RoutedEventArgs e)
        {
            ListBoxItem item = (ListBoxItem)list.SelectedItem;
            ItemSpellCategory cat = (ItemSpellCategory)item.Tag;

            Project.Current.Config.ItemConfig.Categories.Remove(cat);
            list.SelectedItem = null;
            list.Items.Remove(item);
        }

        private void tbCode_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (list.SelectedItem == null || ((ListBoxItem)list.SelectedItem).Tag == null) return;

            ListBoxItem item = (ListBoxItem)list.SelectedItem;
            ItemSpellCategory cat = (ItemSpellCategory)item.Tag;
            cat.Scheme.Code = tbCode.Text;
        }

        private void tbName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (list.SelectedItem == null || ((ListBoxItem)list.SelectedItem).Tag == null) return;

            ListBoxItem item = (ListBoxItem)list.SelectedItem;
            ItemSpellCategory cat = (ItemSpellCategory)item.Tag;
            cat.Name = tbName.Text;

            item.Content = cat.Name;
        }
    }
}
