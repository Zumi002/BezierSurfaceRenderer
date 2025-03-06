using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK_BezierSurf.Drawing.Samplers
{
    class StaticSampler : ISampler
    {
        Color staticColor;

        public StaticSampler(Color staticColor)
        {
            this.staticColor = staticColor;
        }

        public Color GetColor(float u, float v)
        {
            return staticColor;
        }
    }
}
