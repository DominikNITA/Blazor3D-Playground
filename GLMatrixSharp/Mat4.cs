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
    }
}
