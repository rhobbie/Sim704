using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;
namespace Sim704
{




    class Program
    {
        



        static void Main(string[] args)
        {
            if (args.Length != 1)
            { 
                Console.WriteLine("Usage Sim704 config.xml");
                return;
            }
            Bootdev boot=Io704.Init(args[0]);
            //Io704.WriteFileConfig(@"C:\temp\t.xml");            
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(Io704.OnProcessExit);
            
            switch (boot)
            {
                case Bootdev.CRD:
                    CPU704.LoadCrd();
                    break;
                case Bootdev.DR:
                    CPU704.LoadDrm();
                    break;
                case Bootdev.MT:
                    CPU704.LoadTape();
                    break;
            }

        }
    }
}
