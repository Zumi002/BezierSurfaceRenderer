using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GK_BezierSurf.Drawing.Primitive
{
    class Vertex
    {
        public Vector3 point = new Vector3();
        public Vector2 cameraPoint = new Vector2();
        public Vector2 pointUV = new Vector2();
        public Vector3 normal = new Vector3();
        public Vector3 normalU = new Vector3();
        public Vector3 normalV = new Vector3();
        public Vertex(Vector3 xyz, Vector2 uv,Vector3 n)
        {
            point = xyz;
            pointUV = uv;
            normal = Vector3.Normalize(n);
        }

        public void SetCameraPoint(Point p)
        {
            cameraPoint = new Vector2(p.X, p.Y);
        }

    }
}
