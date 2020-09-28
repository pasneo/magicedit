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

        public void RefreshList()
        {
            list.Items.Clear();

            List<Text> StringConsts = Project.Current?.Config.StringConsts;
            if (StringConsts == null) return;

            foreach (var text in StringConsts)
            {
                AddListBoxItem(text);
            }

        }

        private ListBoxItem AddListBoxItem(Text text)
        {

            ListBoxItem listBoxItem = new ListBoxItem();
            listBoxItem.Tag = text;

            listBoxItem.Content = text.ID;

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

                Text text = (Text)((ListBoxItem)list.SelectedItem).Tag;

                tbID.Text = text.ID;
                tbCode.Text = text.Content;
            }
            else ClearInfo();
        }

        private void listTexts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshInfo();
        }

        private void bAddText_Click(object sender, RoutedEventArgs e)
        {
            string newTextId = IdGenerator.Generate("text");
            while (Project.Current.Config.GetStringConstByName(newTextId) != null) newTextId = IdGenerator.Generate("text");

            Text text = new Text();
            Project.Current.Config.AddStringConst(newTextId, text);

            var item = AddListBoxItem(text);

            list.SelectedItem = item;
        }

        private void tbTextID_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (list.SelectedItem == null || ((ListBoxItem)list.SelectedItem).Tag == null) return;
            var item = ((ListBoxItem)list.SelectedItem);
            Text text = (Text)item.Tag;
            text.ID = tbID.Text;
            item.Content = tbID.Text;
        }

        private void tbTextContent_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (list.SelectedItem == null || ((ListBoxItem)list.SelectedItem).Tag == null) return;
            var item = ((ListBoxItem)list.SelectedItem);
            Text text = (Text)item.Tag;
            text.Content = tbCode.Text;
        }

        private void bDelete_Click(object sender, RoutedEventArgs e)
        {
            ListBoxItem item = (ListBoxItem)list.SelectedItem;
            Text text = (Text)item.Tag;

            Project.Current.Config.StringConsts.Remove(text);
            list.SelectedItem = null;
            list.Items.Remove(item);
        }
    }
}
