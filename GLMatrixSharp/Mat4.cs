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

        public static float[] Perspective(float fovy, float aspect, float near, float far)
        {
            var res = new float[16];
            var f = (float)(1.0 / Math.Tan(fovy / 2));
            res[0] = (f / aspect);
            res[1] = 0;
            res[2] = 0;
            res[3] = 0;
            res[4] = 0;
            res[5] = f;
            res[6] = 0;
            res[7] = 0;
            res[8] = 0;
            res[9] = 0;
            res[11] = -1;
            res[12] = 0;
            res[13] = 0;
            res[15] = 0;
            if (!float.IsInfinity(far))
            {
                var nf = 1 / (near - far);
                res[10] = (far + near) * nf;
                res[14] = 2 * far * near * nf;
            }
            else
            {
                res[10] = -1;
                res[14] = -2 * near;
            }
            return res;
        }

        public static float[] Rotate(float[] a, float rad, float[] axis)
        {
            float x = axis[0], y = axis[1], z = axis[2];
            float[] res = new float[16];
            float len = (float)Math.Sqrt(x*x + y*y + z*z);
            float s, c, t;
            float a00, a01, a02, a03;
            float a10, a11, a12, a13;
            float a20, a21, a22, a23;
            float b00, b01, b02;
            float b10, b11, b12;
            float b20, b21, b22;
            //if (len < glMatrix.EPSILON)
            //{
            //    return null;
            //}
            len = 1 / len;
            x *= len;
            y *= len;
            z *= len;
            s = (float)Math.Sin(rad);
            c = (float)Math.Cos(rad);
            t = 1 - c;
            a00 = a[0];
            a01 = a[1];
            a02 = a[2];
            a03 = a[3];
            a10 = a[4];
            a11 = a[5];
            a12 = a[6];
            a13 = a[7];
            a20 = a[8];
            a21 = a[9];
            a22 = a[10];
            a23 = a[11];
            // Construct the elements of the rotation matrix
            b00 = x * x * t + c;
            b01 = y * x * t + z * s;
            b02 = z * x * t - y * s;
            b10 = x * y * t - z * s;
            b11 = y * y * t + c;
            b12 = z * y * t + x * s;
            b20 = x * z * t + y * s;
            b21 = y * z * t - x * s;
            b22 = z * z * t + c;
            // Perform rotation-specific matrix multiplication
            res[0] = a00 * b00 + a10 * b01 + a20 * b02;
            res[1] = a01 * b00 + a11 * b01 + a21 * b02;
            res[2] = a02 * b00 + a12 * b01 + a22 * b02;
            res[3] = a03 * b00 + a13 * b01 + a23 * b02;
            res[4] = a00 * b10 + a10 * b11 + a20 * b12;
            res[5] = a01 * b10 + a11 * b11 + a21 * b12;
            res[6] = a02 * b10 + a12 * b11 + a22 * b12;
            res[7] = a03 * b10 + a13 * b11 + a23 * b12;
            res[8] = a00 * b20 + a10 * b21 + a20 * b22;
            res[9] = a01 * b20 + a11 * b21 + a21 * b22;
            res[10] = a02 * b20 + a12 * b21 + a22 * b22;
            res[11] = a03 * b20 + a13 * b21 + a23 * b22;

            //copy the unchanged last row
            res[12] = a[12];
            res[13] = a[13];
            res[14] = a[14];
            res[15] = a[15];
            return res;
        }
    }
}
