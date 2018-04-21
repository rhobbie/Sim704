using System;
using System.Collections.Generic;


namespace Sim704
{
    class CardPunch : IDisposable, I704dev
    {
        TapeFile f; /* for tape file access */

        List<ulong> WRecord; /* write record */        
        bool WriteActive; /* write is active */        
        public void MountDeck(string file)
        {
            if (f != null)
                throw new InvalidOperationException("Deck already mounted");
            if (file != null)
                f = new TapeFile(file,false);
            else
                f = null;
            WriteActive = false;

            WRecord = new List<ulong>(24);
        }
        public void UnMountDeck()
        {
            Disconnect();            
            f.Dispose();
            f = null;
        }
        void EndRW() /* finish current reading or writing operation */
        {            
            if (WriteActive)
            {
                if (Io704.Config.LogIO!=null)
                    Io704.LogIO.WriteLine("Punch: record with length {0} written", WRecord.Count);
                while (WRecord.Count < 24)
                    WRecord.Add(0);
                byte[] CBN = CBNConverter.ToCBN(WRecord.ToArray());
                f.WriteRecord(true, CBN);
                WriteActive = false;
                WRecord.Clear();
            }
        }
        public void WPU() /* Write Punch*/
        {
            EndRW();
            if (Io704.Config.LogIO!=null)
                Io704.LogIO.WriteLine("Punch selected");
            WriteActive = true;
        }
        public int CPY(ref ulong w) /* Copy */
        {
            int ret = 0;
            if (WriteActive)
            {
                ALU.MQ = (W36)w;
                if (Io704.Config.LogIO!=null)
                    Io704.LogIO.WriteLine("Copy to punch {0}", ALU.MQ);
                WRecord.Add(w);
                if (WRecord.Count >= 24)
                    EndRW();
            }
            else
                throw new InvalidOperationException("CPY while device not selected");
            return ret;
        }
        public void SPU(uint unit) /* Sense Punch */
        {            
            Io704.LogIO.WriteLine("Sense Punch {0}", unit);
            throw new NotImplementedException("SPU");
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
                    UnMountDeck();
                }
            }
        }
    }
}
