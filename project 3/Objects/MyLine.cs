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
    public class MyLine : MyObject
    {
        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }

        public MyLine(Point startPoint, Point endPoint)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
        }

        public MyLine()
        {
            StartPoint = EndPoint = new Point(0, 0);
        }

        public void UpdateBoundaries()
        {
            MyBoundary.Reset();
            MyBoundary.UpdateBoundary(StartPoint.X, StartPoint.Y);
            MyBoundary.UpdateBoundary(EndPoint.X, EndPoint.Y);
        }

        public void DrawAndAddLine(WriteableBitmap wb, MyLine myLine, Color color)
        {
            Color = color;
            StartPoint = myLine.StartPoint;
            EndPoint = myLine.EndPoint;
            UpdateBoundaries();

            wb.DrawLine(myLine.StartPoint, myLine.EndPoint, color, Width);
        }

        public override void DrawObject(WriteableBitmap wb)
        {
            wb.DrawLine(StartPoint, EndPoint, Colors.Black, Width);
        }

    }
}
