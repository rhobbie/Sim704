using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim704
{
    struct W15
    {
        const int alen = 15; /* Adress length */        
        const uint a0mask = (1U << alen) - 1U; /* mask for Adress at bit 0*/

        uint w;
        public int W
        {
            get { return w; }
            set { w = value & 0x7FFF; }
        }
        public override string ToString()
        {

            return Convert.ToString(w, 8).PadLeft(5, '0');
        }
    }
#if false
    struct W36
    {
        long w;
        public long W /* 36 Bit unsigned */
        {
            get { return w; }
            set { w = value & 0xFFFFFFFFFL; }
        }
        public int S /* Sign */
        {
            get { return (int)(w >> 35); }
            set { w = (w & 0x7FFFFFFFFL) | ((long)(value & 1) << 35); }
        }
        public long M /* 35 Bit Magnitude */
        {
            get { return (w & 0x7FFFFFFFFL); }
            set { w = (w & 0x800000000L) | (value & 0x7FFFFFFFFL); }
        }
        public int P /* 3 Bit Prefix */
        {
            get { return (byte)(w >> 33); }
            set { w = (w & 0x1FFFFFFFFL) | ((long)(value & 7) << 33); }
        }
        public int T /* 3 Bit Tag */
        {
            get { return (int)((w >> 15)&0x7); }
            set { w = (w & 0xFFFFC7FFFL) | ((long)(value & 7) << 15); }
        }
        public W15 A /* 15 Bit Address */
        {
            get { return new W15(){ W = (int)w }; }
            set { w = (w & 0xFFFFF8000) | (long)(value.W); }
        }
        public W15 D /* 15 Bit Decrement */
        {
            get { return new W15() { W = (int)(w >> 18) }; }
            set { w = (w & 0xE0003FFFF) | (long)(value.W)<<18; }
        }
        public override string ToString()
        {
            StringBuilder B = new StringBuilder();
            if (S != 0)
                B.Append('-');
            else
                B.Append(' ');
            string A = Convert.ToString(M, 8).PadLeft(12, '0');
            B.Append(A[0]);
            B.Append(' ');
            B.Append(A.Substring(1, 5));
            B.Append(' ');
            B.Append(A[6]);
            B.Append(' ');
            B.Append(A.Substring(7, 5));
            return B.ToString();
        }
    }
#else
    public struct W704 /* 36 Bit Word with access to Adress, Tag, Decrement, Prefix,  Magnitude, Sign, Logical Word, Arithmetical Word */
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

        public uint A  /* Adress 15 bit */
        {
            get
            {
                return (uint)((w >> apos) & a0mask);
            }
            set
            {
                w = ((w & iawmask) | ((value & a0mask) << apos));
            }
        }
        public uint T  /* Tag 3 bit */
        {
            get
            {
                return (uint)((w >> tpos) & t0mask);
            }
            set
            {
                w = ((w & itwmask) | ((value & t0mask) << tpos));
            }
        }
        public uint D  /* Decrement 15 bit */
        {
            get
            {
                return (uint)((w >> dpos) & d0mask);
            }
            set
            {
                w = ((w & idwmask) | ((value & d0mask) << dpos));
            }
        }
        public uint P  /* Prefix 3 bit */
        {
            get
            {
                return (uint)((w >> ppos) & p0mask);
            }
            set
            {
                w = ((w & ipwmask) | ((value & p0mask) << ppos));
            }
        }
        public ulong M  /* Magnitude 35 bit */
        {
            get
            {
                return ((w >> mpos) & m0mask);
            }
            set
            {
                w = ((w & imwmask) | ((value & m0mask) << mpos));
            }
        }
        public uint S  /* Sign 1 bit*/
        {
            get
            {
                return (uint)((w >> spos) & s0mask);
            }
            set
            {
                w = ((w & iswmask) | ((value & s0mask) << spos));
            }
        }
        public ulong LW  /* Logical Word 36 bit unsigned */
        {
            get
            {
                return w & wmask;
            }
            set
            {
                w = value & wmask;
            }
        }
        public long AW  /* Arithmetical Word 35 bit with sign */
        {
            get
            {
                if ((w & swmask) != 0)
                    return (long)(w & m0mask);
                else
                    return -(long)(w & m0mask);
            }
            set
            {
                if (value >= 0)
                {
                    w = (ulong)value & m0mask;
                }
                else
                {
                    w = swmask | ((ulong)-value & m0mask);
                }
            }
        }
        public override string ToString()
        {
            StringBuilder s = new StringBuilder(16);
            if (S != 0)
                s.Append('-');
            else
                s.Append(' ');

            string C = Convert.ToString((long)M, 8).PadLeft(12, '0');
            s.Append(C[0]);
            s.Append(' ');
            s.Append(C.Substring(1, 5));
            s.Append(' ');
            s.Append(C[6]);
            s.Append(' ');
            s.Append(C.Substring(7, 5));
            return s.ToString();
        }
    }
#endif
    struct W38
    {
        long w;
        public long W /* 38 Bit unsigned */
        {
            get { return w; }
            set { w = value & 0x3FFFFFFFFL; }
        }
        public int S /* Sign */
        {
            get { return (int)(w >> 37); }
            set { w = (w & 0x1FFFFFFFFFL) | ((long)(value & 1) << 37); }
        }
        public int Q /* Q bit */
        {
            get { return (int)(w >> 36)&1; }
            set { w = (w & 0x2FFFFFFFFFL) | ((long)(value & 1) << 36); }
        }
        public int P /* P bit */
        {
            get { return (int)(w >> 35) & 1; }
            set { w = (w & 0x37FFFFFFFFL) | ((long)(value & 1) << 35); }
        }
        public W15 A /* 15 Bit Address */
        {
            get { return new W15() { W = (int)w }; }
            set { w = (w & 0x3FFFFF8000) | (long)(value.W); }
        }
        public long M37 /* 37 Bit Magnitude */
        {
            get { return (w & 0x1FFFFFFFFFL); }
            set { w = (w & 0x2000000000L) | (value & 0x1FFFFFFFFFFL); }
        }
        public long M36 /* 36 Bit Mangnitude */
        {
            get { return (w & 0xFFFFFFFFFL); }
            set { w = (w & 0x3000000000L) | (value & 0xFFFFFFFFFL); }
        }
        public long M35 /* 35 Bit Mangnitude */
        {
            get { return (w & 0x7FFFFFFFL); }
            set { w = (w & 0x3800000000L) | (value & 0x7FFFFFFFL); }
        }
        public override string ToString()
        {
            StringBuilder B = new StringBuilder();
            if (S != 0)
                B.Append('-');
            else
                B.Append(' ');
            B.Append(Q);
            B.Append(' ');
            B.Append(P);
            B.Append(' ');
            string A = Convert.ToString(M35, 8).PadLeft(12, '0');
            B.Append(A[0]);
            B.Append(' ');
            B.Append(A.Substring(1, 5));
            B.Append(' ');
            B.Append(A[6]);
            B.Append(' ');
            B.Append(A.Substring(7, 5));
            return B.ToString();
        }
    }
   
    static class CPU704
    {
        static bool trapping = false;
        static bool acoflag = false;
        public static W15 IC; /* Instruction Counter */
        public static W15 NIC; /* next values for Instruction Counter */
        public static W38 AC;
        public static W36 MQ;
        static W15[] X = new W15[3];
        public static bool halt = false;
        public static bool repeat = false;
        static public void Clr()
        {
            AC = new W38() { W = 0 };
            MQ = new W36() { W = 0 };
            X[0].W = 0;
            X[1].W = 0;
            X[2].W = 0;
            IC.W = 0;
            CoreMemory.Clear();
        }
        static public void LoadCrd()
        {
            
            Io704.RCD();

            Io704.CPY(ref CoreMemory.Mem[0]);
            Io704.CPY(ref CoreMemory.Mem[1]);
            
            IC.W = 0;
            Go(false);
        }
        static public void LoadTape()
        {
            
            Io704.RTB(1);
            Io704.CPY(ref CoreMemory.Mem[0]);
            Io704.CPY(ref CoreMemory.Mem[1]);
            IC.W = 0;
            Go(false);
        }
        static public void LoadDrm()
        {
            Io704.RDR(1);
            Io704.CPY(ref CoreMemory.Mem[0]);
            Io704.CPY(ref CoreMemory.Mem[1]);
            IC.W = 0;
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
                    Console.ReadLine();
                    if (repeat)
                        NIC = IC;
                    halt = false;
                }
                else if(step)
                {
                    Console.ReadLine();
                }
                IC = NIC;
            }
            while (true);
        }
        static void SetX(int T, W15 A)
        {
            if (0 != (T & 1))
                X[0] = A;
            if (0 != (T & 2))
                X[1] = A;
            if (0 != (T & 4))
                X[2] = A;
        }
        static W15 GetX(int T)
        {
            int A = 0;
            if (0 != (T & 1))
                A=X[0].W;
            if (0 != (T & 2))
                A |= X[1].W;
            if (0 != (T & 4))
                A |= X[2].W;
            return new W15() { W = A };
        }
        static W15 GetY(W36 SR) /* Get Y*/
        {
            return new W15() { W = SR.A.W - GetX(SR.T).W };
        }
        static W36 LoadCY(W36 SR) /* Load C(Y) */
        {             
            return CoreMemory.GetW(SR.A.W - GetX(SR.T).W);
        }
        static void StoreCY(W36 SR,W36 V) /* Store V to C(Y) */
        {
            CoreMemory.SetW(SR.A.W - GetX(SR.T).W, V);
        }
        static void WriteAT(W36 SR)
        {
            if(SR.T!=0)
            {
                Console.WriteLine(" {0},{1}", SR.A.W, SR.T);
            }
            else
            {
                Console.WriteLine(" {0}", SR.A.W);
            }
        }
        static void WriteATD(W36 SR)
        {
        
                short d = (short)(SR.D.W<<1);
                Console.WriteLine(" {0},{1},{2}", SR.A.W, SR.T,d>>1);
        
        }
        static public void Step()
        {

            W36 SR = CoreMemory.GetW(IC.W);

            Console.Write("{0} {1} {2} {3} {4} {5} {6} ", IC, SR, AC, MQ, X[0], X[1], X[2]);
            /* check if Type A or Type B instruction */
            int S = SR.S;
            int P = (int)(SR.M>>33);
            switch (P)
            {
                case 0:
                    /* Type B instruction */
                    P = (int)(SR.M >> 24);
                    switch (P)
                    {

                        case 241:
                            if (S == 0) /*+361 ACL Add and Carry Logival Word*/
                            {
                                Console.Write("ACL");
                                WriteAT(SR);
                                long tmp = AC.M36+LoadCY(SR).W;
                                if (0 != (tmp & 0x1000000000L))
                                    tmp++;
                                AC.M36 = tmp;
                                NIC.W = IC.W + 1;

                            }
                            else
                            {
                                Console.WriteLine("Operation {0}{1} not implemented ->{2}", S != 0 ? '-' : '+', Convert.ToString(P, 8).PadLeft(3, '0'), P);
                                halt = true;
                            }
                            break;
                        case 256:                      
                            if (S == 0) /*+400 ADD Add */
                            {
                                Console.Write("ADD");
                                WriteAT(SR);
                                SR = LoadCY(SR);
                                
                                bool f1 = false; /* True Signs of AC and SR differs */
                                bool f2 = false; /* True AC Negative */
                                bool f8 = false; /* True P was set in AC */
                                /* Make AC Positive */
                                if(0!=AC.S)
                                {
                                    f2 = true;
                                    AC.S = 0;
                                }
                                if (0 != AC.P)
                                    f8=true;
                                /* Check signes of SR & AC */
                                if((0!=SR.S) != f2)
                                {
                                    AC.M37 ^= 0x1FFFFFFFFFL; /* One's compliment */
                                    f1=true;
                                }

                                AC.W += SR.M;

                                /* Check carry from Q */
                                if (f1)
                                {    /* Check if signs were not same */
                                    if (0 != AC.S)
                                    {
                                        f2  = !f2;
                                        AC.W++;
                                        if ((0 != AC.P) != f8)
                                            acoflag = true;
                                    }
                                    else
                                        AC.M37 ^= 0x1FFFFFFFFFL; /* One's compliment */
                                }
                                else if((0 != AC.P) != f8)
                                     acoflag = true;
                                /* Restore sign to AC */
                                AC.W = AC.M37;
                                if (f2)
                                    AC.S = 1;
                                
                                NIC.W = IC.W + 1;
                            }
                            else
                            {
                                Console.WriteLine("Operation {0}{1} not implemented ->{2}", S != 0 ? '-' : '+', Convert.ToString(P, 8).PadLeft(3, '0'), P);
                                halt = true;
                            }
                            break;
                        case 320:
                            if (S == 0) /*+500 CLA Clear and ADD */
                            {
                                Console.Write("CLA");
                                WriteAT(SR);
                                W36 tmp = LoadCY(SR);
                                AC.W = 0;
                                AC.M35 = tmp.M;
                                AC.S = tmp.S;
                                NIC.W = IC.W + 1;
                            }
                            else /*-500 CAL Clear and ADD logical Word */
                            {
                                Console.Write("CAL");
                                WriteAT(SR);
                                AC.W = LoadCY(SR).W;
                                NIC.W = IC.W + 1;
                            }
                            break;
                        case 348: 
                            if (S == 0) /*+534 LXA Load Index from Address*/
                            {
                                Console.Write("LXA");
                                WriteAT(SR);
                                int T = SR.T;
                                SR = CoreMemory.GetW(SR.A.W);
                                SetX(T, SR.A);
                                NIC.W = IC.W + 1;
                            }
                            else
                            {
                                Console.WriteLine("Operation {0}{1} not implemented ->{2}", S != 0 ? '-' : '+', Convert.ToString(P, 8).PadLeft(3, '0'), P);
                                halt = true;
                            }
                            break;
                        case 386:
                            if (S == 0) /*+602 SLW Store Logical Word*/
                            {
                                Console.Write("SLW");
                                WriteAT(SR);
                                StoreCY(SR, new W36() { W = AC.M36 });
                                NIC.W = IC.W + 1;
                            }
                            else
                            {
                                Console.WriteLine("Operation {0}{1} not implemented ->{2}", S != 0 ? '-' : '+', Convert.ToString(P, 8).PadLeft(3, '0'), P);
                                halt = true;
                            }
                            break;
                        case 401:
                            if (S == 0) /*+621 STA Store Address*/
                            {
                                Console.Write("STA");
                                WriteAT(SR);
                                W15 Adr = GetY(SR);
                                W36 tmp = CoreMemory.GetW(Adr.W);
                                tmp.A = AC.A;
                                CoreMemory.SetW(Adr.W, tmp);
                                NIC.W = IC.W + 1;

                            }
                            else
                            {
                                Console.WriteLine("Operation {0}{1} not implemented ->{2}", S != 0 ? '-' : '+', Convert.ToString(P, 8).PadLeft(3, '0'), P);
                                halt = true;
                            }
                            break;
                        case 448:
                            if (S == 0) /*+700 CPY Copy and Skip */
                            {
                                Console.Write("CPY");
                                WriteAT(SR);
                                NIC.W = IC.W+1+Io704.CPY(ref CoreMemory.Mem[GetY(SR).W & CoreMemory.AdrMask]);                               
                            }
                            else
                            {
                                Console.WriteLine("Operation {0}{1} not implemented ->{2}", S != 0 ? '-' : '+', Convert.ToString(P, 8).PadLeft(3, '0'), P);
                                halt = true;
                            }
                            break;
                        case 476:
                            if (S == 0) /* +734 PAX Place Address in Index */
                            {
                                Console.Write("PAX");
                                WriteAT(SR);
                                SetX(SR.T, AC.A);
                                NIC.W = IC.W + 1;                                
                            }
                            else 
                            {
                                Console.WriteLine("Operation {0}{1} not implemented ->{2}", S != 0 ? '-' : '+', Convert.ToString(P, 8).PadLeft(3, '0'), P);

                                halt = true;
                            }
                            break;
                        case 496: 
                            if (S == 0)
                            {
                                Console.WriteLine("Operation {0}{1} not implemented ->{2}", S != 0 ? '-' : '+', Convert.ToString(P, 8).PadLeft(3, '0'), P);

                                halt = true;
                            }
                            else /*-760 */
                            {
                                switch (SR.A.W)
                                {
                                    case 7: /*-760...007 LTM*/
                                        Console.WriteLine("LTM");
                                        trapping = false;
                                        NIC.W = IC.W + 1;
                                        break;
                                    default:
                                        Console.WriteLine("Operation {0}{1}...{2} not implemented ->{3}", S != 0 ? '-' : '+', Convert.ToString(P, 8).PadLeft(3, '0'), Convert.ToString(SR.A.W, 8).PadLeft(3, '0'), SR.A.W);
                                        halt = true;
                                        break;
                                }                                
                            }
                            break;
                        case 498:
                            if (S == 0) /* +762 RDS Read Select */
                            {
                                Console.Write("RDS");
                                WriteAT(SR);
                                Io704.RDS(GetY(SR).W);
                                NIC.W = IC.W + 1;
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
                                int shift = GetY(SR).W%256;
                                for(int i=0;i<shift;i++)
                                {
                                    int Sign = MQ.S;
                                    MQ.W <<= 1;                                                                      
                                    AC.M37 = (AC.M37 << 1) | (long)MQ.S;
                                    AC.S= MQ.S = Sign;
                                }
                                NIC.W = IC.W + 1;
                            }
                            else 
                            {
                                Console.WriteLine("Operation {0}{1} not implemented ->{2}", S != 0 ? '-' : '+', Convert.ToString(P, 8).PadLeft(3, '0'), P);

                                halt = true;
                            }
                            break;
                        default:
                            Console.WriteLine("Operation {0}{1} not implemented ->{2}", S != 0 ? '-' : '+',Convert.ToString(P,8).PadLeft(3,'0'),P);
                            halt = true;
                            break;
                    }
                    break;
                case 1:
                    if(S==0) /* +1 : TXI Transfer with index incremeted */
                            {
                        Console.Write("TXI");
                        WriteATD(SR);
                        SetX(SR.T, new W15() { W = (GetX(SR.T).W + SR.D.W) });
                        NIC= SR.A;
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
                        W15 X = GetX(SR.T);
                        if(X.W>SR.D.W)
                        {
                            SetX(SR.T, new W15() { W = X.W - SR.D.W });
                            NIC = SR.A;
                        }
                        else
                            NIC.W = IC.W+1;
                    }
                    else
                    {
                        Console.WriteLine("Type A operation {0}{1} not implemented", S != 0 ? '-' : '+', P);
                        halt = true;
                    }
                    break;
                case 3:
                    if (S == 0) 
                    {
                        Console.WriteLine("Type A operation {0}{1} not implemented", S != 0 ? '-' : '+', P);
                        halt = true;
                    }
                    else /* -3 : TXL */
                    {
                        Console.Write("TXL");
                        WriteATD(SR);
                        if (GetX(SR.T).W <= SR.D.W)
                            NIC = SR.A;
                        else
                            NIC.W = IC.W + 1;
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

