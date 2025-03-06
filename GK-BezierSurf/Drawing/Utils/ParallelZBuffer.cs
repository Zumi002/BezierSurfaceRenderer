using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK_BezierSurf.Drawing.Utils
{
    class ParallelZBuffer : ZBuffer
    {
        object[,] locks;
        public ParallelZBuffer(int w, int h) : base(w, h) 
        { 
            locks = new object[w, h];
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    locks[i, j] = new object();
                }
            }
        }

        public override bool Check(int i, int j,float z, int p)
        {
            if (i < 0 || j < 0 || i >= buffer.GetLength(0) || j >= buffer.GetLength(1))
            {
                return false;
            }

            lock (locks[i, j])
            {
                if (Math.Abs(buffer[i, j] - z) < 1e-6 ? prio[i, j] < p : buffer[i, j] > z)
                {
                    Set(i, j, z, p);
                    return true;
                }
            }



            return false;
        }
    }
}
