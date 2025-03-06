using GK_BezierSurf.Drawing.Lightning;
using GK_BezierSurf.Drawing.Primitive;
using GK_BezierSurf.Drawing.Utils;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using GK_BezierSurf.Drawing.Samplers;
using GK_BezierSurf.Drawing.Samplers.NormalVectorSampler;

namespace GK_BezierSurf.Drawing.Functions
{
    class FillPolygon
    {
        public static void Fill(Vertex[] vertices, DirectBitmap bmp,ZBuffer zb,Triangle t,ISampler sampler, INormalVectorSampler normalSampler)
        {
            if (vertices.Length < 3)
                return;
            int minY = bmp.Height, maxY = 0;
            Point[] points = new Point[vertices.Length];
            int k = 0;
            foreach (Vertex v in vertices)
            {
                points[k++] = new Point((int)v.cameraPoint.X, (int)v.cameraPoint.Y);
            }



            FillEdge[] fillEdges = new FillEdge[bmp.Height];
            (FillEdge, int)[] fillEdgesTails = new (FillEdge, int)[bmp.Height];
            int edges = 0;
            int y = CreateET(points, fillEdges, fillEdgesTails, ref edges);
            FillEdge AET = null;
            while (AET != null || edges != 0)
            {
                if (fillEdgesTails[y].Item1 != null)
                {
                    fillEdgesTails[y].Item1.next = AET;
                    edges -= fillEdgesTails[y].Item2;
                    AET = fillEdges[y];
                }
                AET = Sort(AET);
                FillEdge p = AET, pp = AET.next;
                while (pp != null)
                {
                    for (int i = (int)p.X; i <= (int)pp.X; i++)
                    {
                        (Vector3 coord, Vector3[] n, Vector2 uv) = t.GetInterpolatedEssentials(new Point(i, y));
                        if (zb.Check(i, y, coord.Z,1))
                        {
                            
                            Color drawColor = GlobalLambertianLightning.
                                              CalcDirectionalLightning(normalSampler.GetNormalVector(n[0], n[1], n[2],uv.X,uv.Y),
                                                                       sampler.GetColor(uv.X,uv.Y));
                           /* Color drawColor = GlobalLambertianLightning.
                                              CalcPointLightning(normalSampler.GetNormalVector(n[0], n[1], n[2], uv.X, uv.Y),
                                                                 sampler.GetColor(uv.X, uv.Y),
                                                                 coord);*/
                            bmp.SetPixel(i, y, drawColor);
                        }
                    }
                    p.X += p.d;
                    pp.X += pp.d;
                    p = pp.next;
                    pp = p != null ? p.next : null;
                }
                y++;
                pp = null;
                p = AET;
                //usuwanie starych
                while (p != null)
                {
                    if (p.maxY <= y)
                    {
                        if (pp == null)
                        {
                            AET = p.next;
                        }
                        else
                        {
                            pp.next = p.next;
                        }
                    }
                    else
                    {
                        pp = p;
                    }
                    p = p.next;
                }
            }
        }

        public static FillEdge Sort(FillEdge AET)
        {
            FillEdge sorted = null;
            FillEdge current = AET;
            while (current != null)
            {
                FillEdge next = current.next;
                sorted = Insert(sorted, current);
                current = next;
            }
            return sorted;
        }
        public static FillEdge Insert(FillEdge sorted, FillEdge toInsert)
        {
            FillEdge head = sorted;
            if (sorted == null || sorted.X > toInsert.X)
            {
                FillEdge tmp = sorted;
                toInsert.next = tmp;
                head = toInsert;
            }
            else
            {
                while (sorted.next != null)
                {
                    if (sorted.next.X > toInsert.X)
                    {
                        FillEdge tmp = sorted.next;
                        toInsert.next = tmp;
                        sorted.next = toInsert;
                        return head;
                    }
                    sorted = sorted.next;
                }
                sorted.next = toInsert;
            }
            return head;
        }

        public static int CreateET(Point[] points, FillEdge[] fillEdges, (FillEdge, int)[] fillEdgesTails, ref int edges)
        {
            int minY = int.MaxValue;
            int n = points.Length;
            for (int i = 1; i <= points.Length; i++)
            {
                int minYBet = 0;
                int maxYBet = 0;
                float X = 0;
                float slope = 0;

                Point A = points[i % n];
                Point B = points[i - 1];

                if (A.Y < B.Y)
                {
                    minYBet = A.Y;
                    maxYBet = B.Y;
                    X = A.X;
                    if (Math.Abs(B.Y - A.Y) < 1e-9)
                        continue;
                    else
                        slope = (float)(B.X - A.X) / (B.Y - A.Y);
                }
                else
                {

                    minYBet = B.Y;
                    maxYBet = A.Y;
                    X = B.X;
                    if (Math.Abs(A.Y - B.Y) < 1e-9)
                        continue;
                    else
                        slope = (float)(A.X - B.X) / (A.Y - B.Y);

                }

                minY = Math.Min(minYBet, minY);


                FillEdge? fe = fillEdges[minYBet];
                FillEdge create = new FillEdge(maxYBet, X, slope);
                if (fe == null)
                {
                    fillEdgesTails[minYBet] = (create, 1);
                }
                else
                {
                    fillEdgesTails[minYBet].Item2++;
                }
                create.next = fe;
                fillEdges[minYBet] = create;
                edges++;
            }

            return minY;
        }
    }
}
