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

        public void MountPaper(string file)
        {
            if (f != null)
                throw new InvalidOperationException("Paper already mounted");
            if (file != null)
                f = new StreamWriter(file);
            else
                f = null;
            WriteActive = false;

            WRecord = new List<ulong>();
        }
        public void UnMountPaper()
        {
            Disconnect();
            f.Close();
            f.Dispose();
            f = null;
        }
        public void RDS() /* Read Select*/
        {
            throw new NotImplementedException("PRT-RDS");
        }
        public void WRS() /* Write Select*/
        {
            EndRW();
            WriteActive = true;
        }
        void EndRW() /* finish current reading or writing operation */
        {
            if (WriteActive)
            {
                if (Io704.Config.logIO)
                    Console.WriteLine("Printer: record length {0} written", WRecord.Count);
                while (WRecord.Count != 24)
                    WRecord.Add(0);
                byte[] CBN = CBNConverter.ToCBN(WRecord.ToArray());
                if (HollerithConverter.CBNToString(CBN, 0, 72, out string s) != 0)
                {
                    throw new Exception("invalid printer data");
                }
                f.WriteLine(s);
                Console.Error.WriteLine("{0}", s);
                WriteActive = false;
                WRecord.Clear();
            }
        }
        public int CPY(ref ulong w) /* Copy */
        {
            int ret=0;
            if (WriteActive)
            {
                ALU.MQ = (W36)w;
                if (Io704.Config.logIO)
                    Console.WriteLine("Printer Written {1}", ALU.MQ);
                WRecord.Add(w);
                if (WRecord.Count == 24)
                    EndRW();
            }
            else
                throw new InvalidOperationException("CPY while device not selected");
            return ret;
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
