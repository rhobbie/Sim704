using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace Sim704
{

    public class TapeFile /* read and write access to p7b file */: IDisposable
    {
        FileStream f;
        Stack<long> recpos; /* start positions of records in file */ 
        bool stored;
        int last;
        public int NumOfRecords()
        {
            return recpos.Count;
        }
        public TapeFile(string name, FileAccess Acc)
        {
            /* P7B Format */
            /* Ein Tape ist eine Folge von Records */
            /* Record ist folge von Zeichen. Bits 0-6: Zeichen mit Parity, Bit 7: Recordmarker */
            /* Das erste Byte des Records hat Bit 7 gesetzt, alle folgenden Bytes des Records haben Bit 7 nicht gesetzt*/
            /* bcd record mit länge 1 und wert 15 ist EOF Marker */

            f = new FileStream(name, FileMode.OpenOrCreate, Acc);
            recpos = new Stack<long>();
            stored = false;
        }
        public TapeFile(string name)
        {
            f = new FileStream(name, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            recpos = new Stack<long>();
            stored = false;
        }
        public int ReadRecord(out bool binary, out byte[] mrecord) /* liest ein record als array von 6-bit werten aus tape-file, rückgabe -1:ende der eingabedatei, 0: EOF, 1: record gelesen; binary true: binärformat false: bcdformat */
        {
            int b; /* aktuelles zeichen */
            mrecord = null;
            binary = false;

            if (stored) /* schon ein zeichen gemerkt */
            {
                b = last; /* übernehmen */
                stored = false;
            }
            else
                b = f.ReadByte(); /* zeichen lesen */

            if (b < 0) /* end of media ? */
                return -1; /* EOM */
            recpos.Push(f.Position - 1); /* store startposition of record for Backspace*/
            if ((b & 128) == 0)  /* das erste zeichen eines records muss msb gesetzt haben */
                throw new InvalidDataException("TapeFile:Bit 8 not set at record start");
            List<byte> trecord = new List<byte>(160) { (byte)(b & 127) }; /* record start marker entfernen, zeichen speichern */
            do
            {
                b = f.ReadByte();
                if (b < 0 || (b & 128) != 0) /* nächster record oder EOF */
                {
                    stored = true; /* merker setzen */
                    last = b; /* gelesenen wert speichern für nächsten aufruf*/
                    break;
                }
                trecord.Add((byte)b);
            }
            while (true);
            TapeConverter.FromTape(trecord.ToArray(), out binary, out mrecord);
            if (!binary && mrecord.Length == 1 && mrecord[0] == 15)
                return 0; /* EOF */
            return 1; /* kein end of file */
        }
        public void BackSpace()
        {
            if (recpos.Count > 0)
            {
                long pos=recpos.Pop();
                f.Seek(pos, SeekOrigin.Begin);
            }
            else
                f.Seek(0, SeekOrigin.Begin);
            stored = false;
        }
        public void Rewind()
        {
            f.Seek(0, SeekOrigin.Begin);
            recpos.Clear();
            stored = false;
        }
        public void WriteRecord(bool binary, byte[] mrecord)
        {
            if (stored && last != -1)
                f.Seek(-1, SeekOrigin.Current);
            stored = false;
            TapeConverter.ToTape(binary, mrecord, out byte[] trecord);
            recpos.Push(f.Position);
            trecord[0] |= 0x80;
            f.Write(trecord, 0, trecord.Length);

        }
        public void WriteEOF()
        {
            WriteRecord(false, new byte[] { 15 });
        }
        public void Dispose() /* IDisposable-Handling */
        {
            Dispose(true);
        }
        protected virtual void Dispose(bool disposing) /* IDisposable-Handling */
        {
            if (disposing)
            {
                // free managed resources  
                if (f != null)
                {
                    f.Dispose();
                    f = null;
                }
            }
        }
    }
    public static class TapeConverter /* Converter between raw tape records and bcd/binary records */
    {
        /* Daten auf Tape ist folge von 6 bit werten: Bits 0-5: Zeichen, Bit 6: Parity */
        /* Die Parity (grade/ungerade) der Bits 0-6 aller Bytes eines Records ist geich*/
        /* Bei ungerader Parity: Binary Record */
        /* Binary Records enthalten beliebige 6 Bit Daten */
        /* Bei geradeder Parity: BCD Record*/
        /* Ein BCD Record darf keine Null enthalten */
        /* Bei BCD Records erfolgt eine Umwandlung vom Tapeformat zum Memoryformat: */
        /* Tape 10dez -> Memory 0 (BCD '0') und  Wenn Bit 4 gesetzt wird Bit 5 invertiert, d.h. von Tape zu Mem werden BDC Zeichen  'A'-'I' mit 'S'-'Z' getauscht,  */

        static int[] tape2mem = null;  /* bcd umwandlung tape <-> mem */
        static int[] mem2tape = null;  /* bcd umwandlung tape <-> mem */
        static bool[] oddparity = null; /* 7-Bit parity table: true = ungerade parität */
        static TapeConverter() /* statischer Konstruktor, füllt oddparity und tape2mem,mem2tape */
        {
            oddparity = new bool[128];
            for (int i = 0; i < 128; i++) /* alle möglichen 7-bit werte */
            {
                bool o = false; /* mit gerader parität anfangen */
                for (int b = 0; b < 7; b++) /* alle bits durchgehen*/
                {
                    if (0 != (i & (1 << b)))  /* bit gesetzt? */
                        o = !o;   /* parity togglen */
                }
                oddparity[i] = o;  /* parity speichern */
            }
            tape2mem = new int[64];
            mem2tape = new int[64];
            for (int m = 0; m < 64; m++)  /* alle möglichen 6-bit werte */
            {
                int t;
                if (m == 10)
                    t = -1;  /* bcd-wert 10 in mem nicht erlaubt */
                else if (m == 0) /* konvertiere 0 */
                    t = 10;
                else if (0 != (m & 16)) /* konvertiere A-I <-> S-Z */
                    t = m ^ 32;
                else
                    t = m;
                mem2tape[m] = t;  /* wert speichern */
            }
            for (int t = 0; t < 64; t++)  /* alle möglichen 6-bit werte */
            {
                int m;
                if (t == 0)
                    m = -1;  /* bcd-wert 0 auf tape nicht erlaubt */
                else if (t == 10) /* konvertiere 0 */
                    m = 0;
                else if (0 != (t & 16)) /* konvertiere A-I <-> S-Z */
                    m = t ^ 32;
                else
                    m = t;
                tape2mem[t] = m;  /* wert speichern */
            }
        }
        public static void ToTape(bool binary, byte[] mrecord, out byte[] trecord) /* converts record from Binary/BCD into raw tape format */
        {
            trecord = new byte[mrecord.Length];
            for (int i = 0; i < mrecord.Length; i++)
            {
                byte b = mrecord[i];
                if ((b & 128) != 0)
                    throw new InvalidDataException("TapeConverter:invalid bit 7 set in mem");
                if ((b & 64) != 0)
                    throw new InvalidDataException("TapeConverter:invalid bit 6 set in mem");
                if (!binary)
                {
                    if (mem2tape[b] == -1)
                        throw new Exception("TapeConverter:invalid BCD character in mem");
                    b = (byte)mem2tape[b];
                }
                if (binary != oddparity[b])
                    b |= 0x40;
                trecord[i] = b;
            }
        }
        public static void FromTape(byte[] trecord, out bool binary, out byte[] mrecord) /* converts record from raw tape format into Binary/BCD record */
        {
            binary = oddparity[trecord[0]]; /* ungerade parität -> binärfile */
            mrecord = new byte[trecord.Length];
            for (int j = 0; j < trecord.Length; j++)
            {
                if ((trecord[j] & 128) != 0)
                    throw new InvalidDataException("TapeConverter:bit 7 is set on tape");
                if (binary != oddparity[trecord[j]]) /* weitere zeichen auf parität prüfen */
                    throw new InvalidDataException("TapeConverter:parity error on tape");
                if (binary)
                    mrecord[j] = (byte)(trecord[j] & 63); /* binärdaten direkt übernehmen */
                else
                {
                    int c = tape2mem[trecord[j] & 63];   /* bcddaten konverteren */
                    if (c == -1) /*  (0 auf tape) -> ungültig */
                        throw new InvalidDataException("TapeConverter:invalid BCD char 0 on tape");
                    mrecord[j] = (byte)c; /* speichern*/
                }
            }
        }
    }
    public static class CBNConverter /* Converter betwen binary cards in RCD format and CBN card format */
    {
        public static byte[] ToCBN(ulong[] mrecord) /* convert card from RCD Format to CBN format */
        {
            byte[] trecord = new byte[160];
            for (int y = 0; y < 12; y++)  /* for all rows */
                for (int x = 0; x < 72; x++)  /* for all columns */
                {
                    /* index and bitpos in RCD format*/
                    int mpos = x / 36 + (11 - y) * 2;
                    int mbit = 35 - x % 36;
                    /* index and bitpos in CBN format */
                    int tpos = x * 2 + y / 6;
                    int tbit = 5 - y % 6;

                    if ((mrecord[mpos] & (1UL << mbit)) != 0) /*bit in mrecord set? */
                        trecord[tpos] |= (byte)(1 << tbit); /* set bit in trecord  */
                }
            return trecord;
        }
        public static void FromCBN(byte[] trecord, out ulong[] mrecord) /* convert  card from CBN Format to RCD format */
        {
            if (trecord.Length != 160)
                throw new Exception("wrong record length");
            mrecord = new ulong[24];

            for (int y = 0; y < 12; y++)  /* for all rows */
                for (int x = 0; x < 72; x++)  /* for all columns */
                {
                    /* index and bitpos in RCD format*/
                    int mpos = x / 36 + (11 - y) * 2;
                    int mbit = 35 - x % 36;
                    /* index and bitpos in CBN format */
                    int tpos = x * 2 + y / 6;
                    int tbit = 5 - y % 6;
                    if ((trecord[tpos] & (1 << tbit)) != 0) /* bit in trecord set ? */
                        mrecord[mpos] |= 1UL << mbit;  /* set bit in mrecord */
                }
        }
    }
}
