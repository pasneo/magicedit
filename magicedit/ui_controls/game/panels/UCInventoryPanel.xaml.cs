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

        public Game Game
        {
            get { return Project.Current?.Game; }
        }

        Character _character;
        Character Character {
            get { return _character; }
            set
            {
                _character = value;
                Refresh();
            }
        }

        public UCInventoryPanel()
        {
            InitializeComponent();
        }

        /* *** */

        public void Refresh()
        {
            if (Character == null) return;

            List<Object> slotItems = new List<Object>();

            foreach(var slot in Game.Config.CharacterConfig.InventorySlots)
            {
                string slotName = slot.Name;
                var slotItem = Character.GetItemBySlot(slotName, Game.Config);

                if (slotItem != null)
                {
                    slotItems.Add(slotItem);
                    spSlotItems.Children.Add(new UCEItemRow(slotItem));
                }

            }

            foreach(Object item in Character.Items)
            {
                if (!slotItems.Contains(item))
                {
                    spBagItems.Children.Add(new UCEItemRow(item));
                }
            }

        }

    }
}
