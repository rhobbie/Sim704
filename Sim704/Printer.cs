using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace Sim704
{
    class Printer : IDisposable, I704dev
    {
        StreamWriter f;

        bool WriteActive; /* write is active */
        List<ulong> WRecord; /* write record */

        char[] printerline; /* current line that was printed but not written to file */
        bool linewasprinted;  /* a line was printed but not written to file */
        bool[] sense;
        

        public void MountPaper(string file)
        {
            if (f != null)
                throw new InvalidOperationException("Paper already mounted");
            if (file != null)
                f = new StreamWriter(file, false, Encoding.ASCII);
            else
                f = null;
            WriteActive = false;
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
                Writeprintedlinetofile();
            f.Dispose();
            f = null;
        }
        public void RPR() /* Read Printer*/
        {
            throw new NotImplementedException("RPR");
        }
        public void WPR() /* Write Printer*/
        {
            EndRW();
            if (Io704.Config.logIO)
                Console.WriteLine("Printer selected");
            WriteActive = true;

            /* reset all senses */
            for (int i = 1; i < 11; i++)
                sense[i] = false;
        }
        void Writeprintedlinetofile()
        {
            int end;
            for (end = 119; end >= 0; end--) /* search for last char != space */
            {
                if (printerline[end] != ' ')
                    break;
            }
            if (end >= 0)
            {
                for (int j = 0; j <= end; j++)
                {
                    f.Write(printerline[j]);
                    printerline[j] = ' '; /* reset char */
                }
            }
            linewasprinted = false;

           
        }
        void EndRW() /* finish current reading or writing operation */
        {
            if (WriteActive)
            {
                if (Io704.Config.logIO)
                    Console.WriteLine("Printer: record with length {0} written", WRecord.Count);
                while (WRecord.Count < 24)
                    WRecord.Add(0);
                byte[] CBN = CBNConverter.ToCBN(WRecord.ToArray());
                if (HollerithConverter.CBNToBCD(CBN, 0, 72, out byte[] BCD) > 0)
                    throw new Exception("non Hollerith printer data");
                int pos = (sense[9] | sense[10]) ? 72 : 0; /* SPT9 or SPT 10 -> Columns 1-48 are printed  on type wheels 73-120*/
                for (int i = 0; i < 72; i++, pos++)
                {
                    if (pos == 120)
                        pos -= 72;
                    char c = BcdConverter.BcdToChar(BCD[i]);
                    if (c != ' ')
                    {
                        if (printerline[pos] != ' ' && printerline[pos] != c)  /* alread a different char there */
                            Console.Error.WriteLine("Printer Warning: overprinting {0} with {1}", printerline[pos], c);
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
                        f.Write("\f\r"); /* form feed*/
                    else            /*SPR2: half page skip */
                        f.Write("\v\r"); /* vertical tab*/                                      
                }

                if (sense[3] || sense[6]) /* SPR3 or SPR6 ?*/
                {
                    if (linewasprinted)
                        Writeprintedlinetofile();
                    if (sense[6])  /* SPR6 two spaces after the line */
                        f.WriteLine();
                    f.WriteLine();  /* SPR3 one space after the line */
                }

                WriteActive = false;

                /* reset all senses */
                for (int i = 1; i < 11; i++)
                    sense[i] = false;

                WRecord.Clear();
            }
        }
        public int CPY(ref ulong w) /* Copy */
        {
            int ret = 0;
            if (WriteActive)
            {
                if (WRecord.Count == 0) /* first copy */
                {
                    /* check senses that were given before the copy loop*/
                    if (sense[1] || sense[2]) /* SPR1 or SPR 2 ?*/
                    {
                        if (linewasprinted)
                            Writeprintedlinetofile();
                            if (sense[1])    /*SPR1: eject page */
                                f.Write("\f\r"); /* form feed*/
                            else            /*SPR2: half page skip */
                                f.Write("\v\r"); /* vertical tab*/
                        /*reset senses */
                        sense[1] = false;
                        sense[2] = false;
                    }

                    if (!sense[5] && !sense[9]) /*not SPR 5 and not 9 -> space is needed*/
                    {
                        if (linewasprinted)
                            Writeprintedlinetofile();
                        if (sense[4]) /* SPR 4 ->two spaces before the line*/
                            f.WriteLine();
                        f.WriteLine();   /* default-> one space before the line */
                    }

                }
                else if (WRecord.Count == 24) /* to many copies */
                    throw new InvalidOperationException("too many CPYs for printer");
                ALU.MQ = (W36)w;
                if (Io704.Config.logIO)
                    Console.WriteLine("Copy to Printer {0}", ALU.MQ);
                WRecord.Add(w);

            }
            else
                throw new InvalidOperationException("CPY while device not selected");
            return ret;
        }
        public uint SPT() /* Sense Printer test */
        {
            if (Io704.Config.logIO)
                Console.WriteLine("Sense Printer Test");
            return sense[7] ? 1u : 0;  /* return status of SPR 7 sense*/
        }
        public void SPR(uint unit) /* Sense Printer */
        {
            /*set sense */
            if (WriteActive)
                sense[unit] = true;
            else
                throw new InvalidOperationException("SPR while printer is not selected");
            if (Io704.Config.logIO)
                Console.WriteLine("Sense Printer {0}", unit);
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
