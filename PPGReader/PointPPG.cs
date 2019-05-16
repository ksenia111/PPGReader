using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPGReader
{
    class PointPPG
    {
        public int x { get;}
        public int y { get;}

        public PointPPG(int  a, int  b)
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
