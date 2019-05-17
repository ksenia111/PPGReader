using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPGReader
{
    class PeriodPPG
    {
        public int Begin;
        public int End;
        public int A;
        public int B;
        public int C;
        public int D;
        public int E;

        public int IterationBegin= 0;
        public int IterationEnd = 0;
        public int IterationA = 0;
        public int IterationB = 0;
        public int IterationC = 0;
        public int IterationD = 0;
        public int IterationE = 0;

        public PeriodPPG()
        {
            Begin = 0;
            End = 0;
            A = 0;
            B = 0;
            C = 0;
            D = 0;
            E = 0;
        }

        public int Length()
        {
            return End - Begin;
        }

    }
}
