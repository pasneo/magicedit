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
    /// Interaction logic for UCInventoryPanel.xaml
    /// </summary>
    public partial class UCInventoryPanel : UserControl
    {

        public event UCEItemRow.OnSelectedDelegate OnItemSelected;

        public Game Game
        {
            get { return Project.Current?.Game; }
        }

        private Character _character;
        public Character Character {
            get { return _character; }
            set
            {
                _character = value;
                Refresh();
            }
        }

        private UCEItemRow SelectedItemRow { get; set; }

        public UCInventoryPanel()
        {
            InitializeComponent();
        }

        /* *** */

        public void Refresh()
        {
            spSlotItems.Children.Clear();
            spBagItems.Children.Clear();

            if (Character == null) return;

            List<Object> slotItems = new List<Object>();

            foreach(var slot in Game.Config.CharacterConfig.InventorySlots)
            {
                string slotName = slot.Name;
                var slotItem = Character.GetItemBySlot(slotName, Game.Config);

                if (slotItem != null)
                {
                    slotItems.Add(slotItem);
                    UCEItemRow row = new UCEItemRow(slotItem, slotName);
                    row.OnSelected += ItemRow_OnSelected;
                    row.OnMoveClicked += ItemRow_OnMoveClicked;
                    spSlotItems.Children.Add(row);
                }

            }

            foreach(Object item in Character.Items)
            {
                if (!slotItems.Contains(item))
                {
                    UCEItemRow row = new UCEItemRow(item);
                    row.OnSelected += ItemRow_OnSelected;
                    row.OnMoveClicked += ItemRow_OnMoveClicked;
                    spBagItems.Children.Add(row);
                }
            }

        }

        private void ItemRow_OnMoveClicked(UCEItemRow itemRow)
        {
            var slotSelectorDialog = new SlotSelectorDialog(itemRow.Item, Game);

            if (slotSelectorDialog.ShowDialog() == true)
            {
                if (slotSelectorDialog.SelectedSlot == SpecialSlots.Bag)
                {
                    Game.CurrentPlayer.Character.MoveItemToBag(itemRow.Item, Game.Config);
                }
                else
                    Game.CurrentPlayer.Character.MoveItemToSlot(itemRow.Item, slotSelectorDialog.SelectedSlot, Game.Config);

                Refresh();
            }

        }

        private void ItemRow_OnSelected(UCEItemRow itemRow)
        {
            if (itemRow.Selected)
            {
                SelectedItemRow?.Deselect();
                SelectedItemRow = itemRow;
                OnItemSelected?.Invoke(itemRow);
            }
            else
            {
                SelectedItemRow = null;
                OnItemSelected?.Invoke(null);
            }

        }
    }
}
