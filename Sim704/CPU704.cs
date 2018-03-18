using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim704
{
    struct W1
    {
        const int wlen = 1; /* Word length */
        const uint wmask = (1U << wlen) - 1U; /* mask for word */

        uint w; /* 1 bit Word stored in 32 bit uint*/

        public W1(uint value)
        {
            w = value & wmask;
        }
        public W1(ulong value)
        {
            w = (uint)value & wmask;
        }
        public static implicit operator uint(W1 d)
        {
            return d.w;
        }
        public static implicit operator ulong(W1 d)
        {
            return d.w;
        }
        public static explicit operator W1(uint d)
        {
            return new W1(d);
        }
        public static explicit operator W1(ulong d)
        {
            return new W1(d);
        }
        public override string ToString()
        {
            return Convert.ToString(w);
        }
    }
    struct W3
    {
        const int wlen = 3; /* Word length */
        const uint wmask = (1U << wlen) - 1U; /* mask for word */

        uint w; /* 3 bit Word stored in 32 bit uint*/

        public W3(uint value)
        {
            w = value & wmask;
        }
        public W3(ulong value)
        {
            w = (uint)value & wmask;
        }
        public static implicit operator uint(W3 d)
        {
            return d.w;
        }
        public static implicit operator ulong(W3 d)
        {
            return d.w;
        }
        public static explicit operator W3(uint d)
        {
            return new W3(d);
        }
        public static explicit operator W3(ulong d)
        {
            return new W3(d);
        }
        public override string ToString()
        {
            return Convert.ToString(w);
        }
    }
    struct W15
    {
        const int wlen = 15; /* Word length */
        const uint wmask = (1U << wlen) - 1U; /* mask for word */

        uint w; /* 15 bit Word stored in 32 bit uint*/

        public W15(uint value)
        {
            w = value & wmask;
        }
        public W15(ulong value)
        {
            w = (uint)value & wmask;
        }
        public static implicit operator uint(W15 d)
        {
            return d.w;
        }
        public static implicit operator ulong(W15 d)
        {
            return d.w;
        }
        public static explicit operator W15(uint d)
        {
            return new W15(d);
        }
        public static explicit operator W15(ulong d)
        {
            return new W15(d);
        }
        public override string ToString()
        {
            return Convert.ToString(w, 8).PadLeft(5, '0');
        }
    }
    struct W35
    {
        const int wlen = 35; /* Word length */
        const ulong wmask = (1UL << wlen) - 1UL; /* mask for word */

        ulong w; /* 35 bit Word stored in 64 bit ulong*/

        public W35(ulong value)
        {
            w = value & wmask;
        }
        public static implicit operator ulong(W35 d)
        {
            return d.w;
        }
        public static explicit operator W35(ulong d)
        {
            return new W35(d);
        }
        public override string ToString()
        {
            return Convert.ToString((long)w, 8).PadLeft(12, '0');
        }
    }
    struct W36
    {
        const int alen = 15; /* Adress length */
        const int tlen = 3; /* Tag length */
        const int dlen = 15; /* Decrement length */
        const int plen = 3; /* Prefix length */
        const int wlen = alen + tlen + dlen + plen; /* Word length */
        const int slen = 1; /* Sign length */
        const int mlen = wlen - slen; /* Magnitude length */

        const int apos = 0; /* bit pos for Adress*/
        const int tpos = alen; /* bit pos for Tag*/
        const int dpos = tpos + tlen; /* bit pos for Decrement*/
        const int ppos = dpos + dlen; /* bit pos for Prefix*/
        const int mpos = 0;           /* bit pos for Magniude */
        const int spos = wlen - slen; /* bit pos for Sign */

        const ulong a0mask = (1UL << alen) - 1UL; /* mask for Adress at bit 0*/
        const ulong t0mask = (1UL << tlen) - 1UL; /* mask for Tag at bit 0*/
        const ulong d0mask = (1UL << dlen) - 1UL; /* mask for Decrement at bit 0 */
        const ulong p0mask = (1UL << plen) - 1UL; /* mask for Prefix at bit 0 */
        const ulong wmask = (1UL << wlen) - 1UL;  /* mask for Word */
        const ulong m0mask = (1UL << mlen) - 1UL; /* mask for Magnitude at bit 0*/
        const ulong s0mask = (1UL << slen) - 1UL; /* mask for Sign at bit 0 */

        const ulong awmask = a0mask << apos; /* mask for Adress in word */
        const ulong twmask = t0mask << tpos; /* mask for Tag in word */
        const ulong dwmask = d0mask << dpos; /* mask for Decrement in word */
        const ulong pwmask = p0mask << ppos; /* mask for Prefix in word */
        const ulong mwmask = m0mask << mpos; /* mask for Magnitude in word  */
        const ulong swmask = s0mask << spos; /* mask for Sign in word */

        const ulong iawmask = (wmask ^ awmask); /* inv mask for Adress in word */
        const ulong itwmask = (wmask ^ twmask); /* inv mask for Tag in word  */
        const ulong idwmask = (wmask ^ dwmask); /* inv mask for Decrement in word  */
        const ulong ipwmask = (wmask ^ pwmask); /* inv mask for Prefix in word  */
        const ulong imwmask = (wmask ^ mwmask); /* inv mask for Magnitude in word  */
        const ulong iswmask = (wmask ^ swmask); /* inv mask for Sign in word  */
        ulong w; /* 36 bit Word stored in 64 bit ulong*/

        public W36(ulong value)
        {
            w = value & wmask;
        }
        public static implicit operator ulong(W36 d)
        {
            return d.w;
        }
        public static explicit operator W36(ulong d)
        {
            return new W36(d);
        }
        public W15 A  /* Adress 15 bit */
        {
            get
            {
                return (W15)w;
            }
            set
            {
                w = (w & iawmask) | value;
            }
        }
        public W3 T  /* Tag 3 bit */
        {
            get
            {
                return (W3)(w >> tpos);
            }
            set
            {
                w = (w & itwmask) | ((ulong)value << tpos);
            }
        }
        public W15 D  /* Decrement 15 bit */
        {
            get
            {
                return (W15)(w >> dpos);
            }
            set
            {
                w = (w & idwmask) | ((ulong)value << dpos);
            }
        }
        public W3 P  /* Prefix 3 bit */
        {
            get
            {
                return (W3)(w >> ppos);
            }
            set
            {
                w = (w & ipwmask) | ((ulong)value << ppos);
            }
        }
        public W35 M  /* Magnitude 35 bit */
        {
            get
            {
                return (W35)w;
            }
            set
            {
                w = (w & imwmask) | value;
            }
        }
        public W1 S  /* Sign 1 bit*/
        {
            get
            {
                return (W1)(w >> spos);
            }
            set
            {
                w = (w & iswmask) | ((ulong)value << spos);
            }
        }
        public override string ToString()
        {
            return Convert.ToString((long)w, 8).PadLeft(12, '0');
        }
    }
    struct W37
    {
        const int wlen = 37; /* Word length */
        const ulong wmask = (1UL << wlen) - 1UL; /* mask for word */

        ulong w; /* 37 bit Word stored in 64 bit ulong*/

        public W37(ulong value)
        {
            w = value & wmask;
        }
        public static implicit operator ulong(W37 d)
        {
            return d.w;
        }
        public static explicit operator W37(ulong d)
        {
            return new W37(d);
        }
        public override string ToString()
        {
            return Convert.ToString((long)w, 8).PadLeft(13, '0');
        }
    }
    struct W38
    {
        const int alen = 15; /* Adress length */
        const int tlen = 3; /* Tag length */
        const int dlen = 15; /* Decrement length */
        const int prlen = 3; /* Prefix length */
        const int slen = 1; /* Sign length */
        const int m36len = alen + tlen + dlen + prlen; /* 36Bit Magnitude length */
        const int m35len = m36len - slen; /* 35Bit Magnitude length */
        const int pblen = 1; /* Lenght for P Bit */
        const int qlen = 1; /* Length of Q BIT */
        const int wlen = m35len + pblen + qlen + slen; /* Word length */
        const int m37len = wlen - slen; /* 37Bit Magnitude length */

        const int apos = 0; /* bit pos for Adress*/
        const int tpos = apos + alen; /* bit pos for Tag*/
        const int dpos = tpos + tlen; /* bit pos for Decrement*/
        const int prpos = dpos + dlen; /* bit pos for Prefix*/
        const int qpos = prpos + prlen; /* bit pos for Q Bit*/
        const int spos = qpos + qlen; /* bit pos for Sign */
        const int pbpos = qpos - pblen; /* bit pos for P Bit */
        const int m35pos = 0;           /* bit pos for 35 Bit Magniude */
        const int m36pos = 0;           /* bit pos for 36 Bit Magniude */
        const int m37pos = 0;           /* bit pos for 37 Bit Magniude */

        const ulong a0mask = (1UL << alen) - 1UL; /* mask for Adress at bit 0*/
        const ulong t0mask = (1UL << tlen) - 1UL; /* mask for Tag at bit 0*/
        const ulong d0mask = (1UL << dlen) - 1UL; /* mask for Decrement at bit 0 */
        const ulong pr0mask = (1UL << prlen) - 1UL; /* mask for Prefix at bit 0 */
        const ulong wmask = (1UL << wlen) - 1UL;  /* mask for Word */
        const ulong m350mask = (1UL << m35len) - 1UL; /* mask for 35 Bit Magnitude at bit 0*/
        const ulong m360mask = (1UL << m36len) - 1UL; /* mask for 36 Bit Magnitude at bit 0*/
        const ulong m370mask = (1UL << m37len) - 1UL; /* mask for 37 Bit Magnitude at bit 0*/
        const ulong s0mask = (1UL << slen) - 1UL; /* mask for Sign at bit 0 */
        const ulong q0mask = (1UL << qlen) - 1UL;/* mask for Q Bit at bit 0 */
        const ulong pb0mask = (1UL << pblen) - 1UL;/* mask for P Bit at bit 0 */

        const ulong awmask = a0mask << apos; /* mask for Adress in word */
        const ulong twmask = t0mask << tpos; /* mask for Tag in word */
        const ulong dwmask = d0mask << dpos; /* mask for Decrement in word */
        const ulong prwmask = pr0mask << prpos; /* mask for Prefix in word */
        const ulong m35wmask = m350mask << m35pos; /* mask for 35 Bit Magnitude in word  */
        const ulong m36wmask = m360mask << m36pos; /* mask for 36 Bit Magnitude in word  */
        const ulong m37wmask = m370mask << m37pos; /* mask for 37 Bit Magnitude in word  */
        const ulong swmask = s0mask << spos; /* mask for Sign in word */
        const ulong qwmask = q0mask << qpos; /* mask for Q Bit in word */
        const ulong pbwmask = pb0mask << pbpos; /* mask for P Bit in word */


        const ulong iawmask = (wmask ^ awmask); /* inv mask for Adress in word */
        const ulong itwmask = (wmask ^ twmask); /* inv mask for Tag in word  */
        const ulong idwmask = (wmask ^ dwmask); /* inv mask for Decrement in word  */
        const ulong iprwmask = (wmask ^ prwmask); /* inv mask for Prefix in word  */
        const ulong im35wmask = (wmask ^ m35wmask); /* inv mask for 35 Bit Magnitude in word  */
        const ulong im36wmask = (wmask ^ m36wmask); /* inv mask for 36 Bit Magnitude in word  */
        const ulong im37wmask = (wmask ^ m37wmask); /* inv mask for 37 Bit Magnitude in word  */
        const ulong iswmask = (wmask ^ swmask); /* inv mask for Sign in word  */
        const ulong iqwmask = (wmask ^ qwmask); /* inv mask for Q Bit in word  */
        const ulong ipbwmask = (wmask ^ pbwmask); /* inv mask for P Bit in word  */

        ulong w; /* 38 bit Word stored in 64 bit ulong*/

        public W38(ulong value)
        {
            w = value & wmask;
        }
        public static implicit operator ulong(W38 d)
        {
            return d.w;
        }
        public static explicit operator W38(ulong d)
        {
            return new W38(d);
        }
        public W15 A  /* Adress 15 bit */
        {
            get
            {
                return (W15)w;
            }
            set
            {
                w = (w & iawmask) | value;
            }
        }
        public W3 T  /* Tag 3 bit */
        {
            get
            {
                return (W3)(w >> tpos);
            }
            set
            {
                w = (w & itwmask) | ((ulong)value << tpos);
            }
        }
        public W15 D  /* Decrement 15 bit */
        {
            get
            {
                return (W15)(w >> dpos);
            }
            set
            {
                w = (w & idwmask) | ((ulong)value << dpos);
            }
        }
        public W3 PR  /* Prefix 3 bit */
        {
            get
            {
                return (W3)(w >> prpos);
            }
            set
            {
                w = (w & iprwmask) | ((ulong)value << prpos);
            }
        }
        public W1 PB  /* P Bit 1 bit*/
        {
            get
            {
                return (W1)(w >> pbpos);
            }
            set
            {
                w = (w & ipbwmask) | ((ulong)value << pbpos);
            }
        }
        public W1 Q  /* Q Bit 1 bit*/
        {
            get
            {
                return (W1)(w >> qpos);
            }
            set
            {
                w = (w & iqwmask) | ((ulong)value << qpos);
            }
        }
        public W35 M35  /* Magnitude 35 bit */
        {
            get
            {
                return (W35)w;
            }
            set
            {
                w = (w & im35wmask) | value;
            }
        }
        public W36 M36  /* Magnitude 36 bit */
        {
            get
            {
                return (W36)w;
            }
            set
            {
                w = (w & im36wmask) | value;
            }
        }
        public W37 M37  /* Magnitude 37 bit */
        {
            get
            {
                return (W37)w;
            }
            set
            {
                w = (w & im37wmask) | value;
            }
        }
        public W1 S  /* Sign 1 bit*/
        {
            get
            {
                return (W1)(w >> spos);
            }
            set
            {
                w = (w & iswmask) | ((ulong)value << spos);
            }
        }
        public override string ToString()
        {
            return Convert.ToString((long)w, 8).PadLeft(13, '0');
        }
    }
    struct WA
    {

        public static uint wmask; /* mask for word */

        uint w; /* 15 bit Word stored in 32 bit uint*/

        public WA(uint value)
        {
            w = value & wmask;
        }
        public WA(ulong value)
        {
            w = (uint)value & wmask;
        }
        public static implicit operator uint(WA d)
        {
            return d.w;
        }
        public static implicit operator ulong(WA d)
        {
            return d.w;
        }
        public static explicit operator WA(uint d)
        {
            return new WA(d);
        }
        public static explicit operator WA(ulong d)
        {
            return new WA(d);
        }
        public override string ToString()
        {
            return Convert.ToString(w, 8).PadLeft(5, '0');
        }
    }

    static class CPU704
    {
        static bool trapping = false;
        static bool acoflag = false;
        public static WA IC; /* Instruction Counter */
        public static WA NIC; /* Next Instruction Counter */
        public static W38 AC;
        public static W36 MQ;
        static WA[] X = new WA[3];
        public static bool halt = false;
        public static bool repeat = false;
        static public void Clr()
        {
            AC = (W38)0;
            MQ = (W36)0;
            X[0] = (WA)0;
            X[1] = (WA)0;
            X[2] = (WA)0;
            IC = (WA)0;
            CoreMemory.Clear();
        }
        static public void LoadCrd()
        {

            Io704.RDS(13 * 16 + 1);
            Io704.CPY(ref CoreMemory.Mem[0]);
            Io704.CPY(ref CoreMemory.Mem[1]);

            IC = (WA)0;
            Go(false);
        }
        static public void LoadTape()
        {

            Io704.RDS(9 * 16 + 1);
            Io704.CPY(ref CoreMemory.Mem[0]);
            Io704.CPY(ref CoreMemory.Mem[1]);
            IC = (WA)0;
            Go(false);
        }
        static public void LoadDrm()
        {
            Io704.RDS(12 * 16 + 1);
            Io704.CPY(ref CoreMemory.Mem[0]);
            Io704.CPY(ref CoreMemory.Mem[1]);
            IC = (WA)0;
            Go(false);
        }
        static public void Go(bool step)
        {
            do
            {
                repeat = false;
                Step();
                if (halt)
                {
                    Console.WriteLine("HALT at {0}", IC);
                    string l = Console.ReadLine();
                    if (l == "g")
                        halt = false;
                    else if (l == "x")
                        break;
                    if (repeat)
                        NIC = IC;
                }
                else if (step)
                {
                    Console.ReadLine();
                }
                IC = NIC;
            }
            while (true);
        }
        static void SetX(W3 T, WA A)
        {
            if (0 != (T & 1))
                X[0] = A;
            if (0 != (T & 2))
                X[1] = A;
            if (0 != (T & 4))
                X[2] = A;
        }
        static WA GetX(W3 T)
        {
            uint A = 0;

            if (0 != (T & 1))
                A |= X[0];
            if (0 != (T & 2))
                A |= X[1];
            if (0 != (T & 4))
                A |= X[2];
            return (WA)A;
        }
        static WA GetY(W36 SR) /* Get Y*/
        {
            return (WA)(SR.A - GetX(SR.T));
        }
        static W36 LoadCY(W36 SR) /* Load C(Y) */
        {
            return CoreMemory.GetW((WA)(SR.A - GetX(SR.T)));
        }
        static void StoreCY(W36 SR, W36 V) /* Store V to C(Y) */
        {
            CoreMemory.SetW((WA)(SR.A - GetX(SR.T)), V);
        }
        static void WriteAT(W36 SR)
        {
            if (SR.T != 0)
            {
                Console.WriteLine(" {0},{1}", (uint)SR.A, (uint)SR.T);
            }
            else
            {
                Console.WriteLine(" {0}", (uint)SR.A);
            }
        }
        static void WriteATD(W36 SR)
        {
            short d = (short)(SR.D << 1);
            Console.WriteLine(" {0},{1},{2}", (uint)SR.A, (uint)SR.T, d >> 1);
        }
        static void ADD(W36 SR)
        {
            bool f1 = false; /* True Signs of AC and SR differs */

            W1 f2 = AC.S; /* Sign of AC */
            W1 f8 = AC.PB; /* P of in AC */

            /* Make AC Positive  */
            AC.S = (W1)0;

            /* Check signes of SR & AC */
            if (SR.S != f2)
            {
                AC.M37 = (W37)(~AC); /* One's compliment */
                f1 = true;
            }
            AC = (W38)(AC + SR.M);

            /* Check carry from Q */
            if (f1)
            {    /* Check if signs were not same */
                if (0 != AC.S)
                {
                    f2 = (W1)(~f2);
                    AC = (W38)(AC + 1);
                    if (AC.PB != f8)
                        acoflag = true;
                }
                else
                    AC.M37 = (W37)(~AC); /* One's compliment */
            }
            else if (AC.PB != f8)
                acoflag = true;
            /* Restore sign to AC */
            AC.S = f2;
        }
        static public void Step()
        {

            W36 SR = CoreMemory.GetW(IC);

            Console.Write("{0} {1} {2} {3} {4} {5} {6} ", IC, SR, AC, MQ, X[0], X[1], X[2]);
            /* check if Type A or Type B instruction */
            uint S = SR.S;
            uint P = (uint)(SR.M >> 33);
            switch (P)
            {
                case 0:
                    /* Type B instruction */
                    P = (uint)(SR.M >> 24);
                    switch (P)
                    {
                        case 0: /*0*/
                            if (S == 0) /*+000 HTR Hat and Transfer*/
                            {
                                Console.Write("HTR");
                                WriteAT(SR);
                                halt = true;
                                NIC = GetY(SR);
                            }
                            else
                            {
                                Console.WriteLine("Operation {0}{1} not implemented ->{2}", S != 0 ? '-' : '+', Convert.ToString(P, 8).PadLeft(3, '0'), P);
                                halt = true;
                            }
                            break;
                        case 16: /*20*/
                            if (S == 0) /*+20 TRA Transfer*/
                            {
                                Console.Write("TRA");
                                WriteAT(SR);
                                NIC = GetY(SR);
                            }
                            else
                            {
                                Console.WriteLine("Operation {0}{1} not implemented ->{2}", S != 0 ? '-' : '+', Convert.ToString(P, 8).PadLeft(3, '0'), P);
                                halt = true;
                            }
                            break;
                        case 60: /*74*/
                            if (S == 0) /*+74 TSX Transfer and set index */
                            {
                                Console.Write("TSX");
                                WriteAT(SR);
                                SetX(SR.T, (WA)(0U - IC));
                                NIC = (WA)(uint)SR.A;
                            }
                            else
                            {
                                Console.WriteLine("Operation {0}{1} not implemented ->{2}", S != 0 ? '-' : '+', Convert.ToString(P, 8).PadLeft(3, '0'), P);
                                halt = true;
                            }
                            break;
                        case 64: /*100*/
                            if (S == 0) /*+100 TZE Transfer on Zero*/
                            {
                                Console.Write("TZE");
                                WriteAT(SR);
                                if (AC.M37 == 0)
                                    NIC = GetY(SR);
                                else
                                    NIC = (WA)(IC + 1);
                            }
                            else   /*+100 TZE Transfer on No Zero*/
                            {
                                Console.Write("TNZ");
                                WriteAT(SR);
                                if (AC.M37 != 0)
                                    NIC = GetY(SR);
                                else
                                    NIC = (WA)(IC + 1);
                            }
                            break;
                        case 80: /*120*/
                            if (S == 0)
                            {
                                Console.WriteLine("Operation {0}{1} not implemented ->{2}", S != 0 ? '-' : '+', Convert.ToString(P, 8).PadLeft(3, '0'), P);
                                halt = true;
                            }
                            else /*-120 TMI Transfer on Minus*/
                            {
                                Console.Write("TZE");
                                WriteAT(SR);
                                if (AC.S != 0)
                                    NIC = GetY(SR);
                                else
                                    NIC = (WA)(IC + 1);
                            }
                            break;
                        case 208: /*320*/
                            if (S == 0) /*-320 ANS AND to Storage*/
                            {
                                Console.Write("ANS");
                                WriteAT(SR);
                                StoreCY(SR, (W36)(AC & LoadCY(SR)));
                                NIC = (WA)(IC + 1);
                            }
                            else /*-320 ANA AND to Accumulator*/
                            {
                                Console.Write("ANA");
                                WriteAT(SR);
                                AC = (W38)(AC & LoadCY(SR));
                                NIC = (WA)(IC + 1);
                            }
                            break;
                        case 224: /*340*/
                            if (S == 0) /*+340 CAS Compare Accumulator with Storage*/
                            {
                                Console.Write("CAS");
                                WriteAT(SR);
                                W36 tmp = LoadCY(SR);
                                int skip = 0;
                                if (0 != AC.S)
                                {
                                    if (0 != tmp.S)
                                    {
                                        if (AC.M37 == tmp.M)
                                            skip = 1;
                                        else if (AC.M37 > tmp.M)
                                            skip = 2;
                                    }
                                    else
                                        skip = 2;
                                }
                                else if (0 == tmp.S)
                                {
                                    if (AC.M37 == tmp.M)
                                        skip = 1;
                                    else if (AC.M37 < tmp.M)
                                        skip = 2;
                                }
                                NIC = (WA)(IC + 1 + skip);
                            }
                            else
                            {
                                Console.WriteLine("Operation {0}{1} not implemented ->{2}", S != 0 ? '-' : '+', Convert.ToString(P, 8).PadLeft(3, '0'), P);
                                halt = true;
                            }
                            break;
                        case 241: /*361*/
                            if (S == 0) /*+361 ACL Add and Carry Logical Word*/
                            {
                                Console.Write("ACL");
                                WriteAT(SR);
#if QisZeroAtACL
                                AC.M37= (W37)(AC.M36 + LoadCY(SR));
                                if (0 != AC.Q)
                                {
                                    AC.Q = (W1)0;
                                    AC.M37 = (W37)(AC.M37 + 1);
                                }
#else
                                W38 tmp = new W38() { M37 = (W37)(AC.M36 + LoadCY(SR)) };
                                if (0 != tmp.Q)
                                    tmp.M37 = (W37)(tmp.M37 + 1);
                                AC.M36 = tmp.M36;
#endif
                                NIC = (WA)(IC + 1);
                            }
                            else
                            {
                                Console.WriteLine("Operation {0}{1} not implemented ->{2}", S != 0 ? '-' : '+', Convert.ToString(P, 8).PadLeft(3, '0'), P);
                                halt = true;
                            }
                            break;
                        case 256: /*400*/
                            if (S == 0) /*+400 ADD Add */
                            {
                                Console.Write("ADD");
                                WriteAT(SR);
                                ADD(LoadCY(SR));
                                NIC = (WA)(IC + 1);
                            }
                            else
                            {
                                Console.WriteLine("Operation {0}{1} not implemented ->{2}", S != 0 ? '-' : '+', Convert.ToString(P, 8).PadLeft(3, '0'), P);
                                halt = true;
                            }
                            break;
                        case 257: /*401*/
                            if (S == 0) /*+401 ADM Add Magnitude */
                            {
                                Console.Write("ADM");
                                WriteAT(SR);
                                ADD((W36)(ulong)(LoadCY(SR).M));
                                NIC = (WA)(IC + 1);
                            }
                            else
                            {
                                Console.WriteLine("Operation {0}{1} not implemented ->{2}", S != 0 ? '-' : '+', Convert.ToString(P, 8).PadLeft(3, '0'), P);
                                halt = true;
                            }
                            break;
                        case 258: /*402 */
                            if (S == 0) /*+402 SUB*/
                            {
                                Console.Write("SUB");
                                WriteAT(SR);
                                W36 tmp = LoadCY(SR);
                                tmp.S = (W1)~tmp.S;
                                ADD(tmp);
                                NIC = (WA)(IC + 1);
                            }
                            else
                            {
                                Console.WriteLine("Operation {0}{1} not implemented ->{2}", S != 0 ? '-' : '+', Convert.ToString(P, 8).PadLeft(3, '0'), P);
                                halt = true;
                            }
                            break;
                        case 320: /*500*/
                            if (S == 0) /*+500 CLA Clear and ADD */
                            {
                                Console.Write("CLA");
                                WriteAT(SR);
                                W36 tmp = LoadCY(SR);
                                AC = (W38)0;
                                AC.M35 = tmp.M;
                                AC.S = tmp.S;
                                NIC = (WA)(IC + 1);
                            }
                            else /*-500 CAL Clear and ADD logical Word */
                            {
                                Console.Write("CAL");
                                WriteAT(SR);
                                AC = (W38)(ulong)LoadCY(SR);
                                NIC = (WA)(IC + 1);
                            }
                            break;
                        case 321: /*501*/
                            if (S == 0) /*+500 CLA Clear and ADD */
                            {
                                Console.WriteLine("Operation {0}{1} not implemented ->{2}", S != 0 ? '-' : '+', Convert.ToString(P, 8).PadLeft(3, '0'), P);
                                halt = true;
                            }
                            else /*-501 ORA Or to Accumulator*/
                            {
                                Console.Write("ORA");
                                WriteAT(SR);
                                AC = (W38)(AC | LoadCY(SR));
                                NIC = (WA)(IC + 1);
                            }
                            break;
                        case 348: /*534*/
                            if (S == 0) /*+534 LXA Load Index from Address*/
                            {
                                Console.Write("LXA");
                                WriteAT(SR);
                                SetX(SR.T, (WA)(uint)CoreMemory.GetW((WA)(uint)SR.A).A);
                                NIC = (WA)(IC + 1);
                            }
                            else   /* -534 LXD Load Index from Decrement */
                            {
                                Console.Write("LXD");
                                WriteAT(SR);
                                SetX(SR.T, (WA)(uint)CoreMemory.GetW((WA)(uint)SR.A).D);
                                NIC = (WA)(IC + 1);
                            }
                            break;
                        case 368: /*560*/
                            if (S == 0) /*+560 LDQ Load MQ*/
                            {
                                Console.Write("LDQ");
                                WriteAT(SR);
                                MQ = LoadCY(SR);
                                NIC = (WA)(IC + 1);
                            }
                            else
                            {
                                Console.WriteLine("Operation {0}{1} not implemented ->{2}", S != 0 ? '-' : '+', Convert.ToString(P, 8).PadLeft(3, '0'), P);
                                halt = true;
                            }
                            break;

                        case 384: /*600*/
                            if (S == 0)
                            {
                                Console.WriteLine("Operation {0}{1} not implemented ->{2}", S != 0 ? '-' : '+', Convert.ToString(P, 8).PadLeft(3, '0'), P);
                                halt = true;
                            }
                            else /*-600 STQ Store MQ*/
                            {
                                Console.Write("STQ");
                                WriteAT(SR);
                                StoreCY(SR, MQ);
                                NIC = (WA)(IC + 1);
                            }
                            break;
                        case 386: /*602*/
                            if (S == 0) /*+602 SLW Store Logical Word*/
                            {
                                Console.Write("SLW");
                                WriteAT(SR);
                                StoreCY(SR, AC.M36);
                                NIC = (WA)(IC + 1);
                            }
                            else /*-602 ORS Or to Storage*/
                            {
                                Console.Write("ORS");
                                WriteAT(SR);
                                StoreCY(SR, (W36)(AC | LoadCY(SR)));
                                NIC = (WA)(IC + 1);
                            }
                            break;
                        case 401:/*621*/
                            if (S == 0) /*+621 STA Store Address*/
                            {
                                Console.Write("STA");
                                WriteAT(SR);
                                WA Adr = GetY(SR);
                                W36 tmp = CoreMemory.GetW(Adr);
                                tmp.A = AC.A;
                                CoreMemory.SetW(Adr, tmp);
                                NIC = (WA)(IC + 1);
                            }
                            else
                            {
                                Console.WriteLine("Operation {0}{1} not implemented ->{2}", S != 0 ? '-' : '+', Convert.ToString(P, 8).PadLeft(3, '0'), P);
                                halt = true;
                            }
                            break;
                        case 402:/*622*/
                            if (S == 0) /*+622 Store Decrement*/
                            {
                                Console.Write("STD");
                                WriteAT(SR);
                                WA Adr = GetY(SR);
                                W36 tmp = CoreMemory.GetW(Adr);
                                tmp.D = AC.D;
                                CoreMemory.SetW(Adr, tmp);
                                NIC = (WA)(IC + 1);
                            }
                            else
                            {
                                Console.WriteLine("Operation {0}{1} not implemented ->{2}", S != 0 ? '-' : '+', Convert.ToString(P, 8).PadLeft(3, '0'), P);
                                halt = true;
                            }
                            break;
                        case 412: /*634*/
                            if (S == 0)
                            {
                                Console.WriteLine("Operation {0}{1} not implemented ->{2}", S != 0 ? '-' : '+', Convert.ToString(P, 8).PadLeft(3, '0'), P);
                                halt = true;
                            }
                            else /*-634 SXD Store index in Decrement*/
                            {

                                Console.Write("SXD");
                                WriteAT(SR);
                                if (SR.T != 0 && SR.T != 1 && SR.T != 2 && SR.T != 4)
                                    throw new Exception("Warning, multiple index registers, see page 26 / SXD");

                                W36 temp = CoreMemory.GetW((WA)(uint)SR.A);
                                temp.D = (W15)(uint)GetX(SR.T);
                                CoreMemory.SetW((WA)(uint)SR.A, temp);

                                NIC = (WA)(IC + 1);
                            }
                            break;
                        case 448:/*700*/
                            if (S == 0) /*+700 CPY Copy and Skip */
                            {
                                Console.Write("CPY");
                                WriteAT(SR);
                                NIC = (WA)(IC + 1 + Io704.CPY(ref CoreMemory.Mem[GetY(SR) & CoreMemory.AdrMask]));
                            }
                            else /*-700 CAD Copy and Add Logical Word */
                            {
                                Console.Write("CAD");
                                WriteAT(SR);
                                int skip = Io704.CPY(ref CoreMemory.Mem[GetY(SR) & CoreMemory.AdrMask]);
                                if (skip == 0)
                                {
                                    W38 tmp = new W38() { M37 = (W37)(AC.M36 + MQ) };
                                    if (0 != tmp.Q)
                                        tmp.M37 = (W37)(tmp.M37 + 1);
                                    AC.M36 = tmp.M36;
                                }
                                NIC = (WA)(IC + 1 + skip);


                            }
                            break;
                        case 476:/*734*/
                            if (S == 0) /* +734 PAX Place Address in Index */
                            {
                                Console.Write("PAX");
                                WriteAT(SR);
                                SetX(SR.T, (WA)(uint)AC.A);
                                NIC = (WA)(IC + 1);
                            }
                            else
                            {
                                Console.WriteLine("Operation {0}{1} not implemented ->{2}", S != 0 ? '-' : '+', Convert.ToString(P, 8).PadLeft(3, '0'), P);

                                halt = true;
                            }
                            break;
                        case 492:/*754*/
                            if (S == 0)
                            {
                                Console.WriteLine("Operation {0}{1} not implemented ->{2}", S != 0 ? '-' : '+', Convert.ToString(P, 8).PadLeft(3, '0'), P);

                                halt = true;
                            }
                            else /* -754 PXD Place Index in Decrement*/
                            {
                                Console.Write("PXD");
                                WriteAT(SR);
                                if (SR.T != 0 && SR.T != 1 && SR.T != 2 && SR.T != 4)
                                    throw new Exception("Warning, multiple index registers, see page 26 / PXD");
                                AC = (W38)0;
                                AC.D = (W15)(uint)GetX(SR.T);

                                NIC = (WA)(IC + 1);

                            }
                            break;
                        case 496: /*760*/
                            if (SR.A >> 4 == 0)
                                switch ((uint)SR.A)
                                {
                                    case 6:
                                        if (S == 0)/*+760...006 COM Complement Magnitude*/
                                        {
                                            Console.WriteLine("COM");
                                            AC.M37 = (W37)~AC.M37;
                                            NIC = (WA)(IC + 1);
                                        }
                                        else
                                        {
                                            Console.WriteLine("Operation {0}{1}...{2} not implemented ->{3}", S != 0 ? '-' : '+', Convert.ToString(P, 8).PadLeft(3, '0'), Convert.ToString(SR.A, 8).PadLeft(3, '0'), (uint)SR.A);
                                            halt = true;
                                        }
                                        break;
                                    case 7:
                                        if (S == 0)
                                        {
                                            Console.WriteLine("Operation {0}{1}...{2} not implemented ->{3}", S != 0 ? '-' : '+', Convert.ToString(P, 8).PadLeft(3, '0'), Convert.ToString(SR.A, 8).PadLeft(3, '0'), (uint)SR.A);
                                            halt = true;
                                        }
                                        else /*-760...007 LTM*/
                                        {
                                            Console.WriteLine("LTM");
                                            trapping = false;
                                            NIC = (WA)(IC + 1);
                                        }
                                        break;
                                    case 10:
                                        if (S == 0)
                                        {
                                            Console.WriteLine("Operation {0}{1}...{2} not implemented ->{3}", S != 0 ? '-' : '+', Convert.ToString(P, 8).PadLeft(3, '0'), Convert.ToString(SR.A, 8).PadLeft(3, '0'), (uint)SR.A);

                                            halt = true;
                                        }
                                        else /*-760...012 RTT */
                                        {
                                            Console.WriteLine("RTT");
                                            NIC = (WA)(IC + 1 + Io704.RTT());
                                            break;
                                        }
                                        break;

                                    default:
                                        Console.WriteLine("Operation {0}{1}...{2} not implemented ->{3}", S != 0 ? '-' : '+', Convert.ToString(P, 8).PadLeft(3, '0'), Convert.ToString(SR.A, 8).PadLeft(3, '0'), (uint)SR.A);
                                        halt = true;
                                        break;
                                }
                            else
                            {
                                if (S == 0) /* +760 PSE Plus sense*/
                                {
                                    Console.Write("PSE");
                                    WriteAT(SR);
                                    NIC = (WA)(IC + 1 + Io704.PSE(SR.A));
                                }
                                else
                                {
                                    Console.Write("MSE");
                                    WriteAT(SR);
                                    NIC = (WA)(IC + 1 + Io704.MSE(SR.A));
                                }
                            }
                            break;
                        case 497:/*761*/
                            if (S == 0) /* +761 NOP No Operation*/
                            {
                                Console.Write("NOP");
                                WriteAT(SR);
                                NIC = (WA)(IC + 1);
                            }
                            else
                            {
                                Console.WriteLine("Operation {0}{1} not implemented ->{2}", S != 0 ? '-' : '+', Convert.ToString(P, 8).PadLeft(3, '0'), P);

                                halt = true;
                            }
                            break;
                        case 498:/*762*/
                            if (S == 0) /* +762 RDS Read Select */
                            {
                                Console.Write("RDS");
                                WriteAT(SR);
                                Io704.RDS(GetY(SR));
                                NIC = (WA)(IC + 1);
                            }
                            else
                            {
                                Console.WriteLine("Operation {0}{1} not implemented ->{2}", S != 0 ? '-' : '+', Convert.ToString(P, 8).PadLeft(3, '0'), P);

                                halt = true;
                            }
                            break;
                        case 499:
                            if (S == 0) /* +763 LLS Long Left Shift */
                            {
                                Console.Write("LLS");
                                WriteAT(SR);
                                uint shift = GetY(SR) % 256;
                                for (uint i = 0; i < shift; i++)
                                {
                                    W1 Sign = MQ.S;
                                    MQ = (W36)(MQ << 1);
                                    AC = (W38)((AC << 1) | MQ.S);
                                    AC.S = MQ.S = Sign;
                                    if (AC.PB != 0)
                                        acoflag = true;
                                }
                                NIC = (WA)(IC + 1);
                            }
                            else/* -763 LGL Logical Left */
                            {
                                Console.Write("LGL");
                                WriteAT(SR);
                                uint shift = GetY(SR) % 256;
                                for (uint i = 0; i < shift; i++)
                                {
                                    AC.M37 = (W37)((AC.M37 << 1) | MQ.S);
                                    MQ = (W36)(MQ << 1);
                                    if (AC.PB != 0)
                                        acoflag = true;
                                }
                                NIC = (WA)(IC + 1);
                            }
                            break;
                        case 501: /*765*/
                            if (S == 0) /*+765 LRS Long Right Shift*/
                            {
                                Console.Write("LRS");
                                WriteAT(SR);
                                uint shift = GetY(SR) % 256;
                                for (uint i = 0; i < shift; i++)
                                {
                                    MQ.S = (W1)(uint)AC;
                                    MQ = (W36)(MQ >> 1);
                                    MQ.S = AC.S;
                                    AC.M37 = (W37)(AC.M37 >> 1);
                                }
                                NIC = (WA)(IC + 1);
                            }
                            else
                            {
                                Console.WriteLine("Operation {0}{1} not implemented ->{2}", S != 0 ? '-' : '+', Convert.ToString(P, 8).PadLeft(3, '0'), P);
                                halt = true;
                            }
                            break;
                        case 502: /*766*/
                            if (S == 0) /*+766 WTB Write Tape Binary*/
                            {
                                Console.Write("WTB");
                                WriteAT(SR);
                                Io704.WRS(GetY(SR));
                                NIC = (WA)(IC + 1);
                            }
                            else
                            {
                                Console.WriteLine("Operation {0}{1} not implemented ->{2}", S != 0 ? '-' : '+', Convert.ToString(P, 8).PadLeft(3, '0'), P);
                                halt = true;
                            }
                            break;
                        case 503: /*767*/
                            if (S == 0) /*+767*/
                            {
                                Console.Write("ALS");
                                WriteAT(SR);
                                uint shift = GetY(SR) % 256;
                                for (uint i = 0; i < shift; i++)
                                {
                                    AC.M37 = (W37)(AC.M37 << 1);
                                    if (0 != AC.PB)
                                        acoflag = true;
                                }
                                NIC = (WA)(IC + 1);
                            }
                            else
                            {
                                Console.WriteLine("Operation {0}{1} not implemented ->{2}", S != 0 ? '-' : '+', Convert.ToString(P, 8).PadLeft(3, '0'), P);
                                halt = true;
                            }
                            break;
                        case 505: /*771*/
                            if (S == 0) /*+771 Accumulator Right Shift*/
                            {
                                Console.Write("ARS");
                                WriteAT(SR);
                                uint shift = GetY(SR) % 256;
                                for (uint i = 0; i < shift; i++)
                                    AC.M37 = (W37)(AC.M37 >> 1);
                                NIC = (WA)(IC + 1);
                            }
                            else
                            {
                                Console.WriteLine("Operation {0}{1} not implemented ->{2}", S != 0 ? '-' : '+', Convert.ToString(P, 8).PadLeft(3, '0'), P);
                                halt = true;
                            }
                            break;
                        case 504: /*770*/
                            if (S == 0) /*+770 WEF Write End of File*/
                            {
                                Console.Write("WEF");
                                WriteAT(SR);
                                Io704.WEF(GetY(SR));
                                NIC = (WA)(IC + 1);
                            }
                            else
                            {
                                Console.WriteLine("Operation {0}{1} not implemented ->{2}", S != 0 ? '-' : '+', Convert.ToString(P, 8).PadLeft(3, '0'), P);
                                halt = true;
                            }
                            break;
                        case 506: /*772*/
                            if (S == 0) /*+772 REW Rewind Tape*/
                            {
                                Console.Write("REW");
                                WriteAT(SR);
                                Io704.REW(GetY(SR));
                                NIC = (WA)(IC + 1);
                            }
                            else
                            {
                                Console.WriteLine("Operation {0}{1} not implemented ->{2}", S != 0 ? '-' : '+', Convert.ToString(P, 8).PadLeft(3, '0'), P);
                                halt = true;
                            }
                            break;
                        default:
                            Console.WriteLine("Operation {0}{1} not implemented ->{2}", S != 0 ? '-' : '+', Convert.ToString(P, 8).PadLeft(3, '0'), P);
                            halt = true;
                            break;
                    }
                    break;
                case 1:
                    if (S == 0) /* +1 : TXI Transfer with index incremeted */
                    {
                        Console.Write("TXI");
                        WriteATD(SR);
                        SetX(SR.T, (WA)(GetX(SR.T) + SR.D));
                        NIC = (WA)(uint)SR.A;
                    }
                    else
                    {
                        Console.WriteLine("Type A operation {0}{1} not implemented", S != 0 ? '-' : '+', P);
                        halt = true;
                    }
                    break;
                case 2:
                    if (S == 0) /* +2 : TIX  Transfer on Index  */
                    {
                        Console.Write("TIX");
                        WriteATD(SR);
                        WA X = GetX(SR.T);
                        if (X > (WA)(uint)SR.D)
                        {
                            SetX(SR.T, (WA)(X - SR.D));
                            NIC = (WA)(uint)SR.A;
                        }
                        else
                            NIC = (WA)(IC + 1);
                    }
                    else /* -2 : TNX  Transfer on No Index  */
                    {
                        Console.Write("TNX");
                        WriteATD(SR);
                        WA X = GetX(SR.T);
                        if (X <= (WA)(uint)SR.D)
                            NIC = (WA)(uint)SR.A;
                        else
                        {
                            SetX(SR.T, (WA)(X - SR.D));
                            NIC = (WA)(IC + 1);
                        }
                    }
                    break;
                case 3:
                    if (S == 0) /* +3 : TXH */
                    {
                        Console.Write("TXH");
                        WriteATD(SR);
                        if (GetX(SR.T) > (WA)(uint)SR.D)
                            NIC = (WA)(uint)SR.A;
                        else
                            NIC = (WA)(IC + 1);
                    }
                    else /* -3 : TXL */
                    {
                        Console.Write("TXL");
                        WriteATD(SR);
                        if (GetX(SR.T) <= (WA)(uint)SR.D)
                            NIC = (WA)(uint)SR.A;
                        else
                            NIC = (WA)(IC + 1);
                    }
                    break;
                default:
                    Console.WriteLine("Type A operation {0}{1} not implemented", S != 0 ? '-' : '+', P);
                    halt = true;
                    break;
            }
        }

    }


}

