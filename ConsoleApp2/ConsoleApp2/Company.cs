using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    public class Company : IComparable<Company>
    {
        double compare = 0;
        double rateOfReturn = -1;
        double[] companyPrice;
        int initIndex = 0;
        int[] top;
        public Company(double[] companyPrice, int observation, int holding, int skip)
        {
            this.companyPrice = companyPrice;
            computeCompareValue(this.companyPrice, observation);
            computeRateOfReturn(this.companyPrice, observation, holding, skip);
        }
        public void reload(int initIndex, int observation, int holding, int skip)
        {
            this.initIndex = initIndex;
            computeCompareValue(this.companyPrice, observation);
            computeRateOfReturn(this.companyPrice, observation, holding, skip);
        }

        public void setTop(int[] top)
        {
            this.top = top;
        }
        public int getTop(int index)
        {
            return top[index];
        }
        public bool hasTop()
        {
            return top != null && top.Count() > 0;
        }
        int IComparable<Company>.CompareTo(Company other)
        {
            return compare - other.compare > 0 ? 1 : compare - other.compare == 0 ? 0 : -1;
        }

        private void computeCompareValue(double[] companyPrice, int observation)
        {
            double max = 0;
            int size = Math.Min(observation + initIndex, companyPrice.Length);
            for (int i = initIndex; i < size; i++)
            {
                if (companyPrice[i] > max)
                {
                    max = companyPrice[i];
                }
            }
            compare = companyPrice[initIndex + observation - 1] / max;
        }
        private void computeRateOfReturn(double[] companyPrice, int observation, int holding, int skip)
        {
            int first = Math.Min((initIndex + observation + skip - 1), companyPrice.Length);
            int last = Math.Min((initIndex + observation + skip + holding - 1), companyPrice.Length);
            rateOfReturn = (companyPrice[last] - companyPrice[first]) / companyPrice[first];

        }
        public double getRate()
        {
            return rateOfReturn;
        }
        public string getString()
        {
            return "compare :" + compare + ", rate : " + rateOfReturn;
        }
        public bool hasZero(int observation)
        {
            int size = Math.Min(companyPrice.Length, observation + initIndex);
            for (int i = initIndex; i < size; i++)
            {
                if (companyPrice[i] == 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
