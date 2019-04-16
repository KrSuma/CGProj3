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
    public class MyCircle : MyObject
    {
        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }
        public int distance { get; set; }

        public MyCircle(Point startPoint, Point endPoint)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
            distance = 0;
        }

        public MyCircle()
        {
            StartPoint = EndPoint = new Point(0, 0);
            distance = 0;
        }

        public void UpdateBoundaries()
        {
            MyBoundary.Reset();
            MyBoundary.UpdateBoundary(StartPoint.X, StartPoint.Y);
            MyBoundary.UpdateBoundary(EndPoint.X, EndPoint.Y);
        }

        public void DrawAndAddCircle(WriteableBitmap wb, MyCircle myCircle, Color color)
        {
            Color = color;
            StartPoint = myCircle.StartPoint;
            EndPoint = myCircle.EndPoint;

            int distance = (int)Point.Subtract(EndPoint, StartPoint).Length;

            UpdateBoundaries();

            wb.DrawCircle(myCircle.StartPoint, distance, color);
        }

        public override void DrawObject(WriteableBitmap wb)
        {
            wb.DrawCircle(StartPoint, distance, Colors.Black);
        }
    }
}
