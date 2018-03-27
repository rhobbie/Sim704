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
        uint sense;

        public void MountPaper(string file)
        {
            if (f != null)
                throw new InvalidOperationException("Paper already mounted");
            if (file != null)
                f = new StreamWriter(file,false,Encoding.ASCII);
            else
                f = null;
            WriteActive = false;

            WRecord = new List<ulong>(24);
        }
        public void UnMountPaper()
        {
            Disconnect();
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
                if (HollerithConverter.CBNToString(CBN, 0, 72, out string s) != 0)
                {
                    throw new Exception("invalid printer data");
                }
                f.WriteLine(s);                
                WriteActive = false;
                WRecord.Clear();
                sense = 0;
            }
        }
        public int CPY(ref ulong w) /* Copy */
        {
            int ret=0;
            if (WriteActive)
            {
                ALU.MQ = (W36)w;
                if (Io704.Config.logIO)
                    Console.WriteLine("Copy to Printer {0}", ALU.MQ);
                WRecord.Add(w);
                if (WRecord.Count == 24)
                    EndRW();
            }
            else
                throw new InvalidOperationException("CPY while device not selected");
            return ret;
        }
        public uint SPT() /* Sense Printer test */
        {
            if (Io704.Config.logIO)
                Console.WriteLine("Sense Printer Test");
            return sense == 7?1u:0;
        }
        public void SPR(uint unit) /* Sense Printer */
        {
            sense = unit;
            f.WriteLine("§{0}", unit);
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
