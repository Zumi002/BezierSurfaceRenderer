using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GK_BezierSurf.Drawing.Samplers.NormalVectorSampler
{
    interface INormalVectorSampler
    {
        public Vector3 GetNormalVector(Vector3 pu, Vector3 pv, Vector3 n, float u, float v);
    }
}
