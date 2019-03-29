using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPGReader
{
    class PPG
    {
        public PointPPG[] points { get; }
        //добавить массивы найденных характеристик

        public PPG(PointPPG [] pointPPGs, int n)
        {
            points = new PointPPG[n];
            Array.Copy(pointPPGs, points, n);
        }

        public int [] GetX()
        {
            return points.Select( p => p.GetX()).ToArray();
        }

        public int[] GetY()
        {
            return points.Select(p => p.GetY()).ToArray();
        }

        public void Mark10Periods()
        {

        }


    }
}
