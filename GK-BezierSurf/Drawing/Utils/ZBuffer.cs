using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK_BezierSurf.Drawing.Utils
{
    class ZBuffer
    {
        protected float[,] buffer;
        protected int[,] prio;


        public ZBuffer(int width, int height)
        {
            buffer = new float[width, height];
            prio = new int[width, height];
            Clear();
        }

        public virtual bool Check(int i, int j, float z,int p)
        {
            
                if (i < 0 || j < 0 || i >= buffer.GetLength(0) || j >= buffer.GetLength(1))
                {
                    return false;
                }
            
                if (Math.Abs(buffer[i, j] - z) < 1e-6 ? prio[i, j] < p : buffer[i, j] > z)
                {
                    Set(i, j, z, p);
                    return true;
                }
            
            

                return false;
            
        }

        public void Set(int i, int j, float z,int p)
        {
            
            buffer[i, j] = z;
            prio[i, j] = p;
            
        }

        public void Clear()
        {
            for(int i = 0; i < buffer.GetLength(0); i++)
            {
                for (int j = 0; j < buffer.GetLength(1); j++)
                {
                    buffer[i,j] = float.MaxValue;
                    prio[i, j] = 0;
                }
            }
        }
    }
}
