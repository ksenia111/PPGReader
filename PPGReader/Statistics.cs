using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPGReader
{
    class Statistics
    {
        public int CountStatPeriodsBegin = 2;
        public int CountStatPeriodsEnd = 10;
        public int CountStatCharacteristics = 6;
        public double[] IterationEnd;
        public double[] IterationA;
        public double[] IterationB;
        public double[] IterationC;
        public double[] IterationD;
        public double[] IterationE;
        

        public Statistics()
        {
            IterationEnd = new double[CountStatPeriodsEnd - 1];
            IterationA = new double[CountStatPeriodsEnd - 1];
            IterationB = new double[CountStatPeriodsEnd - 1];
            IterationC = new double[CountStatPeriodsEnd-1];
            IterationD = new double[CountStatPeriodsEnd-1];
            IterationE = new double[CountStatPeriodsEnd-1];
        }

    }
}
