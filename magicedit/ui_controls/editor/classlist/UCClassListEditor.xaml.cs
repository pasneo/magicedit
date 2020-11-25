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

        public ClassList SelectedClassList { get; set; }
        public Class SelectedClass { get; set; }

        //todo: add delete classlist and delete class to ui

        public UCClassListEditor()
        {
            InitializeComponent();

            tviClasslists.MouseRightButtonDown += TviClasslists_MouseRightButtonDown;
        }

        public void Refresh()
        {
            textSelector.Refresh();
            itemSelector.Refresh();
            spellSelector.Refresh();
        }

        private void ClearInfo()
        {
            SelectedClass = null;
            SelectedClassList = null;

            tbName.IsEnabled = false;
            textSelector.IsEnabled = false;

            gridSettings.Visibility = Visibility.Hidden;
            spClassSettings.Visibility = Visibility.Hidden;
        }

        public void RebuildTree()
        {
            ClearInfo();

            tviClasslists.Items.Clear();

            var classLists = Project.Current.Config.ClassLists;
            foreach(var classList in classLists)
            {
                var item = AddNewClasslistItem(classList);
                foreach(var @class in classList.Classes)
                {
                    AddNewClassItem(item, @class);
                }
                item.IsExpanded = true;
            }

            tviClasslists.IsExpanded = true;
        }

        private void BuildClasslistContent(ClassList classList)
        {
            SelectedClassList = classList;
            SelectedClass = null;

            tbName.Text = classList.Name;
            textSelector.SelectByTag(classList.ShownName);

            tbName.IsEnabled = true;
            textSelector.IsEnabled = true;
            gridSettings.Visibility = Visibility.Visible;
            spClassSettings.Visibility = Visibility.Hidden;
        }

        private void BuildClassContent(Class @class)
        {
            SelectedClass = @class;
            SelectedClassList = null;

            gridSettings.Visibility = Visibility.Visible;
            spClassSettings.Visibility = Visibility.Visible;

            tbName.Text = @class.Name;
            textSelector.SelectByTag(@class.ShownName);

            tbName.IsEnabled = true;
            textSelector.IsEnabled = true;

            spAbilityModifiers.Children.Clear();

            foreach(var ability in Project.Current.Config.CharacterConfig.Abilities)
            {
                spAbilityModifiers.Children.Add(new UCEAbilityModifier(@class, ability.Name));
            }

            tlSetAttributes.ClearList();
            tlForbiddenAttributes.ClearList();
            spItemModifiers.Children.Clear();
            spSpellModifiers.Children.Clear();

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
                else if (modif is ItemModifier)
                {
                    var textElem = CreateItemModifierElem((ItemModifier)modif);
                    spItemModifiers.Children.Add(textElem);
                }
                else if (modif is SpellModifier)
                {
                    var textElem = CreateSpellModifierElem((SpellModifier)modif);
                    spSpellModifiers.Children.Add(textElem);
                }
            }

        }

        private void cmiNewClasslist_Click(object sender, RoutedEventArgs e)
        {
            ClassList classList = new ClassList("classlist");
            Project.Current.Config.AddClassList(classList);
            var item = AddNewClasslistItem(classList);
            item.IsSelected = true;
            item.IsExpanded = true;
        }

        private void tviClassList_MouseRightButton(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void tviClass_MouseRightButton(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void TviClasslists_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContextMenu cm = this.FindResource("cmClasslists") as ContextMenu;
            cm.PlacementTarget = sender as TreeViewItem;
            cm.Tag = sender;
            cm.IsOpen = true;
            e.Handled = true;
        }

        private void tviClassList_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContextMenu cm = this.FindResource("cmNewClass") as ContextMenu;
            cm.PlacementTarget = sender as TreeViewItem;
            cm.Tag = sender;
            cm.IsOpen = true;
            //e.Handled = true;
        }

        private void tviClass_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContextMenu cm = this.FindResource("cmClass") as ContextMenu;
            cm.PlacementTarget = sender as TreeViewItem;
            cm.Tag = sender;
            cm.IsOpen = true;
            e.Handled = true;
        }

        private void miNewClass_Click(object sender, RoutedEventArgs e)
        {
            ContextMenu cm = this.FindResource("cmNewClass") as ContextMenu;
            TreeViewItem tvi = cm.Tag as TreeViewItem;

            ClassList classList = (ClassList)tvi.Tag;
            Class @class = new Class("class");

            classList.AddClass(@class);

            var item = AddNewClassItem(tvi, @class);
            item.IsSelected = true;
        }

        private void miDeleteClassList_Click(object sender, RoutedEventArgs e)
        {
            ContextMenu cm = this.FindResource("cmNewClass") as ContextMenu;
            TreeViewItem tvi = cm.Tag as TreeViewItem;

            ClassList classList = (ClassList)tvi.Tag;

            Project.Current.Config.ClassLists.Remove(classList);
            RebuildTree();
        }

        private void miDeleteClass_Click(object sender, RoutedEventArgs e)
        {
            ContextMenu cm = this.FindResource("cmClass") as ContextMenu;
            TreeViewItem tvi = cm.Tag as TreeViewItem;

            Class c = (Class)tvi.Tag;

            Project.Current.Config.ClassLists.ForEach(cl => cl.Classes.Remove(c));
            RebuildTree();
        }

        private TreeViewItem AddNewClasslistItem(ClassList classList)
        {
            TreeViewItem item = new TreeViewItem();
            item.Header = classList.Name;
            item.Tag = classList;

            item.PreviewMouseRightButtonDown += tviClassList_MouseRightButtonDown;

            //item.MouseRightButtonDown += tviClassList_MouseRightButton;
            //item.PreviewMouseRightButtonUp += tviClassList_MouseRightButton;
            //item.MouseRightButtonUp += tviClassList_MouseRightButton;

            tviClasslists.Items.Add(item);

            return item;
        }

        private TreeViewItem AddNewClassItem(TreeViewItem classListItem, Class @class)
        {
            TreeViewItem item = new TreeViewItem();
            item.Header = @class.Name;
            item.Tag = @class;

            //item.PreviewMouseLeftButtonDown += tviClass_PreviewMouseLeftButtonDown;

            item.PreviewMouseRightButtonDown += tviClass_MouseRightButtonDown;

            //item.MouseRightButtonDown += tviClass_MouseRightButton;
            //item.PreviewMouseRightButtonUp += tviClass_MouseRightButton;
            //item.MouseRightButtonUp += tviClass_MouseRightButton;

            classListItem.Items.Add(item);

            return item;
        }

        //private void tviClass_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    Class @class = (Class)((TreeViewItem)sender).Tag;
        //    BuildClassContent(@class);
        //}

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

        private void tbName_TextChanged(object sender, TextChangedEventArgs e)
        {
            var item = (TreeViewItem)tvClasslists.SelectedItem;
            if (item == null) return;

            if (item.Tag is ClassList)
            {
                ClassList classList = (ClassList)item.Tag;
                classList.Name = tbName.Text;
                item.Header = tbName.Text;
            }
            else if (item.Tag is Class)
            {
                Class @class = (Class)item.Tag;
                @class.Name = tbName.Text;
                item.Header = tbName.Text;
            }

        }

        private void textSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (TreeViewItem)tvClasslists.SelectedItem;
            if (item == null) return;

            if (item.Tag is ClassList)
            {
                ClassList classList = (ClassList)item.Tag;
                classList.ShownName = textSelector.SelectedTag;
            }
            else if (item.Tag is Class)
            {
                Class @class = (Class)item.Tag;
                @class.ShownName = textSelector.SelectedTag;
            }
        }

        private void tvClasslists_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var item = (TreeViewItem)tvClasslists.SelectedItem;

            if (item == null || (!(item.Tag is Class) && !(item.Tag is ClassList))) ClearInfo();

            if (item.Tag is Class)
            {
                Class @class = (Class)item.Tag;
                BuildClassContent(@class);
            }
            else if (item.Tag is ClassList)
            {
                ClassList classList = (ClassList)item.Tag;
                BuildClasslistContent(classList);
            }
        }

        private UCETextListElem CreateItemModifierElem(ItemModifier modif)
        {
            UCETextListElem textElem = new UCETextListElem();
            textElem.Content = $"{modif.ItemName} ({modif.ItemNumber})";
            textElem.Tag = modif;
            textElem.DeleteClicked += ItemTextElem_DeleteClicked;

            return textElem;
        }

        private UCETextListElem CreateSpellModifierElem(SpellModifier modif)
        {
            UCETextListElem textElem = new UCETextListElem();
            textElem.Content = $"{modif.SpellName}";
            textElem.Tag = modif;
            textElem.DeleteClicked += SpellTextElem_DeleteClicked; ;

            return textElem;
        }

        private void SpellTextElem_DeleteClicked(object sender, RoutedEventArgs e)
        {
            Class selectedClass = (Class)((TreeViewItem)tvClasslists.SelectedItem).Tag;

            UCETextListElem textElem = (UCETextListElem)sender;

            selectedClass.Modifiers.Remove((SpellModifier)textElem.Tag);
            spSpellModifiers.Children.Remove(textElem);
        }

        private void bAddItem_Click(object sender, RoutedEventArgs e)
        {
            if (itemSelector.SelectedTag != null)
            {
                Class selectedClass = (Class)((TreeViewItem)tvClasslists.SelectedItem).Tag;

                ItemModifier modif = new ItemModifier(itemSelector.SelectedTag.Name, nItemNumber.NumValue);
                selectedClass.AddModifier(modif);

                var textElem = CreateItemModifierElem(modif);
                spItemModifiers.Children.Add(textElem);
            }

            itemSelector.SelectByTag(null);
            nItemNumber.NumValue = 1;
        }

        private void ItemTextElem_DeleteClicked(object sender, RoutedEventArgs e)
        {
            Class selectedClass = (Class)((TreeViewItem)tvClasslists.SelectedItem).Tag;

            UCETextListElem textElem = (UCETextListElem)sender;

            selectedClass.Modifiers.Remove((ItemModifier)textElem.Tag);
            spItemModifiers.Children.Remove(textElem);
        }

        private void bAddSpell_Click(object sender, RoutedEventArgs e)
        {
            if (spellSelector.SelectedTag != null)
            {
                Class selectedClass = (Class)((TreeViewItem)tvClasslists.SelectedItem).Tag;

                SpellModifier modif = new SpellModifier(spellSelector.SelectedTag.Name);
                selectedClass.AddModifier(modif);

                var textElem = CreateSpellModifierElem(modif);
                spSpellModifiers.Children.Add(textElem);
            }

            spellSelector.SelectByTag(null);
        }
    }
}
