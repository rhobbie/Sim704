using System;

using System.IO;
using System.Xml.Serialization;

namespace Sim704
{
    public enum Bootdev { MT, CRD, DRM };
    public class Config704
    {
        public string[] MTn;
        public string[] DRMn;
        public string CRDn;
        public string PRTn;
        public bool[] Switch;
        public int MemSize;
        public Bootdev boot;        
        public bool logIO;
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
        static CardReader CRD = null;
        static Printer PRT = null;
        static Drum[] DRM = null;
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
        static void opentape(uint ActiveSubUnit)
        {
            if (MT == null)
            {
                if (Config.MTn == null)
                    throw new InvalidOperationException(string.Format("Invalid tape {0} ", ActiveSubUnit));
                MT = new Tape[Config.MTn.Length];
            }
            if (MT.Length < ActiveSubUnit)
                throw new InvalidOperationException(string.Format("Invalid tape {0}", ActiveSubUnit));
            if (MT[ActiveSubUnit - 1] == null && Config.MTn[ActiveSubUnit - 1] != null)
            {
                MT[ActiveSubUnit - 1] = new Tape(ActiveSubUnit);
                MT[ActiveSubUnit - 1].MountTape(Config.MTn[ActiveSubUnit - 1]);
            }
            if (MT[ActiveSubUnit - 1] == null)
                throw new InvalidOperationException(string.Format("Invalid tape {0}", ActiveSubUnit));
        }
        static void opendrum(uint ActiveSubUnit)
        {
            if (DRM == null)
            {
                if (Config.DRMn == null)
                    throw new InvalidOperationException(string.Format("Invalid drum {0} ", ActiveSubUnit));
                DRM = new Drum[Config.DRMn.Length];
            }
            if (DRM.Length < ActiveSubUnit)
                throw new InvalidOperationException(string.Format("Invalid drum {0}", ActiveSubUnit));
            if (DRM[ActiveSubUnit - 1] == null && Config.DRMn[ActiveSubUnit - 1] != null)
            {
                DRM[ActiveSubUnit - 1] = new Drum(ActiveSubUnit);
                DRM[ActiveSubUnit - 1].MountDrum(Config.DRMn[ActiveSubUnit - 1]);
            }
            if (DRM[ActiveSubUnit - 1] == null)
                throw new InvalidOperationException(string.Format("Invalid drum {0}", ActiveSubUnit));
        }
        static void opencardreader()
        {
            if (CRD == null && Config.CRDn != null)
            {
                CRD = new CardReader();
                CRD.MountDeck(Config.CRDn);
            }
            if (CRD == null)
                throw new InvalidOperationException("Invalid cardreader selected for read");
        }
        static void openprinter()
        {
            if (PRT == null && Config.PRTn != null)
            {
                PRT = new Printer();
                PRT.MountPaper(Config.PRTn);
            }
            if (PRT == null)
                throw new InvalidOperationException("Invalid printer selected for write");
        }
        public static void RDS(uint unit) /* Read Select */
        {

            if (currdev != null)
                currdev.Disconnect();
            currdev = null;
            currdrum = -1;

            uint CurrUnit = unit / 16;
            uint CurrSubUnit = unit - CurrUnit * 16;

            switch (CurrUnit)
            {
                case 8: /* BCD Tape */
                case 9: /* Bin Tape */
                    if (CurrSubUnit < 1 || CurrSubUnit > 10)
                        throw new InvalidOperationException(string.Format("Invalid tape {0} selected for read", CurrSubUnit));
                    opentape(CurrSubUnit);
                    MT[CurrSubUnit - 1].RDS(CurrUnit == 9);
                    currdev = MT[CurrSubUnit - 1];
                    break;
                case 12: /* Drum */
                    if (CurrSubUnit < 1 || CurrSubUnit > 8)
                        throw new InvalidOperationException(string.Format("Invalid drum {0} selected for read", CurrSubUnit));
                    opendrum(CurrSubUnit);
                    DRM[CurrSubUnit - 1].RDS();
                    currdev = DRM[CurrSubUnit - 1];
                    currdrum = (int)CurrSubUnit - 1;
                    break;
                case 13: /* Card Reader */
                    if (CurrSubUnit != 1)
                        throw new InvalidOperationException(string.Format("Invalid Cardreader {0} selected for read", CurrSubUnit));
                    opencardreader();
                    CRD.RDS();
                    currdev = CRD;
                    break;
                case 15: /* Printer */
                    if (CurrSubUnit != 1)
                        throw new InvalidOperationException(string.Format("Invalid Printer {0} selected for read", CurrSubUnit));
                    openprinter();
                    PRT.RDS();
                    currdev = PRT;
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

            uint CurrUnit = unit / 16;
            uint CurrSubUnit = unit - CurrUnit * 16;

            switch (CurrUnit)
            {
                case 1: /* CRT */
                    if (CurrSubUnit != 8)
                        throw new InvalidOperationException(string.Format("Invalid device %d selected for write", unit));
                    throw new NotImplementedException("CRT");
                case 8: /* BCD Tape */
                case 9: /* Bin Tape */
                    if (CurrSubUnit < 1 || CurrSubUnit > 10)
                        throw new InvalidOperationException(string.Format("Invalid tape %d selected for write", CurrSubUnit));
                    opentape(CurrSubUnit);
                    MT[CurrSubUnit - 1].WRS(CurrUnit == 9);
                    currdev = MT[CurrSubUnit - 1];
                    break;
                case 12: /* Drum */
                    if (CurrSubUnit < 1 || CurrSubUnit > 8)
                        throw new InvalidOperationException(string.Format("Invalid drum {0} selected for read", CurrSubUnit));
                    opendrum(CurrSubUnit);
                    DRM[CurrSubUnit - 1].WRS();
                    currdev = DRM[CurrSubUnit - 1];
                    currdrum = (int)CurrSubUnit - 1;
                    break;
                case 13: /* Sim Tape */
                    if (CurrSubUnit == 11) /* IOD */
                        return;
                    else if (CurrSubUnit < 1 || CurrSubUnit > 10)
                        throw new InvalidOperationException(string.Format("Invalid tape %d selected for sim write", CurrSubUnit));
                    throw new NotImplementedException("Sim Tape");
                case 14: /* Card Punch */
                    if (CurrSubUnit != 1)
                        throw new InvalidOperationException(string.Format("Invalid device %d selected for write", unit));
                    throw new NotImplementedException("Card Punch");
                case 15: /* Printer */
                    if (CurrSubUnit != 1)
                        throw new InvalidOperationException(string.Format("Invalid device %d selected for write", unit));
                    openprinter();
                    PRT.WRS();
                    currdev = PRT;
                    break;
                default:
                    throw new InvalidOperationException(string.Format("Invalid device %d selected for write", unit));
            }
        }
        public static void BST(uint unit) /* Backspace Tape */
        {
            uint CurrUnit = unit / 16;
            uint CurrSubUnit = unit - CurrUnit * 16;
            if (currdev != null)
                currdev.Disconnect();
            currdev = null;
            if ((CurrUnit < 8) || (CurrUnit > 9) || (CurrSubUnit < 1 || CurrSubUnit > 10))
                throw new InvalidOperationException("invalid tape for BST");
            opentape(CurrSubUnit);
            MT[CurrSubUnit - 1].BST();
        }
        public static void WEF(uint unit) /* Write End of File */
        {
            uint CurrUnit = unit / 16;
            uint CurrSubUnit = unit - CurrUnit * 16;
            if (currdev != null)
                currdev.Disconnect();
            currdev = null;
            if ((CurrUnit < 8) || (CurrUnit > 9) || (CurrSubUnit < 1 || CurrSubUnit > 10))
                throw new InvalidOperationException("invalid tape for WEF");
            opentape(CurrSubUnit);
            MT[CurrSubUnit - 1].WEF();
        }
        public static void REW(uint unit) /* Rewind */
        {
            uint CurrUnit = unit / 16;
            uint CurrSubUnit = unit - CurrUnit * 16;
            if (currdev != null)
                currdev.Disconnect();
            currdev = null;
            if ((CurrUnit < 8) || (CurrUnit > 9) || (CurrSubUnit < 1 || CurrSubUnit > 10))
                throw new InvalidOperationException("invalid tape for Rewind");
            opentape(CurrSubUnit);
            MT[CurrSubUnit - 1].REW();
        }
        public static uint ETT() /* End of Tape Test */
        {
            throw new NotImplementedException("ETT");
        }
        public static void LDA(uint address) /* Locate Drum Address */
        {
            if (currdev==null||currdrum == -1)
                throw new InvalidOperationException("LDA not after Drum RDS/WRS");
            DRM[currdrum].LDA(address);
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
        public static int PSE(uint unit) /* plus sense */
        {
            int skip = 0;
            uint CurrUnit = unit / 16;
            uint CurrSubUnit = unit - CurrUnit * 16;
            switch (CurrUnit)
            {
                case 6: /* Sense Lights */
                    if (CurrSubUnit == 0)
                        SenseLights.Off();
                    else if (CurrSubUnit >= 1 && CurrSubUnit <= 4)
                        SenseLights.On(CurrSubUnit);
                    else
                        throw new InvalidOperationException("invalid Sense Light");
                    break;
                case 7: /* Sense Switches */
                    if (CurrSubUnit >= 1 && CurrSubUnit <= 6)
                        skip = SenseSwitches.Test(CurrSubUnit);
                    else
                        throw new InvalidOperationException("invalid Sense Light");
                    break;
                case 14: /* Punch */
                    throw new NotImplementedException("Punch");
                case 15: /* Printer */
                    throw new NotImplementedException("Printer");
                default:
                    throw new InvalidOperationException("invalid device");
            }
            return skip;
        }
        public static int MSE(uint unit) /* minus sense */
        {
            int skip = 0;

            uint CurrUnit = unit / 16;
            uint CurrSubUnit = unit - CurrUnit * 16;
            switch (CurrUnit)
            {
                case 6: /* Sense Lights */
                    if (CurrSubUnit >= 1 && CurrSubUnit <= 4)
                        skip = SenseLights.Test(CurrSubUnit);
                    else
                        throw new InvalidOperationException("invalid Sense Light");
                    break;
                default:
                    throw new InvalidOperationException("invalid device");

            }
            return skip;
        }

        public static void OnProcessExit(object sender, EventArgs e)
        {
            if (currdev != null)
                currdev.Disconnect();
            if (CRD != null)
                CRD.Dispose();
            if (MT != null)
                for (int i = 0; i < MT.Length; i++)
                    if (MT[i] != null)
                        MT[i].Dispose();
            if (DRM != null)
                for (int i = 0; i < DRM.Length; i++)
                    if (DRM[i] != null)
                        DRM[i].Dispose();
            if (PRT != null)
                PRT.Dispose();
            Console.WriteLine("finished");
        }
    }
}
