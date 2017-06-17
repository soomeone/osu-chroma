using Corale.Colore.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osu_Chroma
{
    class TimingPoint
    {
        public int Time;
        public int Color;

        public TimingPoint(int time, int color)
        {
            this.Time = time;
            this.Color = color;
        }
    }
}
