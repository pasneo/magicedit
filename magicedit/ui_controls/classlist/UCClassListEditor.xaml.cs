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
    /// Interaction logic for UCClassListEditor.xaml
    /// </summary>
    public partial class UCClassListEditor : UserControl
    {

        public Class SelectedClass { get; set; }


        public UCClassListEditor()
        {
            InitializeComponent();
        }

        public void RebuildTree()
        {
            SelectedClass = null;

            spClassSettings.Visibility = Visibility.Hidden;
            tviClasslists.Items.Clear();

            var classLists = Project.Current.Config.ClassLists;
            foreach(var classList in classLists)
            {
                var item = AddNewClasslistItem(classList);
                foreach(var @class in classList.Classes)
                {
                    AddNewClassItem(item, @class);
                }
            }
        }

        private void BuildClassContent(Class @class)
        {
            SelectedClass = @class;

            spClassSettings.Visibility = Visibility.Visible;

            tbName.Text = @class.Name;
            textSelector.SelectByTag(@class.ShownName);

            spAbilityModifiers.Children.Clear();

            foreach(var ability in Project.Current.Config.CharacterConfig.Abilities)
            {
                spAbilityModifiers.Children.Add(new UCEAbilityModifier(@class, ability.Name));
            }

            tlSetAttributes.ClearList();
            tlForbiddenAttributes.ClearList();

            foreach(var modif in @class.Modifiers)
            {
                if (modif is AttributeModifier)
                {
                    AttributeModifier attributeModifier = (AttributeModifier)modif;
                    if (attributeModifier.Option == AttributeModifier.AttributeModifierOptions.SET)
                    {
                        var elem = tlSetAttributes.AddTextToList(attributeModifier.AttributeName);
                        elem.Tag = attributeModifier;
                    }
                    else if (attributeModifier.Option == AttributeModifier.AttributeModifierOptions.FORBID)
                    {
                        var elem = tlForbiddenAttributes.AddTextToList(attributeModifier.AttributeName);
                        elem.Tag = attributeModifier;
                    }
                }
            }

        }

        private void cmiNewClasslist_Click(object sender, RoutedEventArgs e)
        {
            AddNewClasslistItem(new ClassList("classlist"));
        }

        private void tviClassList_MouseRightButton(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void tviClassList_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContextMenu cm = this.FindResource("cmNewClass") as ContextMenu;
            cm.PlacementTarget = sender as TreeViewItem;
            cm.Tag = sender;
            cm.IsOpen = true;
            e.Handled = true;
        }

        private void miNewClass_Click(object sender, RoutedEventArgs e)
        {
            ContextMenu cm = this.FindResource("cmNewClass") as ContextMenu;
            TreeViewItem tvi = cm.Tag as TreeViewItem;

            AddNewClassItem(tvi, new Class("class"));
        }

        private TreeViewItem AddNewClasslistItem(ClassList classList)
        {
            TreeViewItem item = new TreeViewItem();
            item.Header = classList.Name;

            item.PreviewMouseRightButtonDown += tviClassList_MouseRightButtonDown;

            item.MouseRightButtonDown += tviClassList_MouseRightButton;
            item.PreviewMouseRightButtonUp += tviClassList_MouseRightButton;
            item.MouseRightButtonUp += tviClassList_MouseRightButton;

            tviClasslists.Items.Add(item);

            return item;
        }

        private TreeViewItem AddNewClassItem(TreeViewItem classListItem, Class @class)
        {
            TreeViewItem item = new TreeViewItem();
            item.Header = @class.Name;
            item.Tag = @class;

            item.PreviewMouseLeftButtonDown += tviClass_PreviewMouseLeftButtonDown;

            classListItem.Items.Add(item);

            return item;
        }

        private void tviClass_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Class @class = (Class)((TreeViewItem)sender).Tag;
            BuildClassContent(@class);
        }

        private void tlSetAttributes_TextAdded(UCETextListElem elem)
        {
            AttributeModifier modif = new AttributeModifier(AttributeModifier.AttributeModifierOptions.SET, elem.Content);
            elem.Tag = modif;
            SelectedClass.AddModifier(modif);
        }

        private void tlSetAttributes_TextDeleted(UCETextListElem elem)
        {
            SelectedClass.Modifiers.Remove((AttributeModifier)elem.Tag);
        }

        private void tlForbiddenAttributes_TextAdded(UCETextListElem elem)
        {
            AttributeModifier modif = new AttributeModifier(AttributeModifier.AttributeModifierOptions.FORBID, elem.Content);
            elem.Tag = modif;
            SelectedClass.AddModifier(modif);
        }

        private void tlForbiddenAttributes_TextDeleted(UCETextListElem elem)
        {
            SelectedClass.Modifiers.Remove((AttributeModifier)elem.Tag);
        }
    }
}
