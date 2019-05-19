using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPGReader
{
    class DDPPG
    {
        public PointDDPPG[] points { get; }

        public DDPPG(PointDDPPG[] pointDDPPGs, int n)
        {
            points = new PointDDPPG[n];
            Array.Copy(pointDDPPGs, points, n);
        }

        public int[] GetX()
        {
            return points.Select(p => p.GetX()).ToArray();
        }

        public double[] GetY()
        {
            return points.Select(p => p.GetY()).ToArray();
        }
    }
}
