using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPGReader
{
    class DPPG
    {
        public PointDPPG[] points { get; }

        public DPPG(PointDPPG[] pointDPPGs, int n)
        {
            points = new PointDPPG[n];
            Array.Copy(pointDPPGs, points, n);
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
