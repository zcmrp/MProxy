using MProxy.Net;
using MProxy.Setup;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MProxy
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: ./lspi conf.xml");
                return;
            }
            XmlSerializer serializer = new XmlSerializer(typeof(ProxySetupXml));
            if (!File.Exists(args[0]))
            {
                Console.WriteLine("Configuration file not found");
            }
            using (StreamReader reader = new StreamReader(args[0]))
            {
                ProxySetupXml xml = (ProxySetupXml)serializer.Deserialize(reader);
                ProxySetup setup = new ProxySetup();
                if (!setup.Set(xml))
                {
                    Console.WriteLine("Bad format in file.");
                    return;
                }
                ProxyControl ctrl = new ProxyControl(setup);
                ctrl.Run();
            }
            while (true) Console.ReadLine();
        }
    }
}
