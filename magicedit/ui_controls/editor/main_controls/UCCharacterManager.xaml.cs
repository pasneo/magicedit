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
    /// Interaction logic for UCCharacterManager.xaml
    /// </summary>
    public partial class UCCharacterManager : MainUserControl
    {
        public UCCharacterManager()
        {
            InitializeComponent();
        }

        public override void Open()
        {
            RefreshLists();
            slotSchemeSelector.Refresh();

            iAbilityPoints.NumValue = Project.Current.Config.CharacterConfig.StartingAbilityPoints;
            iActionPoints.NumValue = Project.Current.Config.CharacterConfig.ActionPoints;
            iMovementCost.NumValue = Project.Current.Config.CharacterConfig.MovementActionPoints;
        }

        private void RefreshLists()
        {
            tlAbilities.ClearList();

            foreach (var ability in Project.Current.Config.CharacterConfig.Abilities)
            {
                tlAbilities.AddTextToList(ability.Name);
            }

            spSlots.Children.Clear();

            foreach (var slot in Project.Current.Config.CharacterConfig.InventorySlots)
            {
                var elem = CreateSlotElem(slot);
                spSlots.Children.Add(elem);
            }
        }

        private UCETextListElem CreateSlotElem(ObjectVariable slot)
        {
            UCETextListElem elem = new UCETextListElem();
            elem.Content = $"{slot.Name} [{slot.Type}]";
            elem.Tag = slot;
            elem.DeleteClicked += SlotElem_DeleteClicked;
            return elem;
        }

        private void SlotElem_DeleteClicked(object sender, RoutedEventArgs e)
        {
            UCETextListElem elem = (UCETextListElem)sender;
            Project.Current.Config.CharacterConfig.InventorySlots.Remove((ObjectVariable)elem.Tag);
            spSlots.Children.Remove(elem);
        }

        private void bAddItem_Click(object sender, RoutedEventArgs e)
        {
            if (tbSlotName.Text == null || tbSlotName.Text.Length == 0 || slotSchemeSelector.SelectedTag == null) return;

            string slotType = slotSchemeSelector.SelectedTag.Name;
            string slotName = tbSlotName.Text;

            ObjectVariable slot = new ObjectVariable(slotType, slotName, null);

            Project.Current.Config.CharacterConfig.InventorySlots.Add(slot);

            var elem = CreateSlotElem(slot);
            spSlots.Children.Add(elem);
        }

        private void tlAbilities_TextAdded(UCETextListElem elem)
        {
            Project.Current.Config.CharacterConfig.Abilities.Add(new ObjectVariable(VariableTypes.Ability, elem.Content, 0));
        }

        private void tlAbilities_TextDeleted(UCETextListElem elem)
        {
            Project.Current.Config.CharacterConfig.Abilities.RemoveAll(ab => ab.Name == elem.Content);
        }

        private void iAbilityPoints_ValueChanged(IntegerUpDown sender)
        {
            Project.Current.Config.CharacterConfig.StartingAbilityPoints = iAbilityPoints.NumValue;
        }

        private void iActionPoints_ValueChanged(IntegerUpDown sender)
        {
            Project.Current.Config.CharacterConfig.ActionPoints = iActionPoints.NumValue;
        }

        private void iMovementCost_ValueChanged(IntegerUpDown sender)
        {
            Project.Current.Config.CharacterConfig.MovementActionPoints = iMovementCost.NumValue;
        }
    }
}
