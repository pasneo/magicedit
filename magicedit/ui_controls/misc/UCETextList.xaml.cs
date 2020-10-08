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
    /// Interaction logic for UCETextList.xaml
    /// </summary>
    public partial class UCETextList : UserControl
    {

        public delegate void TextElemModifiedDelegate(UCETextListElem elem);

        public event TextElemModifiedDelegate TextAdded;
        public event TextElemModifiedDelegate TextDeleted;

        public UCETextList()
        {
            InitializeComponent();
        }

        public void ClearList()
        {
            spTexts.Children.Clear();
        }

        public void AddElemToList(UCETextListElem elem)
        {
            spTexts.Children.Add(elem);
        }

        public void RemoveElem(UCETextListElem elem)
        {
            spTexts.Children.Remove(elem);
        }

        public UCETextListElem AddTextToList(string text)
        {
            UCETextListElem elem = new UCETextListElem();
            elem.Content = text;
            elem.DeleteClicked += TextElem_DeleteClicked;

            spTexts.Children.Add(elem);

            return elem;
        }

        private void bAdd_Click(object sender, RoutedEventArgs e)
        {
            if (tbText.Text != null && tbText.Text.Length > 0)
            {
                var elem = AddTextToList(tbText.Text);
                TextAdded?.Invoke(elem);
                tbText.Text = "";
            }
        }

        private void TextElem_DeleteClicked(object sender, RoutedEventArgs e)
        {
            UCETextListElem elem = (UCETextListElem)sender;
            spTexts.Children.Remove(elem);
            TextDeleted?.Invoke(elem);
        }
    }
}
