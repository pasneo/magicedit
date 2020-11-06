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
    /// Interaction logic for UCSquareTypeManager.xaml
    /// </summary>
    public partial class UCSquareTypeManager : MainUserControl
    {

        /* *** */

        public UCSquareTypeManager()
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

            List<SquareType> squareTypes = Project.Current?.Config.Map.SquareTypes;
            if (squareTypes == null) return;

            foreach (SquareType squareType in squareTypes)
            {
                AddListBoxItem(squareType);
            }

        }

        private void RefreshActions(string selectedActionName)
        {
            cbActionName.Items.Clear();

            Scheme scheme = Project.Current.Config.GetSchemeByName(SpecialSchemes.Map);
            
            //add default item with 'null' (so we can select 'none' action)
            ComboBoxItem defaultItem = new ComboBoxItem();
            defaultItem.Content = "(no action)";
            defaultItem.Tag = null;
            cbActionName.Items.Add(defaultItem);
            if (selectedActionName == null) defaultItem.IsSelected = true;

            if (scheme == null) return;

            var errors = SchemeLang.CompileWithErrors(scheme, Project.Current.Config);

            foreach(var action in scheme.CompiledScheme.ActionFunctions)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = item.Tag = action.Name;
                cbActionName.Items.Add(item);

                if (selectedActionName == action.Name) item.IsSelected = true;
            }
        }

        private ListBoxItem AddListBoxItem(SquareType squareType)
        {
            ListBoxItem listBoxItem = new ListBoxItem();
            listBoxItem.Tag = squareType;

            listBoxItem.Content = squareType.Name;

            list.Items.Add(listBoxItem);

            return listBoxItem;
        }

        private void ClearInfo()
        {
            tbID.Text = "";
            tbID.IsEnabled = false;
            iVisualImage.Source = null;
            tbAllowedAttributes.Text = "";
            tbAllowedAttributes.IsEnabled = false;
            tbForbiddenAttributes.Text = "";
            tbForbiddenAttributes.IsEnabled = false;

            cbActionName.Items.Clear();
            cbActionName.SelectedItem = null;
            cbActionName.IsEnabled = false;

            bDelete.IsEnabled = false;
            visualSelector.Visibility = Visibility.Hidden;
            lVisualID.Content = "";
        }

        private void RefreshInfo()
        {
            if (list.SelectedItem != null)
            {
                visualSelector.Visibility = Visibility.Hidden;

                tbID.IsEnabled = true;
                tbAllowedAttributes.IsEnabled = true;
                tbForbiddenAttributes.IsEnabled = true;

                cbActionName.IsEnabled = true;

                bDelete.IsEnabled = true;

                SquareType squareType = (SquareType)((ListBoxItem)list.SelectedItem).Tag;

                tbID.Text = squareType.Name;
                tbAllowedAttributes.Text = string.Join(" ", squareType.AllowedAttributes.ToArray());
                tbForbiddenAttributes.Text = string.Join(" ", squareType.ForbiddenAttributes.ToArray());
                RefreshActions(squareType.ActionName);

                iVisualImage.Source = (squareType.Visual?.BitmapFrame == null) ? DefaultResources.VisualPlaceholder : squareType.Visual.BitmapFrame;
                lVisualID.Content = (squareType.Visual == null) ? "" : squareType.Visual.ID;
            }
            else ClearInfo();
        }

        private void list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshInfo();
        }

        private void bDelete_Click(object sender, RoutedEventArgs e)
        {
            ListBoxItem item = (ListBoxItem)list.SelectedItem;
            SquareType squareType = (SquareType)item.Tag;

            Project.Current.Config.Map.SquareTypes.Remove(squareType);
            list.SelectedItem = null;
            list.Items.Remove(item);

        }

        private void bAdd_Click(object sender, RoutedEventArgs e)
        {
            string newId = IdGenerator.Generate("squaretype");
            while (Project.Current.Config.Map.GetSquareTypeByName(newId) != null) newId = IdGenerator.Generate("squaretype");

            SquareType squareType = new SquareType(newId);

            Project.Current.Config.Map.SquareTypes.Add(squareType);
            var item = AddListBoxItem(squareType);

            list.SelectedItem = item;
        }

        private void tbID_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (list.SelectedItem == null || ((ListBoxItem)list.SelectedItem).Tag == null) return;

            ListBoxItem item = (ListBoxItem)list.SelectedItem;
            SquareType squareType = (SquareType)item.Tag;

            squareType.Name = tbID.Text;

            item.Content = tbID.Text;
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

            ListBoxItem item = (ListBoxItem)list.SelectedItem;
            SquareType squareType = (SquareType)item.Tag;
            squareType.Visual = selectedVisual;

            RefreshInfo();
        }

        private void tbAllowedAttributes_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (list.SelectedItem == null || ((ListBoxItem)list.SelectedItem).Tag == null) return;

            ListBoxItem item = (ListBoxItem)list.SelectedItem;
            SquareType squareType = (SquareType)item.Tag;

            squareType.AllowedAttributes = tbAllowedAttributes.Text.Split(' ').ToList();
        }

        private void tbForbiddenAttributes_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (list.SelectedItem == null || ((ListBoxItem)list.SelectedItem).Tag == null) return;

            ListBoxItem item = (ListBoxItem)list.SelectedItem;
            SquareType squareType = (SquareType)item.Tag;

            squareType.ForbiddenAttributes = tbForbiddenAttributes.Text.Split(' ').ToList();
        }

        private void cbActionName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (list.SelectedItem == null || ((ListBoxItem)list.SelectedItem).Tag == null || cbActionName.SelectedItem == null) return;

            ListBoxItem item = (ListBoxItem)list.SelectedItem;
            SquareType squareType = (SquareType)item.Tag;

            squareType.ActionName = (string)((ComboBoxItem)cbActionName.SelectedItem).Tag;
        }
    }
}
