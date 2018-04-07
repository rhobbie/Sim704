using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace Sim704
{
    class Drum : IDisposable, I704dev
    {
        FileStream f;
        W36[] Buffer;
        uint unit;
        uint DrumAddress;
        bool ReadActive; /* read is active */
        bool WriteActive; /* write is active */
        bool dirty; /* data was changed */
        public Drum(uint u)
        {
            unit = u;
            ReadActive = false;
            WriteActive = false;
        }
        public void RDR() /* Read Drum*/
        {
            ReadActive = true;
            WriteActive = false;
            DrumAddress = 0;
        }
        public void WDR() /* Write Drum*/
        {
            ReadActive = false;
            WriteActive = true;
            DrumAddress = 0;
        }
        public void LDA(uint Adr) /* Load Drum Adress*/
        {
            DrumAddress = Adr % (uint)Buffer.Length;
        }
        public void MountDrum(string file) /* Mount Drum and load from file */
        {

            Buffer = new W36[2048];
            byte[] byteBuffer = new byte[2048 * 5];
            if (f != null)
                throw new InvalidOperationException(string.Format("Drum {0} already mounted", unit));
            if (file != null)
                f = new FileStream(file, FileMode.OpenOrCreate);
            else
                f = null;
            int rlen = f.Read(byteBuffer, 0, byteBuffer.Length);
            if (rlen == byteBuffer.Length&& f.Length == byteBuffer.Length)
                for (int i = 0, p = 0; i < 2048; i++, p += 5)
                {
                    W15 A = new W15(BitConverter.ToUInt16(byteBuffer, p));
                    W15 D = new W15(BitConverter.ToUInt16(byteBuffer, p + 2));
                    W3 T = new W3((uint)(byteBuffer[p + 4] & 15));
                    W3 P = new W3((uint)(byteBuffer[p + 4] >> 4));
                    Buffer[i] = new W36() { A = A, T = T, D = D, P = P };
                }
            else if (rlen != 0)
                throw new FileLoadException("Drum file load error");
            dirty = false;
        }
        public void UnMountDrum() /* Unmount Drum and save to file*/
        {
            if (dirty)
            {
                byte[] byteBuffer = new byte[2048 * 5];
                if (f == null)
                    throw new InvalidOperationException(string.Format("Drum {0} not mounted", unit));
                for (int i = 0, p = 0; i < 2048; i++, p+=5)
                {
                    BitConverter.GetBytes((UInt16)(Buffer[i].A)).CopyTo(byteBuffer, p);
                    BitConverter.GetBytes((UInt16)(Buffer[i].D)).CopyTo(byteBuffer, p + 2);
                    byteBuffer[p + 4] = (byte)(Buffer[i].T | (Buffer[i].P << 4));
                }
                f.Seek(0, SeekOrigin.Begin);
                f.Write(byteBuffer, 0, byteBuffer.Length);
            }
            f.Close();
            f.Dispose();
            f = null;
            Buffer = null;
        }
        public int CPY(ref ulong w) /* Copy */
        {
            int ret = 0;
            if (ReadActive)
            {
                w = Buffer[DrumAddress];
                ALU.MQ = (W36)w;
                if (Io704.Config.LogIO != null)
                    Io704.LogIO.WriteLine("Drum {0} Read {1} from Address {2}", unit, ALU.MQ, (W15)DrumAddress);
                DrumAddress++;
                if (DrumAddress >= Buffer.Length)
                    DrumAddress = 0;
            }
            else if (WriteActive)
            {
                ALU.MQ = (W36)w;
                if (Io704.Config.LogIO != null)
                    Io704.LogIO.WriteLine("Drum {0} Written {1} to Address {2}", unit, ALU.MQ, (W15)DrumAddress);
                Buffer[DrumAddress++] = (W36)w;
                if (DrumAddress >= Buffer.Length)
                    DrumAddress = 0;
                dirty = true;
            }
            else
                throw new InvalidOperationException("CPY while device not selected");
            return ret;
        }
        public void Disconnect() /* Disconnect from Device */
        {
            ReadActive = false;
            WriteActive = false;
        }
        public void Dispose() /* IDisposable-Handling */
        {
            Dispose(true);
        }
        protected virtual void Dispose(bool disposing) /* IDisposable-Handling */
        {
            if (disposing)
            {
                Disconnect();
                // free managed resources  
                if (f != null)
                    UnMountDrum();
            }
        }
    }
}
