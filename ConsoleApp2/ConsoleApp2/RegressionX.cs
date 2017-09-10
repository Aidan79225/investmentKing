using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compute
{
    class RegressionX
    {
//        public double first = 1.0;
//        public double remuneration = 0.0;
//        public double size = 0.0;
//        public double isWinner = 0.0;
//        public double isGoodWinner = 0.0;
        public double[] x;
        public double y = 0.0;
        public static int Size = 5; 
        public RegressionX(double remuneration, double size, double isWinner, double isGoodWinner, double y)
        {
            x = new double[Size];
            x[0] = 1.0;
            x[1] = remuneration;
            x[2] = size;
            x[3] = isWinner;
            x[4] = isGoodWinner;
            this.y = y;
        }
    }
}
