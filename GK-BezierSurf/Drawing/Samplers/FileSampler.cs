using GK_BezierSurf.Drawing.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK_BezierSurf.Drawing.Samplers
{
    class FileSampler : ISampler, IDisposable
    {
        public DirectBitmap img;
        int W, H;
        public FileSampler(string filename)
        {
            Bitmap imgFromFile =(Bitmap) Image.FromFile(filename);
            W = imgFromFile.Width;
            H = imgFromFile.Height;
            img = new DirectBitmap(W,H);
            img.loadFromBitmap(imgFromFile);
            imgFromFile.Dispose();
            W -= 1;
            H-= 1;  
           
        }
        public FileSampler(DirectBitmap img) 
        {
            W = img.Width;
            H = img.Height;
            this.img = img;
            W -= 1;
            H -= 1;
        }

        public void Dispose()
        {
            img.Dispose();
        }
        public Color GetColor(float u, float v)
        {
            u = Math.Clamp(u, 0, 1);
            v = Math.Clamp(v, 0, 1);
            int w = (int)(W * u);
            int h = (int)(H * v);
            return img.GetPixel(w,H-h);
        }
    }
}
