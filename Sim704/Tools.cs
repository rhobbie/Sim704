using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace Sim704
{

    public class TapeFile /* read and write access to p7b file */: IDisposable
    {
        FileStream f;
        Stack<long> recpos; /* start positions of records in file */ 
        bool stored; /* true:fist byte of record already read */
        int last;  /* value of first byte or -1 for end of media (EOM) */
        public int NumOfRecords()
        {
            return recpos.Count;
        }
        public TapeFile(string name, bool rdonly)
        {
            /* P7B Format */
            /* A tape is a sequence of records */
            /* A record is a sequence of bytes. Bits 0-5: character, Bit 6:parity, Bit 7: recordmarker */
            /* The first byte of a record has bit 7 set, all following bytes have bit 7 not set */
            /* The parity of all bytes in a recored is the same 
            /* the recordmarker is ignored for parity calculation */
            /* even parity= bcd record, odd parity=binary record */
            /* A bcd record with length 1 and value bit 0-5=15dez is EOF (End of File) marker */

            f = new FileStream(name, rdonly ? FileMode.Open:FileMode.OpenOrCreate, rdonly? FileAccess.Read:FileAccess.ReadWrite);
            recpos = new Stack<long>(); /* stack to store the startpositions of the records, used for backspace */
            stored = false;
        }

        public int ReadRecord(out bool binary, out bool parityerror, out byte[] mrecord) /* reads a record as array of 6-bit values from tape-file, return -1:end of tape-file 0: EOF marker read, 1: a record was read; binary=true: binary record false: bcd record, parityerror=true if parity error */
        {
            int b; /* current character */
            mrecord = null;
            binary = false;
            parityerror = false;
            if (stored) /* fist byte of record already read? */
            {
                b = last; /* use it */
                stored = false;
            }
            else
                b = f.ReadByte(); /* read first byte */

            if (b < 0) /* end of media ? */
                return -1; /* EOM */
            recpos.Push(f.Position - 1); /* store the startposition of the record on stack, used for backspace*/
            if ((b & 128) == 0)  /* The first byte of a record must have bit 7 set */
                throw new InvalidDataException("TapeFile:Bit 7 not set at record start");
            List<byte> trecord = new List<byte>() { (byte)(b & 127) }; /* remove record start marker, store character */
            do
            {
                b = f.ReadByte();
                if (b < 0 || (b & 128) != 0) /* next record or EOF */
                {
                    stored = true; /* first byte of next record is already read, set flag */
                    last = b; /* store byte value */
                    break;
                }
                trecord.Add((byte)b);
            }
            while (true);
            parityerror=TapeConverter.FromTape(trecord.ToArray(), out binary, out mrecord); /* convert to memory format, check parity */
            if (!binary && mrecord.Length == 1 && mrecord[0] == 15) /* check for EOF */
                return 0; /* EOF */
            return 1; /* no EOF */
        }
        public void BackSpace()
        {
            if (recpos.Count > 0) /* any records already read ?*/
            {
                long pos=recpos.Pop(); /* read startpos of previous record from stack */
                f.Seek(pos, SeekOrigin.Begin); /* go to that position*/
            }
            else
                f.Seek(0, SeekOrigin.Begin); /* go to start of file */
            stored = false;
        }
        public void Rewind()
        {
            f.Seek(0, SeekOrigin.Begin); /* go to start of file */
            recpos.Clear(); /* clear stack */
            stored = false;
        }
        public void WriteRecord(bool binary, byte[] mrecord)
        {
            if (stored && last != -1)  /* first byte of record already read ? */
                f.Seek(-1, SeekOrigin.Current); /* go back to start of record */
            stored = false;
            TapeConverter.ToTape(binary, mrecord, out byte[] trecord); /* convert to tape format, generate parity */
            recpos.Push(f.Position); /* store startpos of record on stack */
            trecord[0] |= 0x80;  /* add record marker */
            f.Write(trecord, 0, trecord.Length);
            if (f.Position != f.Length)  /* currently not at end of the tape-file? */
                f.SetLength(f.Position); /* cut remaining parts of the file */
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
        /* The raw data on tape is a sequence of 6 bit values: bits 0-5: character, Bit 6: parity */
        /* The parity (even/odd) of all bytes from a record is the same*/
        /* Odd parity: binary record */
        /* binary records can contain any 6 bit data */
        /* Even Parity: BCD record*/
        /* BCD Records cannot contain a zero */
        /* For BCD records a conversion from tapeformat to memoryformat takes place */
        /* tape 10dez <-> memory 0dez (=BCD '0') and if bit 4 is set, bit 5 is inverted ('A'-'I' <-> 'S'-'Z') */

        static int[] tape2mem = null;  /* bcd conversion tape -> mem */
        static int[] mem2tape = null;  /* bcd conversion mem -> tape */
        static bool[] oddparity = null; /* 7-Bit parity table: true = odd parity */
        static TapeConverter() /* static constructor, called at start, fills oddparity and tape2mem,mem2tape */
        {
            oddparity = new bool[128];
            for (int i = 0; i < 128; i++) /* for all 7-bit values */
            {
                bool o = false; /* start with even parity  */
                for (int b = 0; b < 7; b++) /* for all bits*/
                {
                    if (0 != (i & (1 << b)))  /* bit set? */
                        o = !o;   /* toggle parity */
                }
                oddparity[i] = o;  /* store parity */
            }
            tape2mem = new int[64];
            mem2tape = new int[64];
            for (int m = 0; m < 64; m++)  /* for all 6-bit values */
            {
                int t;
                if (m == 10)
                    t = -1;  /* bcd-value 10 in mem not allowed */
                else if (m == 0) /* convert 0 */
                    t = 10;
                else if (0 != (m & 16)) /* convert A-I <-> S-Z */
                    t = m ^ 32;
                else
                    t = m;
                mem2tape[m] = t;  /* store value */
            }
            for (int t = 0; t < 64; t++)  /* for all 6-bit values */
            {
                int m;
                if (t == 0)
                    m = -1;  /* bcd-value 0 on tape not allowed */
                else if (t == 10) /* convert 10 */
                    m = 0;
                else if (0 != (t & 16)) /* convert A-I <-> S-Z */
                    m = t ^ 32;
                else
                    m = t;
                tape2mem[t] = m;  /* store value */
            }
        }
        public static void ToTape(bool binary, byte[] mrecord, out byte[] trecord) /* converts a record from Binary/BCD into raw tape format */
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
                    b |= 0x40; /* add parity */
                trecord[i] = b;
            }
        }
        public static bool FromTape(byte[] trecord, out bool binary, out byte[] mrecord) /* converts a record from raw tape format into Binary/BCD record.  return value true=parity error*/
        {
            bool parityerror = false;

            binary = oddparity[trecord[0]]; /* odd parity on first char -> binary file */
            mrecord = new byte[trecord.Length];
            for (int j = 0; j < trecord.Length; j++)
            {
                if ((trecord[j] & 128) != 0)
                    throw new InvalidDataException("TapeConverter:bit 7 is set on tape");
                if (binary != oddparity[trecord[j]]) /* parity check */
                    parityerror = true;
                if (binary)
                    mrecord[j] = (byte)(trecord[j] & 63); /* copy binary data */
                else
                {
                    int c = tape2mem[trecord[j] & 63];   /* convert bcd data */
                    if (c == -1) /*  (0 on bcd record) -> invalid */
                        throw new InvalidDataException("TapeConverter:invalid BCD char on tape");
                    mrecord[j] = (byte)c; 
                }
            }
            return parityerror;
        }
    }
    public static class CBNConverter /* Converter between binary cards in RCD format and CBN card format */
    {
        public static byte[] ToCBN(ulong[] mrecord) /* convert card from RCD Format to CBN format */
        {
            if (mrecord.Length != 24)
                throw new Exception("wrong record length");
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
        public static void FromCBN(byte[] trecord, out ulong[] mrecord) /* convert card from CBN Format to RCD format */
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
    public static class HollerithConverter /* Converter between Hollerith codes in CBN card format and BCD data or strings*/
    {
        static readonly string[] hcode = new string[64] {
             "0",    "1",    "2",    "3",    "4",    "5",    "6",    "7",    "8",    "9",    "8-2",    "8-3",     "8-4",    "8-5",    "8-6",    "8-7",
            "12", "12-1", "12-2", "12-3", "12-4", "12-5", "12-6", "12-7", "12-8", "12-9", "12-8-2", "12-8-3",  "12-8-4", "12-8-5", "12-8-6", "12-8-7",
            "11", "11-1", "11-2", "11-3", "11-4", "11-5", "11-6", "11-7", "11-8", "11-9", "11-8-2", "11-8-3",  "11-8-4", "11-8-5", "11-8-6", "11-8-7",
              "",  "0-1",  "0-2",  "0-3",  "0-4",  "0-5",  "0-6",  "0-7",  "0-8",  "0-9",  "0-8-2",  "0-8-3",   "0-8-4",  "0-8-5",  "0-8-6",  "0-8-7" };
        static int[] bcd2hollerith;
        static Dictionary<int, byte> hollerith2bcd;
        static HollerithConverter()
        {
            bcd2hollerith = new int[64];
            hollerith2bcd = new Dictionary<int, byte>();
            for (byte i = 0; i < 64; i++)
            {
                int w = 0;

                string[] s = hcode[i].Split(new char[] { '-' });
                foreach (string c in s)
                    if (c != "")
                    {
                        int d = Convert.ToInt32(c);
                        if (d < 10)
                            d = 10 - d;
                        w |= 1 << (d - 1);
                    }
                bcd2hollerith[i] = w;
                hollerith2bcd.Add(w, i);
            }
        }
        public static int CBNToBCD(byte[] trecord, int start, int length, out byte[] bcd) /* Convert Hollerith Column Binary format to BCD */
        {                                                                            /* return: number of invalid Hollerith codes */
            bcd = new byte[length];
            int err = 0;
            for (int i = 0, x = start * 2; i < length; i++, x += 2)
            {
                int w = (trecord[x] << 6) | trecord[x + 1];
                if (hollerith2bcd.TryGetValue(w, out byte value))
                    bcd[i] = value;
                else
                {
                    bcd[i] = 0x30;  /* invalid */
                    err++;
                }
            }
            return err;
        }
        public static void BCDToCBN(byte[] bcd, int start, byte[] trecord) /* Convert BCD to Hollerith Column Binary  */
        {
            int x = start * 2;
            foreach (byte b in bcd)
            {
                if (0 != (b & 0xC0))
                    throw new FormatException("invalid BCD char");
                int w = bcd2hollerith[b];
                trecord[x] = (byte)((w >> 6) & 0x3f);
                trecord[x + 1] = (byte)(w & 0x3f);
                x += 2;
            }
        }
        public static int CBNToString(byte[] trecord, int start, int length, out string s) /* Convert Hollerith Column Binary format to string */
        {                                                                                  /* return: number of invalid Hollerith codes */
            int err; 
            err = CBNToBCD(trecord, start, length, out byte[] bcd);
            s = BcdConverter.BcdToString(bcd);
            return err;
        }
        public static void StringToCBN(string s, int start, byte[] trecord) /* Convert string to Hollerith Column Binary  */
        {
            BCDToCBN(BcdConverter.StringToBcd(s), start, trecord);
        }
    }
    public static class BcdConverter/* Converter betwen BCD data and char/string data */
    {
        
        static int[] bcd2asc = new int[64]; /*  6 bit bcd to char, value 10dez=invalid, results to -1 */
        static Dictionary<char, int> asc2bcd;
        static readonly string[] ibm704bcd = new string[] /* Conversion table, first two chars: octal value, third: char value */
        {
            "000","011","022","033","044","055","066","077","108","119","12_","13=","14'","15:","16>","17{",  
            "20+","21A","22B","23C","24D","25E","26F","27G","30H","31I","32?","33.","34)","35[","36<","37}", 
            "40-","41J","42K","43L","44M","45N","46O","47P","50Q","51R","52!","53$","54*","55]","56;","57^", 
            "60 ","61/","62S","63T","64U","65V","66W","67X","70Y","71Z","72|","73,","74(","75~","76\\","77\"" 
        };
        static BcdConverter() /* static constructor */
        {
            int i;
            asc2bcd = new Dictionary<char, int>();
            for (i = 0; i < ibm704bcd.Length; i++)
            {
                int b = Convert.ToInt32(ibm704bcd[i].Substring(0, 2), 8);
                if (bcd2asc[b] != 0 || b != i)
                    Console.Error.WriteLine("BcdConverter: error in bcd table");
                char c = ibm704bcd[i][2];
                bcd2asc[b] = c;
                char cl = Char.ToLower(c);
                asc2bcd.Add(c, b);
                if (cl != c)
                    asc2bcd.Add(cl, b);
            }
        }
        public static char BcdToChar(byte bcd) /* converts BCD to char */
        {
            if (0 != (bcd & 0xC0u)) /* Bits 7 oder 6 have to be zero */
                throw new Exception("BcdConverter:invalid BCD char");
            if (bcd2asc[bcd] == -1)
                return '?';
            return (char)bcd2asc[bcd];
        }
        public static byte CharToBcd(char chr)/* converts char to BCD */
        {
            if (asc2bcd.TryGetValue(chr, out int v))
                return (byte)v;
             return (byte)asc2bcd['?'];
        }
        public static byte[] StringToBcd(string s)/* converts string to BCD array */
        {
            byte[] B = new byte[s.Length];
            for (int i = 0; i < s.Length; i++)
                B[i] = CharToBcd(s[i]);
            return B;
        }
        public static string BcdToString(byte[] bcd) /* converts BCD array to string */
        {
            StringBuilder s = new StringBuilder(bcd.Length);
            foreach (byte b in bcd)
                s.Append(BcdToChar(b));
            return s.ToString();
        }
    }
}
