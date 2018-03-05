using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compute
{
    class RegressionRow
    {
        public double[] x;
        public double y = 0.0;
        public static int maxSize = 5;
        public static int Size = 5;
        public static bool[] use = new bool[maxSize];
        public static string[] tag = {
            "Constant",
            "Remuneration",
            "Size",
            "Winner",
            "GoodWinner"
        };
        public RegressionRow(double remuneration, double size, double isWinner, double isGoodWinner, double y)
        {
            x = new double[maxSize];
            x[0] = 1.0;
            x[1] = remuneration;
            x[2] = size;
            x[3] = isWinner;
            x[4] = isGoodWinner;
            int count = 0;
            for (int i = 0; i < maxSize; i++ ) {
                if (use[i]) {
                    x[count] = x[i];
                    count++;
                }
            }
            this.y = y;
        }
    }
}
