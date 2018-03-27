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
        public TapeFile(string name, bool rdonly)
        {
            /* P7B Format */
            /* Ein Tape ist eine Folge von Records */
            /* Record ist folge von Zeichen. Bits 0-6: Zeichen mit Parity, Bit 7: Recordmarker */
            /* Das erste Byte des Records hat Bit 7 gesetzt, alle folgenden Bytes des Records haben Bit 7 nicht gesetzt*/
            /* bcd record mit länge 1 und wert 15 ist EOF Marker */

            f = new FileStream(name, rdonly ? FileMode.Open:FileMode.OpenOrCreate, rdonly? FileAccess.Read:FileAccess.ReadWrite);
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
            List<byte> trecord = new List<byte>() { (byte)(b & 127) }; /* record start marker entfernen, zeichen speichern */
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
            return 1; /* no EOF */
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
            if (f.Position != f.Length)  /* not at end of file */
                f.SetLength(f.Position); /* cut remaining parts */

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
                        throw new InvalidDataException("TapeConverter:invalid BCD char on tape");
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
        public static int CBNToBCD(byte[] trecord, int start, int length, out byte[] bcd) /* Wandle Hollerith Column Binary Format zu BCD */
        {                                                                            /* rückgabe anzahl ungütiger Hollerith codes */
            bcd = new byte[length];
            int err = 0;
            for (int i = 0, x = start * 2; i < length; i++, x += 2)
            {
                int w = (trecord[x] << 6) | trecord[x + 1];
                if (hollerith2bcd.TryGetValue(w, out byte value))
                    bcd[i] = value;
                else
                {
                    bcd[i] = 0x30;  /* ungültig */
                    err++;
                }
            }
            return err;
        }
        public static void BCDToCBN(byte[] bcd, int start, byte[] trecord) /* Wandle BCD zu Hollerith Column Binary  */
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
        public static int CBNToString(byte[] trecord, int start, int length, out string s) /* Wandle Hollerith Column Binary Format zu string */
        {
            int err;/* rückgabe anzahl ungütiger Hollerith codes */
            err = CBNToBCD(trecord, start, length, out byte[] bcd);
            s = BcdConverter.BcdToString(bcd);
            return err;
        }
        public static void StringToCBN(string s, int start, byte[] trecord) /* Wandle string zu Hollerith Column Binary  */
        {
            BCDToCBN(BcdConverter.StringToBcd(s), start, trecord);
        }
    }
    public static class BcdConverter/* Converter betwen BCD data and char/string data */
    {
        /* Umwandlung bcd mem format nach char, quelle: fortran-ii listing, listtape.c */
        static int[] bcd2asc = new int[64]; /*  6 bit bcd to char, wert 10dez ungültig, ergibt -1 */
        static Dictionary<char, int> asc2bcd;
        static readonly string[] ibm704bcd = new string[] /* Umwandlungstabelle, Erste zwei Zeichen Oktalwert, drittes Zeichen Charwert */
        {
            "000",
            "011",
            "022",
            "033",
            "044",
            "055",
            "066",
            "077",
            "108",
            "119",
            "12_",  /* Soll: b durchgestrichen ␢  slashed b substitute blank SM670000 Substitute Blank [That is the small b shape with slash.]  U+2422 BLANK SYMBOL */
            "13=",  /* Report # */                
            "14'",  /* Report @ */
            "15:",
            "16>",
            "17{",  /* auch &"  Soll Wurzel: √ square root tape mark*/
            "20+",  /* Report & */
            "21A",
            "22B",
            "23C",
            "24D",
            "25E",
            "26F",
            "27G",
            "30H",
            "31I",
            "32?",
            "33.",
            "34)", /* Report Kleines quadrat ⌑ SM490000	Lozenge U+2311 SQUARE LOZENGE */
            "35[",
            "36<",
            "37}", /*auch %| soll: Senkrechter strich dreimal durchgestrichen  SS970000 Group Mark [Vertical bar across three short horizontal bars]  U+241D SYMBOL FOR GROUP SEPARATOR ␝ */
            "40-",
            "41J",
            "42K",
            "43L",
            "44M",
            "45N",
            "46O",
            "47P",
            "50Q",
            "51R",
            "52!",
            "53$",
            "54*",
            "55]",
            "56;",
            "57^", /* auch _ soll: dreieck ∆ Δ Greek capital delta mode change*/
            "60 ",
            "61/",
            "62S",
            "63T",
            "64U",
            "65V",
            "66W",
            "67X",
            "70Y",
            "71Z",
            "72|", /* auch #  soll: senkrechter strich zweimal durchgestrichen ‡ ‡  not equals record mark SS950000 Record Mark [Vertical bar across two short horizontal bars]  U+241E SYMBOL FOR RECORD SEPARATOR  U+29E7 THERMODYNAMIC [Vertical bar across two short horizontal bars] */
            "73,",
            "74(", /* Report: % */
            "75~", /* auch ^` soll: inverses ^  ˅ γ U+02C7 ˇ caron  inverted caret or equals  word separator  U+22CE CURLY LOGICAL OR */
            "76\\",
            "77\"" /* auch { soll: wagerechter strich 3 mal durchgestrichen ⧻ triple vertical bar slashed segment mark SS960000 Segment Mark [Horizontal bar across 3 short verticals]   U+241F SYMBOL FOR UNIT SEPARATOR U+29FB TRIPLE PLUS [Horizontal bar across three short verticals] */
        };
        static BcdConverter() /* statischer Konstruktor */
        {
            int i;
            asc2bcd = new Dictionary<char, int>();
            for (i = 0; i < ibm704bcd.Length; i++)
            {
                int b = Convert.ToInt32(ibm704bcd[i].Substring(0, 2), 8);
                if (bcd2asc[b] != 0 || b != i)
                    throw new Exception("BcdConverter: error in bcd table");
                char c = ibm704bcd[i][2];
                bcd2asc[b] = c;
                char cl = Char.ToLower(c);
                asc2bcd.Add(c, b);
                if (cl != c)
                    asc2bcd.Add(cl, b);
            }
        }
        public static char BcdToChar(byte bcd) /* wandelt BCD nach char mit Prüfung */
        {
            if (0 != (bcd & 0xC0u)) /* bits 7 oder 6 müssen Null sein */
                throw new Exception("BcdConverter:invalid BCD char");
            if (bcd2asc[bcd] == -1)
            {
                Console.Write("{0} ", Convert.ToString(bcd, 8).PadLeft(2, '0'));
                return '?';
            }
            return (char)bcd2asc[bcd];
        }
        public static byte CharToBcd(char chr)/* wandelt char nach BCD */
        {
            if (asc2bcd.TryGetValue(chr, out int v))
                return (byte)v;
            else
                return (byte)asc2bcd['?'];
        }
        public static byte[] StringToBcd(string s)/* wandelt string nach BCD Array */
        {
            byte[] B = new byte[s.Length];
            for (int i = 0; i < s.Length; i++)
                B[i] = CharToBcd(s[i]);
            return B;
        }
        public static string BcdToString(byte[] bcd) /* wandelt BCD array string char */
        {
            StringBuilder s = new StringBuilder(bcd.Length);
            foreach (byte b in bcd)
                s.Append(BcdToChar(b));
            return s.ToString();
        }
    }
}
