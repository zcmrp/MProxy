using Mono.Unix;
using Mono.Unix.Native;
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

        private static UnixSignal[] Signals = new UnixSignal[] { 
            new UnixSignal(Signum.SIGINT), 
            new UnixSignal(Signum.SIGTERM), 
        };

        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: ");
                Console.WriteLine("Windows: MProxy.exe Conf.xml");
                Console.WriteLine("Linux: mono MProxy.exe Conf.xml");
                return;
            }
            XmlSerializer serializer = new XmlSerializer(typeof(ProxySetupXml));
            if (!File.Exists(args[0]))
            {
                Console.WriteLine("Configuration file not found");
            }
            ProxyControl ctrl = null;
            using (StreamReader reader = new StreamReader(args[0]))
            {
                ProxySetupXml xml = (ProxySetupXml)serializer.Deserialize(reader);
                ProxySetup setup = new ProxySetup();
                if (!setup.Set(xml))
                {
                    Console.WriteLine("Bad format in file");
                    return;
                }
                ctrl = new ProxyControl(setup);
                ctrl.Run();
            }
            while (true)
            {
                int id = UnixSignal.WaitAny(Signals);
                if (id >= 0 && id < Signals.Length)
                    return;
            }
        }
    }
}
