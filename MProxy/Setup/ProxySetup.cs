using MProxy.Setup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MProxy
{
    class ProxySetup
    {
        public IPAddress DeliveryServerHost;
        public ushort DeliveryServerPort;
        public IPAddress LinkProxyIP;
        public ushort LinkProxyPort;
        public List<Tuple<Tuple<IPAddress, ushort>, Tuple<IPAddress, ushort>, Tuple<IPAddress, ushort>, Tuple<IPAddress, ushort>>> Proxies;
        public ProxySetup() 
        {
            Proxies = new List<Tuple<Tuple<IPAddress, ushort>, Tuple<IPAddress, ushort>, Tuple<IPAddress, ushort>, Tuple<IPAddress, ushort>>>();
        }
        public bool Set(ProxySetupXml xml)
        {
            if(Decode(xml.DeliveryServerAddr, ref DeliveryServerHost, ref DeliveryServerPort) &&
                Decode(xml.LinkProxyAddr, ref LinkProxyIP, ref LinkProxyPort))
            {
                foreach (ProxyXml pxml in xml.Proxies)
                {
                    IPAddress ip1 = IPAddress.Any, ip2 = IPAddress.Any, ip3 = IPAddress.Any, ip4 = IPAddress.Any;
                    ushort port1 = 0, port2 = 0, port3 = 0, port4 = 0;
                    if (Decode(pxml.UserProxyAddr, ref ip1, ref port1) && Decode(pxml.LinkServerAddr, ref ip2, ref port2) && Decode(pxml.GsProxyAddr, ref ip3, ref port3) && Decode(pxml.ProviderServerAddr, ref ip4, ref port4))
                        Proxies.Add(new Tuple<Tuple<IPAddress, ushort>, Tuple<IPAddress, ushort>, Tuple<IPAddress, ushort>, Tuple<IPAddress, ushort>>(new Tuple<IPAddress, ushort>(ip1, port1), new Tuple<IPAddress, ushort>(ip2, port2), new Tuple<IPAddress, ushort>(ip3, port3), new Tuple<IPAddress, ushort>(ip4, port4)));
                    else
                        return false;
                }
                return true;
            }
            return false;
        }

        public bool Decode(string str, ref IPAddress ip, ref ushort port)
        {
            if (str == null) return false;
            string[] split = str.Split(new char[] { ':' });
            if (split.Length != 2) return false;
            return (IPAddress.TryParse(split[0], out ip) && UInt16.TryParse(split[1], out port)) || (IPAddress.TryParse(split[1], out ip) && UInt16.TryParse(split[0], out port));
        }
    }
}
