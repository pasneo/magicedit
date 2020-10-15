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
    public partial class UCStringConstManager : MainUserControl
    {
        

        public UCStringConstManager()
        {
            InitializeComponent();
        }

        public override void Open()
        {
            RefreshList();
        }

        public void RefreshList()
        {
            listTexts.Items.Clear();

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

            listTexts.Items.Add(listBoxItem);

            return listBoxItem;
        }

        private void ClearInfo()
        {
            tbTextID.Text = "";
            tbTextContent.Text = "";
            tbTextID.IsEnabled = false;
            tbTextContent.IsEnabled = false;
            bDelete.IsEnabled = false;
        }

        private void RefreshInfo()
        {
            if (listTexts.SelectedItem != null)
            {
                tbTextID.IsEnabled = true;
                tbTextContent.IsEnabled = true;
                bDelete.IsEnabled = true;

                Text text = (Text)((ListBoxItem)listTexts.SelectedItem).Tag;

                tbTextID.Text = text.ID;
                tbTextContent.Text = text.Content;
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

            listTexts.SelectedItem = item;
        }

        private void tbTextID_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (listTexts.SelectedItem == null || ((ListBoxItem)listTexts.SelectedItem).Tag == null) return;
            var item = ((ListBoxItem)listTexts.SelectedItem);
            Text text = (Text)item.Tag;
            text.ID = tbTextID.Text;
            item.Content = tbTextID.Text;
        }

        private void tbTextContent_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (listTexts.SelectedItem == null || ((ListBoxItem)listTexts.SelectedItem).Tag == null) return;
            var item = ((ListBoxItem)listTexts.SelectedItem);
            Text text = (Text)item.Tag;
            text.Content = tbTextContent.Text;
        }

        private void bDelete_Click(object sender, RoutedEventArgs e)
        {
            ListBoxItem item = (ListBoxItem)listTexts.SelectedItem;
            Text text = (Text)item.Tag;

            Project.Current.Config.StringConsts.Remove(text);
            listTexts.SelectedItem = null;
            listTexts.Items.Remove(item);
        }
    }
}
