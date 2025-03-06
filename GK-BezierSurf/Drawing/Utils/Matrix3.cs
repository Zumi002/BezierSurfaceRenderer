using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GK_BezierSurf.Drawing.Utils
{
    struct Matrix3
    {
        float[,] mat = new float[3, 3];

        public Matrix3() { }
        public Matrix3(float[,] mat)
        {
            this.mat = mat;
        }

        public Matrix3(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            mat = new float[3, 3] { { v1.X, v2.X, v3.X },
                                    { v1.Y, v2.Y, v3.Y },
                                    { v1.Z ,v2.Z, v3.Z } };
        }

        public Matrix3(float r1c1, float r1c2, float r1c3,
                       float r2c1, float r2c2, float r2c3,
                       float r3c1, float r3c2, float r3c3)
        {
            mat = new float[3, 3] { { r1c1, r1c2, r1c3 },
                                    { r2c1, r2c2, r2c3 },
                                    { r3c1, r3c2, r3c3 } };
        }

        public float this[int i, int j]
        {
            get => mat[i, j];
            set => mat[i, j] = value;
        }

        public static Vector3 operator *(Matrix3 a, Vector3 b)
        {
            Vector3 v;
            v.X = a[0, 0] * b.X + a[0, 1] * b.Y + a[0, 2] * b.Z;
            v.Y = a[1, 0] * b.X + a[1, 1] * b.Y + a[1, 2] * b.Z;
            v.Z = a[2, 0] * b.X + a[2, 1] * b.Y + a[2, 2] * b.Z;
            return v;
        }

        public static Matrix3 operator *(Matrix3 a, Matrix3 b)
        {
            Matrix3 c = new Matrix3();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    c[i, j] = 0;
                    for (int k = 0; k < 3; k++)
                    {
                        c[i, j] += a[i, k] * b[k, j];
                    }
                }
            }
            return c;
        }

        public static Matrix3 operator +(Matrix3 a, Matrix3 b)
        {
            Matrix3 c = new Matrix3();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    c[i, j] = a[i, j] + b[i, j];
                }
            }
            return c;
        }
    }
}
