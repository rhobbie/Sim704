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
        int CPY(ref long d);
        void Disconnect();
    }
    static class Io704
    {
        /* Device numbers*/

        static int ActiveUnit;
        static int ActiveSubUnit;
        public static bool tapecheck;        
        static CardReader CRD = null;        
        static Tape[] MT = null;
        static I704dev currdev = null;
        static Config704 Config=null;
        static Io704()
        {
            XmlSerializer SerializerObj = new XmlSerializer(typeof(Config704));
            FileStream ReadFileStream = new FileStream(@"c:\temp\test.xml", FileMode.Open, FileAccess.Read, FileShare.Read);
            Config = (Config704)SerializerObj.Deserialize(ReadFileStream);
            ReadFileStream.Close();
            SenseSwitches.Init(Config.Switch);
            CoreMemory.Init(Config.MemSize);
        }
        public static void RDS(int unit) /* Read Select */
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
                    if (MT == null)
                    {
                        if (Config.MTn == null)
                            throw new InvalidOperationException(string.Format("Invalid tape %d  selected for read", ActiveSubUnit));
                        MT = new Tape[Config.MTn.Length];
                    }
                    if (MT.Length < ActiveSubUnit)
                        throw new InvalidOperationException(string.Format("Invalid tape %d  selected for read", ActiveSubUnit));
                    if (MT[ActiveSubUnit - 1] == null && Config.MTn[ActiveSubUnit - 1] != null)
                    {
                        MT[ActiveSubUnit - 1] = new Tape(ActiveSubUnit);
                        MT[ActiveSubUnit - 1].MountTape(Config.MTn[ActiveSubUnit - 1]);
                    }
                    if (MT[ActiveSubUnit - 1] == null)
                        throw new InvalidOperationException(string.Format("Invalid tape %d selected for read", ActiveSubUnit));
                    MT[ActiveSubUnit].RDS(ActiveUnit == 9);
                    currdev = MT[ActiveSubUnit];
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
        public static void WRS(int unit) /* Write Select */
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
                    if (MT == null)
                    {
                        if (Config.MTn == null)
                            throw new InvalidOperationException(string.Format("Invalid tape %d  selected for write", ActiveSubUnit));
                        MT = new Tape[Config.MTn.Length];
                    }
                    if (MT.Length < ActiveSubUnit)
                        throw new InvalidOperationException(string.Format("Invalid tape %d  selected for write", ActiveSubUnit));
                    if (MT[ActiveSubUnit - 1] == null && Config.MTn[ActiveSubUnit - 1] != null)
                    {
                        MT[ActiveSubUnit - 1] = new Tape(ActiveSubUnit);
                        MT[ActiveSubUnit - 1].MountTape(Config.MTn[ActiveSubUnit - 1]);
                    }
                    if (MT[ActiveSubUnit - 1] == null)
                        throw new InvalidOperationException(string.Format("Invalid tape %d selected for write", ActiveSubUnit));
                    MT[ActiveSubUnit].WRS(ActiveUnit == 9);
                    currdev = MT[ActiveSubUnit];
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
        public static void BST(int unit) /* Backspace Tape */
        {
            if (currdev != null)
                currdev.Disconnect();
            currdev = null;
            if (unit < 1 || unit > 10)
                throw new InvalidOperationException("invalid tape for backspace");
            MT[unit - 1].BST();
        }
        public static void WEF(int unit) /* Write End of File */
        {
            if (currdev != null)
                currdev.Disconnect();
            currdev = null;
            if (unit < 1 || unit > 10)
                throw new InvalidOperationException("invalid tape for Write End of File");
            MT[unit - 1].WEF();
        }
        public static void REW(int unit) /* Rewind */
        {
            if (currdev != null)
                currdev.Disconnect();
            currdev = null;
            if (unit < 1 || unit > 10)
                throw new InvalidOperationException("invalid tape for Rewind");
            MT[unit - 1].REW();
        }
        public static bool ETT() /* End of Tape Test */
        {
            throw new NotImplementedException("ETT");
        }
        public static void LDA(int address) /* Locate Drum Address */
        {
            throw new NotImplementedException("LDA");
        }
        public static int CPY(ref long data) /* Copy and skip */
        {
            if (currdev == null)
                throw new InvalidOperationException("CPY while no device selected");
            return currdev.CPY(ref data);
        }
        public static int RTT() /* Redundancy Tape Test */
        {
            int ret = tapecheck?1:0;
            tapecheck = false;
            return ret;
        }
        public static bool PSE(int unit) /* plus sense */
        {
            bool ret = false;
            int CurrUnit = unit / 16;
            int CurrSubUnit = unit - CurrUnit * 16;
            switch(CurrUnit)
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
                        ret=SenseSwitches.Test(CurrSubUnit);
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
            return ret;
        }
        public static bool MSE(int unit) /* minus sense */
        {
            int CurrUnit = unit / 16;
            int CurrSubUnit = unit - CurrUnit * 16;
            switch (CurrUnit)
            {
                case 6: /* Sense Lights */
                    if (CurrSubUnit >= 1 && CurrSubUnit <= 4)
                        return SenseLights.Test(CurrSubUnit);
                    else
                        throw new InvalidOperationException("invalid Sense Light");
                default:
                    throw new InvalidOperationException("invalid device");
                    
            }
        }

        /* Extended Operations */

        public static void WTV() /* Write CRT */
        {
            WRS(16+8); 
        }
        public static bool SLF() /* sense lights off */
        {
            return PSE(6*16);
        }
        public static bool SLN(int unit) /* sense light on */
        {
            if (unit < 1 || unit > 4)
                throw new InvalidOperationException("invalid sense light");
            return PSE(6*16+unit);
        }
        public static bool SLT(int unit) /* sense light test */
        {
            if (unit < 1 || unit > 4)
                throw new InvalidOperationException("invalid sense light");
            return MSE(6*16 + unit); ;
        }
        public static bool SWT(int unit) /* sense switch test */
        {
            if (unit < 1 || unit > 6)
                throw new InvalidOperationException("invalid sense Switch");
            return PSE(7*16+unit) ;
        }
        public static void RTD(int unit) /* Read tape - decimal  */
        {
            if (unit < 1 || unit > 10)
                throw new InvalidOperationException("invalid tape");
            RDS(8*16 + unit);
        }
        public static void WTD(int unit) /* Write tape - decimal  */
        {
            if (unit < 1 || unit > 10)
                throw new InvalidOperationException("invalid tape");
            WRS(8*16+ unit);
        }
        public static void RTB(int unit) /* Read tape - binary  */
        {
            if (unit < 1 || unit > 10)
                throw new InvalidOperationException("invalid tape");
            RDS(9 * 16 + unit);
        }
        public static void WTB(int unit) /* Write tape - binary  */
        {
            if (unit < 1 || unit > 10)
                throw new InvalidOperationException("invalid tape");
            WRS(9 * 16 + unit);
        }
        public static void RDR(int unit) /* Read Drum */
        {
            if (unit < 1 || unit > 8)
                throw new InvalidOperationException("invalid drum");
            RDS(12*16 + unit);
        }
        public static void WDR(int unit) /* Write Drum */
        {
            if (unit < 1 || unit > 8)
                throw new InvalidOperationException("invalid drum");
            WRS(12 * 16 + unit);
        }
        public static void RCD() /* Read Card Reader */
        {
            RDS(13*16+1);
        }
        public static void WTS(int unit) /* Write tape - simultaneously */
        {
            if (unit < 1 || unit > 5)
                throw new InvalidOperationException("invalid tape");
            WRS(13 * 16 + unit);
        }
        public static void IOD() /* Input-output delay */
        {
            WRS(13*16+11);
        }
        public static bool SPU(int unit) /* sense punch */
        {
            if (unit < 1 || unit > 2)
                throw new InvalidOperationException("invalid punch exit hub");
            return PSE(14 * 16 + unit);
        }
        public static void WPU() /* Write punch */
        {
            WRS(14*16+1);
        }
        public static bool SPT() /* sense printer test */
        {
            return PSE(15*16);
        }
        public static bool SPR(int unit) /* sense printer */
        {
            if (unit < 1 || unit > 10)
                throw new InvalidOperationException("invalid printer exit hub");
            return PSE(15 * 16 + unit);
        }
        public static void RPR() /* Read Printer */
        {
            RDS(15 * 16 + 1);
        }       
        public static void WPR() /* Write Printer */
        {
            WRS(15 * 16+1);
        }


        
        


        
        
 
        public static void OnProcessExit(object sender, EventArgs e)
        {
            if (currdev != null)
                currdev.Disconnect();
            if (CRD != null)
                CRD.Dispose();
            if(MT!=null)
                for (int i = 0; i < MT.Length; i++)
                    if (MT[i] != null)
                        MT[i].Dispose();
        }
    }
}
