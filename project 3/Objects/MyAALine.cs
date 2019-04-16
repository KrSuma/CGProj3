using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace project_3.Objects
{
    public class MyAALine : MyObject
    {
        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }
        public int colorCodeWithAlpha { get; set; }

        public MyAALine(Point startPoint, Point endPoint)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
        }

        public MyAALine()
        {
            StartPoint = EndPoint = new Point(0, 0);
        }

        public void UpdateBoundaries()
        {
            MyBoundary.Reset();
            MyBoundary.UpdateBoundary(StartPoint.X, StartPoint.Y);
            MyBoundary.UpdateBoundary(EndPoint.X, EndPoint.Y);
        }

        public int ConvertColortoInt(Color color)
        {
            return colorCodeWithAlpha = BitConverter.ToInt32(new byte[] { color.B, color.G, color.R, color.A }, 0);
        }

        public void DrawAndAddAALine(WriteableBitmap wb, MyAALine myAALine, Color color)
        {
            Color = color;
            StartPoint = myAALine.StartPoint;
            EndPoint = myAALine.EndPoint;
            UpdateBoundaries();
            int colorCodeWithAlpha = BitConverter.ToInt32(new byte[] { color.B, color.G, color.R, color.A }, 0);

            //wb.DrawLine(myLine.StartPoint, myLine.EndPoint, color, Width);
            wb.DrawAALine(wb.PixelWidth, wb.PixelHeight, StartPoint, EndPoint, color, Width);
        }

        public override void DrawObject(WriteableBitmap wb)
        {
            wb.DrawAALine(wb.PixelWidth, wb.PixelHeight, StartPoint, EndPoint, Colors.Black, Width);
        }

    }
}