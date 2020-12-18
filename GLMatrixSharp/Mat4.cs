using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GLMatrixSharp
{
    public class Mat4
    {
        public static float[] Create()
        {
            var res = new float[16];
            res[1] = 0;
            res[2] = 0;
            res[3] = 0;
            res[4] = 0;
            res[6] = 0;
            res[7] = 0;
            res[8] = 0;
            res[9] = 0;
            res[11] = 0;
            res[12] = 0;
            res[13] = 0;
            res[14] = 0;
            res[0] = 1;
            res[5] = 1;
            res[10] = 1;
            res[15] = 1;
            return res;
        }

        public static float[] Translate(float[] a, float[] v)
        {
            var res = new float[16];

            if (a.Length < 16 || v.Length < 3) throw new ArgumentException();

            float x = v[0], y = v[1], z = v[2];

            res[0] = a[0];
            res[1] = a[1];
            res[2] = a[2];
            res[3] = a[3];
            res[4] = a[4];
            res[5] = a[5];
            res[6] = a[6];
            res[7] = a[7];
            res[8] = a[8];
            res[9] = a[9];
            res[10] = a[10];
            res[11] = a[11];

            res[12] = a[0] * x + a[4] * y + a[8] * z + a[12];
            res[13] = a[1] * x + a[5] * y + a[9] * z + a[13];
            res[14] = a[2] * x + a[6] * y + a[10] * z + a[14];
            res[15] = a[3] * x + a[7] * y + a[11] * z + a[15];

            return res;
        }
    }
}
