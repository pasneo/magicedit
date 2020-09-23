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
    /// Interaction logic for UCMapEditor.xaml
    /// </summary>
    public partial class UCMapEditor : UserControl
    {
        private int rows = 12;
        private int cols = 12;
        
        public int Rows
        {
            get { return rows; }
            set { rows = value; Redraw(); }
        }
        
        public int Cols
        {
            get { return cols; }
            set { cols = value; Redraw(); }
        }

        private const double DefaultCellSize = 40.0;
        private double CellSize = DefaultCellSize;

        private double translateX = 0.0;
        private double translateY = 0.0;

        private int hoveredX = -1;
        private int hoveredY = -1;

        private int selectX = -1;
        private int selectY = -1;
        private MapObject selectedMapObject = null;


        public Position HoveredPosition { get; set; }
        public List<Position> SelectedPositions { get; set; } = new List<Position>();


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

        protected double GetWidth() { return cols * CellSize; }
        protected double GetHeight() { return rows * CellSize; }

        protected void Redraw()
        {
            canvas.Children.Clear();

            foreach (var mapObject in Map.Objects)
            {
                Image image = new Image();
                image.Width = image.Height = CellSize;
                image.Margin = new Thickness(TranslateX(mapObject.Position.X * CellSize), TranslateY(mapObject.Position.Y * CellSize), 0.0, 0.0);
                image.Source = DefaultResources.GetVisualImageOrDefault(mapObject.Visual);
                canvas.Children.Add(image);
            }

            for (int row = 0; row <= rows; ++row)
            {
                Line line = new Line();
                line.Stroke = Brushes.Black;
                line.Y1 = line.Y2 = TranslateY(row * CellSize);
                line.X1 = TranslateX(0);
                line.X2 = TranslateX(cols * CellSize);
                canvas.Children.Add(line);
            }

            for (int col = 0; col <= cols; ++col)
            {
                Line line = new Line();
                line.Stroke = Brushes.Black;
                line.X1 = line.X2 = TranslateX(col * CellSize);
                line.Y1 = TranslateY(0);
                line.Y2 = TranslateY(rows * CellSize);
                canvas.Children.Add(line);
            }

            if (selectX != -1)
            {
                Rectangle rect = new Rectangle();
                rect.Stroke = Brushes.GreenYellow;
                rect.StrokeThickness = 3.0;
                rect.Margin = new Thickness(TranslateX(selectX * CellSize) + 4.0, TranslateY(selectY * CellSize) + 4.0, 0.0, 0.0);
                rect.Width = rect.Height = CellSize - 8.0;
                canvas.Children.Add(rect);
            }

            if (hoveredX != -1)
            {
                Rectangle rect = new Rectangle();
                rect.Stroke = Brushes.DarkCyan;
                rect.Margin = new Thickness(TranslateX(hoveredX * CellSize) + 4.0, TranslateY(hoveredY * CellSize) + 4.0, 0.0, 0.0);
                rect.Width = rect.Height = CellSize - 8.0;
                canvas.Children.Add(rect);
            }

            canvas.InvalidateVisual();
        }

        private void CheckMouseHover(double mX, double mY)
        {

            double tmX = TranslateMouseX(mX);
            double tmY = TranslateMouseY(mY);

            int newHoveredX = -1;
            int newHoveredY = -1;

            if (tmX > 0 && tmX < GetWidth() && tmY > 0 && tmY < GetHeight())
            {
                newHoveredX = (int)(tmX / CellSize);
                newHoveredY = (int)(tmY / CellSize);
            }

            bool redraw = false;

            if (newHoveredX != hoveredX || newHoveredY != hoveredY)
                redraw = true;

            hoveredX = newHoveredX;
            hoveredY = newHoveredY;

            if (redraw) Redraw();

        }

        private bool pan = false;
        private double panX, panY;  //Position of mouse's drag point when panning

        public MapObject GetSelectedMapObject()
        {
            foreach (MapObject mapObject in Map.Objects)
            {
                if (mapObject.Position.X == selectX && mapObject.Position.Y == selectY) return mapObject;
            }
            return null;
        }

        private void CheckMapObjectSelection()
        {
            if (selectX == -1 || selectY == -1)
                selectedMapObject = null;
            else
                selectedMapObject = GetSelectedMapObject();
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
                selectX = hoveredX;
                selectY = hoveredY;

                CheckMapObjectSelection();

                if (selectX != -1)
                {
                    //mNewObject.IsEnabled = true;
                    //if (selectedMapObject != null) mDeleteObject.IsEnabled = true;
                    //else mDeleteObject.IsEnabled = false;
                }
                else
                {
                    //mNewObject.IsEnabled = mDeleteObject.IsEnabled = false;
                }

                Redraw();
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
