using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPGReader
{
    class PointDPPG
    {
        public int x { get; }
        public double y { get; }

        public PointDPPG(int a, double b)
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
