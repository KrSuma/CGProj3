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


using project_3.Objects;

namespace project_3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MyObject _temporaryObject;

        private WriteableBitmap _wb;

        public int LineWidthValue { get; set; }
        public int colorCodeWithAlpha { get; set; }

        public Color BackgroundColor { get; set; } = Color.FromRgb(255, 255, 255);

        private Point _lastPoint;
        private Point _firstPoint;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public bool DrawingLine { get; set; }
        public bool DrawingCircle { get; set; }
        public bool DrawingAALine { get; set; }

        private void DrawingType_Checked(object sender, RoutedEventArgs e)
        {
            var button = sender as RadioButton;

            if (button != null)
            {
                switch (button.Name)
                {
                    case "LineRadioButton":
                        DrawingLine = true;
                        DrawingCircle = false;
                        DrawingAALine = false;
                        break;
                    case "CircleRadioButton":
                        DrawingCircle = true;
                        DrawingLine = false;
                        DrawingAALine = false;
                        break;
                    case "AALineRadioButton":
                        DrawingAALine = true;
                        DrawingLine = false;
                        DrawingCircle = false;
                        break;
                }
            }
        }

        private void LineWidth_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            LineWidthValue = (int)e.NewValue;
        }

        private void RedrawObject(MyObject myObject)
        {
            if(myObject is MyLine)
            {
                _wb.DrawLine(((MyLine)myObject).StartPoint, ((MyLine)myObject).EndPoint, Colors.Black,
                        myObject.Width);
            }
            else if (myObject is MyCircle) 
            {
                _wb.DrawCircle(((MyCircle)myObject).StartPoint, myObject.Width, Colors.Black);
            }
            else if (myObject is MyAALine)
            {
                //_wb.DrawAALine(_wb.PixelHeight, _wb.PixelWidth,((MyAALine)myObject).StartPoint,((MyAALine)myObject).EndPoint, ConvertColortoInt(Colors.Black));
                _wb.DrawAALine(_wb.PixelHeight, _wb.PixelWidth,((MyAALine)myObject).StartPoint,((MyAALine)myObject).EndPoint, Colors.Black, myObject.Width);
            }
        }

        //MyImage is the canvas
        //e would be the mouse position
        private void MyImage_ButtonDown(object sender, MouseButtonEventArgs e) 
        {
            Point p = e.GetPosition(MyImage);

            if(DrawingLine)
            {
                _temporaryObject = new MyLine();
                _temporaryObject.Color = Colors.Black;
                _temporaryObject.Width = LineWidthValue;
            }
            else if (DrawingCircle)
            {
                _temporaryObject = new MyCircle();
                _temporaryObject.Color = Colors.Black;
                _temporaryObject.Width = 0;
            }
            else if (DrawingAALine)
            {
                _temporaryObject = new MyAALine();
                _temporaryObject.Color = Colors.Black;
                _temporaryObject.Width = LineWidthValue;
            }
            _firstPoint = _lastPoint = p;
        }

        private void MyImage_ButtonUp(object sender, MouseButtonEventArgs e)
        {
            Point point = e.GetPosition(MyImage);

            if(DrawingLine)
            {
                if(point != _firstPoint)
                {
                    ((MyLine)_temporaryObject).DrawAndAddLine(_wb, new MyLine(_lastPoint, point), _temporaryObject.Color);
                }
            }
            else if (DrawingCircle)
            {
                if(point != _firstPoint)
                {
                    ((MyCircle)_temporaryObject).DrawAndAddCircle(_wb, new MyCircle(_lastPoint, point), _temporaryObject.Color);
                }
            }
            else if (DrawingAALine)
            {
                if (point != _firstPoint)
                {
                    ((MyAALine)_temporaryObject).DrawAndAddAALine(_wb, new MyAALine(_lastPoint, point), _temporaryObject.Color);
                }
            }
        }

        private void MyImage_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {


        }

        private void MyImage_OnMouseMove(object sender, MouseEventArgs e)
        {
            Point p = e.GetPosition(MyImage);

            if (e.LeftButton == MouseButtonState.Pressed && DrawingLine || DrawingCircle)
            {
                RedrawObject(_temporaryObject);
            }

        }

        private void ImageGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _wb = new WriteableBitmap((int)e.NewSize.Width, (int)e.NewSize.Height, 96, 96, PixelFormats.Bgra32, null);
            MyImage.Source = _wb;

            _wb.Clear(BackgroundColor);
        }

        private int ConvertColortoInt(Color color)
        {
            return colorCodeWithAlpha = BitConverter.ToInt32(new byte[] { color.B, color.G, color.R, color.A }, 0);
        }



    }

}
