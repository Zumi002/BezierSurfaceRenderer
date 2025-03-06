using GK_BezierSurf.Drawing.Primitive;
using GK_BezierSurf.Drawing.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Drawing2D;

namespace GK_BezierSurf.Drawing.Functions
{
    static class DrawLine
    {

        public static void DrawLines(Vertex[] vs, DirectBitmap bmp, ZBuffer zb, Triangle t)
        {
            int n = vs.Length;
            for (int i = 1; i <= vs.Length; i++)
            {
                Draw(vs[i % n], vs[i-1], bmp, zb, t);
            }
        }
        public static void Draw(Vertex v1, Vertex v2, DirectBitmap bmp, ZBuffer zb, Triangle t)
        {
            Point[] points = new Point[2];

            points[0] = new Point((int)v1.cameraPoint.X, (int)v1.cameraPoint.Y);
            points[1] = new Point((int)v2.cameraPoint.X, (int)v2.cameraPoint.Y);

            int x1 = (int)points[0].X,
                x2 = (int)points[1].X,
                y1 = (int)points[0].Y,
                y2 = (int)points[1].Y;

            int dx, dy, g, h, c;

            dx = x2 - x1;
            if (dx > 0)
                g = +1;
            else
                g = -1;

            dx = Math.Abs(dx);

            dy = y2 - y1;

            if (dy > 0)
                h = +1;
            else
                h = -1;

            dy = Math.Abs(dy);

            if (dx > dy)
            {
                c = -dx;
                while (x1 != x2)
                {
                    if (x1 > 0 && x1 < bmp.Width && y1 < bmp.Height && y1 > 0)
                    {
                        float z = t.GetInterpolatedEssentials(new Point(x1, y1)).Item1.Z;
                        if (zb.Check(x1, y1, z,2))
                        {
                            bmp.SetPixel(x1, y1, Color.Black);
                        }
                    }
                    c += 2 * dy;
                    if (c > 0)
                    {
                        y1 += h;
                        c -= 2 * dx;
                    }
                    x1 += g;
                }
            }
            else
            {
                c = -dy;
                while (y1 != y2)
                {
                    if (x1 > 0 && x1 < bmp.Width && y1 < bmp.Height && y1 > 0)
                    {
                        float z = t.GetInterpolatedEssentials(new Point(x1, y1)).Item1.Z;
                        if (zb.Check(x1, y1, z,2))
                        {
                            bmp.SetPixel(x1, y1, Color.Black);
                        }
                    }
                    c += 2 * dx;
                    if (c > 0)
                    {
                        x1 += g;
                        c -= 2 * dy;
                    }
                    y1 += h;
                }
            }
        }
    }
}
