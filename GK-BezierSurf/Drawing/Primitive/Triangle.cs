using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Drawing2D;

namespace GK_BezierSurf.Drawing.Primitive
{
    class Triangle
    {
        public Vertex[] vert = new Vertex[3];
        float invDenom,d00,d01,d11;
        public Triangle(Vertex[] vs)
        {
            vert = vs;
        }

        public float[] barycentricInterpolation(Point t)
        {
            float[] f = new float[3];
            Vector2 v0 = vert[2].cameraPoint - vert[0].cameraPoint;
            Vector2 v1 = vert[1].cameraPoint - vert[0].cameraPoint;
            Vector2 d = new Vector2(t.X, t.Y);
            Vector2 v2 = d - vert[0].cameraPoint;

            float d02 = Vector2.Dot(v0, v2);
            float d12 = Vector2.Dot(v1, v2);

            f[2] = (d11 * d02 - d01 * d12) * invDenom;
            f[1] = (d00 * d12 - d01 * d02) * invDenom;
            f[0] = 1 - f[2] - f[1];

            return f;
        }
        public Vector3 GetInterpolatedCoords(Point t, float[] f)
        {
            
            Vector3 res = Vector3.Zero;
            for(int i = 0;i<3;i++)
            {
                res += vert[i].point * f[i];
            }
            return res;
        }
        public Vector3[] GetInterpolatedNormals(Point t, float[] f)
        {

            Vector3[] normals = new Vector3[3];
            for (int i = 0; i < 3; i++)
                normals[0] = Vector3.Zero;
            for (int i = 0; i < 3; i++)
            {
                normals[0] += vert[i].normalU * f[i];
                normals[1] += vert[i].normalV * f[i];
            }

            normals[2] = Vector3.Cross(normals[0],normals[1]);
            return normals;
        }

        public Vector2 GetInterpolatedUV(Point t, float[] f)
        {
            Vector2 res = Vector2.Zero;
            for (int i = 0; i < 3; i++)
            {
                res += vert[i].pointUV* f[i];

            }
            return res;
        }

        public (Vector3, Vector3[],Vector2) GetInterpolatedEssentials(Point t)
        {
            float[] f = barycentricInterpolation(t);
            Vector3 z = GetInterpolatedCoords(t, f);
            Vector3[] normals = GetInterpolatedNormals(t, f);
            Vector2 uv = GetInterpolatedUV(t, f);

            return (z, normals, uv);
        }

        public void setVerteciesCameraPositions(Point[] points)
        {
            for (int i = 0; i < 3; i++)
            {
                vert[i].SetCameraPoint(points[i]);
            }

            Vector2 v0 = vert[2].cameraPoint - vert[0].cameraPoint;
            Vector2 v1 = vert[1].cameraPoint - vert[0].cameraPoint;

            d00 = Vector2.Dot(v0, v0);
            d01 = Vector2.Dot(v0, v1);
            d11 = Vector2.Dot(v1, v1);
            invDenom = 1 / (d00 * d11 - d01 * d01);
        }

    }
}
