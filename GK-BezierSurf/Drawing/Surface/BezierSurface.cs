using GK_BezierSurf.Drawing.Primitive;
using GK_BezierSurf.Drawing.Utils;
using GK_BezierSurf.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace GK_BezierSurf.Drawing.Surface
{
    class BezierSurface
    {
        Vector3[] orgPoints = new Vector3[16];
        Vector3[] afterRotationPoints = new Vector3[16];

        //x-axis rotation
        float alphaAngle = float.NegativeInfinity;
        Matrix3 alphaRotationMatrix;
        //z-axis rotation
        float betaAngle = float.NegativeInfinity;
        Matrix3 betaRotationMatrix;
        bool rotatedSinceLastCalc;

        int divide = 10;
        bool trianglesChangedSinceLastCalc;
        Vector3[,] trianglePoints;

        List<Triangle> triangles;

        public string activeContent;

        public BezierSurface()
        {
            Parse(Resources.test1);
        }

        public BezierSurface(string fileName)
        {
            LoadFromTxtFile(fileName);
            
            rotatedSinceLastCalc = true;
            trianglesChangedSinceLastCalc = true;
            Rotate(0, 0);
            int o = binomialCof(4, 1);
            int l = binomialCof(4, 0);
            int w = binomialCof(4, 2);
            int p = binomialCof(4, 3);
        }

        public void LoadFromTxtFile(string fileName)
        {
            using (StreamReader sr = File.OpenText(fileName))
            {
                string content = sr.ReadToEnd();
                Parse(content);
                
            }
        }

        private void Parse(string content)
        {
            
            string[] lines = content.Split('\n');
            if (lines.Length < 16)
            {
                throw new Exception("Too small number of points");
            }
            for (int i = 0; i < 16; i++)
            {
                lines[i] = lines[i].Replace(',', '.');
                string[] coords = lines[i].Split(';');
                float[] c = new float[3];
                for (int j = 0; j < 3; j++)
                {
                    c[j] = float.Parse(coords[j], CultureInfo.InvariantCulture);
                }
                orgPoints[i] = new Vector3(c);
            }
            activeContent = content;
            trianglesChangedSinceLastCalc = true;
        }

        public void Rotate(float a, float b)
        {
            if (a != alphaAngle)
            {
                alphaAngle = a;
                double rad = Math.PI * alphaAngle / 180;
                (double sin, double cos) = Math.SinCos(rad);
                alphaRotationMatrix = new Matrix3(1, 0, 0,
                                                  0, (float)cos, (float)-sin,
                                                  0, (float)sin, (float)cos);
                rotatedSinceLastCalc = true;
            }
            if (b != betaAngle)
            {
                betaAngle = b;
                double rad = Math.PI * betaAngle / 180;
                (double sin, double cos) = Math.SinCos(rad);
                betaRotationMatrix = new Matrix3((float)cos, (float)-sin, 0,
                                                  (float)sin, (float)cos, 0,
                                                            0, 0, 1);
                rotatedSinceLastCalc = true;
            }
        }
        public Vector3[,] GetTrianglePoints()
        {
            if (rotatedSinceLastCalc || trianglesChangedSinceLastCalc)
            {
                CalcPoints();
                rotatedSinceLastCalc = false;
                trianglesChangedSinceLastCalc = false;
            }
            return trianglePoints;
        }

        void CalcPoints()
        {
            trianglePoints = new Vector3[divide + 1, divide + 1];
            Vector3[,] partDerivPointsV = new Vector3[divide + 1, divide + 1];
            Vector3[,] partDerivPointsU = new Vector3[divide + 1, divide + 1];
            Vector3[,] horizontalPart = new Vector3[4, divide + 1];
            Vector3[,] verticalPart = new Vector3[divide + 1, 4];

            Vector3[] contr = new Vector3[4];
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    contr[j] = orgPoints[4 * i + j];
                }
                int k = 0;
                foreach (Vector3 v3 in CalcBezier(contr, divide).Item1)
                {
                    horizontalPart[i, k++] = v3;
                }
                for (int j = 0; j < 4; j++)
                {
                    contr[j] = orgPoints[4 * j + i];
                }
                k = 0;
                foreach (Vector3 v3 in CalcBezier(contr, divide).Item1)
                {
                    verticalPart[k++, i] = v3;
                }
            }
            Matrix3 rotMat = alphaRotationMatrix * betaRotationMatrix;

            for (int i = 0; i <= divide; i++)
            {

                for (int j = 0; j < 4; j++)
                {
                    contr[j] = horizontalPart[j, i];
                }
                int k = 0;
                (Vector3[] tr, Vector3[] partDerivV) = CalcBezier(contr, divide);
                foreach (Vector3 v3 in tr)
                {
                    trianglePoints[i, k++] = rotMat * v3;
                }
                k = 0;
                foreach (Vector3 v3 in partDerivV)
                {
                    partDerivPointsV[i, k++] = rotMat * v3;
                }
                k = 0;
                for (int j = 0; j < 4; j++)
                {
                    contr[j] = verticalPart[i, j];
                }
                foreach (Vector3 v3 in CalcBezier(contr, divide).Item2)
                {
                    partDerivPointsU[k++, i] = rotMat * v3;
                }
            }



            //making triangle list
            
            triangles = new List<Triangle>();
            Vertex[,] vs = new Vertex[divide + 1, divide + 1];

            for (int i = 0; i <= divide; i++)
            {
                for (int j = 0; j <= divide; j++)
                {
                    vs[i, j] = new Vertex(trianglePoints[i, j],
                                          new Vector2((float)i/divide,(float)j/divide),
                                          Vector3.Cross(partDerivPointsU[i, j], partDerivPointsV[i, j]));
                    vs[i, j].normalV =partDerivPointsV[i, j];
                    vs[i, j].normalU =partDerivPointsU[i, j];
                    
                }
            }
            for (int i = 0; i < divide; i++)
            {
                for (int j = 0; j < divide; j++)
                {
                    Vertex[] v = new Vertex[3];
                    v[0] = vs[i, j];
                    v[1] = vs[i, j + 1];
                    v[2] = vs[i + 1, j + 1];


                    Triangle triangle = new Triangle(v);
                    triangles.Add(triangle);
                    v = new Vertex[3];
                    v[0] = vs[i, j];
                    v[1] = vs[i + 1, j];
                    v[2] = vs[i + 1, j + 1];

                    triangle = new Triangle(v);
                    triangles.Add(triangle);
                }
            }
        }

        public List<Triangle> GetTriangles()
        {
            return triangles;
        }

        (Vector3[], Vector3[]) CalcBezier(Vector3[] controls, int divide)
        {
            float step = 1f / (float)divide;
            float step2 = step * step;
            float step3 = step * step2;
            Vector3[] A = new Vector3[4];
            Vector3[] P = new Vector3[4];
            Vector3[] P2 = new Vector3[3];

            A[0] = controls[0];
            A[1] = 3 * (controls[1] - controls[0]);
            A[2] = 3 * (controls[2] - 2 * controls[1] + controls[0]);
            A[3] = controls[3] - 3 * controls[2] + 3 * controls[1] - controls[0];

            P[0] = A[0];
            P[1] = A[3] * step3 + A[2] * step2 + A[1] * step;
            P[2] = A[3] * step3 * 6 + A[2] * step2 * 2;
            P[3] = A[3] * step3 * 6;

            P2[0] = A[1];
            P2[1] = A[3] * step2 * 3 + A[2] * step * 2;
            P2[2] = A[3] * step2 * 6;

            Vector3[] result = new Vector3[divide+1];
            Vector3[] res2 = new Vector3[divide+1];

            for (int i = 0; i <= divide; i++)
            {
                result[i] = P[0];
                res2[i] = P2[0];
                P[0] += P[1];
                P[1] += P[2];
                P[2] += P[3];
                P2[0] += P2[1];
                P2[1] += P2[2];
            }

            return (result, res2);
        }

        Vector3 CalcFirstDivByV(Vector3[,] controls, float u, float v)
        {
            Vector3 res = new Vector3(0, 0, 0);
            for (int i = 0; i < controls.GetLength(0); i++)
            {
                for (int j = 0; j < controls.GetLength(1) - 1; j++)
                {
                    res += (controls[i, j + 1] - controls[i, j]) * bernPol(controls.GetLength(0) - 1, i, u) * bernPol(controls.GetLength(1) - 2, j, v);
                }
            }
            return res * (controls.GetLength(1) - 1);
        }
        Vector3 CalcFirstDivByU(Vector3[,] controls, float u, float v)
        {
            Vector3 res = new Vector3(0, 0, 0);
            for (int i = 0; i < controls.GetLength(0) - 1; i++)
            {
                for (int j = 0; j < controls.GetLength(1); j++)
                {
                    res += (controls[i + 1, j] - controls[i, j]) * bernPol(controls.GetLength(0) - 2, i, u) * bernPol(controls.GetLength(1) - 1, j, v);
                }
            }
            return res * (controls.GetLength(0) - 1);
        }

        Vector3 calcPoint(Vector3[,] controls, float u, float v)
        {
            Vector3 res = new Vector3(0, 0, 0);
            for (int i = 0; i < controls.GetLength(0); i++)
            {
                for (int j = 0; j < controls.GetLength(1); j++)
                {
                    res += controls[i, j] * bernPol(controls.GetLength(0) - 1, i, u) * bernPol(controls.GetLength(1) - 1, j, v);
                }
            }
            return res;
        }

        int binomialCof(int n, int k)
        {
            int ans = 1;
            for (int j = 1; j <= k; j++)
            {
                ans *= (n + j - k);
                ans /= j;
            }
            return ans;
        }

        float bernPol(int n, int i, float u)
        {
            return (float)binomialCof(n, i) * (float)Math.Pow(u, i) * (float)Math.Pow((1 - u), n - i);
        }

        public void Divide(int divs)
        {
            divide = divs;
            trianglesChangedSinceLastCalc = true;
        }

        public void Restore()
        {
            Parse(activeContent);
        }
    }
}
