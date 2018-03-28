using System;

using System.IO;
using System.Xml.Serialization;

namespace Sim704
{
    public enum Bootdev { CRD, MT, DR };
    public class Config704
    {
        public string[] MT;
        public bool[] MTro;
        public string[] DR;
        public string CRD;
        public string CPU;
        public string LP;
        public bool[] Switch;
        public int MemSize;
        public Bootdev boot;        
        public bool logIO;
        public bool logCPU;
    }
    interface I704dev
    {
        int CPY(ref ulong d);
        void Disconnect();
    }
    static class Io704
    {
        /* Device numbers*/
        public static bool tapecheck;
        public static bool tapeindicator;
        static CardReader CRD = null;
        static Printer LP = null;
        static CardPunch CPU = null; 
        static Drum[] DR = null;
        static Tape[] MT = null;
        static I704dev currdev = null;
        static int currdrum=-1;
        public static Config704 Config = null;
        public static void WriteFileConfig(string path)
        {
            XmlSerializer SerializerObj = new XmlSerializer(typeof(Config704));
            TextWriter WriteFileStream = new StreamWriter(path);
            SerializerObj.Serialize(WriteFileStream, Config);
            WriteFileStream.Close();
        }
        public static Bootdev Init(string configfile)
        {
            XmlSerializer SerializerObj = new XmlSerializer(typeof(Config704));
            FileStream ReadFileStream = new FileStream(configfile, FileMode.Open, FileAccess.Read, FileShare.Read);
            Config = (Config704)SerializerObj.Deserialize(ReadFileStream);
            ReadFileStream.Close();
            SenseSwitches.Init(Config.Switch);
            CoreMemory.Init(Config.MemSize);
            return Config.boot;
        }
        static void OpenTape(uint ActiveSubUnit)
        {
            if (MT == null)
            {
                if (Config.MT == null)
                    throw new InvalidOperationException(string.Format("Invalid tape {0} ", ActiveSubUnit));
                MT = new Tape[Config.MT.Length];
            }
            if (MT.Length < ActiveSubUnit)
                throw new InvalidOperationException(string.Format("Invalid tape {0}", ActiveSubUnit));
            if (MT[ActiveSubUnit - 1] == null && Config.MT[ActiveSubUnit - 1] != null)
            {
                MT[ActiveSubUnit - 1] = new Tape(ActiveSubUnit);
                bool ro = false;
                if (Config.MTro != null && Config.MTro.Length >= ActiveSubUnit)
                    ro = Config.MTro[ActiveSubUnit - 1];
                MT[ActiveSubUnit - 1].MountTape(Config.MT[ActiveSubUnit - 1], ro);
            }
            if (MT[ActiveSubUnit - 1] == null)
                throw new InvalidOperationException(string.Format("Invalid tape {0}", ActiveSubUnit));
        }
        static void OpenDrum(uint ActiveSubUnit)
        {
            if (DR == null)
            {
                if (Config.DR == null)
                    throw new InvalidOperationException(string.Format("Invalid drum {0} ", ActiveSubUnit));
                DR = new Drum[Config.DR.Length];
            }
            if (DR.Length < ActiveSubUnit)
                throw new InvalidOperationException(string.Format("Invalid drum {0}", ActiveSubUnit));
            if (DR[ActiveSubUnit - 1] == null && Config.DR[ActiveSubUnit - 1] != null)
            {
                DR[ActiveSubUnit - 1] = new Drum(ActiveSubUnit);
                DR[ActiveSubUnit - 1].MountDrum(Config.DR[ActiveSubUnit - 1]);
            }
            if (DR[ActiveSubUnit - 1] == null)
                throw new InvalidOperationException(string.Format("Invalid drum {0}", ActiveSubUnit));
        }
        static void OpenCardReader()
        {
            if (CRD == null && Config.CRD != null)
            {
                CRD = new CardReader();
                CRD.MountDeck(Config.CRD);
            }
            if (CRD == null)
                throw new InvalidOperationException("Invalid cardreader selected for read");
        }
        static void OpenPrinter()
        {
            if (LP == null && Config.LP != null)
            {
                LP = new Printer();
                LP.MountPaper(Config.LP);
            }
            if (LP == null)
                throw new InvalidOperationException("Invalid printer selected for write");
        }
        static void OpenCardPunch()
        {
            if (CPU == null && Config.CPU != null)
            {
                CPU = new CardPunch();
                CPU.MountDeck(Config.CPU);
            }
            if (CPU == null)
                throw new InvalidOperationException("Invalid printer selected for write");
        }
        public static void RDS(uint unit) /* Read Select */
        {

            if (currdev != null)
                currdev.Disconnect();
            currdev = null;
            currdrum = -1;

            uint CurrUnit = unit >> 4;
            uint CurrSubUnit = unit & 0xF;

            switch (CurrUnit)
            {
                case 8: /* BCD Tape */
                case 9: /* Bin Tape */
                    if (CurrSubUnit < 1 || CurrSubUnit > 10)
                        throw new InvalidOperationException(string.Format("Invalid tape {0} selected for read", CurrSubUnit));
                    OpenTape(CurrSubUnit);
                    MT[CurrSubUnit - 1].RT(CurrUnit == 9);
                    currdev = MT[CurrSubUnit - 1];
                    break;
                case 12: /* Drum */
                    if (CurrSubUnit < 1 || CurrSubUnit > 8)
                        throw new InvalidOperationException(string.Format("Invalid drum {0} selected for read", CurrSubUnit));
                    OpenDrum(CurrSubUnit);
                    DR[CurrSubUnit - 1].RDR();
                    currdev = DR[CurrSubUnit - 1];
                    currdrum = (int)CurrSubUnit - 1;
                    break;
                case 13: /* Card Reader */
                    if (CurrSubUnit != 1)
                        throw new InvalidOperationException(string.Format("Invalid Cardreader {0} selected for read", CurrSubUnit));
                    OpenCardReader();
                    CRD.RDS();
                    currdev = CRD;
                    break;
                case 15: /* Printer */
                    if (CurrSubUnit != 1)
                        throw new InvalidOperationException(string.Format("Invalid Printer {0} selected for read", CurrSubUnit));
                    OpenPrinter();
                    LP.RPR();
                    currdev = LP;
                    break;
                default:
                    throw new InvalidOperationException(string.Format("Invalid device {0} selected for read", unit));
            }
        }
        public static void WRS(uint unit) /* Write Select */
        {
            if (currdev != null)
                currdev.Disconnect();
            currdev = null;
            currdrum = -1;

            uint CurrUnit = unit >> 4;
            uint CurrSubUnit = unit & 0xF;

            switch (CurrUnit)
            {
                case 1: /* CRT */
                    if (CurrSubUnit != 8)
                        throw new InvalidOperationException(string.Format("Invalid CRT {0} selected for write", CurrSubUnit));
                    throw new NotImplementedException("CRT");
                case 8: /* BCD Tape */
                case 9: /* Bin Tape */
                    if (CurrSubUnit < 1 || CurrSubUnit > 10)
                        throw new InvalidOperationException(string.Format("Invalid tape {0} selected for write", CurrSubUnit));
                    OpenTape(CurrSubUnit);
                    MT[CurrSubUnit - 1].WT(CurrUnit == 9);
                    currdev = MT[CurrSubUnit - 1];
                    break;
                case 12: /* Drum */
                    if (CurrSubUnit < 1 || CurrSubUnit > 8)
                        throw new InvalidOperationException(string.Format("Invalid drum {0} selected for read", CurrSubUnit));
                    OpenDrum(CurrSubUnit);
                    DR[CurrSubUnit - 1].WDR();
                    currdev = DR[CurrSubUnit - 1];
                    currdrum = (int)CurrSubUnit - 1;
                    break;
                case 13: /* Sim Tape */
                    if (CurrSubUnit == 11) /* IOD */
                        return;
                    else if (CurrSubUnit < 1 || CurrSubUnit > 10)
                        throw new InvalidOperationException(string.Format("Invalid tape {0} selected for sim write", CurrSubUnit));
                    throw new NotImplementedException("WTS");
                case 14: /* Card Punch */
                    if (CurrSubUnit != 1)
                        throw new InvalidOperationException(string.Format("Invalid punch {0} selected for write", CurrSubUnit));
                    OpenCardPunch();
                    CPU.WPU();
                    currdev = CPU;
                    break;
                case 15: /* Printer */
                    if (CurrSubUnit != 1)
                        throw new InvalidOperationException(string.Format("Invalid printer {0} selected for write", CurrSubUnit));
                    OpenPrinter();
                    LP.WPR();
                    currdev = LP;
                    break;
                default:
                    throw new InvalidOperationException(string.Format("Invalid device {0} selected for write", unit));
            }
        }
        public static void BST(uint unit) /* Backspace Tape */
        {
            uint CurrUnit = unit >> 4;
            uint CurrSubUnit = unit & 0xF;
            if (currdev != null)
                currdev.Disconnect();
            currdev = null;
            if ((CurrUnit < 8) || (CurrUnit > 9) || (CurrSubUnit < 1 || CurrSubUnit > 10))
                throw new InvalidOperationException("invalid tape for BST");
            OpenTape(CurrSubUnit);
            MT[CurrSubUnit - 1].BST();
        }
        public static void WEF(uint unit) /* Write End of File */
        {
            uint CurrUnit = unit >> 4;
            uint CurrSubUnit = unit & 0xF;
            if (currdev != null)
                currdev.Disconnect();
            currdev = null;
            if ((CurrUnit < 8) || (CurrUnit > 9) || (CurrSubUnit < 1 || CurrSubUnit > 10))
                throw new InvalidOperationException("invalid tape for WEF");
            OpenTape(CurrSubUnit);
            MT[CurrSubUnit - 1].WEF();
        }
        public static void REW(uint unit) /* Rewind */
        {
            uint CurrUnit = unit >> 4;
            uint CurrSubUnit = unit & 0xF;
            if (currdev != null)
                currdev.Disconnect();
            currdev = null;
            if ((CurrUnit < 8) || (CurrUnit > 9) || (CurrSubUnit < 1 || CurrSubUnit > 10))
                throw new InvalidOperationException("invalid tape for REW");
            OpenTape(CurrSubUnit);
            MT[CurrSubUnit - 1].REW();
        }
        public static uint ETT() /* End of Tape Test */
        {
            uint ret = 1u;
            if(tapeindicator)
            {
                tapeindicator = false;
                ret = 0;
            }
            return ret;

        }
        public static void LDA(uint address) /* Locate Drum Address */
        {
            if (currdev==null||currdrum == -1)
                throw new InvalidOperationException("LDA not after RDR/WDR");
            DR[currdrum].LDA(address);
            currdrum = - 1;
        }
        public static int CPY(ref ulong data) /* Copy and skip */
        {
            currdrum = -1;
            if (currdev == null)
                throw new InvalidOperationException("CPY while no device selected");
            return currdev.CPY(ref data);
        }
        public static uint RTT() /* Redundancy Tape Test */
        {
            uint ret = tapecheck ? 0 : 1u;
            tapecheck = false;
            return ret;
        }
        public static uint PSE(uint unit) /* plus sense */
        {
            uint skip = 0;
            uint CurrUnit = unit>>4;
            uint CurrSubUnit = unit&0xF;
            switch (CurrUnit)
            {
                case 6: /* Sense Lights */
                    if (CurrSubUnit == 0)
                        SenseLights.SLF();
                    else if (CurrSubUnit >= 1 && CurrSubUnit <= 4)
                        SenseLights.SLN(CurrSubUnit);
                    else
                        throw new InvalidOperationException("invalid Sense Light");
                    break;
                case 7: /* Sense Switches */
                    if (CurrSubUnit >= 1 && CurrSubUnit <= 6)
                        skip = SenseSwitches.SWT(CurrSubUnit);
                    else
                        throw new InvalidOperationException("invalid Sense Switch");
                    break;
                case 14: /* Punch */
                    if (CPU != null && CurrSubUnit >= 1 && CurrSubUnit <= 2)
                        CPU.SPU(CurrSubUnit);
                    else
                        throw new InvalidOperationException("invalid Punch Sense");
                    break;
                case 15: /* Printer */
                    if (LP!=null&&CurrSubUnit == 0)                        
                        skip=LP.SPT();
                    else if (LP != null && CurrSubUnit >= 1 && CurrSubUnit <= 10)
                        LP.SPR(CurrSubUnit);
                    else
                        throw new InvalidOperationException("invalid Printer Sense");
                    break;
                default:
                    throw new InvalidOperationException("invalid device for PSE");
            }
            return skip;
        }
        public static uint MSE(uint unit) /* minus sense */
        {
            uint skip = 0;
            uint CurrUnit = unit >> 4;
            uint CurrSubUnit = unit & 0xF;
            switch (CurrUnit)
            {
                case 6: /* Sense Lights */
                    if (CurrSubUnit >= 1 && CurrSubUnit <= 4)
                        skip = SenseLights.SLT(CurrSubUnit);
                    else
                        throw new InvalidOperationException("invalid Sense Light");
                    break;
                default:
                    throw new InvalidOperationException("invalid device for MSE");
            }
            return skip;
        }
        public static void OnProcessExit(object sender, EventArgs e)
        {
            if (currdev != null)
                currdev.Disconnect();
            currdev = null;
            if (CRD != null)
                CRD.Dispose();
            if (CPU != null)
                CPU.Dispose();
            if (MT != null)
                for (int i = 0; i < MT.Length; i++)
                    if (MT[i] != null)
                        MT[i].Dispose();
            if (DR != null)
                for (int i = 0; i < DR.Length; i++)
                    if (DR[i] != null)
                        DR[i].Dispose();
            if (LP != null)
                LP.Dispose();
            Console.Error.WriteLine("finished");
        }
    }
}
