using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using GK_BezierSurf.Drawing.Primitive;

namespace GK_BezierSurf.Drawing.Lightning
{
    class GlobalLambertianLightning
    {
        public static int m;
        public static float ks, kd;
        public static Vector3 lightningDir = new Vector3(0,0,10);
        public static Color lightningColor = Color.White;
        public static Color CalcDirectionalLightning(Vector3 normal, Color obejctColor)
        {
            Vector3 light = Vector3.Normalize(lightningDir);
            float left = Vector3.Dot(normal, light) * kd;
            float right = Vector3.Dot(new Vector3(0, 0, 1), -Vector3.Reflect(light, normal));

            left = left > 0 ? left : 0;
            right = right > 0 ? right : 0;

            right = (float)Math.Pow(right, m);
            right *= ks;

            float R = (float)obejctColor.R / 255 * (float)lightningColor.R / 255;
            float G = (float)obejctColor.G / 255 * (float)lightningColor.G / 255;
            float B = (float)obejctColor.B / 255 * (float)lightningColor.B / 255;

            R = R * left + R * right;
            G = G * left + G * right;
            B = B * left + B * right;

            R = R > 1 ? 1 : R;
            G = G > 1 ? 1 : G;
            B = B > 1 ? 1 : B;

            return Color.FromArgb(255, (int)(255 * R), (int)(255 * G), (int)(255 * B));
        }

        public static Color CalcPointLightning(Vector3 normal, Color obejctColor, Vector3 pos)
        {
            Vector3 light = Vector3.Normalize(lightningDir-pos);
            float left = Vector3.Dot(normal, light) * kd;
            float right = Vector3.Dot(new Vector3(0, 0, 1), -Vector3.Reflect(light, normal));

            left = left > 0 ? left : 0;
            right = right > 0 ? right : 0;

            right = (float)Math.Pow(right, m);
            right *= ks;

            float R = (float)obejctColor.R / 255 * (float)lightningColor.R / 255;
            float G = (float)obejctColor.G / 255 * (float)lightningColor.G / 255;
            float B = (float)obejctColor.B / 255 * (float)lightningColor.B / 255;

            R = R * left + R * right;
            G = G * left + G * right;
            B = B * left + B * right;

            R = R > 1 ? 1 : R;
            G = G > 1 ? 1 : G;
            B = B > 1 ? 1 : B;

            return Color.FromArgb(255, (int)(255 * R), (int)(255 * G), (int)(255 * B));
        }

        public static void ChangeLighningDir(Vector3 dir)
        {
            lightningDir = dir;
        }
    }
}
