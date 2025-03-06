using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using GK_BezierSurf.Drawing.Utils;
using GK_BezierSurf.Drawing.Surface;
using System.Drawing.Drawing2D;
using GK_BezierSurf.Drawing.Primitive;
using System.Drawing.Design;
using GK_BezierSurf.Drawing.Functions;
using GK_BezierSurf.Drawing.Samplers;
using GK_BezierSurf.Drawing.Samplers.NormalVectorSampler;

namespace GK_BezierSurf.Drawing.Camera
{
    class OrthographicCamera
    {

        DirectBitmap active, buffer;
        ZBuffer zBuffer;
        int W, H;
        public float scale = 1;
        public bool drawMesh;
        public bool drawTriangle;

        public ISampler surfColor;
        public INormalVectorSampler normalVectorSampler;

        public OrthographicCamera(int width, int height)
        {
            W = width; H = height;
            active = new DirectBitmap(W, H);
            buffer = new DirectBitmap(W, H);
            zBuffer = new ZBuffer(W, H);
            drawMesh = false;
            drawTriangle = true;
            surfColor = new StaticSampler(Color.Pink);
            normalVectorSampler = new NormalVectorSampler();
        }

        void Swap()
        {
            DirectBitmap tmp = active;
            active = buffer;
            buffer = tmp;
            
        }

        public void ChangeScale(float scale)
        {
            this.scale = scale;
        }

        public void Draw(BezierSurface surf)
        {
            surf.GetTrianglePoints();
            List<Triangle> ts = surf.GetTriangles();

            try
            {
                
                using (Graphics g = Graphics.FromImage(buffer.Bitmap))
                {
                    g.ScaleTransform(1, -1);
                    g.TranslateTransform(buffer.Width / 2, -buffer.Height / 2);
                    g.ScaleTransform(scale, scale);
                    g.Clear(Color.White);
                    zBuffer.Clear();
                    Matrix matrix = g.Transform;

                    foreach (Triangle t in ts)
                    {
                        PointF[] points = new PointF[3];
                        int k = 0;

                        foreach (Vertex v3 in t.vert)
                        {
                            points[k++] = new PointF(v3.point.X, v3.point.Y);
                        }
                        matrix.TransformPoints(points);
                        Point[] ps = new Point[3];
                        for(int i = 0; i < 3; i++)
                        {
                            ps[i] = new Point((int)Math.Round(points[i].X), (int)Math.Round(points[i].Y));
                        }
                        t.setVerteciesCameraPositions(ps);
                    }

                    Parallel.ForEach(ts, t => 
                    {

                        if (drawTriangle)
                            FillPolygon.Fill(t.vert, buffer, zBuffer, t, surfColor, normalVectorSampler);
                        if (drawMesh)
                            DrawLine.DrawLines(t.vert, buffer, zBuffer, t);

                    });
                }
            }
            catch
            {
                using (Graphics g = Graphics.FromImage(buffer.Bitmap))
                {
                   g.Clear(Color.White);
                }
            }
        }

        

        public Bitmap getNewFrame()
        {
            Swap();
            return active.Bitmap;
        }

    }
}
