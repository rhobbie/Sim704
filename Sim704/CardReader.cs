using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim704
{
    class CardReader : IDisposable ,I704dev
    {
        bool eof; /* end of cards */
        TapeFile f; 
        long[] RRecord;
        bool ReadActive;
        int PosInRecord;
        bool cardwasread;
        public CardReader()
        {
            f = null;
            ReadActive = false;
            cardwasread = false;
        }
        public void MountDeck(string file)
        {
            if (f != null)
                throw new InvalidOperationException("Deck already mounted");
            if (file != null)
                f = new TapeFile(file, FileAccess.Read);
            else
                f = null;
            cardwasread = false;
        }
        public void UnMountDeck()
        {
            Disconnect();
            f.Dispose();
            f = null;
        }
        public void RDS() /* Read Select*/
        {
            if (f == null)
                eof = true;
            else
            {
                int r = f.ReadRecord(out bool binary, out byte[] mrecord);
                if (r < 1)
                { 
                    eof = true;
                    if(cardwasread)
                    {
                        Console.WriteLine("Card Reader hopper empty");
                        CPU704.halt = true;
                        CPU704.repeat = true;
                        cardwasread = false;
                    }
                }
                else
                {
                    cardwasread = true;
                    if (!binary || mrecord.Length != 160)
                        throw new InvalidDataException("invalid cbn record on Card Reader");
                    CBNConverter.FromCBN(mrecord, out RRecord);
                    eof = false;
                }
            }
            ReadActive = true;
            PosInRecord = 0;
        }
        public int CPY(ref long w) /* Copy */
        {
            if (!ReadActive)
                throw new InvalidOperationException("CPY while device not selected");
            {
                if (eof)
                    return 1;
                if (PosInRecord >= RRecord.Length)
                    return 2;

                w = CPU704.MQ.W=RRecord[PosInRecord++];
                return 0;
            }
        }
        public void Disconnect() /* Disconnect from Device */
        {
            ReadActive = false;            
            RRecord = null;                
        }
        public void Dispose() /* IDisposable-Handling */
        {
            Dispose(true);
        }
        protected virtual void Dispose(bool disposing) /* IDisposable-Handling */
        {
            if (disposing)
            {
                if (ReadActive)
                    Disconnect();
                // free managed resources  
                if (f != null)
                {
                    f.Dispose();
                    f = null;
                }
            }
        }
    }
}
