using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim704
{
    static class CoreMemory
    {
        public static ulong[] Mem;
        public static uint AdrMask;
        public static void Init(int Size)
        {
            switch (Size)
            {
                case 4:
                case 8:
                case 32:
                    Mem = new ulong[Size * 1024];
                    AdrMask = (uint)(Mem.Length - 1);
                    WA.SetMask(AdrMask);
                    break;
                default:
                    throw new InvalidOperationException("Wong Mem Size");
            }
        }
        public static W36 C(WA Y)
        {
            return (W36)Mem[Y];
        }
        public static void C(WA Y, W36 V)
        {
            Mem[Y] = V;
        }
        public static void Clear()
        {
            Array.Clear(Mem, 0, Mem.Length);
        }
    }
}
