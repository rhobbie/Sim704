using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace Sim704
{
    class Printer : IDisposable, I704dev
    {
        StreamWriter f;

        bool WriteActive; /* write is active */
        bool ReadActive; /* write with checking is active */
        int chkcounter;  /* counter for write with checking */
        List<ulong> WRecord; /* write record */

        char[] printerline; /* current line that was printed but not written to file */
        bool linewasprinted;  /* a line was printed but not written to file */
        bool[] sense;
        bool ConsoleOut;

        public void MountPaper(string file)
        {
            ConsoleOut = false;
            if (f != null)
                throw new InvalidOperationException("Paper already mounted");
            if (file != null)
                f = new StreamWriter(file, false, Encoding.ASCII);
            else
            {
                /* no file given: print on console */
                f = new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true };
                Console.SetOut(f);
                ConsoleOut = true;
            }
            WriteActive = false;
            ReadActive = false;
            chkcounter = 0;
            printerline = new char[120];
            for (int i = 0; i < 120; i++)
                printerline[i] = ' ';
            sense = new bool[11];
            linewasprinted = false;
            WRecord = new List<ulong>(24);
        }
        public void UnMountPaper()
        {
            Disconnect();
            if (linewasprinted)
            {
                Writeprintedlinetofile();
                f.Write('\n');
            }
            f.Dispose();
            f = null;
        }
        public void Flush()
        {
            if (ConsoleOut&&linewasprinted)
            { 
                Writeprintedlinetofile();
                f.Write('\n'); /* new line*/
            }
        }
        public void RPR() /* Read Printer*/
        {
            EndRW();
            if (Io704.Config.LogIO != null)
                Io704.LogIO.WriteLine("Printer selected");
            ReadActive = true;
            chkcounter = 0;
            /* reset all senses */
            for (int i = 1; i < 11; i++)
                sense[i] = false;
        }
        public void WPR() /* Write Printer*/
        {
            EndRW();
            if (Io704.Config.LogIO != null)
                Io704.LogIO.WriteLine("Printer selected");
            WriteActive = true;

            /* reset all senses */
            for (int i = 1; i < 11; i++)
                sense[i] = false;
        }
        void Writeprintedlinetofile()
        {
            int end;
            for (end = 119; end >= 0; end--) /* search for last char != space */
                if (printerline[end] != ' ')
                    break;
            if (end >= 0)
            {
                for (int j = 0; j <= end; j++)
                {
                    f.Write(printerline[j]);
                    printerline[j] = ' '; /* reset char */
                }
                f.Write('\r'); /* carriage return */
            }
            linewasprinted = false;
        }
        void EndRW() /* finish current reading or writing operation */
        {
            if (WriteActive || ReadActive)
            {
                if (Io704.Config.LogIO != null)
                    Io704.LogIO.WriteLine("Printer: record with length {0} written", WRecord.Count);

                while (WRecord.Count < 24)
                    WRecord.Add(0);

                byte[] CBN = CBNConverter.ToCBN(WRecord.ToArray());
                if (HollerithConverter.CBNToBCD(CBN, 0, 72, out byte[] BCD) > 0)
                    throw new Exception("non Hollerith printer data");

                int pos = (sense[9] | sense[10]) ? 72 : 0; /* SPR9 or SPR 10 -> Columns 1-48 are printed  on type wheels 73-120*/

                for (int i = 0; i < 72; i++, pos++)
                {
                    if (pos == 120)
                        pos -= 72;
                    char c = BcdConverter.BcdToChar(BCD[i]);
                    if (c != ' ')
                    {
                        if (printerline[pos] != ' ' && printerline[pos] != c)  /* already a different char there */
                            Console.Error.WriteLine("Printer warning: printing {0} over {1}", c, printerline[pos]);
                        printerline[pos] = c; /* print character to line.*/
                        linewasprinted = true;
                    }
                }

                /* check for senses that were given after the copy loop */
                if (sense[1] || sense[2]) /* SPR1 or SPR 2 ?*/
                {
                    if (linewasprinted)
                        Writeprintedlinetofile();
                    if (sense[1])    /*SPR1: eject page */
                        f.Write('\f'); /* form feed*/
                    else            /*SPR2: half page skip */
                        f.Write('\v'); /* vertical tab*/
                }

                if (sense[3] || sense[6]) /* SPR3 or SPR6 ?*/
                {
                    if (linewasprinted)
                        Writeprintedlinetofile();
                    if (sense[6])  /* SPR6 two spaces after the line */
                        f.Write('\n'); /* new line*/
                    f.Write('\n');  /* SPR3 one space after the line */
                }

                WriteActive = false;
                ReadActive = false;
                chkcounter = 0;
                /* reset all senses */
                for (int i = 1; i < 11; i++)
                    sense[i] = false;

                WRecord.Clear();
            }
        }
        public int CPY(ref ulong w) /* Copy */
        {
            int ret = 0;
            if (WriteActive || ReadActive)
            {
                if (WRecord.Count == 0) /* first copy */
                {
                    /* check senses that were given before the copy loop*/
                    if (sense[1] || sense[2]) /* SPR1 or SPR 2 ?*/
                    {
                        if (linewasprinted)
                            Writeprintedlinetofile();
                        if (sense[1])    /*SPR1: eject page */
                            f.Write('\f'); /* form feed*/
                        else            /*SPR2: half page skip */
                            f.Write('\v'); /* vertical tab*/
                        /*reset senses */
                        sense[1] = false;
                        sense[2] = false;
                    }

                    if (!sense[5] && !sense[9]) /*not SPR 5 and not 9 -> space is needed*/
                    {
                        if (linewasprinted)
                            Writeprintedlinetofile();
                        if (sense[4]) /* SPR 4 ->two spaces before the line*/
                            f.Write('\n'); /* new line*/
                        f.Write('\n');    /* default-> one space before the line */
                    }

                }
            }
            if (WriteActive)
            {
                if (WRecord.Count == 24) /* to many copies */
                    throw new InvalidOperationException("too many CPYs for printer");
                ALU.MQ = (W36)w;
                if (Io704.Config.LogIO != null)
                    Io704.LogIO.WriteLine("Copy to Printer {0}", ALU.MQ);
                WRecord.Add(w);
            }
            else if (ReadActive) /*write with checking? */
            {
                if (chkcounter == 46) /* to many copies */
                    throw new InvalidOperationException("too many CPYs for printer");

                bool r = true; /* true:this copy is a read, false: a write */
                switch (chkcounter)
                {   /* generate printer echo */
                    case 18: /* 8&4 */
                    case 19:
                        w = (WRecord[chkcounter - 16] & WRecord[chkcounter - 8]);
                        break;
                    case 22: /* 8&3 */
                    case 23:
                        w = (WRecord[chkcounter - 20] & WRecord[chkcounter - 10]);
                        break;
                    case 26:  /* 9 */
                    case 27:
                        w = WRecord[chkcounter - 26];
                        break;
                    case 30:  /* 8&!4&!3 */
                    case 31:
                        w = (WRecord[chkcounter - 28] & (~WRecord[chkcounter - 20]) & (~WRecord[chkcounter - 18]));
                        break;
                    case 32:  /* 7*/
                    case 33:
                    case 34:  /* 6 */
                    case 35:
                    case 36:  /* 5 */
                    case 37:
                    case 42:  /* 2 */
                    case 43:
                    case 44:  /* 1 */
                    case 45:
                        w = WRecord[chkcounter - 28];
                        break;
                    case 38:  /* 4&!8 */
                    case 39:
                        w = (WRecord[chkcounter - 28] & (~WRecord[chkcounter - 36]));
                        break;
                    case 40: /* 3&!8 */
                    case 41:
                        w = (WRecord[chkcounter - 28] & (~WRecord[chkcounter - 38]));
                        break;
                    default:
                        ALU.MQ = (W36)w;
                        if (Io704.Config.LogIO != null)
                            Io704.LogIO.WriteLine("Copy to Printer {0}", ALU.MQ);
                        WRecord.Add(ALU.MQ);
                        r = false;
                        break;

                }
                if (r)
                {
                    ALU.MQ = (W36)w;
                    if (Io704.Config.LogIO != null)
                        Io704.LogIO.WriteLine("Echo from Printer {0}", ALU.MQ);
                }
                chkcounter++;
            }
            else
                throw new InvalidOperationException("CPY while device not selected");
            return ret;
        }
        public uint SPT() /* Sense Printer test */
        {
            if (!WriteActive && !ReadActive)
                throw new InvalidOperationException("SPT while printer is not selected");

            if (Io704.Config.LogIO != null)
                Io704.LogIO.WriteLine("Sense Printer Test");
            return sense[7] ? 1u : 0;  /* return status of SPR 7 */
        }
        public void SPR(uint unit) /* Sense Printer */
        {
            /*set sense */
            if (WriteActive || ReadActive)
                sense[unit] = true;
            else
                throw new InvalidOperationException("SPR while printer is not selected");
            if (Io704.Config.LogIO != null)
                Io704.LogIO.WriteLine("Sense Printer {0}", unit);
        }
        public void Disconnect() /* Disconnect from Device */
        {
            EndRW();
        }
        public void Dispose() /* IDisposable-Handling */
        {
            Dispose(true);
        }
        protected virtual void Dispose(bool disposing) /* IDisposable-Handling */
        {
            if (disposing)
            {
                if (f != null)
                {
                    UnMountPaper();
                }
            }
        }

    }
}
