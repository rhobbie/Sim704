
namespace Sim704
{
    static class SenseLights
    {
        static bool[] Lights = new bool[4];
        public static void SLF() /* Sense light off */
        {
            for (int i = 0; i < 4; i++)
                Lights[i] = false;
        }
        public static void SLN(uint i) /* Sense light on */
        {
            Lights[i - 1] = true;
        }
        public static uint SLT(uint i) /* Sense light test */
        {
            bool skip = Lights[i - 1];
            Lights[i - 1] = false;
            return skip?1u:0;
        }
    }
}
