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

        public bool IsMax {get;set;} = false; // автосвойство, тут почитать можно подробнее https://metanit.com/sharp/tutorial/3.4.php
        public bool IsPeriod { get; set; } = false;
        /// <summary>
        /// точка перегиба ФПГ, обяз-я, находится между началом периода и точкой B, самый большой max ДФПГ на периоде
        /// </summary>
        public bool IsA { get; set; } = false;
        /// <summary>
        /// точка max-ой амплитуды ФПГ, обяз-я, самый большой max на периоде
        /// </summary>
        public bool IsB { get; set; } = false;
        /// <summary>
        /// точка амплитуды систолической волны, если есть D, то она обяз-я, нах-ся между B и D
        /// </summary>
        public bool IsС { get; set; } = false;
        /// <summary>
        /// точка амплитуды дикротического зубца, необяз-я, первый max после B
        /// </summary>
        public bool IsD { get; set; } = false;
        /// <summary>
        /// необяз-я, третий max после D
        /// </summary>
        public bool IsE { get; set; } = false;

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
