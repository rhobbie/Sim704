using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim704
{
    static class SenseSwitches
    {
        static bool[] Switches = new bool[6];
        public static void Init(bool[] init)
        {
            if (init != null)
                for (int i = 0; i < 6; i++)
                    Switches[i] = init[i];
        }
        public static int Test(uint unit)
        {
            return Switches[unit - 1] ? 1 : 0;
        }
    }
}
