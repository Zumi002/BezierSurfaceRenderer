using GK_BezierSurf.Drawing.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GK_BezierSurf.Drawing.Samplers.NormalVectorSampler
{
    class FileNormalVectorSampler : INormalVectorSampler
    {
        public FileSampler sampler;

        public FileNormalVectorSampler(string filename)
        {
            sampler = new FileSampler(filename);
        }

        public Vector3 GetNormalVector(Vector3 pu, Vector3 pv, Vector3 n, float u, float v)
        {
            Matrix3 mat = new Matrix3(Vector3.Normalize(pu), Vector3.Normalize(pv), Vector3.Normalize(n));
            Color fromSampler = sampler.GetColor(u, v);
            Vector3 normalFromSampler = new Vector3(((float)fromSampler.R / 255 - 0.5f) * 2f,
                                                    ((float)fromSampler.G / 255 - 0.5f) * 2f,
                                                     (float)fromSampler.B / 255);
            return Vector3.Normalize(mat * normalFromSampler);
        }
    }
}
