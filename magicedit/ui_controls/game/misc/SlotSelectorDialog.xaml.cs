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
using System.Windows.Shapes;

namespace magicedit
{
    /// <summary>
    /// Interaction logic for SlotSelectorDialog.xaml
    /// </summary>
    public partial class SlotSelectorDialog : Window
    {

        public string SelectedSlot { get; set; }

        public SlotSelectorDialog(Object item, Game game)
        {
            InitializeComponent();

            ComboBoxItem bagItem = new ComboBoxItem();
            bagItem.Content = "Bag";
            bagItem.Tag = SpecialSlots.Bag;
            cb.Items.Add(bagItem);

            foreach(var slot in game.Config.CharacterConfig.InventorySlots)
            {
                if (SchemeExecutor.CheckTypeCompatibility(slot.Type, item.Scheme?.Name, game.Config))
                {
                    ComboBoxItem cbItem = new ComboBoxItem();
                    cbItem.Content = slot.Name;
                    cbItem.Tag = slot.Name;
                    cb.Items.Add(cbItem);

                    if (game.CurrentPlayer.Character.GetItemBySlot(slot.Name, game.Config) == item) cbItem.IsSelected = true;
                }
            }

            if (cb.SelectedItem == null) bagItem.IsSelected = true;

        }

        private void bMove_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (ComboBoxItem)cb.SelectedItem;

            if (item == null) SelectedSlot = null;
            else SelectedSlot = (string)item.Tag;
        }
    }
}
