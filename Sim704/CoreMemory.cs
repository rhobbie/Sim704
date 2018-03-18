using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim704
{
    static class CoreMemory
    {
        public static long[] Mem;
        public static int AdrMask;
        public static W36 GetW(int Adr)
        {
            return new W36() { W = Mem[Adr & AdrMask] };
        }
        public static void SetW(int Adr,W36 V)
        {
            Mem[Adr & AdrMask] = V.W;
        }
        public static void Init(int Size)
        {
            switch(Size)
            {
                case 4:
                case 8:
                case 32:                    
                    Mem = new long[Size * 1024];
                    AdrMask = Mem.Length - 1;
                    break;
                default:
                    throw new InvalidOperationException("Wong Mem Size");
            }
        }
        public static void Clear()
        {
            Array.Clear(Mem, 0, Mem.Length);
        }
    }
}
