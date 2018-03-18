using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Sim704
{
    public class Config704
    {
        public string[] MTn;
        public string CRDn;
        public bool[] Switch;
        public int MemSize;
    }
    interface I704dev
    {
        int CPY(ref ulong d);
        void Disconnect();
    }
    static class Io704
    {
        /* Device numbers*/

        static uint ActiveUnit;
        static uint ActiveSubUnit;
        public static bool tapecheck;
        static CardReader CRD = null;
        static Tape[] MT = null;
        static I704dev currdev = null;
        static Config704 Config = null;
        static Io704()
        {
            XmlSerializer SerializerObj = new XmlSerializer(typeof(Config704));
            FileStream ReadFileStream = new FileStream(@"c:\temp\test.xml", FileMode.Open, FileAccess.Read, FileShare.Read);
            Config = (Config704)SerializerObj.Deserialize(ReadFileStream);
            ReadFileStream.Close();
            SenseSwitches.Init(Config.Switch);
            CoreMemory.Init(Config.MemSize);
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

        public static void RDS(uint unit) /* Read Select */
        {
            if (currdev != null)
                currdev.Disconnect();
            currdev = null;
            ActiveUnit = unit / 16;
            ActiveSubUnit = unit - ActiveUnit * 16;
            switch (ActiveUnit)
            {
                case 8: /* BCD Tape */
                case 9: /* Bin Tape */
                    if (ActiveSubUnit < 1 || ActiveSubUnit > 10)
                        throw new InvalidOperationException(string.Format("Invalid tape %d selected for read", ActiveSubUnit));
                    opentape(ActiveSubUnit);
                    MT[ActiveSubUnit - 1].RDS(ActiveUnit == 9);
                    currdev = MT[ActiveSubUnit - 1];
                    break;
                case 12: /* Drum */
                    if (ActiveSubUnit < 1 || ActiveSubUnit > 8)
                        throw new InvalidOperationException(string.Format("Invalid drum %d selected for read", ActiveSubUnit));
                    throw new NotImplementedException("Drum");
                case 13: /* Card Reader */
                    if (ActiveSubUnit != 1)
                        throw new InvalidOperationException(string.Format("Invalid device %d selected for read", unit));
                    if (CRD == null && Config.CRDn != null)
                    {
                        CRD = new CardReader();
                        CRD.MountDeck(Config.CRDn);
                    }
                    if (CRD == null)
                        throw new InvalidOperationException("Invalid cardreader selected for read");
                    CRD.RDS();
                    currdev = CRD;
                    break;
                case 15: /* Printer */
                    if (ActiveSubUnit != 1)
                        throw new InvalidOperationException(string.Format("Invalid device %d selected for read", unit));
                    throw new NotImplementedException("Printer");
                default:
                    throw new InvalidOperationException(string.Format("Invalid device %d selected for read", unit));
            }
        }
        public static void WRS(uint unit) /* Write Select */
        {
            if (currdev != null)
                currdev.Disconnect();
            currdev = null;

            ActiveUnit = unit / 16;
            ActiveSubUnit = unit - ActiveUnit * 16;

            switch (ActiveUnit)
            {
                case 1: /* CRT */
                    if (ActiveSubUnit != 8)
                        throw new InvalidOperationException(string.Format("Invalid device %d selected for write", unit));
                    throw new NotImplementedException("CRT");
                case 8: /* BCD Tape */
                case 9: /* Bin Tape */
                    if (ActiveSubUnit < 1 || ActiveSubUnit > 10)
                        throw new InvalidOperationException(string.Format("Invalid tape %d selected for write", ActiveSubUnit));
                    opentape(ActiveSubUnit);
                    MT[ActiveSubUnit - 1].WRS(ActiveUnit == 9);
                    currdev = MT[ActiveSubUnit - 1];
                    break;
                case 12: /* Drum */
                    if (ActiveSubUnit < 1 || ActiveSubUnit > 8)
                        throw new InvalidOperationException(string.Format("Invalid drum %d selected for write", ActiveSubUnit));
                    throw new NotImplementedException("Drum");
                case 13: /* Sim Tape */
                    if (ActiveSubUnit == 11) /* IOD */
                        return;
                    else if (ActiveSubUnit < 1 || ActiveSubUnit > 10)
                        throw new InvalidOperationException(string.Format("Invalid tape %d selected for sim write", ActiveSubUnit));
                    throw new NotImplementedException("Sim Tape");
                case 14: /* Card Punch */
                    if (ActiveSubUnit != 1)
                        throw new InvalidOperationException(string.Format("Invalid device %d selected for write", unit));
                    throw new NotImplementedException("Card Punch");
                case 15: /* Printer */
                    if (ActiveSubUnit != 1)
                        throw new InvalidOperationException(string.Format("Invalid device %d selected for write", unit));
                    throw new NotImplementedException("Printer");
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
        public static bool ETT() /* End of Tape Test */
        {
            throw new NotImplementedException("ETT");
        }
        public static void LDA(uint address) /* Locate Drum Address */
        {
            throw new NotImplementedException("LDA");
        }
        public static int CPY(ref ulong data) /* Copy and skip */
        {
            if (currdev == null)
                throw new InvalidOperationException("CPY while no device selected");
            return currdev.CPY(ref data);
        }
        public static int RTT() /* Redundancy Tape Test */
        {
            int ret = tapecheck ? 0 : 1;
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
            Console.WriteLine("finished");
        }
    }
}
