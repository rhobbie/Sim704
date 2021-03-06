﻿using System;
using System.Text;
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
        public static implicit operator uint(W1 d)
        {
            return d.w;
        }
        public static explicit operator W1(uint d)
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
        public static implicit operator uint(W3 d)
        {
            return d.w;
        }
        public static explicit operator W3(uint d)
        {
            return new W3(d);
        }
        public override string ToString()
        {
            return Convert.ToString(w);
        }
    }
    struct W8
    {
        const int wlen = 8; /* Word length */
        const uint wmask = (1U << wlen) - 1U; /* mask for word */

        uint w; /* 8 bit Word stored in 32 bit uint*/

        public W8(uint value)
        {
            w = value & wmask;
        }
        public static implicit operator uint(W8 d)
        {
            return d.w;
        }
        public static explicit operator W8(uint d)
        {
            return new W8(d);
        }
        public override string ToString()
        {
            return Convert.ToString(w, 8).PadLeft(3, '0');
        }
    }
    struct W10
    {
        const int wlen = 10; /* Word length */
        const uint wmask = (1U << wlen) - 1U; /* mask for word */

        uint w; /* 10 bit Word stored in 32 bit uint*/

        public W10(uint value)
        {
            w = value & wmask;
        }

        public static implicit operator uint(W10 d)
        {
            return d.w;
        }
        public static explicit operator W10(uint d)
        {
            return new W10(d);
        }
        public override string ToString()
        {
            return Convert.ToString(w, 8).PadLeft(4, '0');
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
        public static implicit operator uint(W15 d)
        {
            return d.w;
        }
        public static explicit operator W15(uint d)
        {
            return new W15(d);
        }
        public override string ToString()
        {
            return Convert.ToString(w, 8).PadLeft(5, '0');
        }
    }
    struct W18
    {
        const int wlen = 18; /* Word length */
        const uint wmask = (1U << wlen) - 1U; /* mask for word */

        uint w; /* 18 bit Word stored in 32 bit uint*/

        public W18(uint value)
        {
            w = value & wmask;
        }
        public static implicit operator uint(W18 d)
        {
            return d.w;
        }
        public static explicit operator W18(uint d)
        {
            return new W18(d);
        }
        public override string ToString()
        {
            return Convert.ToString(w, 8).PadLeft(6, '0');
        }
    }
    struct W27
    {
        const int wlen = 27; /* Word length */
        const uint wmask = (1U << wlen) - 1U; /* mask for word */

        uint w; /* 27 bit Word stored in 32 bit uint*/

        public W27(uint value)
        {
            w = value & wmask;
        }
        public static implicit operator uint(W27 d)
        {
            return d.w;
        }
        public static explicit operator W27(uint d)
        {
            return new W27(d);
        }
        public override string ToString()
        {
            return Convert.ToString(w, 8).PadLeft(9, '0');
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
        const int b1len = 1; /* lenght of Bit 1 */
        const int llen = 18; /* length of left half */
        const int flen = 27; /* lenght of the fraction */
        const int clen = 8; /* lenght of the characteristic */
        const int b9len = 1; /* lenght of Bit 9 */

        const int apos = 0; /* bit pos for Adress*/
        const int tpos = alen; /* bit pos for Tag*/
        const int dpos = tpos + tlen; /* bit pos for Decrement*/
        const int ppos = dpos + dlen; /* bit pos for Prefix*/
        const int mpos = 0;           /* bit pos for Magniude */
        const int spos = wlen - slen; /* bit pos for Sign */
        const int b1pos = spos - b1len; /* bit pos for Bit 1 */
        const int lpos = 18; /* bit pos of left half */
        const int fpos = 0;   /* bit pos  of the fraction */
        const int cpos = flen; /* bit pos of the characteristic */
        const int b9pos = cpos - b9len; /* bit pos for Bit 9 */

        const ulong a0mask = (1UL << alen) - 1UL; /* mask for Adress at bit 0*/
        const ulong t0mask = (1UL << tlen) - 1UL; /* mask for Tag at bit 0*/
        const ulong d0mask = (1UL << dlen) - 1UL; /* mask for Decrement at bit 0 */
        const ulong p0mask = (1UL << plen) - 1UL; /* mask for Prefix at bit 0 */
        const ulong wmask = (1UL << wlen) - 1UL;  /* mask for Word */
        const ulong m0mask = (1UL << mlen) - 1UL; /* mask for Magnitude at bit 0*/
        const ulong s0mask = (1UL << slen) - 1UL; /* mask for Sign at bit 0 */
        const ulong b10mask = (1UL << b1len) - 1UL; /* mask for Bit 1 at bit 0 */
        const ulong l0mask = (1UL << llen) - 1UL;  /* mask for left half at bit 0*/
        const ulong f0mask = (1UL << flen) - 1UL; /* mask for fraction at bit 0 */
        const ulong c0mask = (1UL << clen) - 1UL; /* mask for characteristic at bit 0  */
        const ulong b90mask = (1UL << b9len) - 1UL; /* mask for Bit 9 at bit 0 */

        const ulong awmask = a0mask << apos; /* mask for Adress in word */
        const ulong twmask = t0mask << tpos; /* mask for Tag in word */
        const ulong dwmask = d0mask << dpos; /* mask for Decrement in word */
        const ulong pwmask = p0mask << ppos; /* mask for Prefix in word */
        const ulong mwmask = m0mask << mpos; /* mask for Magnitude in word  */
        const ulong swmask = s0mask << spos; /* mask for Sign in word */
        const ulong b1wmask = b10mask << b1pos; /* mask for Bit 1 in word */
        const ulong lwmask = l0mask << lpos; /* mask for left half in word */
        const ulong fwmask = f0mask << fpos; /* mask for fraction in word */
        const ulong cwmask = c0mask << cpos; /* mask for characteristic int word */
        const ulong b9wmask = b90mask << b9pos; /* mask for Bit 9 in word */

        const ulong iawmask = (wmask ^ awmask); /* inv mask for Adress in word */
        const ulong itwmask = (wmask ^ twmask); /* inv mask for Tag in word  */
        const ulong idwmask = (wmask ^ dwmask); /* inv mask for Decrement in word  */
        const ulong ipwmask = (wmask ^ pwmask); /* inv mask for Prefix in word  */
        const ulong imwmask = (wmask ^ mwmask); /* inv mask for Magnitude in word  */
        const ulong iswmask = (wmask ^ swmask); /* inv mask for Sign in word  */
        const ulong ib1wmask = (wmask ^ b1wmask); /* inv mask for Bit 1 in word */
        const ulong ilwmask = (wmask ^ lwmask); /* inv mask for left half in word  */
        const ulong ifwmask = (wmask ^ fwmask); /* inv mask for fraction in word  */
        const ulong icwmask = (wmask ^ cwmask); /* inv mask for characteristic in word  */
        const ulong ib9wmask = (wmask ^ b9wmask); /* inv mask for Bit 1 in word */

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
        public W1 B1  /* Bit1 1 Bit*/
        {
            get
            {
                return (W1)(w >> b1pos);
            }
            set
            {
                w = (w & ib1wmask) | ((ulong)value << b1pos);
            }
        }

        public W18 L  /* left half */
        {
            get
            {
                return (W18)(w >> lpos);
            }
            set
            {
                w = (w & ilwmask) | ((ulong)value << lpos);
            }
        }
        public W27 F  /* Fraction 27 bit */
        {
            get
            {
                return (W27)w;
            }
            set
            {
                w = (w & ifwmask) | value;
            }
        }
        public W8 C  /* Charcteristic 8 bit*/
        {
            get
            {
                return (W8)(w >> cpos);
            }
            set
            {
                w = (w & icwmask) | ((ulong)value << cpos);
            }
        }

        public W1 B9  /* Bit9 1 Bit*/
        {
            get
            {
                return (W1)(w >> b9pos);
            }
            set
            {
                w = (w & ib9wmask) | ((ulong)value << b9pos);
            }
        }
        public override string ToString()
        {
            return Convert.ToString((long)w, 8).PadLeft(12, '0');
        }
        public string MQtoString()
        {
            StringBuilder sb = new StringBuilder();
            if (S != 0)
                sb.Append('-');
            else
                sb.Append(' ');
            sb.Append(M.ToString());
            return sb.ToString();
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
        public static explicit operator W37(W35 d)
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
        const int flen = 27; /* lenght of the fraction */
        const int c10len = 10; /* lenght of the 10 bit characteristic */
        const int b9len = 1; /* lenght of Bit 9 */

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
        const int fpos = 0;   /* bit pos  of the fraction */
        const int c10pos = flen; /* bit pos of the 10 bit characteristic */
        const int b9pos = c10pos - b9len; /* bit pos for Bit 9 */


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
        const ulong f0mask = (1UL << flen) - 1UL; /* mask for fraction at bit 0 */
        const ulong c100mask = (1UL << c10len) - 1UL; /* mask for 10 bit characteristic at bit 0  */
        const ulong b90mask = (1UL << b9len) - 1UL; /* mask for Bit 9 at bit 0 */

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
        const ulong fwmask = f0mask << fpos; /* mask for fraction in word */
        const ulong c10wmask = c100mask << c10pos; /* mask for 10 bit characteristic int word */
        const ulong b9wmask = b90mask << b9pos; /* mask for Bit 9 in word */

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
        const ulong ifwmask = (wmask ^ fwmask); /* inv mask for fraction in word  */
        const ulong ic10wmask = (wmask ^ c10wmask); /* inv mask for 10 bit characteristic in word  */
        const ulong ib9wmask = (wmask ^ b9wmask); /* inv mask for Bit 1 in word */

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
        public W27 F  /* Fraction 27 bit */
        {
            get
            {
                return (W27)w;
            }
            set
            {
                w = (w & ifwmask) | value;
            }
        }
        public W10 C10  /* Charcteristic 10 bit*/
        {
            get
            {
                return (W10)(w >> c10pos);
            }
            set
            {
                w = (w & ic10wmask) | ((ulong)value << c10pos);
            }
        }
        public W1 B9  /* Bit9 1 Bit*/
        {
            get
            {
                return (W1)(w >> b9pos);
            }
            set
            {
                w = (w & ib9wmask) | ((ulong)value << b9pos);
            }
        }
        public static explicit operator W38(W36 d)
        {
            return new W38(d);
        }
        public override string ToString()
        {
            return Convert.ToString((long)w, 8).PadLeft(13, '0');
        }
        public string ACToString()
        {
            StringBuilder sb = new StringBuilder();
            uint l = S + Q + PB;
            while (l < 3)
            {
                sb.Append(' ');
                l++;
            }
            if (S != 0)
                sb.Append('-');
            if (Q != 0)
                sb.Append('Q');
            if (PB != 0)
                sb.Append('P');
            sb.Append(M35.ToString());
            return sb.ToString();
        }
    }
    struct WA
    {
        static uint wmask; /* mask for word */
        public static int olen; /* octal lenght */
        uint w; /* N bit Word stored in 32 bit uint*/

        public static void SetMask(uint AddressMask)
        {
            olen = 0;
            wmask = AddressMask;
            while (AddressMask != 0)
            {
                AddressMask >>= 3;
                olen++;
            }
        }
        public WA(uint value)
        {
            w = value & wmask;
        }
        public static implicit operator uint(WA d)
        {
            return d.w;
        }
        public static explicit operator WA(uint d)
        {
            return new WA(d);
        }
        public override string ToString()
        {
            return Convert.ToString(w, 8).PadLeft(olen, '0');
        }
    }
    static class CPU704
    {
        static bool trapping = false; /* Trapping mode active */
        public static bool repeat; /* repeat last instruction */
        public static WA ILC; /* Instruction Counter */
        public static WA NIC; /* Next Instruction Counter for Transfer*/
        public static W36 SR; /* Storage register */
        static WA[] X = new WA[3]; /* Index Register */
        public static bool halt = false;
        static public void Clr()
        {
            ALU.Clear();
            X[0] = (WA)0;
            X[1] = (WA)0;
            X[2] = (WA)0;
            ILC = (WA)0;
            halt = false;
            trapping = false;
            repeat = false;
            CoreMemory.Clear();
        }
        static void SetX(WA A)
        {
            W3 T = SR.T;
            if (0 != (T & 1))
                X[0] = A;
            if (0 != (T & 2))
                X[1] = A;
            if (0 != (T & 4))
                X[2] = A;
        }
        static WA GetX()
        {
            uint A = 0;
            W3 T = SR.T;
            if (0 != (T & 1))
                A |= X[0];
            if (0 != (T & 2))
                A |= X[1];
            if (0 != (T & 4))
                A |= X[2];

            return (WA)A;
        }
        static WA GetY() /* Get Y*/
        {
            return (WA)(SR.A - GetX());
        }
        static W36 LoadCY() /* Load C(Y) */
        {
            return CoreMemory.C((WA)(SR.A - GetX()));
        }
        static void StoreCY(W36 V) /* Store V to C(Y) */
        {
            CoreMemory.C((WA)(SR.A - GetX()), V);
        }
        static WA GetYni() /* Get Y not indexed*/
        {
            return (WA)(uint)SR.A;
        }
        static W36 LoadCYni() /* Load C(Y) not indexed*/
        {
            return CoreMemory.C((WA)(uint)SR.A);
        }
        static void StoreCYni(W36 V) /* Store V to C(Y) not indexed*/
        {
            CoreMemory.C((WA)(uint)SR.A, V);
        }
        static void DebugHdr()
        {
            if (Io704.Config.LogCPU != null)
            {
                if (WA.olen == 4)
                    Io704.LogCPU.WriteLine("IC    AC            MQ             XR1   XR2  XR4");
                else
                    Io704.LogCPU.WriteLine("IC     AC            MQ              XR1    XR2   XR4");
            }
        }
        static void Debug(string OPC)
        {
            
            if (Io704.Config.LogCPU != null)
            {
                Io704.LogCPU.Write("{0} {1} {2} {3} {4} {5}  {6}   ", ILC, ALU.AC.ACToString(), ALU.MQ.MQtoString(), X[0], X[1], X[2], SR.MQtoString());
                Io704.LogCPU.WriteLine(OPC);
            }
            if (halt)
            {
                Io704.Flush();
                Console.Error.Write("{0} {1} {2} {3} {4} {5}  {6}   ", ILC, ALU.AC.ACToString(), ALU.MQ.MQtoString(), X[0], X[1], X[2], SR.MQtoString());
                Console.Error.WriteLine(OPC);
            }
        }
        static void DebugAT(string OPC)
        {
            if (Io704.Config.LogCPU != null)
            {
                Io704.LogCPU.Write("{0} {1} {2} {3} {4} {5}  {6}   ", ILC, ALU.AC.ACToString(), ALU.MQ.MQtoString(), X[0], X[1], X[2], SR.MQtoString());
                Io704.LogCPU.Write(OPC);
                if (SR.T != 0)
                    Io704.LogCPU.WriteLine(" {0},{1}", SR.A, SR.T);
                else
                    Io704.LogCPU.WriteLine(" {0}", SR.A);
            }
            if (halt)
            {
                Io704.Flush();
                Console.Error.Write("{0} {1} {2} {3} {4} {5}  {6}   ", ILC, ALU.AC.ACToString(), ALU.MQ.MQtoString(), X[0], X[1], X[2], SR.MQtoString());
                Console.Error.Write(OPC);
                if (SR.T != 0)
                    Console.Error.WriteLine(" {0},{1}", SR.A, SR.T);
                else
                    Console.Error.WriteLine(" {0}", SR.A);
            }
        }
        static void DebugAT0(string OPC)
        {
            if (Io704.Config.LogCPU != null)
            {
                Io704.LogCPU.Write("{0} {1} {2} {3} {4} {5}  {6}   ", ILC, ALU.AC.ACToString(), ALU.MQ.MQtoString(), X[0], X[1], X[2], SR.MQtoString());
                Io704.LogCPU.Write(OPC);
                Io704.LogCPU.WriteLine(" {0},{1}", SR.A, SR.T);
            }
            if (halt)
            {
                Io704.Flush();
                Console.Error.Write("{0} {1} {2} {3} {4} {5}  {6}   ", ILC, ALU.AC.ACToString(), ALU.MQ.MQtoString(), X[0], X[1], X[2], SR.MQtoString());
                Console.Error.Write(OPC);
                Console.Error.WriteLine(" {0},{1}", SR.A, SR.T);
            }
        }
        static void DebugATD(string OPC)
        {
            if (Io704.Config.LogCPU != null)
            {
                Io704.LogCPU.Write("{0} {1} {2} {3} {4} {5}  {6}   ", ILC, ALU.AC.ACToString(), ALU.MQ.MQtoString(), X[0], X[1], X[2], SR.MQtoString());
                Io704.LogCPU.Write(OPC);
                Io704.LogCPU.WriteLine(" {0},{1},{2}", SR.A, SR.T, SR.D);
            }
            if (halt)
            {
                Io704.Flush();
                Console.Error.Write("{0} {1} {2} {3} {4} {5}  {6}   ", ILC, ALU.AC.ACToString(), ALU.MQ.MQtoString(), X[0], X[1], X[2], SR.MQtoString());
                Console.Error.Write(OPC);
                Console.Error.WriteLine(" {0},{1},{2}", SR.A, SR.T, SR.D);
            }
        }
        static public bool Step()
        {
            /* Fetch instruction */
            SR = CoreMemory.C(ILC);
            bool transferInst = false;
            bool doTransfer = false;
            uint skip = 0;
            /* check if Type A or Type B instruction */
            bool splus = (SR.S == 0); /* Sign positive*/
            uint P = (SR.P & 3); /* lower 2 bits of prefix */
            bool inst = false;
            switch (P)
            {
                case 1: /*1000*/
                    if (splus) /* +1 : TXI Transfer with index incremeted */
                    {
                        DebugATD("TXI");
                        SetX((WA)(GetX() + SR.D));
                        NIC = GetYni();
                        transferInst = true;
                        doTransfer = true;
                        inst = true;
                    }
                    break;
                case 2:/*2000*/
                    if (splus) /* +2 : TIX Transfer on Index  */
                    {
                        DebugATD("TIX");
                        WA X = GetX();
                        transferInst = true;
                        if (X > (WA)(uint)SR.D)
                        {
                            SetX((WA)(X - SR.D));
                            NIC = GetYni();
                            doTransfer = true;
                        }
                        inst = true;
                    }
                    else /* -2 : TNX  Transfer on No Index  */
                    {
                        DebugATD("TNX");
                        WA X = GetX();
                        transferInst = true;
                        if (X <= (WA)(uint)SR.D)
                        {
                            NIC = GetYni();
                            doTransfer = true;
                        }
                        else
                            SetX((WA)(X - SR.D));
                        inst = true;
                    }
                    break;
                case 3: /*3000*/
                    if (splus) /* +3 : TXH */
                    {
                        DebugATD("TXH");
                        transferInst = true;
                        if (GetX() > (WA)(uint)SR.D)
                        {
                            NIC = GetYni();
                            doTransfer = true;
                        }
                        inst = true;
                    }
                    else /* -3 : TXL */
                    {
                        DebugATD("TXL");
                        transferInst = true;
                        if (GetX() <= (WA)(uint)SR.D)
                        {
                            NIC = GetYni();
                            doTransfer = true;
                        }
                        inst = true;
                    }
                    break;
                case 0:
                    /* Type B instruction */
                    P = (uint)(SR.D >> 6); /* upper 9 bits of Decrement */
                    switch (P)
                    {
                        case 0: /*000*/
                            if (splus) /*+000 HTR Halt and Transfer*/
                            {
                                halt = true;
                                DebugAT("HTR");
                                transferInst = true;
                                doTransfer = true;
                                NIC = GetY();
                                inst = true;
                            }
                            break;
                        case 16: /*020*/
                            if (splus) /*+20 TRA Transfer*/
                            {
                                DebugAT("TRA");
                                transferInst = true;
                                doTransfer = true;
                                NIC = GetY();
                                inst = true;
                            }
                            break;
                        case 17: /*021*/
                            if (splus) /*+20 TTR Trap Transfer*/
                            {
                                DebugAT("TTR");
                                doTransfer = true;
                                NIC = GetY();
                                inst = true;
                            }
                            break;
                        case 32: /*040*/
                            if (splus) /*+40 TLQ Transfer on Low MQ*/
                            {
                                DebugAT("TLQ");
                                transferInst = true;
                                doTransfer = ALU.TLQ();
                                NIC = GetY();
                                inst = true;
                            }
                            break;
                        case 60: /*074*/
                            if (splus) /*+74 TSX Transfer and set index */
                            {
                                DebugAT("TSX");
                                SetX((WA)(0x8000U - ILC));
                                transferInst = true;
                                doTransfer = true;
                                NIC = GetYni();
                                inst = true;
                            }
                            break;
                        case 64: /*100*/
                            if (splus) /* +100 TZE Transfer on Zero*/
                            {
                                DebugAT("TZE");
                                transferInst = true;
                                doTransfer = ALU.TZE();
                                NIC = GetY();
                                inst = true;
                            }
                            else   /* -100 TNZ Transfer on No Zero*/
                            {
                                DebugAT("TNZ");
                                transferInst = true;
                                doTransfer = ALU.TNZ();
                                NIC = GetY();
                                inst = true;
                            }
                            break;
                        case 80: /*120*/
                            if (splus) /*+120 TPL Transfer on Plus*/
                            {
                                DebugAT("TPL");
                                transferInst = true;
                                doTransfer = ALU.TPL();
                                NIC = GetY();
                                inst = true;
                            }
                            else /*-120 TMI Transfer on Minus*/
                            {
                                DebugAT("TMI");
                                transferInst = true;
                                doTransfer = ALU.TMI();
                                NIC = GetY();
                                inst = true;
                            }
                            break;
                        case 96: /*140*/
                            if (splus) /*+140 TOV Transfer on Overflow */
                            {
                                DebugAT("TOV");
                                transferInst = true;
                                doTransfer = ALU.TOV();
                                NIC = GetY();
                                inst = true;
                            }
                            else /*-140 TOV Transfer on No Overflow */
                            {
                                DebugAT("TNO");
                                transferInst = true;
                                doTransfer = ALU.TNO();
                                NIC = GetY();
                                inst = true;
                            }
                            break;
                        case 113: /*161*/
                            if (splus) /*+161 TQO Transfer on MQ Overflow */
                            {
                                DebugAT("TQO");
                                transferInst = true;
                                doTransfer = ALU.TQO();
                                NIC = GetY();
                                inst = true;
                            }
                            break;
                        case 114: /*162*/
                            if (splus) /*+161 TQP Transfer on MQ Plus*/
                            {
                                DebugAT("TQP");
                                transferInst = true;
                                doTransfer = ALU.TQP();
                                NIC = GetY();
                                inst = true;
                            }
                            break;
                        case 128: /*200*/
                            if (splus) /*+200 MPY Multiply*/
                            {
                                DebugAT("MPY");
                                ALU.MPY(GetY());
                                inst = true;
                            }
                            else /*-200 MPY Multiply and Round*/
                            {
                                DebugAT("MPR");
                                ALU.MPR(GetY());
                                inst = true;
                            }
                            break;
                        case 144: /*220*/
                            if (splus) /*+220 DVH Divide or HALT*/
                            {
                                DebugAT("DVH");
                                halt = ALU.DVH(GetY());
                                inst = true;
                            }
                            break;
                        case 145: /*221*/
                            if (splus) /*+221 DVH Divide or Proceed*/
                            {
                                DebugAT("DVP");
                                ALU.DVP(GetY());
                                inst = true;
                            }
                            break;
                        case 160: /*240*/
                            if (splus) /*+240 FDH Floating Divide or HALT*/
                            {
                                halt = ALU.FDH(GetY());
                                DebugAT("FDH");
                                inst = true;
                            }
                            break;
                        case 161: /*241*/
                            if (splus) /*+241 FDP Floating Divide or Proceed*/
                            {
                                DebugAT("FDP");
                                ALU.FDP(GetY());
                                inst = true;
                            }
                            break;
                        case 176: /*260*/
                            if (splus) /*+260 FMP FloatingMultiply */
                            {
                                DebugAT("FMP");
                                ALU.FMP(GetY());
                                inst = true;
                            }
                            else /*-260 UFM Unnormalizied Floating Multiply */
                            {
                                DebugAT("UFM");
                                ALU.UFM(GetY());
                                inst = true;
                            }
                            break;
                        case 192: /*300*/
                            if (splus) /*+300 FAD Floating Add */
                            {
                                DebugAT("FAD");
                                ALU.FAD(GetY());
                                inst = true;
                            }
                            else /*-300 UFA Unnormalizied Floating Add */
                            {
                                DebugAT("UFA");
                                ALU.UFA(GetY());
                                inst = true;
                            }
                            break;
                        case 194: /*302*/
                            if (splus) /* +302 FSB Floating Subtract */
                            {
                                DebugAT("FSB");
                                ALU.FSB(GetY());
                                inst = true;
                            }
                            else /* -302 UFS Unnormalizied Floating Subtract */
                            {
                                DebugAT("UFS");
                                ALU.UFS(GetY());
                                inst = true;
                            }
                            break;
                        case 208: /*320*/
                            if (splus) /*+320 ANS AND to Storage*/
                            {
                                DebugAT("ANS");
                                ALU.ANS(GetY());
                                inst = true;
                            }
                            else /*-320 ANA AND to Accumulator*/
                            {
                                DebugAT("ANA");
                                ALU.ANA(GetY());
                                inst = true;
                            }
                            break;
                        case 224: /*340*/
                            if (splus) /*+340 CAS Compare Accumulator with Storage*/
                            {
                                DebugAT("CAS");
                                skip = ALU.CAS(GetY());
                                inst = true;
                            }
                            break;
                        case 241: /*361*/
                            if (splus) /*+361 ACL Add and Carry Logical Word*/
                            {
                                DebugAT("ACL");
                                ALU.ACL(GetY());
                                inst = true;
                            }
                            break;
                        case 256: /*400*/
                            if (splus) /*+400 ADD Add */
                            {
                                DebugAT("ADD");
                                ALU.ADD(GetY());
                                inst = true;
                            }
                            else/*-400 SBM Subtract Magnitude */
                            {
                                DebugAT("SBM");
                                ALU.SBM(GetY());
                                inst = true;
                            }
                            break;
                        case 257: /*401*/
                            if (splus) /*+401 ADM Add Magnitude */
                            {
                                DebugAT("ADM");
                                ALU.ADM(GetY());
                                inst = true;
                            }
                            break;
                        case 258: /*402 */
                            if (splus) /*+402 SUB*/
                            {
                                DebugAT("SUB");
                                ALU.SUB(GetY());
                                inst = true;
                            }
                            break;
                        case 272: /*420*/
                            if (splus) /*+420 HTR Halt and Proceed*/
                            {
                                halt = true;
                                DebugAT("HPR");
                                inst = true;
                            }
                            break;
                        case 304: /*460 */
                            if (splus) /*+460 LDA Locate Drum Address*/
                            {
                                DebugAT("LDA");
                                Io704.LDA((uint)LoadCY());
                                inst = true;
                            }
                            break;
                        case 320: /*500*/
                            if (splus) /*+500 CLA Clear and ADD */
                            {
                                DebugAT("CLA");
                                ALU.CLA(GetY());
                                inst = true;
                            }
                            else /*-500 CAL Clear and ADD logical Word */
                            {
                                DebugAT("CAL");
                                ALU.CAL(GetY());
                                inst = true;
                            }
                            break;
                        case 321: /*501*/
                            if (!splus) /*-501 ORA Or to Accumulator*/
                            {
                                DebugAT("ORA");
                                ALU.ORA(GetY());
                                inst = true;
                            }
                            break;
                        case 322: /*502*/
                            if (splus) /*+502 CLS Clear and Subtract */
                            {
                                DebugAT("CLS");
                                ALU.CLS(GetY());
                                inst = true;
                            }
                            break;
                        case 348: /*534*/
                            if (splus) /*+534 LXA Load Index from Address*/
                            {
                                DebugAT("LXA");
                                SetX((WA)(uint)LoadCYni().A);
                                inst = true;
                            }
                            else   /* -534 LXD Load Index from Decrement */
                            {
                                DebugAT("LXD");
                                SetX((WA)(uint)LoadCYni().D);
                                inst = true;
                            }
                            break;
                        case 368: /*560*/
                            if (splus) /*+560 LDQ Load MQ*/
                            {
                                DebugAT("LDQ");
                                ALU.LDQ(GetY());
                                inst = true;
                            }
                            break;
                        case 384: /*600*/
                            if (splus) /*+600 STZ Store Zero*/
                            {
                                DebugAT("STZ");
                                StoreCY(new W36());
                                inst = true;
                            }
                            else /*-600 STQ Store MQ*/
                            {
                                DebugAT("STQ");
                                ALU.STQ(GetY());
                                inst = true;
                            }
                            break;
                        case 385: /*601*/
                            if (splus) /*+601 STO Store*/
                            {
                                DebugAT("STO");
                                ALU.STO(GetY());
                                inst = true;
                            }
                            break;
                        case 386: /*602*/
                            if (splus) /*+602 SLW Store Logical Word*/
                            {
                                DebugAT("SLW");
                                ALU.SLW(GetY());
                                inst = true;
                            }
                            else /*-602 ORS Or to Storage*/
                            {
                                DebugAT("ORS");
                                ALU.ORS(GetY());
                                inst = true;
                            }
                            break;
                        case 400:/*620*/
                            if (!splus) /*+620 SLQ Store Left Half MQ*/
                            {
                                DebugAT("SLQ");
                                ALU.SLQ(GetY());
                                inst = true;
                            }
                            break;
                        case 401:/*621*/
                            if (splus) /*+621 STA Store Address*/
                            {
                                DebugAT("STA");
                                ALU.STA(GetY());
                                inst = true;
                            }
                            break;
                        case 402:/*622*/
                            if (splus) /*+622 STD Store Decrement*/
                            {
                                DebugAT("STD");
                                ALU.STD(GetY());
                                inst = true;
                            }
                            break;
                        case 408:/*630*/
                            if (splus) /*+630 STP Store Prefix*/
                            {
                                DebugAT("STP");
                                ALU.STP(GetY());
                                inst = true;
                            }
                            break;
                        case 412: /*634*/
                            if (!splus) /*-634 SXD Store index in Decrement*/
                            {
                                DebugAT("SXD");
                                
                                W36 temp = LoadCYni();
                                temp.D = (W15)(uint)GetX();
                                SetX((WA)(uint)temp.D); /* set to handle multiple index register */
                                CoreMemory.C(GetYni(), temp);
                                inst = true;
                            }
                            break;
                        case 448:/*700*/
                            if (splus) /*+700 CPY Copy or Skip */
                            {
                                DebugAT("CPY");
                                skip = (uint)Io704.CPY(ref CoreMemory.Mem[GetY()]);
                                inst = true;
                            }
                            else /*-700 CAD Copy and Add Logical Word */
                            {
                                DebugAT("CAD");
                                skip = (uint)Io704.CPY(ref CoreMemory.Mem[GetY()]);
                                if (skip == 0)
                                    ALU.ACL(GetY());
                                inst = true;
                            }
                            break;
                        case 476:/*734*/
                            if (splus) /* +734 PAX Place Address in Index */
                            {
                                DebugAT("PAX");
                                SetX(ALU.PAX());
                                inst = true;
                            }
                            else /* -734 PDX Place Decrement in Index */
                            {
                                DebugAT("PDX");
                                SetX(ALU.PDX());
                                inst = true;
                            }
                            break;
                        case 492:/*754*/
                            if (!splus) /* -754 PXD Place Index in Decrement*/
                            {
                                DebugAT0("PXD");
                                WA tmp = GetX();
                                SetX(tmp);  /* hanlde case of multiple index register specified*/
                                ALU.PXD(tmp);
                                inst = true;
                            }
                            break;
                        case 496: /*760*/
                            {
                                uint unit = GetY();
                                uint subunit = unit & 15;
                                unit >>= 4;
                                if (unit == 0)
                                    switch (subunit)
                                    {
                                        case 0: /*760...000*/
                                            if (splus)/*+760...000 CLM CLear Magnitude*/
                                            {
                                                Debug("CLM");
                                                ALU.CLM();
                                                inst = true;
                                            }
                                            break;
                                        case 1: /*760...001*/
                                            if (splus)/*+760...001 LBT Low Order Bit Test */
                                            {
                                                Debug("LBT");
                                                skip = ALU.LBT();
                                                inst = true;
                                            }
                                            else /*-760...001 PBT P Bit Test */
                                            {
                                                Debug("PBT");
                                                skip = ALU.PBT();
                                                inst = true;
                                            }
                                            break;
                                        case 2: /*760...002*/
                                            if (splus)/*+760...002 CHS Change Sign*/
                                            {
                                                Debug("CHS");
                                                ALU.CHS();
                                                inst = true;
                                            }
                                            break;
                                        case 3: /*760...003*/
                                            if (splus)/*+760...003 SSP Set Sign Plus*/
                                            {
                                                Debug("SSP");
                                                ALU.SSP();
                                                inst = true;
                                            }
                                            else/*-760...003 SSP Set Sign Plus*/
                                            {
                                                Debug("SSM");
                                                ALU.SSM();
                                                inst = true;
                                            }
                                            break;
                                        case 6: /*760...006*/
                                            if (splus)/*+760...006 COM Complement Magnitude*/
                                            {
                                                Debug("COM");
                                                ALU.COM();
                                                inst = true;
                                            }
                                            break;
                                        case 7:  /*760...007*/
                                            if (splus) /*-760...007 ETM Enter Trapping Mode*/
                                            {
                                                Debug("ETM");
                                                trapping = true;
                                                inst = true;
                                            }
                                            else /*-760...007 LTM Leave Trapping Mode*/
                                            {
                                                Debug("LTM");
                                                trapping = false;
                                                inst = true;
                                            }
                                            break;
                                        case 8: /*760...010*/
                                            if (splus)/*+760...010 RND Round*/
                                            {
                                                Debug("RND");
                                                ALU.RND();
                                                inst = true;
                                            }
                                            break;
                                        case 9: /*760...011*/
                                            if (splus)/*+760...011 ETT End of Tape Test*/
                                            {
                                                Debug("ETT");
                                                skip = Io704.ETT();
                                                inst = true;
                                            }
                                            break;
                                        case 10: /*760...012 */
                                            if (splus) /*+760...012 DCT Divide Check Test */
                                            {
                                                Debug("DCT");
                                                skip = ALU.DCT();
                                                inst = true;
                                            }
                                            else /*-760...012 RTT Redundancy Tape Test*/
                                            {
                                                Debug("RTT");
                                                skip = Io704.RTT();
                                                inst = true;
                                            }
                                            break;
                                    }

                                else
                                {
                                    if (splus) /* +760 PSE Plus sense*/
                                    {
                                        string Opc = "PSE";

                                        if (unit == 6 && subunit == 0)
                                            Debug("SLF");
                                        else if (unit == 6 && subunit != 0)
                                            Debug("SLN" + subunit.ToString());
                                        else if (unit == 7)
                                            Debug("SWT" + subunit.ToString());
                                        else
                                            DebugAT(Opc);
                                        skip = (uint)Io704.PSE(GetY());
                                        inst = true;
                                    }
                                    else /* +760 MSE Minus sense*/
                                    {
                                        string Opc = "MSE";

                                        if (unit == 6)
                                            Debug("SLT" + subunit.ToString());
                                        else
                                            DebugAT(Opc);
                                        skip = (uint)Io704.MSE(GetY());
                                        inst = true;
                                    }
                                }
                            }
                            break;
                        case 497:/*761*/
                            if (splus) /* +761 NOP No Operation*/
                            {
                                Debug("NOP");
                                inst = true;
                            }
                            break;
                        case 498:/*762*/
                            if (splus) /* +762 RDS Read Select */
                            {
                                uint unit = GetY();
                                string OPcode = "RDS";

                                DebugAT(OPcode);
                                Io704.RDS(unit);
                                if (halt && repeat && Io704.Config.LogCPU == null)
                                    DebugAT(OPcode);
                                inst = true;
                            }
                            break;
                        case 499: /*763*/
                            if (splus) /* +763 LLS Long Left Shift */
                            {
                                DebugAT("LLS");
                                ALU.LLS(GetY());
                                inst = true;
                            }
                            else/* -763 LGL Logical Left */
                            {
                                DebugAT("LGL");
                                ALU.LGL(GetY());
                                inst = true;
                            }
                            break;
                        case 500:/*764*/
                            if (splus) /* +764 BST Backspace Tape */
                            {
                                DebugAT("BSR");
                                Io704.BST(GetY());
                                inst = true;
                            }
                            break;
                        case 501: /*765*/
                            if (splus) /*+765 LRS Long Right Shift*/
                            {
                                DebugAT("LRS");
                                ALU.LRS(GetY());
                                inst = true;
                            }
                            break;
                        case 502: /*766*/
                            if (splus) /*+766 WRS Write Select*/
                            {
                                uint unit = GetY();
                                string OPcode = "WRS";

                                DebugAT(OPcode);
                                Io704.WRS(unit);
                                inst = true;
                            }
                            break;
                        case 503: /*767*/
                            if (splus) /*+767 ALS Accumulator Left Shift */
                            {
                                DebugAT("ALS");
                                ALU.ALS(GetY());
                                inst = true;
                            }
                            break;
                        case 504: /*770*/
                            if (splus) /*+770 WEF Write End of File*/
                            {
                                DebugAT("WEF");
                                Io704.WEF(GetY());
                                inst = true;
                            }
                            break;
                        case 505: /*771*/
                            if (splus) /*+771 Accumulator Right Shift*/
                            {
                                DebugAT("ARS");
                                ALU.ARS(GetY());
                                inst = true;
                            }
                            break;
                        case 506: /*772*/
                            if (splus) /*+772 REW Rewind Tape*/
                            {
                                DebugAT("REW");
                                Io704.REW(GetY());
                                inst = true;
                            }
                            break;
                        case 507: /*773*/
                            if (!splus) /*-773 RQL Rotate MQ Left*/
                            {
                                DebugAT("RQL");
                                ALU.RQL(GetY());
                                inst = true;
                            }
                            break;
                    }
                    break;
            }
            if (!inst)
            {
                Console.Error.WriteLine("Operation {0} not implemented", SR);
                halt = true;
            }
            if (trapping && transferInst)
            {
                WA A0 = new WA();
                W36 W0 = CoreMemory.C(A0);
                W0.A = (W15)(uint)ILC;
                CoreMemory.C(A0, W0);
                if (doTransfer)
                    NIC = new WA(1);
            }
            if (!doTransfer)
                ILC = (WA)(ILC + 1 + skip);
            return doTransfer;
        }
        static public void LoadCrd()
        {

            Io704.RDS(13 * 16 + 1);
            Io704.CPY(ref CoreMemory.Mem[0]);
            Io704.CPY(ref CoreMemory.Mem[1]);
            ALU.MQ = new W36();
            ILC = (WA)0;
            Go();
        }
        static public void LoadTape()
        {

            Io704.RDS(9 * 16 + 1);
            Io704.CPY(ref CoreMemory.Mem[0]);
            Io704.CPY(ref CoreMemory.Mem[1]);
            ALU.MQ = new W36();
            ILC = (WA)0;
            Go();
        }
        static public void LoadDrm()
        {
            Io704.RDS(12 * 16 + 1);
            Io704.CPY(ref CoreMemory.Mem[0]);
            Io704.CPY(ref CoreMemory.Mem[1]);
            ALU.MQ = new W36();
            ILC = (WA)0;
            Go();
        }
        static public void Go()
        {
            int HaltCnt = 0;
            bool transfer = false;
            DebugHdr();
            do
            {
                if (transfer)
                    ILC = NIC;
                transfer = Step();
                if (halt)
                {
                    if (repeat)
                    {
                        ILC = (WA)(ILC - 1);
                        repeat = false;
                    }
                    Console.Error.WriteLine("HALT at {0}", ILC);
                    halt = false;
                    if (Io704.Config.ExitOnHalt <= 0)
                    {                        
                        string l = Console.ReadLine();
                        if (l == "x")
                            break;
                        if (l.Length >= 2 && l.Substring(0, 2) == "go")
                        {
                            string[] split = l.Split(new char[] { ' ' });
                            ILC = (WA)(uint)Convert.ToInt32(split[1], 8);
                            transfer = false;
                        }
                    }
                    else
                    {
                        HaltCnt++;
                        if (HaltCnt >= Io704.Config.ExitOnHalt)
                            break;
                    }
                }
            }
            while (true);
        }
    }

}

