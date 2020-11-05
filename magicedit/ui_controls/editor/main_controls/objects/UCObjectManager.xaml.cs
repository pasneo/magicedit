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
    public partial class UCObjectManager : MainUserControl
    {
        

        public UCObjectManager()
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
                if (obj is MapObject)
                    AddListBoxItem((MapObject)obj);
            }

        }

        private ListBoxItem AddListBoxItem(MapObject obj)
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

            iVisualImage.Source = null;
            visualSelector.Visibility = Visibility.Hidden;
            lVisualID.Content = "";

            paramSelector.Object = null;
        }

        private void RefreshInfo()
        {
            if (list.SelectedItem != null)
            {
                MapObject obj = (MapObject)((ListBoxItem)list.SelectedItem).Tag;

                tbID.IsEnabled = true;
                schemeSelector.IsEnabled = true;
                textSelector.IsEnabled = true;
                bDelete.IsEnabled = true;

                var item = ((ListBoxItem)list.SelectedItem);
                MapObject mo = (MapObject)item.Tag;
                iVisualImage.Source = (mo.Visual?.BitmapFrame == null) ? DefaultResources.VisualPlaceholder : mo.Visual.BitmapFrame;
                lVisualID.Content = (mo.Visual == null) ? "" : mo.Visual.ID;

                schemeSelector.SelectByTag(obj.Scheme);
                textSelector.SelectByTag(obj.ShownName);

                paramSelector.Object = obj;

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
            string newId = IdGenerator.Generate("obj");
            while (Project.Current.Config.GetObjectById(newId) != null) newId = IdGenerator.Generate("obj");

            MapObject mo = new MapObject(newId, newId);
            Project.Current.Config.AddObject(mo);

            var item = AddListBoxItem(mo);

            list.SelectedItem = item;
        }

        private void tbID_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (list.SelectedItem == null || ((ListBoxItem)list.SelectedItem).Tag == null) return;
            var item = ((ListBoxItem)list.SelectedItem);
            MapObject mo = (MapObject)item.Tag;
            mo.Name = mo.Id = tbID.Text;
            item.Content = tbID.Text;

            paramSelector.Refresh();
        }

        private void bDelete_Click(object sender, RoutedEventArgs e)
        {
            ListBoxItem item = (ListBoxItem)list.SelectedItem;
            MapObject mo = (MapObject)item.Tag;

            Project.Current.Config.Objects.Remove(mo);
            list.SelectedItem = null;
            list.Items.Remove(item);
        }

        private void textSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (list.SelectedItem == null || ((ListBoxItem)list.SelectedItem).Tag == null) return;
            var item = ((ListBoxItem)list.SelectedItem);
            MapObject mo = (MapObject)item.Tag;
            mo.ShownName = textSelector.SelectedTag;
        }

        private void schemeSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (list.SelectedItem == null || ((ListBoxItem)list.SelectedItem).Tag == null) return;
            var item = ((ListBoxItem)list.SelectedItem);
            MapObject mo = (MapObject)item.Tag;
            mo.Scheme = schemeSelector.SelectedTag;

            paramSelector.Refresh();
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
            MapObject mo = (MapObject)item.Tag;
            mo.Visual = selectedVisual;
            RefreshInfo();
        }
    
    }
}
