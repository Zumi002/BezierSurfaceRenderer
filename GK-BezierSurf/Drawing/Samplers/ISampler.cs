using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK_BezierSurf.Drawing.Samplers
{
    interface ISampler
    {
        public Color GetColor(float u, float v);
    }
}
