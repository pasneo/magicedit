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
    public partial class UCSpellPanel : UserControl
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

        public UCSpellPanel()
        {
            InitializeComponent();
        }

        /* *** */

        public void Refresh()
        {
            spSpells.Children.Clear();

            if (Character == null) return;

            foreach (Object spell in Character.Spells)
            {
                UCEItemRow row = new UCEItemRow(spell, null, false);
                row.OnSelected += ItemRow_OnSelected;
                spSpells.Children.Add(row);
            }

        }

        public void DeselectAll()
        {
            SelectedItemRow?.Deselect();
            SelectedItemRow = null;
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
