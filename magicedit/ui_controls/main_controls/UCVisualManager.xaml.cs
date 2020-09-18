using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for UCVisualManager.xaml
    /// </summary>
    public partial class UCVisualManager : MainUserControl
    {
        
        /* *** */

        public UCVisualManager()
        {
            InitializeComponent();
        }

        public override void Open()
        {
            RefreshList();
        }

        public void RefreshList()
        {
            listVisuals.Items.Clear();

            List<Visual> Visuals = Project.Current?.Config.Visuals;
            if (Visuals == null) return;

            foreach (Visual visual in Visuals)
            {
                AddListBoxItem(visual);
            }

        }

        private ListBoxItem AddListBoxItem(Visual visual)
        {
            ListBoxItem listBoxItem = new ListBoxItem();
            listBoxItem.Tag = visual;

            listBoxItem.Content = visual.ID;

            listVisuals.Items.Add(listBoxItem);

            return listBoxItem;
        }

        private void ClearInfo()
        {
            tbVisualID.Text = "";
            iVisualImage.Source = null;
            tbVisualID.IsEnabled = false;
            bDelete.IsEnabled = false;
        }

        private void RefreshInfo()
        {
            if (listVisuals.SelectedItem != null)
            {
                tbVisualID.IsEnabled = true;
                bDelete.IsEnabled = true;

                Visual visual = (Visual)((ListBoxItem)listVisuals.SelectedItem).Tag;

                tbVisualID.Text = visual.ID;
                iVisualImage.Source = (visual.BitmapFrame == null) ? DefaultResources.VisualPlaceholder : visual.BitmapFrame;
            }
            else ClearInfo();
        }

        private void listVisuals_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshInfo();
        }

        private void bDelete_Click(object sender, RoutedEventArgs e)
        {
            ListBoxItem item = (ListBoxItem)listVisuals.SelectedItem;
            Visual visual = (Visual)item.Tag;

            Project.Current.Config.Visuals.Remove(visual);
            listVisuals.SelectedItem = null;
            listVisuals.Items.Remove(item);

        }

        private void bAddVisual_Click(object sender, RoutedEventArgs e)
        {
            string newVisualId = IdGenerator.Generate("visual");
            while(Project.Current.Config.GetVisualById(newVisualId) != null) newVisualId = IdGenerator.Generate("visual");

            Visual visual = new Visual(newVisualId, "");

            Project.Current.Config.Visuals.Add(visual);
            var item = AddListBoxItem(visual);

            listVisuals.SelectedItem = item;
        }

        private void tbVisualID_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (listVisuals.SelectedItem == null || ((ListBoxItem)listVisuals.SelectedItem).Tag == null) return;

            ListBoxItem item = (ListBoxItem)listVisuals.SelectedItem;
            Visual visual = (Visual)item.Tag;

            visual.ID = tbVisualID.Text;

            item.Content = tbVisualID.Text;
        }

        private void iVisualImage_MouseDown(object sender, MouseButtonEventArgs e)
        {

            if (listVisuals.SelectedItem == null) return;

            if (e.ChangedButton == MouseButton.Left)
            {

                ListBoxItem item = (ListBoxItem)listVisuals.SelectedItem;
                Visual visual = (Visual)item.Tag;

                string imagePath = GetImageFileFromUser();
                if (imagePath != null)
                {
                    try
                    {
                        visual.SetImagePath(imagePath);
                        RefreshInfo();
                    }
                    catch (System.NotSupportedException)
                    {
                        MessageBox.Show("Not supported image format!", "Failure", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private string GetImageFileFromUser()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png, *.jpg, *.jpeg, *.gif, *.bmp)|*.png;*.jpg;*.jpeg;*.gif;*.bmp|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }
            return null;
        }

    }
}