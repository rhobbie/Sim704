
namespace Sim704
{
    static class ALU
    {
        public static W38 AC;
        static W36 SR;
        public static W36 MQ;
        static bool acoflag;
        static bool mqoflag;
        static bool dcheck;
        public static void Clear()
        {
            AC = new W38();
            MQ = new W36();
            acoflag = mqoflag = dcheck = false;
        }
        static void Iadd()
        {
            bool f1 = false; /* True: Signs of AC and SR differs */

            W1 f2 = AC.S; /* Sign of AC */
            W1 f8 = AC.PB; /* P of in AC */

            /* Make AC Positive  */
            AC.S = new W1();

            /* Check signes of SR & AC */
            if (SR.S != f2)
            {
                AC.M37 = (W37)~AC.M37; /* One's compliment */
                f1 = true;
            }
            AC = (W38)(AC + SR.M);

            /* Check carry from Q */
            if (f1) /* Check if signs were not same */
            {
                if (0 != AC.S)
                {
                    f2 = (W1)~f2;
                    AC = (W38)(AC + 1);
                    if (AC.PB != f8)
                        acoflag = true;
                }
                else
                    AC.M37 = (W37)~AC.M37; /* One's compliment */
            }
            else if (AC.PB != f8)
                acoflag = true;
            /* Restore sign to AC */
            AC.S = f2;
        }
        static void Impy(bool rnd)
        {

            int shiftcnt = 35;
            W1 f1 = MQ.S;
            W1 f2 = SR.S;
            SR.S = new W1();
            MQ.S = new W1();
            AC = new W38();  /* clear AC */
            if (SR == 0)
                MQ = new W36();
            else
                while (shiftcnt-- > 0)
                {
                    if ((MQ & 1) != 0)
                        AC = (W38)(AC + SR);
                    MQ = (W36)(MQ >> 1);
                    if ((AC & 1) != 0)
                        MQ.B1 = new W1(1);
                    AC = (W38)(AC >> 1);
                }
            if (rnd && (MQ.B1 != 0))
                AC = (W38)(AC + 1);
            if (0 != (f1 ^ f2))
            {
                MQ.S = new W1(1);
                AC.S = new W1(1);
            }
        }
        static bool Idiv(bool dvh)
        {
            int shiftcnt = 35;
            
            /* Save sign */
            W1 f1 = SR.S;
            SR.S = new W1();
            W1 f2 = AC.S;
            /* Check if SR less then AC */
            if (SR <= AC.M37)
            {
                dcheck = true;
                MQ.S = (W1)(f2 ^ f1);
                if (dvh)
                    return true;
            }
            /* Clear signs */
            MQ.S = new W1();
            AC.S = new W1();

            /* Do divide operation */
            do
            {
                AC.M37 = (W37)(AC.M37 << 1);
                MQ = (W36)(MQ << 1);
                if (MQ.S != 0)
                {
                    MQ.S = new W1();
                    AC = (W38)(AC | 1);
                }
                if (SR <= AC)
                {
                    AC = (W38)(AC - SR);
                    MQ = (W36)(MQ | 1);
                }
            }
            while (--shiftcnt != 0);

            AC.S = f2;
            MQ.S = new W1(f1 ^ f2);
            return false;
        }
        static void Fadd(bool norm)
        {
            /* The C(Y) are algebraically added to the C(AC), 
             * and this sum replaces the C(AC) and the C(MQ). 
             * The C(Y) are unchanged. Floating-point addition 
             * takes place in the following way: */

            bool tmpacoflag = false;
            bool tmpmqoflag = false;
            /*  1. The MQ is cleared. */
            MQ = new W36();
            /*  3. If the characteristic in the SR is less than the 
             *  characteristic in the AC, the C(SR) and the C(AC) 
             *  interchange automatically because the number with 
             *  the smaller characteristic must appear in the AC be-
             *  fore addition can take place. (Positions Q and P of 
             *  the AC are considered as part of the characteristic. 
             *  Consequently, a 1 in either of these positions makes 
             *  the characteristic in the AC larger than that in the 
             *  SR, but the 1's would be lost during the interchange 
             *  and an incorrect answer will result.) */
            /* Extract AC char */
            int shiftcnt = (int)(uint)AC.C10;
            /* Diff SR char */
            shiftcnt -= (int)(uint)SR.C;
            if (shiftcnt > 0) /* AC Bigger */
            {
                /* Exchange AC & SR */
                W38 tmp = AC;
                AC = new W38 { S = SR.S, M37 = (W37)SR.M };
                SR = new W36 { S = tmp.S, M = tmp.M35 };
            }
            else /* SR Bigger then AC, AC Smaller */
                shiftcnt = -shiftcnt; /* Change sign */

            uint fptemp = SR.C; /* Get exponent */
            /* Save AC & SR signs */
            W1 f1 = AC.S;
            W1 f2 = SR.S;

            /* Clear sign */
            SR.S = new W1();
            /* Clear char and sign */
            AC.S = new W1();
            AC.C10 = new W10();

            /*  5. The fraction in the AC is shifted right the num-
             *  ber of positions equal to the magnitude of the differ-
             *  ence in the characteristics. Bits shifted out of the 
             *  AC enter position 9 of the MQ. Bits shifted out of 
             *  position 35 of the MQ are lost. */
            shiftcnt &= 255;
            if (shiftcnt >= 0 && shiftcnt < 63)
            {
                while (shiftcnt > 0)
                {
                    MQ = (W36)(MQ >> 1);
                    MQ.B9 = new W1((uint)AC);
                    AC = (W38)(AC >> 1);
                    shiftcnt--;
                }
            }
            else
                AC = new W38();

            /*  7. The fraction in the SR is algebraically added to 
             *  the fraction in the AC and this sum replaces the 
             *  C(AC)S,9-35.  */
            /* Do add */
            if (f2 != f1)
            {
                AC = (W38)(AC - SR.F);
                /* If AC < 0 then SR was larger */
                if (AC.S != 0)
                {
                    AC = (W38)~AC;
                    if (MQ.F != 0)
                    {
                        MQ.F = (W27)~MQ.F;
                        MQ = (W36)(MQ + 1);
                    }
                    else
                        AC = (W38)(AC + 1);
                }
                else
                    f2 = (W1)~f2; /* Change sign of AC */
            }
            else
                AC = (W38)(AC + SR.F);

            /*  8. If the magnitude of the sum is greater than or 
             *  equal to 1, there is a carry from position 9 to position 
             *  8 of the AC (thus increasing the characteristic by 1).* 
             *  In this event, the C(AC)9-35 and the C(MQ)9-35 are 
             *  shifted right one position and 1 is inserted in posi-
             *  tion 9 of the AC. */

            /* Check for overflow */
            if ((AC.C10 & 1) != 0)
            {
                if ((AC & 1) != 0)
                    MQ.C = (W8)(MQ.C | 1);
                AC = (W38)(AC >> 1);
                MQ = (W36)(MQ >> 1);

                /* OV check */
                if (fptemp == 255)
                {
                    tmpacoflag = true;
                    tmpmqoflag = true;
                }
                fptemp++;
            }

            /*  9b. If the magnitude of the resulting fraction in 
             *  the AC is not in normal form (i.e. less than 1/2 but 
             *  not zero), and the signs of the MQ and AC are the 
             *  same, the C(AC)9-35 and the C(MQ)9-35 are shifted 
             *  left until a 1 is in position 9 of the AC. Bits enter 
             *  position 35 of the AC from position 9 of the MQ. The 
             *  characteristic in the AC is reduced by 1 for each posi-
             *  tion shifted.* If the signs of the MQ and AC are differ
             *  ent, the magnitude of the fraction in the AC is reduced 
             *  by 1 before the shifting is begun. Each bit entering 
             *  position 35 of the AC from position 9 of the MQ is 
             *  inverted. */
            /* Are we normalizing */
            if (norm)
            {
                while (AC.B9 == 0 &&
                       AC.F != 0) /* 704 does not check MQ when normalizing */
                {

                    MQ = (W36)(MQ << 1);
                    AC = (W38)(AC << 1);
                    if ((MQ.C & 1) != 0)
                    {
                        AC = (W38)(AC | 1);
                        MQ.C = (W8)(MQ.C & 0xFE);
                    }
                    if (fptemp == 0 && !tmpmqoflag)
                        tmpacoflag = true;
                    fptemp--;       /* UF Check */
                }
                /*  9a. If the resulting fraction in the AC is zero, the 
                 *  AC is cleared, yielding a normal zero. The sign of the 
                 *  AC is the sign of the number that has the smaller 
                 *  characteristic. If both characteristics are equal, then 
                 *  the sign of the AC is the sign of the number in the AC. */
                if (AC == 0 && MQ == 0)
                {
                    fptemp = 0;
                    f2 |= f1;
                }
            }

            /* Put pieces back together */
            AC.C10 = (W10)fptemp;
            AC.S = new W1();
            MQ.C = new W8();
            MQ.S = new W1();
            if (AC != 0)
            {
                if (fptemp < 27 && !acoflag)
                    mqoflag = true;
                fptemp -= 27;
                MQ.C = (W8)fptemp;
            }
            AC.S = MQ.S = f2;
            if (tmpacoflag)
                acoflag = true;
            if (tmpmqoflag)
                mqoflag = true;
        }
        static void Fmpy(bool norm)
        {
            bool tmpacoflag = false;
            bool tmpmqoflag = false;

            AC = new W38();
            /* Quick out for times 0 */
            if (SR == 0)
            {
                MQ.M = new W35();
                AC.S = MQ.S;
                return;
            }
            W1 f;
            /* Result sign */
            if (MQ.S != SR.S)
                f = (W1)1;
            else
                f = (W1)0;

            /* Extract characteristic */
            int fptemp = (int)(uint)MQ.C;
            fptemp += (int)(uint)SR.C;
            fptemp -= 128;
            MQ = new W36 { F = MQ.F };
            SR = new W36 { F = SR.F };
            /* Do multiply */
            int shiftcnt = 27;
            while (shiftcnt-- > 0)
            {
                if ((MQ & 1) != 0)
                    AC = (W38)(AC + SR);
                MQ = (W36)(MQ >> 1);
                if ((AC & 1) != 0)
                    MQ.B9 = new W1(1);
                AC = new W38 { F = (W27)(AC >> 1) };
            }

            /* Normalize the result */
            if (norm)
            {
                if (AC.B9 == 0)
                {
                    MQ = (W36)(MQ << 1);
                    AC = (W38)(AC << 1);
                    if ((MQ.C & 1) != 0)
                        AC = (W38)(AC | 1);
                    MQ = new W36 { F = MQ.F };
                    fptemp--;
                }
                if (AC == 0)
                    fptemp = 0;
            }
            if (AC != 0 || !norm)
            {
                if (fptemp < 0)
                    tmpacoflag = true;

                else if (fptemp > 0377)
                {
                    tmpacoflag = true;
                    tmpmqoflag = true;
                }
                AC.C10 = (W10)(uint)fptemp;
                fptemp -= 27;
                if (fptemp < 0)
                    tmpmqoflag = true;
                else if (fptemp > 255)
                {
                    tmpacoflag = true;
                    tmpmqoflag = true;
                }
                MQ.C = (W8)(uint)fptemp;  /* UF Check */
            }
            MQ.S = AC.S = f;

            if (tmpacoflag)
                acoflag = true;
            if (tmpmqoflag)
                mqoflag = true;

        }
        static bool Fdiv(bool dvh)
        {

            W1 f1;
            W1 f2;

            /* Sign of SR => MQ */
            if (SR.S != AC.S)
                f1 = (W1)1;
            else
                f1 = (W1)0;
            f2 = AC.S;

            int shiftcnt = 27;

            /* Begin common FDP/FDH code */

            if (AC.F >= 2 * SR.F || SR.F == 0)
            {
                dcheck = true;
                MQ.S = f1;
                return true;
            }

            /* Check for divide by 0 */
            if (AC.F == 0)
                AC = new W38();
            else
            {
                /* Split appart fraction and charateristics */
                int fptemp2 = (int)(AC.C10 & 255);
                int fptemp = (int)(uint)SR.C;
                AC = new W38(AC.F);
                SR = new W36(SR.F);

                /* Precheck SR less then AC */
                if (AC >= SR)
                {
                    if ((AC & 1) != 0)
                        MQ.B9 = (W1)1;
                    AC = (W38)(AC >> 1);
                    fptemp2++;
                }
                /* Do actual divide */
                do
                {
                    AC = (W38)(AC << 1);
                    MQ = (W36)(MQ << 1);
                    if ((MQ.C & 1) != 0)
                    {
                        MQ.C = (W8)(MQ.C & 0xFE);
                        AC = (W38)(AC | 1);
                    }
                    if (SR <= AC)
                    {
                        AC = (W38)(AC - SR);
                        MQ = (W36)(MQ | 1);
                    }
                } while (--shiftcnt != 0);

                /* Compute new characteristic */

                AC = new W38(AC.F);
                fptemp = (fptemp2 - fptemp) + 128;  /* UF check */
                if (fptemp > 255)
                {
                    acoflag = true;
                    mqoflag = true;
                }
                else if (fptemp < 0)
                    mqoflag = true;
                MQ.C = (W8)(uint)fptemp;
                fptemp2 -= 27;
                AC.C10 = (W10)(uint)fptemp2; /* UF check */
            }
            /* Fix signs on results */
            MQ.S = f1;
            AC.S = f2; /* Sign does not change */
            return false;
        }
        #region Fixed-Point Arithmetic Operations
        /* The following instructions refer to aritmetic operations using fixed-point data.*/
        public static void CLA(WA Y) /* Clear and Add */
        {
            /* The C(Y) replace the C(AC)S,1-35. Positions Q and 
             * P of the AC are cleared. The C(Y) are unchanged */
            W36 tmp = CoreMemory.C(Y);
            AC = new W38 { S = tmp.S, M37 = (W37)tmp.M };
        }
        public static void ADD(WA Y) /* Add */

        {
            /* This instruction algebraically adds the C(Y) to the 
             * C(AC) and replaces the C(AC) with this sum. The 
             * C(Y) are unchanged. AC overflow is possible */

            SR = CoreMemory.C(Y);
            Iadd();
        }
        public static void ADM(WA Y) /* Add Magnitude */
        {
            /* This instruction algebraically adds the magnitude 
             * of the C(Y) to the C(AC) and replaces the C(AC) 
             * with this sum. The C(Y) are unchanged. AC overflow 
             * is possible */
            SR = CoreMemory.C(Y);
            SR.S = new W1();
            Iadd();
        }
        public static void CLS(WA Y) /* Clear and Subtract */
        {
            /* The negative of the C(Y) replaces the C(AC)S,1-35.
             * Posotions Q and P of the AC are cleared. The C(Y)
             * are unchanged. */

            W36 tmp = CoreMemory.C(Y);
            AC = new W38 { S = (W1)~tmp.S, M37 = (W37)tmp.M };            
        }
        public static void SUB(WA Y) /* Subtract */
        {
            /* This instruction algebraically subtracts the C(Y) 
             * from the C(AC) and replaces the C(AC) with this 
             * difference. The C(Y) are unchanged. AC overflow 
             * is possible */
            SR = CoreMemory.C(Y);
            SR.S = (W1)~SR.S;
            Iadd();
        }
        public static void SBM(WA Y) /* Subtract Magnitude */
        {
            /* This instruction algebraically subtracts the mag-
             * nitude of the C(Y) from the C(AC) and replaces the 
             * C(AC) with this difference. The C(Y) are unchanged. 
             * AC overflow is possible */
            SR = CoreMemory.C(Y);
            SR.S = new W1(1);
            Iadd();
        }
        public static void MPY(WA Y) /* Multiply */
        {
            /* This instruction multiples the C(Y) by the C(MQ).
             * The most significant bits of the 70-bit product
             * replace the C(AC)1-35 and the 35 least significant bits
             * replace the C(MQ)1-35. the Q and P bits are cleared.
             * The sign of the AC is the algebraic sign of the product.
             * The sign of the MQ agrees with the sign of the AC */
            SR = CoreMemory.C(Y);
            Impy(false);
        }
        public static void MPR(WA Y) /* Multiply and Round */
        {
            /* This instruction executes a multiply followed by 
             * a round. (The latter operation is defined below.) AC
             * overflow is not possible */
            SR = CoreMemory.C(Y);
            Impy(true);
        }
        public static void RND() /* Round */
        {
            /* If position of the MQ contains a 1, the magnitude
             * of the C(AC) is increased by a 1 in position 35. If
             * position 1 of the MA contains a zero, the C(AC) re-
             * main unchanged. In either case, the C(MQ) are un-
             * changed. AC overflow is possible.*/
            if ((MQ & 1) != 0)
            {
                W1 Pold = AC.PB;
                AC.M37 = (W37)(AC.M37 + 1);
                if (AC.PB != Pold)
                    acoflag = true;
            }
        }
        public static bool DVH(WA Y) /* Divide or Halt */
        {
            /* This instruction treats the C(AC)S,Q,P,1-35 and the
             * C(MQ)1,35 a a 72 bit divident plus sign, and the
             * C(Y) as the divisor. If |C(Y)| > |C(AC)|, division
             * takes place, a 35-bit quotiont plus sign replaces the
             * C(MQ) and the remainder replaces the C(AC)S,1-35.
             * The sign of the remainder always agrees with the sign
             * of the dividend.
             * if |C(Y)| <= |C(AC)|, division does not take place
             * and the calculator stops with the divide-check indi-
             * cator and light on. Consequently, if position Q or P
             * of the AC contains a 1, division does not take place
             * since |C(Y)|<|C(AC)|. The dividend remains un-
             * changed in the AC */
            SR = CoreMemory.C(Y);
            return Idiv(true);

        }
        public static void DVP(WA Y)/* Divide or Proceed */
        {
            /* This instruction executes a divison (as defined 
             * above if |C(Y)| > |C(AC)|. If |C(Y)| <= |C(AC)|, 
             * division does not take place, the divide-check indi-
             * cator and light are turned on, and the calculator pro-
             * ceeds to the next instuction The dividend remains 
             * unchanged in the AC.*/
            SR = CoreMemory.C(Y);
            Idiv(false);
        }
        public static void LDQ(WA Y)/* Load MQ */
        {
            /* The C(Y) replace the C(MQ). The C(Y) are un-
             * changed.*/
            MQ = CoreMemory.C(Y);
        }
        public static void STQ(WA Y) /* Store MQ */
        {
            /* The C(MQ) replace the C(Y). The C(MQ) are un-
             * changed.*/
            CoreMemory.C(Y, MQ);
        }
        public static void SLQ(WA Y) /* Store Left-Half MQ */
        {
            /* The C(MQ)S,1-17 replace the C(Y)S,1-17. The
             * C(Y)18-35 and the C(MQ) are unchaged. */
            W36 tmp = CoreMemory.C(Y);
            tmp.L = MQ.L;
            CoreMemory.C(Y, tmp);
        }
        public static void STO(WA Y) /* Store */
        {
            /* The C(AC)S,1-35 replace the C(Y). The C(AC) are un-
             * changed.*/
            CoreMemory.C(Y, new W36 { S = AC.S, M = AC.M35 });
        }
        public static void STP(WA Y) /* Store Prefix */
        {
            /* The C(AC)P,1,2 replace the C(Y)S,1,2. The
             * C(Y)3-35 and the C(AC) are unchaged. */
            W36 tmp = CoreMemory.C(Y);
            tmp.P = AC.PR;
            CoreMemory.C(Y, tmp);
        }
        public static void STD(WA Y) /* Store Decrement */
        {
            /* The C(AC)3-17 replace the C(Y)3-17. The
             * C(Y)S,1,2,18-35 and the C(AC) are unchaged. */
            W36 tmp = CoreMemory.C(Y);
            tmp.D = AC.D;
            CoreMemory.C(Y, tmp);
        }
        public static void STA(WA Y) /* Store Address */
        {
            /* The C(AC)21-35 replace the C(Y)21-35. The
             * C(Y)S,120 and the C(AC) are unchaged. */
            W36 tmp = CoreMemory.C(Y);
            tmp.A = AC.A;
            CoreMemory.C(Y, tmp);
        }
        public static void CLM() /* Clear Magnitude */
        {
            /* The C(AC)Q,P,1-35 are cleared. The ac sign is un-
             * changed. */
            AC.M37 = new W37();
        }
        public static void CHS() /* Change Sign */
        {
            /* If the AC sign bit is negative, it is made 
             * positive, and vice versa.*/
            AC.S = (W1)~AC.S;
        }
        public static void SSP() /* Set Sign Plus */
        {
            /* A positie sign replaces the C(AC)S.*/
            AC.S = new W1();
        }
        public static void SSM() /* Set Sign Minus */
        {
            /* A negative sign replaces the C(AC)S.*/
            AC.S = new W1(1);
        }
        #endregion
        #region Logical Operations
        public static void CAL(WA Y) /* Clear and Add Logical Word */
        {
            /* This instruction replaces the C(AC)P,1-35 with the 
             * C(Y). Thus the sign of the C(Y) apprears in position
             * P of the AC, and the S and Q and are cleared. The 
             * C(Y) are unchanged */

            AC = (W38)CoreMemory.C(Y);
        }
        public static void ACL(WA Y) /* Add and Carry Logical Word */
        {
            /* This instructions adds the C(Y)S,1-35 to the
             * C(AC)P,1-35, repsectively, and repeaces the C(AC)P,1-35
             * with this sum (position S of tegister Y is treated as
             * a numerical bit, and the sign of the AC is ignored).
             * A carry out of the P bit adds into position 35 of the
             * AC, but does not add into Q. Q is notchanged. The
             * C(Y) are unchaged. No overflow is possible. See 
             * Figure 14.*/
            W38 tmp = (W38)(CoreMemory.C(Y) + AC.M36);
            if (tmp.Q != 0)
                tmp = (W38)(tmp + 1);
            AC.M36 = tmp.M36;
        }
        public static void SLW(WA Y) /* Store Logical Word */
        {
            /* THe C(AC)P,1-35 replace the C(Y)S,1-35. The C(AC)
             * are unchanged. */
            CoreMemory.C(Y, AC.M36);
        }
        public static void ANA(WA Y) /* AND to Accumulator */
        {
            /* Each bit of the C(AC)P,1-35 is matched with the
             * correspondig bit of the C(Y)S,1-35, the C(AC)P being
             * matched with the C(Y)S. When the correspondig
             * bit of botch the AC and location Y is one, a one
             * rpelaces the contets of that position in the AC.
             * When the correspondig bit of either the AC or
             * location Y is a zero, a zero replaces the contents of
             * that position in the AC. THe C(AC)S,Q are cleared.
             * The C(Y) are unchanged.*/
            AC = (W38)(AC & CoreMemory.C(Y));
        }
        public static void ANS(WA Y) /* AND to Storage */
        {
            /* Each bit of the C(AC)P,1-35 is matched with the
             * correspondig bit of the C(Y)S,1-35, the C(AC)P be-
             * ing matched with the C(Y)S. When the correspond-
             * ing bit of botch the AC and location Y is one, 
             * a one replaces the contets of that position in loca-
             * tion Y. When the correspondig bit of either the AC 
             * or location Y is a zero, a zero replaces the contents of
             * that position in the location Y. THe C(AC) are unchanged.*/

            CoreMemory.C(Y, (W36)(AC & CoreMemory.C(Y)));
        }
        public static void ORA(WA Y) /* OR to Accumulator */
        {
            /* Each bit of the C(AC)P,1-35 is matched with the
             * correspondig bit of the C(Y)S,1-35, the C(AC)P being
             * matched with the C(Y)S. When the correspondig
             * bit of either the AC or location Y is one, a one
             * replaces the contets of that position in the AC.
             * When the correspondig bit of both the AC and
             * location Y is a zero, a zero replaces the contents of
             * that position in the AC. The C(Y) and the C(AC)S,Q are 
             * unchanged.*/

            AC = (W38)(AC | CoreMemory.C(Y));
        }
        public static void ORS(WA Y) /* OR to Storage */
        {
            /* Each bit of the C(AC)P,1-35 is matched with the
             * correspondig bit of the C(Y)S,1-35, the C(AC)P be-
             * ing matched with the C(Y)S. When the correspond-
             * ing bit of either the AC or location Y is one, 
             * a one replaces the contets of that position in loca-
             * tion Y. When the correspondig bit of both the AC 
             * and location Y is a zero, a zero replaces the contents of
             * that position in the location Y. THe C(AC) are unchanged.*/

            CoreMemory.C(Y, (W36)(AC | CoreMemory.C(Y)));
        }
        public static void COM() /* COM Complement Magnitude */
        {
            /* All ones are replaced by zeros and all zeros are
             * replaced by ones in the C(AC)Q,P,1-35. THe AC sign 
             * is unchanged */
            AC.M37 = (W37)~AC.M37;
        }
        #endregion
        #region Shifting Operations
        public static void ALS(WA Y) /* Accumulator Left Shift */
        {
            /* The C(AC)Q,P,1-35 are shifted left Y module 256
             * places. If a non-zero bit is shifted into or through 
             * positions P, the AC overflow indicator and light are
             * turned on. Bits shifted past position Q are lost.
             * Positions made vacant are filled with zeros. */

            uint shift = Y & 0xFF;
            for (uint i = 0; i < shift; i++)
            {
                AC.M37 = (W37)(AC.M37 << 1);
                if (0 != AC.PB)
                    acoflag = true;
            }
        }
        public static void ARS(WA Y) /* Accumulator Right Shift */
        {
            /* The C(AC)Q,P,1-45 are shifted right Y module 256
             * places. Bits shifted past position 35 are lost.
             * Positions made vacant are filled with zeros. */

            AC.M37 = (W37)(AC.M37 >> (int)(Y & 0xFF));
        }
        public static void LLS(WA Y) /* Long Left Shift */
        {
            /* The C(AC)Q,P,1-35 and the C(MQ)1-35 are shifted left
             * Y module 256 places. Bits enter position 35 of the
             * AC from position 1 of the MQ. If a non zero bit is
             * shifted into or through positioon P, the AC overfolow
             * indicator and light are turned on. Bits shifted past
             * position Q are lost. Postitions made vacant are filled
             * in with zeros THe sign of the AC is replaced by the 
             * same sign as that of the MQ */

            uint shift = Y & 0xFF;
            W1 Sign = MQ.S; /* Save sign */
            for (uint i = 0; i < shift; i++)
            {
                MQ = (W36)(MQ << 1);
                AC = (W38)((AC << 1) | MQ.S);
                if (AC.PB != 0)
                    acoflag = true;
            }
            /* Restore sign when done */
            AC.S = MQ.S = Sign;
        }
        public static void LRS(WA Y) /* Long Right Shift */
        {
            /* The C(AC)Q,P,1-35 and the C(MQ)1-35 are shifted 
             * right Y modulo 256 places. Bits enter position 1 of 
             * the MQ from position 35 of the AC. Bits shifted past
             * position 35 of the MQ are lost. Postitions made vacant 
             * are filled The sign of the MQ is replaced by the 
             * same sign as that of the AC */

            uint shift = Y & 0xFF;

            /* Save sign */
            W1 Sign = AC.S;

            /* Clear it for now */
            AC.S = new W1();

            for (uint i = 0; i < shift; i++)
            {
                MQ.S = new W1((uint)AC & 1);
                MQ = (W36)(MQ >> 1);
                AC = (W38)(AC >> 1);
            }
            /* Restore sign when done */
            AC.S = MQ.S = Sign;
        }
        public static void LGL(WA Y) /* Logical Left */
        {
            /* The C(AC)Q,P,1-35 and the C(MQ)S,1-35 are shifted 
             * left Y module 256 places. Bits enter position S of 
             * the MQ from position 1 of the MQ and enter position 
             * 35 of the AC from position S of the MQ. If a non-
             * zero bit is shifted into or through position P of the
             * AC, the AC overflow indicator and light are turned 
             * on. Bits shifted past position Q are lost. Postitions 
             * made vacant are filled in with zeros The sign of the 
             * AC is unchanged */

            uint shift = Y & 0xFF;
            /* Save sign */
            W1 Sign = AC.S;

            for (uint i = 0; i < shift; i++)
            {
                AC = (W38)((AC << 1) | MQ.S);
                MQ = (W36)(MQ << 1);
                if (AC.PB != 0)
                    acoflag = true;
            }
            /* Restore sign when done */
            AC.S = Sign;
        }
        public static void RQL(WA Y) /* Rotate MQ Left */
        {
            /* The C(MQ)S,1-35 are rotated left Y module 256 
             * places. The bits rotate from position 1 to position S 
             * of the MQ, and from position S to position 35 of the
             * MQ. */

            uint shift = Y & 0xFF;
            for (uint i = 0; i < shift; i++)
                MQ = (W36)((MQ << 1) | MQ.S);

        }
        #endregion
        #region Floating-Point Arithmetic Operations
        public static void FAD(WA Y) /* Floating ADD */
        {
            /* The C(Y) are algebraically added to the C(AC), 
             * and this sum replaces the C(AC) and the C(MQ). 
             * The C(Y) are unchanged. Floating-point addition 
             * takes place in the following way:
             *  1. The MQ is cleared.
             *  2. The C(Y) are placed in the SR.
             *  3. If the characteristic in the SR is less than the 
             *  characteristic in the AC, the C(SR) and the C(AC) 
             *  interchange automatically because the number with 
             *  the smaller characteristic must appear in the AC be-
             *  fore addition can take place. (Positions Q and P of 
             *  the AC are considered as part of the characteristic. 
             *  Consequently, a 1 in either of these positions makes 
             *  the characteristic in the AC larger than that in the 
             *  SR, but the 1's would be lost during the interchange 
             *  and an incorrect answer will result.)
             *  4. The MQ is given the same sign as the AC.
             *  5. The fraction in the AC is shifted right the num-
             *  ber of positions equal to the magnitude of the differ-
             *  ence in the characteristics. Bits shifted out of the 
             *  AC enter position 9 of the MQ. Bits shifted out of 
             *  position 35 of the MQ are lost.
             *  6. The characteristic in the SR replaces the C(AC)1-8.
             *  7. The fraction in the SR is algebraically added to 
             *  the fraction in the AC and this sum replaces the 
             *  C(AC)S,9-35. 
             *  8. If the magnitude of the sum is greater than or 
             *  equal to 1, there is a carry from position 9 to position 
             *  8 of the AC (thus increasing the characteristic by 1).* 
             *  In this event, the C(AC)9-35 and the C(MQ)9-35 are 
             *  shifted right one position and 1 is inserted in posi-
             *  tion 9 of the AC.
             *  9a. If the resulting fraction in the AC is zero, the 
             *  AC is cleared, yielding a normal zero. The sign of the 
             *  AC is the sign of the number that has the smaller 
             *  characteristic. If both characteristics are equal, then 
             *  the sign of the AC is the sign of the number in the AC.
             *  9b. If the magnitude of the resulting fraction in 
             *  the AC is not in normal form (i.e. less than 1/2 but 
             *  not zero), and the signs of the MQ and AC are the 
             *  same, the C(AC)9-35 and the C(MQ)9-35 are shifted 
             *  left until a 1 is in position 9 of the AC. Bits enter 
             *  position 35 of the AC from position 9 of the MQ. The 
             *  characteristic in the AC is reduced by 1 for each posi-
             *  tion shifted.* If the signs of the MQ and AC are differ
             *  ent, the magnitude of the fraction in the AC is reduced 
             *  by 1 before the shifting is begun. Each bit entering 
             *  position 35 of the AC from position 9 of the MQ is 
             *  inverted.
             *  10. The MQ is given a characteristic which is 27 
             *  less than the characteristic in the AC, unless the AC 
             *  contains a normal zero in which case zeros are placed 
             *  in positions 1-8 of the MQ.*
             *  11. If the signs of the MQ and AC are different, 
             *  the magnitude of the fraction in the AC is increased 
             *  by 1. If a carry occurs between positions 8 and 9, 
             *  the C(AC)9-35 are shifted right one place and a one is 
             *  inserted in C(AC)9. If the carry from 9 to 8 occurs, 
             *  the characteristic of the AC is increased by 1.
             *  * During execution of a floating-point addition, 
             *    the AC or MQ overflow indicator and the corre-
             *    sponding light on the operator's console are 
             *    turned on by too large a characteristic (over-
             *    flow-characteristic greater than 255) or too small 
             *    a characteristic (underflow-characteristic nega-
             *    tive) in the AC or the MQ, respectively. */
            SR = CoreMemory.C(Y);
            Fadd(true);
        }
        public static void UFA(WA Y) /* Unnormalized Floating ADD */
        {
            /* Same as floating add except steps 9a, 9b and 11 are 
             * omitted. No test is made for a normal zero in step 10. */
            SR = CoreMemory.C(Y);
            Fadd(false);
        }
        public static void FSB(WA Y) /* Floating SUB */
        {
            /* Same as floating add except that step 2 is replaced 
             * by the following: the negative of the C(Y) is placed 
             * in the storage register */
            SR = CoreMemory.C(Y);
            SR.S = (W1)~SR.S;
            Fadd(true);
        }
        public static void UFS(WA Y) /* Unnormalized Floating SUB */
        {
            /* Same as floating subtract except steps 9a, 9b 
             * and 11 are omitted. No test is made for a normal zero 
             * in step 10. */
            SR = CoreMemory.C(Y);
            SR.S = (W1)~SR.S;
            Fadd(false);
        }
        public static void FMP(WA Y) /* Floating Multiply */
        {
            /* The C(Y) are multiplied by the C(MQ). The most 
             * significant part of the product appears in the AC and 
             * the least significant part appears in the MQ.
             * The product of two floating-point numbers is in 
             * normalized form if the multiplier and multiplicand 
             * are in this form. If either the multiplier or multi-
             * plicand is not in normalized form, the product is in 
             * normalized form only if the shift of one place in step 
             * 4b is sufficient to normalize it.
             * Floating-point multiplication takes place as follows:
             * 1. The C(Y) are placed in the storage register and 
             * the AC is cleared.
             * 2. The sum of the characteristics in the SR and in 
             * the MQ minus 128 is placed in positions 1-8 of the AC.*
             * 3. The C(SR)S,9-35 are algebraically multiplied by 
             * the C(MQ)S,9-35. The most significant 27 bits plus sign 
             * of the product replace the C(AC)S,9-35 and the least 
             * significant 27 bits replace the C(MQ)9-35.
             * 4a. If the fraction in the AC is zero, the 
             * C(AC)Q,P,1-35 are cleared, yielding a normal zero. The 
             * sign of the AC is the algebraic sign of the product.
             * 4b. If position 9 of the AC contains a zero but the 
             * fraction in the AC is not zero, the C(AC)9-35 and the 
             * C(MQ)9-35 are shifted left one position and the char-
             * acteristic in the AC is reduced by 1.* The bit in posi-
             * tion 9 of the MQ enters position 35 of the AC.
             * 5a. If the AC contains a normal zero, positions 1-8 
             * of the MQ are cleared.
             * 5b. If the AC does not contain a normal zero, the 
             * C(MQ)1-8 are replaced by a characteristic which is 27 
             * less than the characteristic in the AC.*
             * 6. The sign of the MQ is replaced by the same sign 
             * as that of the AC.
             * * During execution of floating-point multiplica-
             *   tion, too large or too small a characteristic in the 
             *   AC or the MQ, respectively, turns on the AC or 
             *   the MQ overflow indicator and the corresponding 
             *   light on the operator’s console. */
            SR = CoreMemory.C(Y);
            Fmpy(true);
        }
        public static void UFM(WA Y) /* Unnormalized Floating Multiply */
        {
            /* This operation is the same as floating multiply 
             * except that steps 4a, 4b and 5a are omitted. */
            SR = CoreMemory.C(Y);
            Fmpy(false);
        }
        public static bool FDH(WA Y) /* FDH Floating divide or Halt */
        {
            /* The C(AC) are divided by the C(Y), the quotient 
             * appears in the MQ and the remainder appears in the 
             * AC. The MQ is cleared before actual division takes 
             * place.
             * If positions Q or P of the AC are not zero, divi-
             * sion may take place and either or both of the AC 
             * and/or MQ overflow indicators may be turned on. 
             * When division by zero is attempted, the divide-check 
             * indicator and lights are turned on and the calculator 
             * stops, leaving the dividend in the AC unchanged. The 
             * quotient is in normalized form if both divisor and 
             * dividend are in that form. If divisor or dividend or 
             * both are not in normalized form, the quotient is in 
             * normalized form if
             *    2|C(Y)9-35| > |C(AC)9-35| >= ½|C(Y)9-35|
             * Floating-point division takes place as follows:
             * 1. The C(Y) are placed in the storage register.
             * 2. If the magnitude of the fraction is in the AC is 
             * greater than (or equal to) twice the magnitude of 
             * the fraction in the SR, the divide-check indictor and 
             * light are turned on, the calculator stops, and the divi-
             * dend is left unchanged in the AC.
             * 3. If the fraction in the AC is zero, the C(MQ)1-35 
             * and C(AC)Q, P, 1-35 are cleared and the remaining steps 
             * are skipped. The sign of the MQ is the algebraic sign 
             * of the quotient. The sign of the AC is the sign of the 
             * dividend.
             * 4. If the magnitude of the fraction in the AC is 
             * greater than or equal to the magnitude of the frac-
             * tion in the SR (but less than twice the magnitude of 
             * this fraction), the fraction in the AC is shifted right 
             * one position and the characteristic in the AC is in-
             * creased by 1.* The bit in position 35 of the AC enters 
             * position 9 of the MQ.
             * 5. The characteristic of the AC minus the charac-
             * teristic of the SR plus 128 is placed in positions 1-8 
             * of the MQ.*
             * 6. The fractional part of the dividend, which con-
             * sists of the C(AC)S,9-35 (and the C(MQ)9 if the con-
             * dition of step 4 is met), algebraically divided by the 
             * fraction in the SR replaces the C(MQ)S,9-35.
             * 7. The 27-bit remainder resulting from the divi-
             * sion in step 6 replaces the C(AC) 9-35. The sign of the 
             * AC is unchanged (i.e., the sign of the remainder agrees 
             * with the sign of the dividend.)
             * 8. The characteristic in the AC is reduced by 27.*
             * * During execution of a floating-point division,
             * the AC or MQ overflow indicator and the corre-
             * sponding light on the operator’s console are 
             * turned on for too large or too small a charac-
             * teristic in the AC or MQ respectively. */
            SR = CoreMemory.C(Y);
            return Fdiv(true);
        }
        public static void FDP(WA Y) /* Floating divide or Proceed */
        {
            /* This operation is the same as floating divide or 
             * halt except for division by zero and step 2.
             * When division by zero is attempted, the divide-
             * check indicator and light are turned on, division does 
             * not take places and the calculator proceeds to the next 
             * instruction. If the magnitude of the fraction in the 
             * AC is greater than (or equal to) twice the magnitude 
             * of the fraction in the SR, the divide-check indicator 
             * and light are turned on, division does not take place 
             * and the calculator proceeds to the next instruction. 
             * The dividend in the AC is unchanged. */

            SR = CoreMemory.C(Y);
            Fdiv(false);
        }
        #endregion
        #region Control Operations
        public static bool TZE() /* Transfer on Zero*/
        {
            /* IF the C(AC),Q,P,1-35 are zero, the calculator takes 
             * its next instruction from location Y and proceeds 
             * from there. If they are not zero, the calculator pro-
             * ceeds to the next instruction in sequence. */
            return AC.M37 == 0;
        }
        public static bool TNZ() /* Transfer on No Zero*/
        {
            /* IF the C(AC),Q,P,1-35 are not zero, the calculator 
             * takes its next instruction from location Y and pro-
             * ceeds from there. If they are zero, the calculator pro-
             * ceeds to the next instruction in sequence. */
            return AC.M37 != 0;
        }
        public static bool TPL() /* Transfer on Plus*/
        {
            /* If the sign bit of the AC is positive, the calculator 
             * takes the next instruction from location Y and pro-
             * ceeds from there. If the sign bit of the AC is negative, 
             * the calculator proceeds to the next instruction in 
             * sequence. */
            return AC.S == 0;
        }
        public static bool TMI() /* Transfer on Minus*/
        {
            /* If the sign bit of the AC is negative, the calculator 
             * takes the next instruction from location Y and pro-
             * ceeds from there. If the sign bit of the AC is positive, 
             * the calculator proceeds to the next instruction in 
             * sequence. */
            return AC.S != 0;
        }
        public static bool TOV() /* Transfer on Overflow */
        {
            /* If the AC overflow indicator and light are on as the
             * result of a previous operation, the indicator and light
             * are turned off and the calculator takes the next in-
             * struction from location Y and proceeds from there. 
             * If the indicator and light are off the calculator pro-
             * ceeds to the next instruction in sequence. */
            bool ret = acoflag;
            acoflag = false;
            return ret;
        }
        public static bool TNO() /* Transfer on No Overflow */
        {
            /* If the AC overflow indicator and light are off, the 
             * calculator takes the next instruction from location Y 
             * and proceeds from there. If the indicator and light 
             * are on, the calculator proceeds to the next instruction 
             * in sequence after turning off the indicator and light. */
            bool ret = !acoflag;
            acoflag = false;
            return ret;
        }
        public static bool TQP() /* Transfer on MQ Plus*/
        {
            /* If the sign bit of the MQ is positive, the calculator 
             * takes the next instruction from location Y and pro-
             * ceeds from there. If the sign bit of the MQ is negative, 
             * the calculator proceeds to the next instruction in 
             * sequence. */
            return MQ.S == 0;
        }
        public static bool TQO() /* Transfer on MQ Overflow */
        {
            /* IF the MQ overflow indicator and light have been
             * turned on by an overflow or underflow in thw MQ
             * characteristic during a previous floating-point oper-
             * ation, the indicator and light are turned off, the cal-
             * culator takes the next instruction from location Y 
             * and proceeds from there. If the indicator and light 
             * are not on the calculator proceeds to the next instruc-
             * tion in sequence. */
            bool ret = mqoflag;
            mqoflag = false;
            return ret;
        }
        public static bool TLQ() /* Transfer on Low MQ */
        {

            /* IF the C(MQ) are algebraically less than the C(AC),
             * the calculator takes the next instruction from location
             * Y and proceeds from there. If the C(MQ) are alre-
             * breaically greater than or equal to the C(AC), the cal-
             * culcator proceeds to the next instruction in sequence*/
            /* Is AC - and MQ + */
            if (MQ.S == 0 && AC.S != 0)
                return false;
            /* Same sign, compare magintudes */
            if (MQ.S == 0 && AC.S == 0)
            {
                if (MQ.M >= AC.M37)
                    return false;
            }
            else if (MQ.S != 0 && AC.S != 0)
            {
                if (AC.M37 >= MQ.M)
                    return false;
            }
            /* MQ bigger, so take branch */
            return true;
        }
        public static uint PBT() /* P Bit test */
        {
            /* if the C(AC)P is a one, the calculator skips the next
             * instruction and proceeds from there. If position P
             * contains a zero, the calculator takes the next instruc-
             * tion in sequence. */
            return AC.PB;
        }
        public static uint LBT() /* Low Order Bit Test */
        {
            /* If the C(AC)35 is a one, the calculator skips the
             * next instruction and proceeds from there. If position
             * 35 contains a zero, the calculator takes the  next in-
             * struction in sequence */
            return (uint)AC & 1;
        }
        public static uint DCT() /*Divide Check Test */
        {
            /* IF the divide-check indicator and light are on, the 
             * indicator and light are turned off and the calculator
             * takes the next instruction in sequence. If the indi-
             * cator and light are off the calculator skips the next 
             * instruction and proceeds from there */
            bool ret = !dcheck;
            dcheck = false;
            return ret ? 1U : 0;
        }
        public static uint CAS(WA Y) /* Compare Accumulator with Storage */
        {
            /* If the C(Y) are algebraically less than the C(YC),
             * the calculator takes the nect instruction in sequence.
             * If the C(Y) are algebraiclally equel to the C(AC), the
             * calculator skips the next instruction and proceeds 
             * from there. If the C(Y) are algebraically greater than
             * the C(AC), the calculator skips the next two instruc-
             * tions and proceeds form there. Two numbers are
             * algebraically equal when the magnitude of the num-
             * bers and the sign are both equal. A plus zero is
             * algebraically larger than a minus zero */
            SR = CoreMemory.C(Y);
            if (AC.S != 0)
            {
                if (SR.S != 0)
                {
                    if (AC.M37 == SR.M)
                        return 1;
                    else if (AC.M37 > SR.M)
                        return 2;
                }
                else
                    return 2;
            }
            else
            {
                if (SR.S == 0)
                {
                    if (AC.M37 == SR.M)
                        return 1;
                    else if (AC.M37 < SR.M)
                        return 2;
                }
            }
            return 0;
        }
        #endregion
        #region Indexing Operations
        public static WA PAX() /* Place Address in Index */
        {
            /* Not indexable The adress part of the C(AC) re-
             * places the number in the specified index register. The 
             * C(AC) are unchanged. */
            return (WA)(uint)AC.A;
        }
        public static WA PDX() /* Place Decrement in Index */
        {
            /* Not indexable The decrement part of the C(AC) 
             * replaces the number in the specified index register. 
             * The C(AC) are unchanged. */
            return (WA)(uint)AC.D;
        }
        public static void PXD(WA X)
        {
            /* Not indexable. The AC is cleared and the numer
             * in the specified index register is placed in the decre-
             * ment part of the AC. */
            AC = new W38 { D = (W15)(uint)X };
        }
        #endregion
    }
}
