using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPGReader
{
    class PointDPPG
    {
        int x;
        int y;
        bool isMax = false;

        public PointDPPG(int a, int b)
        {
            x = a;
            y = b;
        }

        public int GetX()
        {
            return x;
        }

        public int GetY()
        {
            return y;
        }
    }
}
