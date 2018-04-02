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
        ulong[] Buffer;
        uint unit;
        uint DrumAddress;
        bool ReadActive; /* read is active */
        bool WriteActive; /* write is active */
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
            DrumAddress = Adr%(uint)Buffer.Length;
        }
        public void MountDrum(string file) /* Mount Drum and load from file */
        {
            Buffer = new ulong[2048];
            byte[] byteBuffer = new byte[2048 * 8];
            if (f != null)
                throw new InvalidOperationException(string.Format("Drum {0} already mounted", unit));
            if (file != null)
                f = new FileStream(file, FileMode.OpenOrCreate);
            else
                f = null;
            f.Read(byteBuffer, 0, byteBuffer.Length);
            for (int i = 0; i < 2048; i++)
                Buffer[i] = BitConverter.ToUInt64(byteBuffer, i * 8);

        }
        public void UnMountDrum() /* Unmount Drum and save to file*/
        {
            byte[] byteBuffer = new byte[2048 * 8];
            if (f == null)
                throw new InvalidOperationException(string.Format("Drum {0} not mounted", unit));
            for (int i = 0; i < 2048; i++)
                BitConverter.GetBytes(Buffer[i]).CopyTo(byteBuffer, i * 8);
            f.Seek(0, SeekOrigin.Begin);
            f.Write(byteBuffer, 0, byteBuffer.Length);
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
                if (Io704.Config.LogIO!=null)
                    Io704.LogIO.WriteLine("Drum {0} Read {1} from Address {2}", unit, ALU.MQ, (W15)DrumAddress);
                DrumAddress++;
                if (DrumAddress >= Buffer.Length)
                    DrumAddress = 0;
            }
            else if (WriteActive)
            {
                ALU.MQ = (W36)w;
                if (Io704.Config.LogIO!=null)
                    Io704.LogIO.WriteLine("Drum {0} Written {1} to Address {2}", unit, ALU.MQ, (W15)DrumAddress);
                Buffer[DrumAddress++] = w;
                if (DrumAddress >= Buffer.Length)
                    DrumAddress = 0;
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
