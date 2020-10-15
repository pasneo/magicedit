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

    public delegate void OnMapPositionSelectionChangedDelegate(UCMapEditor mapEditor);


    /// <summary>
    /// Interaction logic for UCMapEditor.xaml
    /// </summary>
    public partial class UCMapEditor : UserControl
    {
        //private int rows = 12;
        //private int cols = 12;
        
        public int Rows
        {
            get { return Map.Height; }
            set { Map.Height = value; Redraw(); }
        }
        
        public int Cols
        {
            get { return Map.Width; }
            set { Map.Width = value; Redraw(); }
        }

        private const double DefaultCellSize = 40.0;
        private double CellSize = DefaultCellSize;

        private double translateX = 0.0;
        private double translateY = 0.0;

        //private int hoveredX = -1;
        //private int hoveredY = -1;

        //private int selectX = -1;
        //private int selectY = -1;
        //private MapObject selectedMapObject = null;

        public bool Multiselect { get; set; } = false;

        public HashSet<MapObject> SelectedMapObjects = new HashSet<MapObject>();
        public Position HoveredPosition { get; set; }
        public HashSet<Position> SelectedPositions { get; set; } = new HashSet<Position>();

        public event OnMapPositionSelectionChangedDelegate OnMapPositionSelectionChanged;

        private Map Map
        {
            get
            {
                return Project.Current?.Config.Map;
            }
        }

        public UCMapEditor()
        {
            InitializeComponent();
            Redraw();
        }

        protected double TranslateX(double x) { return x + translateX; }
        protected double TranslateY(double y) { return y + translateY; }

        protected double TranslateMouseX(double mX) { return mX - translateX; }
        protected double TranslateMouseY(double mY) { return mY - translateY; }

        protected double GetWidth() { return Cols * CellSize; }
        protected double GetHeight() { return Rows * CellSize; }

        public void Redraw()
        {
            canvas.Children.Clear();

            foreach (var square in Map.Squares)
            {
                Image image = new Image();
                image.Width = image.Height = CellSize;
                image.Margin = new Thickness(TranslateX(square.Key.X * CellSize), TranslateY(square.Key.Y * CellSize), 0.0, 0.0);
                image.Source = DefaultResources.GetVisualImageOrDefault(square.Value.Visual);
                canvas.Children.Add(image);
            }

            foreach (var mapObject in Map.Objects)
            {
                if (mapObject.Position == null || !Map.IsPositionWithin(mapObject.Position)) continue;

                Image image = new Image();
                image.Width = image.Height = CellSize;
                image.Margin = new Thickness(TranslateX(mapObject.Position.X * CellSize), TranslateY(mapObject.Position.Y * CellSize), 0.0, 0.0);
                image.Source = DefaultResources.GetVisualImageOrDefault(mapObject.Visual);
                canvas.Children.Add(image);
            }

            for (int row = 0; row <= Rows; ++row)
            {
                Line line = new Line();
                line.Stroke = Brushes.Black;
                line.Y1 = line.Y2 = TranslateY(row * CellSize);
                line.X1 = TranslateX(0);
                line.X2 = TranslateX(Cols * CellSize);
                canvas.Children.Add(line);
            }

            for (int col = 0; col <= Cols; ++col)
            {
                Line line = new Line();
                line.Stroke = Brushes.Black;
                line.X1 = line.X2 = TranslateX(col * CellSize);
                line.Y1 = TranslateY(0);
                line.Y2 = TranslateY(Rows * CellSize);
                canvas.Children.Add(line);
            }

            foreach(Position spawner in Map.SpawnerPositions)
            {
                Image spawnerSymbol = new Image();
                spawnerSymbol.Width = spawnerSymbol.Height = CellSize / 3;
                spawnerSymbol.Source = DefaultResources.PlusSymbolImage;
                spawnerSymbol.Margin = new Thickness(TranslateX(spawner.X * CellSize) + 4.0, TranslateY(spawner.Y * CellSize) + 4.0, 0.0, 0.0);
                canvas.Children.Add(spawnerSymbol);
            }

            foreach (Position selectedPosition in SelectedPositions)
            {
                Rectangle rect = new Rectangle();
                rect.Stroke = Brushes.GreenYellow;
                rect.StrokeThickness = 3.0;
                rect.Margin = new Thickness(TranslateX(selectedPosition.X * CellSize) + 4.0, TranslateY(selectedPosition.Y * CellSize) + 4.0, 0.0, 0.0);
                rect.Width = rect.Height = CellSize - 8.0;
                canvas.Children.Add(rect);
            }

            if (HoveredPosition != null)
            {
                Rectangle rect = new Rectangle();
                rect.Stroke = Brushes.DarkCyan;
                rect.Margin = new Thickness(TranslateX(HoveredPosition.X * CellSize) + 4.0, TranslateY(HoveredPosition.Y * CellSize) + 4.0, 0.0, 0.0);
                rect.Width = rect.Height = CellSize - 8.0;
                canvas.Children.Add(rect);
            }

            canvas.InvalidateVisual();
        }

        private void CheckMouseHover(double mX, double mY)
        {
            double tmX = TranslateMouseX(mX);
            double tmY = TranslateMouseY(mY);

            Position newHoveredPosition = null;

            if (tmX > 0 && tmX < GetWidth() && tmY > 0 && tmY < GetHeight())
            {
                int newHoveredX = (int)(tmX / CellSize);
                int newHoveredY = (int)(tmY / CellSize);
                newHoveredPosition = new Position(newHoveredX, newHoveredY);
            }

            bool redraw = false;

            if (HoveredPosition == null || !HoveredPosition.Equals(newHoveredPosition))
                redraw = true;

            HoveredPosition = newHoveredPosition;

            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                SelectHoveredPosition(true);
            }

            if (redraw) Redraw();

        }

        private bool pan = false;
        private double panX, panY;  //Position of mouse's drag point when panning

        public HashSet<MapObject> GetSelectedMapObjects()
        {

            HashSet<MapObject> selectedMapObjects = new HashSet<MapObject>();

            foreach (MapObject mapObject in Map.Objects)
            {
                foreach (var selectedPosition in SelectedPositions)
                {
                    if (mapObject.Position != null && mapObject.Position.Equals(selectedPosition))
                    {
                        selectedMapObjects.Add(mapObject);
                        break;
                    }
                }
            }
            return selectedMapObjects;
        }

        private void CheckMapObjectSelection()
        {
            SelectedMapObjects = GetSelectedMapObjects();
        }

        // selects the hovered position.
        private void SelectHoveredPosition(bool drag)
        {
            Multiselect = Keyboard.IsKeyDown(Key.LeftCtrl);

            /****************/

            if (Multiselect)
            {
                if (HoveredPosition != null && Map.IsPositionWithin(HoveredPosition))
                {
                    if (!drag)
                    {
                        if (SelectedPositions.RemoveWhere(pos => pos.Equals(HoveredPosition)) == 0)
                        {
                            SelectedPositions.Add(HoveredPosition);
                        }
                    }
                    else
                    {
                        SelectedPositions.Add(HoveredPosition);
                    }
                }
            }
            else
            {
                bool positionAlreadySelected = SelectedPositions.Where(pos => pos.Equals(HoveredPosition)).Count() > 0;
                bool multiplePositionsSelected = SelectedPositions.Count > 1;

                SelectedPositions.Clear();
                if (HoveredPosition != null && Map.IsPositionWithin(HoveredPosition) && (drag || !positionAlreadySelected || multiplePositionsSelected))
                {
                    SelectedPositions.Add(HoveredPosition);
                }
            }

            /****************/

            CheckMapObjectSelection();

            OnMapPositionSelectionChanged?.Invoke(this);

            Redraw();
        }

        public void CheckSelection()
        {
            //check if any selected position is now out of map
            SelectedPositions.RemoveWhere(pos => !Map.IsPositionWithin(pos));
            CheckMapObjectSelection();
            OnMapPositionSelectionChanged?.Invoke(this);
            Redraw();
        }

        private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                pan = true;
                panX = e.GetPosition(this).X;
                panY = e.GetPosition(this).Y;
                canvas.CaptureMouse();
                e.Handled = true;
            }
            else if (e.ChangedButton == MouseButton.Left)
            {
                SelectHoveredPosition(false);
                e.Handled = true;
            }
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {

            double mX = e.GetPosition(this).X;
            double mY = e.GetPosition(this).Y;

            if (pan)
            {
                translateX += (mX - panX);
                translateY += (mY - panY);

                panX = mX;
                panY = mY;

                Redraw();
                e.Handled = true;
            }
            else
            {
                CheckMouseHover(mX, mY);
                e.Handled = true;
            }
        }

        private void mNewObject_Click(object sender, RoutedEventArgs e)
        {
            /*NewObjectWindow newObjectWindow = new NewObjectWindow();

            if (newObjectWindow.ShowDialog().Value)
            {
                MapObject mapObject = newObjectWindow.MapObject;
                mapObject.X = selectX;
                mapObject.Y = selectY;
                map.Objects.Add(mapObject);
                Redraw();
            }*/

        }

        private void mDeleteObject_Click(object sender, RoutedEventArgs e)
        {
            //map.Objects.Remove(selectedMapObject);
            //Redraw();
        }

        private void UserControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {

            double OldCellSize = CellSize;

            //MoX & MoY are used for setting translateX & translateY so that the position under mouse stays the same in new zoom
            double MoX = (e.GetPosition(canvas).X - translateX);
            double MoY = (e.GetPosition(canvas).Y - translateY);

            if (e.Delta > 0)
                CellSize *= 1.3;
            else
                CellSize /= 1.3;

            if (CellSize < 20.0) CellSize = 20.0;
            else if (CellSize > 200.0) CellSize = 200.0;

            double NewMoX = MoX * (CellSize / OldCellSize);
            double NewMoY = MoY * (CellSize / OldCellSize);

            translateX -= (NewMoX - MoX);
            translateY -= (NewMoY - MoY);

            Redraw();
        }

        private void canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                pan = false;
                //ReleaseMouseCapture();
                e.Handled = true;
                canvas.InvalidateVisual();
            }
            canvas.ReleaseMouseCapture();
        }
    }
}
