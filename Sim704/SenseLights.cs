using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim704
{
    static class SenseLights
    {
        static bool[] Lights = new bool[4];
        public static void Off()
        {
            for (int i = 0; i < 4; i++)
                Lights[i] = false;
        }
        public static void On(int i)
        {
            Lights[i-1] = true;
        }
        public static bool Test(int i)
        {
            bool ret = Lights[i-1];
            Lights[i-1] = false;
            return ret;
        }
    }
}
