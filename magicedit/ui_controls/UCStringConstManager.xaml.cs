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

        public void RefreshList()
        {
            listTexts.Items.Clear();

            Dictionary<string, Text> StringConsts = Project.Current?.Config.StringConsts;
            if (StringConsts == null) return;

            foreach (var text in StringConsts)
            {
                AddListBoxItem(text);
            }

        }

        private ListBoxItem AddListBoxItem(KeyValuePair<string, Text> pair)
        {

            ListBoxItem listBoxItem = new ListBoxItem();
            listBoxItem.Tag = pair;

            listBoxItem.Content = pair.Key;

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

                KeyValuePair<string, Text> text = (KeyValuePair<string, Text>)((ListBoxItem)listTexts.SelectedItem).Tag;

                tbTextID.Text = text.Key;
                tbTextContent.Text = text.Value.Content;
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
            
            Project.Current.Config.AddStringConst(newTextId, new Text());
            var pair = Project.Current.Config.StringConsts.Where(p => p.Key == newTextId).FirstOrDefault();

            var item = AddListBoxItem(pair);

            listTexts.SelectedItem = item;
        }

        private void tbTextID_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (listTexts.SelectedItem == null || ((ListBoxItem)listTexts.SelectedItem).Tag == null) return;
            KeyValuePair<string, Text> pair = (KeyValuePair<string, Text>)((ListBoxItem)listTexts.SelectedItem).Tag;
            //pair.key
        }

        private void tbTextContent_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void bDelete_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
