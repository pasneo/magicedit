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
    public partial class UCItemManager : MainUserControl
    {
        

        public UCItemManager()
        {
            InitializeComponent();
        }

        public override void Open()
        {
            RefreshList();
            schemeSelector.Refresh();
            textSelector.Refresh();
        }

        public void RefreshList()
        {
            list.Items.Clear();

            var objects = Project.Current?.Config.Objects;
            if (objects == null) return;

            foreach (var obj in objects)
            {
                if (obj.TypeTag == ObjectTypeTags.Item)
                    AddListBoxItem(obj);
            }

        }

        private ListBoxItem AddListBoxItem(Object obj)
        {

            ListBoxItem listBoxItem = new ListBoxItem();
            listBoxItem.Tag = obj;

            listBoxItem.Content = obj.Id;

            list.Items.Add(listBoxItem);

            return listBoxItem;
        }

        private void ClearInfo()
        {
            tbID.Text = "";
            tbID.IsEnabled = false;
            bDelete.IsEnabled = false;

            schemeSelector.IsEnabled = textSelector.IsEnabled = false;
            schemeSelector.cb.SelectedItem = textSelector.cb.SelectedItem = null;

            iVisualImage.Source = null;
            visualSelector.Visibility = Visibility.Hidden;
            lVisualID.Content = "";
        }

        private void RefreshInfo()
        {
            if (list.SelectedItem != null)
            {
                Object obj = (Object)((ListBoxItem)list.SelectedItem).Tag;

                tbID.IsEnabled = true;
                schemeSelector.IsEnabled = true;
                textSelector.IsEnabled = true;
                bDelete.IsEnabled = true;
                
                iVisualImage.Source = (obj.Visual?.BitmapFrame == null) ? DefaultResources.VisualPlaceholder : obj.Visual.BitmapFrame;
                lVisualID.Content = (obj.Visual == null) ? "" : obj.Visual.ID;

                schemeSelector.SelectByTag(obj.Scheme);
                textSelector.SelectByTag(obj.ShownName);

                tbID.Text = obj.Id;
            }
            else ClearInfo();
        }

        private void list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshInfo();
        }

        private void bAdd_Click(object sender, RoutedEventArgs e)
        {
            string newId = IdGenerator.Generate("item");
            while (Project.Current.Config.GetObjectById(newId) != null) newId = IdGenerator.Generate("item");

            Object obj = new Object(newId, newId, ObjectTypeTags.Item);
            Project.Current.Config.AddObject(obj);

            var item = AddListBoxItem(obj);

            list.SelectedItem = item;
        }

        private void tbID_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (list.SelectedItem == null || ((ListBoxItem)list.SelectedItem).Tag == null) return;
            var item = ((ListBoxItem)list.SelectedItem);
            Object obj = (Object)item.Tag;
            obj.Name = obj.Id = tbID.Text;
            item.Content = tbID.Text;
        }

        private void bDelete_Click(object sender, RoutedEventArgs e)
        {
            ListBoxItem item = (ListBoxItem)list.SelectedItem;
            Object obj = (Object)item.Tag;

            Project.Current.Config.Objects.Remove(obj);
            list.SelectedItem = null;
            list.Items.Remove(item);
        }

        private void tbParameters_TextChanged(object sender, TextChangedEventArgs e)
        {
            //todo: parse parameters like a=12 b=$STRING1 etc.
        }

        private void textSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (list.SelectedItem == null || ((ListBoxItem)list.SelectedItem).Tag == null) return;
            var item = ((ListBoxItem)list.SelectedItem);
            Object obj = (Object)item.Tag;
            obj.ShownName = textSelector.SelectedTag;
        }

        private void schemeSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (list.SelectedItem == null || ((ListBoxItem)list.SelectedItem).Tag == null) return;
            var item = ((ListBoxItem)list.SelectedItem);
            Object obj = (Object)item.Tag;
            obj.Scheme = schemeSelector.SelectedTag;
        }

        private void iVisualImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (list.SelectedItem == null) return;

            if (e.ChangedButton == MouseButton.Left)
            {
                if (visualSelector.IsVisible)
                    visualSelector.Visibility = Visibility.Hidden;
                else
                {
                    visualSelector.RefreshList();
                    visualSelector.Visibility = Visibility.Visible;
                }
            }
        }

        private void visualSelector_OnVisualSelected(Visual selectedVisual)
        {
            visualSelector.Visibility = Visibility.Hidden;
            if (list.SelectedItem == null || ((ListBoxItem)list.SelectedItem).Tag == null) return;
            var item = ((ListBoxItem)list.SelectedItem);
            Object obj = (Object)item.Tag;
            obj.Visual = selectedVisual;
            RefreshInfo();
        }
    
    }
}
