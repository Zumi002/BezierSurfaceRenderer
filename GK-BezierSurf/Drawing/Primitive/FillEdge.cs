using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK_BezierSurf.Drawing.Primitive
{
    class FillEdge
    {
        public int maxY;
        public float X;
        public float d;
        public FillEdge next;
        public FillEdge(int maxY, float X, float d)
        {
            this.maxY = maxY;
            this.X = X;
            this.d = d;
            next = null;
        }
    }
}
