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
        public int CountPeriods = 0;
        public int CountStatCharacteristics = 7;
        public StatisticPoint[] LenghtPeriod;
        public StatisticPoint[] End;
        public StatisticPoint[] A;
        public StatisticPoint[] B;
        public StatisticPoint[] C;
        public StatisticPoint[] D;
        public StatisticPoint[] E;
       

        public Statistics()
        {
            LenghtPeriod = new StatisticPoint[CountStatPeriodsEnd - 1];
            End = new StatisticPoint[CountStatPeriodsEnd - 1];
            A = new StatisticPoint[CountStatPeriodsEnd - 1];
            B = new StatisticPoint[CountStatPeriodsEnd - 1];
            C = new StatisticPoint[CountStatPeriodsEnd - 1];
            D = new StatisticPoint[CountStatPeriodsEnd - 1];
            E = new StatisticPoint[CountStatPeriodsEnd - 1];
            for(int i =0;i< CountStatPeriodsEnd - 1;i++)
            {
                LenghtPeriod[i] = new  StatisticPoint();
                End[i] = new StatisticPoint();
                A[i] = new StatisticPoint();
                B[i] = new StatisticPoint();
                C[i] = new StatisticPoint();
                D[i] = new StatisticPoint();
                E[i] = new StatisticPoint();
            }

        }

      
    }

    class StatisticPoint
    {
        const int countColumn = 10;
        public double[] Average;
        public double[] Min;
        public double[] Max;
        public double[] Dispersion;

        public StatisticPoint()
        {
            Average = new double[countColumn];
            Min = new double[countColumn];
            Max = new double[countColumn];
            Dispersion = new double[countColumn];
        }

        public void SetStatistics(int countColumn)
        {
            Array.Resize(ref Average,countColumn);
            Array.Resize(ref Min, countColumn);
            Array.Resize(ref Max, countColumn);
            Array.Resize(ref Dispersion, countColumn);
        }
    }
}
