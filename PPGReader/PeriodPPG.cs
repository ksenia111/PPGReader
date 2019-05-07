using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPGReader
{
    class PeriodPPG
    {
        public int begin;
        public int end;
        public int A;
        public int B;
        public int C;
        public int D;
        public int E;

        public PeriodPPG()
        {
            begin = 0;
            end = 0;
            A = 0;
            B = 0;
            C = 0;
            D = 0;
            E = 0;
        }

        public int Length()
        {
            return end - begin;
        }


    }
}
