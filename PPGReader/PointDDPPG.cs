using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPGReader
{
    class PointDDPPG
    {
        public int x { get; }
        public double y { get; }

        public PointDDPPG(int a, double b)
        {
            x = a;
            y = b;
        }

        public int GetX()
        {
            return x;
        }

        public double GetY()
        {
            return y;
        }
    }
}
