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
    // static class cannot be instantiated
    public static class BitmapExtensions
    {
        //public static void DrawLine(this WriteableBitmap wb, Point startPoint, Point endPoint, Color c, int radius)
        //{
        //    int x1 = (int)startPoint.X;
        //    int y1 = (int)startPoint.Y;
        //    int x2 = (int)endPoint.X;
        //    int y2 = (int)endPoint.Y;

        //    wb.DrawLine(x1, y1, x2, y2, c); // draw 'core' line;

        //    var steep = Math.Abs(y2 - y1) > Math.Abs(x2 - x1); // delta(y) > delta(x);

        //    if (steep && radius > 0)
        //    {
        //        for (var i = -radius; i <= radius; i++)
        //        {
        //            wb.DrawLine(x1 + i, y1, x2 + i, y2, c);
        //        }
        //    }
        //    else
        //    {
        //        for (var i = -radius; i <= radius; i++)
        //        {
        //            wb.DrawLine(x1, y1 + i, x2, y2 + i, c);
        //        }
        //    }
        //}

        public static int colorCodeWithAlpha { get; set; }
        private static int ConvertColortoInt(Color color)
        {
            return colorCodeWithAlpha = BitConverter.ToInt32(new byte[] { color.B, color.G, color.R, color.A }, 0);
        }

        //MIDPOINT CIRCLE ALGORITHM

        public static void DrawCircle(this WriteableBitmap image, Point startPoint, int radius, Color color)
        {
            //int centerX, int centerY
            int centerX = (int)startPoint.X;
            int centerY = (int)startPoint.Y;

            //
            int d = (5 - radius * 4) / 4;
            int x = 0;
            int y = radius;

            do
            {
                //w check if the pixel location is within the bounds of the image before setting the pixel
                //8 quadrants (octets)
                if (centerX + x >= 0 && centerX + x <= image.Width - 1 && centerY + y >= 0 && centerY + y <= image.Height - 1)
                    image.SetPixel(centerX + x, centerY + y, color);
                if (centerX + x >= 0 && centerX + x <= image.Width - 1 && centerY - y >= 0 && centerY - y <= image.Height - 1)
                    image.SetPixel(centerX + x, centerY - y, color);
                if (centerX - x >= 0 && centerX - x <= image.Width - 1 && centerY + y >= 0 && centerY + y <= image.Height - 1)
                    image.SetPixel(centerX - x, centerY + y, color);
                if (centerX - x >= 0 && centerX - x <= image.Width - 1 && centerY - y >= 0 && centerY - y <= image.Height - 1)
                    image.SetPixel(centerX - x, centerY - y, color);
                if (centerX + y >= 0 && centerX + y <= image.Width - 1 && centerY + x >= 0 && centerY + x <= image.Height - 1)
                    image.SetPixel(centerX + y, centerY + x, color);
                if (centerX + y >= 0 && centerX + y <= image.Width - 1 && centerY - x >= 0 && centerY - x <= image.Height - 1)
                    image.SetPixel(centerX + y, centerY - x, color);
                if (centerX - y >= 0 && centerX - y <= image.Width - 1 && centerY + x >= 0 && centerY + x <= image.Height - 1)
                    image.SetPixel(centerX - y, centerY + x, color);
                if (centerX - y >= 0 && centerX - y <= image.Width - 1 && centerY - x >= 0 && centerY - x <= image.Height - 1)
                    image.SetPixel(centerX - y, centerY - x, color);
                if (d < 0)
                {
                    d += 2 * x + 1;
                }
                else
                {
                    d += 2 * (x - y) + 1;
                    y--;
                }
                x++;
            } while (x <= y);
        }

        public static void DrawLine(this WriteableBitmap wb, Point startPoint, Point endPoint, Color color, int lineThickness)
        {
            int x1 = (int)startPoint.X;
            int y1 = (int)startPoint.Y;
            int x2 = (int)endPoint.X;
            int y2 = (int)endPoint.Y;

            //delta(y) > delta(x)?
            var steep = Math.Abs(y2 - y1) > Math.Abs(x2 - x1); 

            //gets all the points between the starting point and the endpoint.
            IEnumerable<Point> points = GetPoints(x1, y1, x2, y2);

            //and for each points we set the pixels depending on where the line crosses the middle point.
            if (steep)
            {
                foreach (var p in points)
                {
                    for (var i = -lineThickness; i <= lineThickness; i++)
                    {
                        SetPixel(wb, (int)p.X + i, (int)p.Y, color);
                    }
                }
            }
            else
            {
                foreach (var p in points)
                {
                    for (var i = -lineThickness; i <= lineThickness; i++)
                    {
                        SetPixel(wb, (int)p.X, (int)p.Y + i, color);
                    }
                }
            }
        }

        public static void SetPixel(this WriteableBitmap wb, int x, int y, Color color)
        {
            if (y > wb.PixelHeight - 1 || x > wb.PixelWidth - 1)
            {
                return;
            }

            if (y < 0 || x < 0)
            {
                return;
            }

            if (!wb.Format.Equals(PixelFormats.Bgra32))
            {
                return;
            }

            wb.Lock();
            IntPtr buff = wb.BackBuffer;
            int stride = wb.BackBufferStride;

            unsafe
            {
                byte* pbuff = (byte*)buff.ToPointer();
                int loc = y * stride + x * 4; 

                //set the color, although unnecessary if color is just black
                pbuff[loc] = color.B;
                pbuff[loc + 1] = color.G;
                pbuff[loc + 2] = color.R;
                pbuff[loc + 3] = color.A;
            }

            //inserts pixel values
            wb.AddDirtyRect(new Int32Rect(x, y, 1, 1));
            wb.Unlock();
        }

        public static IEnumerable<Point> GetPoints(int x0, int y0, int x1, int y1)
        {
            bool coefficient = Math.Abs(y1 - y0) > Math.Abs(x1 - x0); // delta(y) > delta(x);

            if (coefficient)
            {
                int t = x0; // swap x0 and y0
                x0 = y0;
                y0 = t;

                t = x1; // swap x1 and y1
                x1 = y1;
                y1 = t;
            }

            if (x0 > x1)
            {
                int t = x0; // swap x0 and x1
                x0 = x1;
                x1 = t;

                t = y0; // swap y0 and y1
                y0 = y1;
                y1 = t;
            }

            int dx = x1 - x0;
            int dy = Math.Abs(y1 - y0);
            int error = dx / 2;
            int ystep = (y0 < y1) ? 1 : -1;
            int y = y0;

            for (int x = x0; x <= x1; x++)
            {
                yield return new Point((coefficient ? y : x), (coefficient ? x : y));
                error -= dy;
                if (error < 0)
                {
                    y += ystep;
                    error += dx;
                }
            }
        }

        //referenced from http://nokola.com/blog/post/2010/10/14/Anti-aliased-Lines-And-Optimizing-Code-for-Windows-Phone-7e28093First-Look.aspx
        public static void DrawAALine(this WriteableBitmap wb, int pixelWidth, int pixelHeight, Point startPoint, Point endPoint, Color color, int lineThickness)
        {
            int x1 = (int)startPoint.X;
            int y1 = (int)startPoint.Y;
            int x2 = (int)endPoint.X;
            int y2 = (int)endPoint.Y;

            //var steep = Math.Abs(y2 - y1) > Math.Abs(x2 - x1);
            //IEnumerable<Point> points = GetPoints(x1, y1, x2, y2);

            if ((x1 == x2) && (y1 == y2)) return; // edge case causing invDFloat to overflow

            if (x1 < 1)
                x1 = 1;
            if (x1 > pixelWidth - 2)
                x1 = pixelWidth - 2;
            if (y1 < 1)
                y1 = 1;
            if (y1 > pixelHeight - 2)
                y1 = pixelHeight - 2;

            if (x2 < 1)
                x2 = 1;
            if (x2 > pixelWidth - 2)
                x2 = pixelWidth - 2;
            if (y2 < 1)
                y2 = 1;
            if (y2 > pixelHeight - 2)
                y2 = pixelHeight - 2;

            var addr = y1 * pixelWidth + x1;
            var addr2 = x1 * pixelHeight + y1;
            
            var dx = x2 - x1;
            var dy = y2 - y1;

            int du;
            int dv;
            int u;
            int v;
            int uincr;
            int vincr;

            // Extract color
            var a = (ConvertColortoInt(color) >> 24) & 0xFF;
            var srb = (uint)(ConvertColortoInt(color) & 0x00FF00FF);
            var sg = (uint)(ConvertColortoInt(color) & 0xFF);

            // By switching to (u,v), we combine all eight octants 
            int adx = dx, ady = dy;
            if (dx < 0) adx = -dx;
            if (dy < 0) ady = -dy;

            if (adx > ady)
            {
                du = adx;
                dv = ady;
                u = x2;
                v = y2;
                uincr = 1;
                vincr = pixelWidth;
                if (dx < 0) uincr = -uincr;
                if (dy < 0) vincr = -vincr;
            }
            else
            {
                du = ady;
                dv = adx;
                u = y2;
                v = x2;
                uincr = pixelWidth;
                vincr = 1;
                if (dy < 0) uincr = -uincr;
                if (dx < 0) vincr = -vincr;
            }

            var uend = u + du;
            var d = (dv << 1) - du;        // Initial value as in Bresenham's 
            var incrS = dv << 1;    //for straight increments 
            var incrD = (dv - du) << 1;    //for diagonal increments

            var invDFloat = 1.0 / (4.0 * Math.Sqrt(du * du + dv * dv));   // Precomputed inverse denominator 
            var invD2DuFloat = 0.75 - 2.0 * (du * invDFloat);   // Precomputed constant

            //color shifter that adds shades
            const int PRECISION_SHIFT = 10; 
            const int PRECISION_MULTIPLIER = 1 << PRECISION_SHIFT;
            var invD = (int)(invDFloat * PRECISION_MULTIPLIER);
            var invD2Du = (int)(invD2DuFloat * PRECISION_MULTIPLIER * a);
            var zeroDot75 = (int)(0.75 * PRECISION_MULTIPLIER * a);

            var invDMulAlpha = invD * a;
            var duMulInvD = du * invDMulAlpha; // used to help optimize twovdu * invD 
            var dMulInvD = d * invDMulAlpha; // used to help optimize twovdu * invD 
            var twovduMulInvD = 0; 
            var incrSMulInvD = incrS * invDMulAlpha;
            var incrDMulInvD = incrD * invDMulAlpha;

            do
            {
                //Test 1: adding line thickness
                for (int i = 0; i < lineThickness; i++)
                {
                    AddAAPixel(wb, addr + i, (zeroDot75 - twovduMulInvD) >> PRECISION_SHIFT, srb, sg);
                    AddAAPixel(wb, addr + i + vincr, (invD2Du + twovduMulInvD) >> PRECISION_SHIFT, srb, sg);
                    AddAAPixel(wb, addr - i - vincr, (invD2Du - twovduMulInvD) >> PRECISION_SHIFT, srb, sg);
                }

                //Test 2: default (only 1 thickness)
                //AddAAPixel(wb, addr, (zeroDot75 - twovduMulInvD) >> PRECISION_SHIFT, srb, sg);
                //AddAAPixel(wb, addr + vincr, (invD2Du + twovduMulInvD) >> PRECISION_SHIFT, srb, sg);
                //AddAAPixel(wb, addr - vincr, (invD2Du - twovduMulInvD) >> PRECISION_SHIFT, srb, sg);

                if (d < 0)
                {
                    // choose straight (u direction) 
                    twovduMulInvD = dMulInvD + duMulInvD;
                    d += incrS;
                    dMulInvD += incrSMulInvD;
                }
                else
                {
                    // choose diagonal (u+v direction) 
                    twovduMulInvD = dMulInvD - duMulInvD;
                    d += incrD;
                    dMulInvD += incrDMulInvD;
                    v++; 
                    addr += vincr;
                }
                u++;

                //Test 2:
                //AddAAPixel(wb, addr2, (zeroDot75 - twovduMulInvD) >> PRECISION_SHIFT, srb, sg);
                //AddAAPixel(wb, addr2 + uincr, (invD2Du + twovduMulInvD) >> PRECISION_SHIFT, srb, sg);
                //AddAAPixel(wb, addr2 - uincr, (invD2Du - twovduMulInvD) >> PRECISION_SHIFT, srb, sg);

                addr += uincr;
   
            } while (u <= uend);
        }
        

        public static void AddAAPixel(WriteableBitmap wb, int index, int sa, uint srb, uint sg)
        {
            unsafe
            {
                using (var context = wb.GetBitmapContext())
                {
                    var pixels = context.Pixels;
                    var destPixel = (uint)pixels[index];

                    //sets shades of colors
                    var da = (destPixel >> 24);
                    var dg = ((destPixel >> 8) & 0xff);
                    var drb = destPixel & 0x00FF00FF;

                    // blends color
                    pixels[index] = (int)(
                       ((sa + ((da * (255 - sa) * 0x8081) >> 23)) << 24) | // alpha 
                       (((sg - dg) * sa + (dg << 8)) & 0xFFFFFF00) | // green 
                       (((((srb - drb) * sa) >> 8) + drb) & 0x00FF00FF) // red and blue 
                    );
                }
            }
        }
        


    }
}
