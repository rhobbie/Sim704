using System;

namespace Sim704
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("Sim704 Version 0.2");
                Console.Error.WriteLine("Usage: Sim704 config.xml");
                return;
            }
            Bootdev boot=Io704.Init(args[0]);
            
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
