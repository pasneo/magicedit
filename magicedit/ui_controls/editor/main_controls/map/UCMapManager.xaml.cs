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
    /// Interaction logic for UCMapManager.xaml
    /// </summary>
    public partial class UCMapManager : MainUserControl
    {
        
        // used when setting square type for multiple squares. 'Set' means all selected squares should be set to the specific type.
        private enum SquareTypeSetEnum
        {
            None,
            Set,
            Remove
        }

        private Config Config
        {
            get { return Project.Current.Config; }
        }

        private Map Map
        {
            get { return Config.Map; }
        }

        public UCMapManager()
        {

            InitializeComponent();

            mapEditor.OnMapPositionSelectionChanged += MapEditor_OnMapPositionSelectionChanged;
        }

        private void MapEditor_OnMapPositionSelectionChanged(UCMapEditor mapEditor)
        {
            RefreshSquareTypeSelector();
            RefreshSpawnerIndicator();
            RefreshMapObjectsPanel();
        }

        private void RefreshSquareTypeSelector()
        {
            var selectedPositions = mapEditor.SelectedPositions;

            if (selectedPositions.Count == 1)
            {
                SquareType sqType = Map.GetSquareTypeAt(selectedPositions.FirstOrDefault());
                squareTypeSelector.SelectBySquareType(sqType);
            }
            else
            {
                bool wasFirst = false;
                SquareType firstType = null;

                foreach(var pos in selectedPositions)
                {
                    SquareType type = Map.GetSquareTypeAt(pos);
                    if (!wasFirst)
                    {
                        firstType = type;
                        wasFirst = true;
                    }
                    else if (firstType != type)
                    {
                        squareTypeSelector.DeselectAll();
                        return;
                    }
                }

                squareTypeSelector.SelectBySquareType(firstType);

            }
        }

        private void RefreshSpawnerIndicator()
        {
            var selectedPositions = mapEditor.SelectedPositions;

            if (selectedPositions.Count != 1)
            {
                cbSpawner.IsChecked = false;
                cbSpawner.IsEnabled = false;
            }
            else
            {
                Position selectedPosition = selectedPositions.FirstOrDefault();
                cbSpawner.IsChecked = Map.HasSpawnerAt(selectedPosition);
                cbSpawner.IsEnabled = true;
            }

        }

        private void RefreshMapObjectsPanel()
        {
            spObjects.Children.Clear();

            if (mapEditor.SelectedPositions.Count != 1) return;

            var selectedPos = mapEditor.SelectedPositions.FirstOrDefault();

            foreach (var obj in Config.Map.Objects)
            {
                UCEObjectRow objectRow = new UCEObjectRow(obj, obj.Position != null && obj.Position.Equals(selectedPos));
                objectRow.CheckChanged += ObjectRow_CheckChanged;
                spObjects.Children.Add(objectRow);
            }
        }

        private void ObjectRow_CheckChanged(UCEObjectRow row)
        {
            if (row.Checked)
            {
                //set obj's position to selected one
                MapObject mo = (MapObject)row.Tag;
                mo.Position = mapEditor.SelectedPositions.FirstOrDefault();
            }
            else
            {
                //set obj's position to null
                MapObject mo = (MapObject)row.Tag;
                mo.Position = null;
            }
            mapEditor.Redraw();
        }

        public override void Open(EditorErrorDescriptor eed)
        {
            Config.Map.RecollectMapObjects(Config.Objects);

            //todo: check if objects or square types have been deleted (in this case remove from map these objects and squares)
            squareTypeSelector.RefreshList();
            mapEditor.Redraw();

            nMapWidth.NumValue = Map.Width;
            nMapHeight.NumValue = Map.Height;

            RefreshMapObjectsPanel();
        }

        private void squareTypeSelector_OnSquareTypeSelected(SquareType selectedSquareType)
        {

            SquareTypeSetEnum setType = SquareTypeSetEnum.None;

            var selectedPositions = mapEditor.SelectedPositions;
            foreach(var pos in selectedPositions)
            {
                // based on the first selected pos, if it already has given square type, we remove any type from each pos. else we set type for each.
                if (setType == SquareTypeSetEnum.None)
                {
                    if (Map.GetSquareTypeAt(pos) == selectedSquareType) setType = SquareTypeSetEnum.Remove;
                    else setType = SquareTypeSetEnum.Set;
                }
                if (setType == SquareTypeSetEnum.Set)
                {
                    Map.SetSquareTypeAt(pos, selectedSquareType);
                }
                else
                {
                    Map.RemoveSquareTypeAt(pos);
                }
            }

            RefreshSquareTypeSelector();
            mapEditor.Redraw();
        }

        private void cbSpawner_CheckChanged(object sender, RoutedEventArgs e)
        {
            var selectedPositions = mapEditor.SelectedPositions;

            if (selectedPositions == null || selectedPositions.Count != 1)
            {
                cbSpawner.IsEnabled = false;
            }
            else
            {
                var selectedPosition = selectedPositions.FirstOrDefault();
                if (cbSpawner.IsChecked.HasValue && cbSpawner.IsChecked.Value) Map.AddSpawner(selectedPosition);
                else Map.RemoveSpawnerAt(selectedPosition);
                mapEditor.Redraw();
            }

        }

        private void nMapSize_ValueChanged(IntegerUpDown sender)
        {
            if (nMapWidth != null) Map.Width = nMapWidth.NumValue;
            if (nMapWidth != null) Map.Height = nMapHeight.NumValue;
            mapEditor.CheckSelection();
        }

        private void hySquareTypes_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Current.SelectMenu(MainWindow.Menus.SquareTypes);
        }
    }
}
