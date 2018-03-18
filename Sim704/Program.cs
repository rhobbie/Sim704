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
        static void WriteFileConfig(Config704 config,string path)
        {
            XmlSerializer SerializerObj = new XmlSerializer(typeof(Config704));
            TextWriter WriteFileStream = new StreamWriter(path);
            SerializerObj.Serialize(WriteFileStream, config);
            WriteFileStream.Close();
        }

   
        static void Main(string[] args)
        {

            AppDomain.CurrentDomain.ProcessExit += new EventHandler(Io704.OnProcessExit);

            CPU704.LoadCrd();
        }
    }
}
