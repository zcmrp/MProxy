using MProxy.Setup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MProxy
{
    [XmlType("Setup")]
    public class ProxySetupXml
    {
        [XmlElementAttribute("DeliveryServerAddr")]
        public string DeliveryServerAddr { get; set; }
        [XmlElementAttribute("LinkProxyAddr")]
        public string LinkProxyAddr { get; set; }

        [XmlArray("Proxies")]
        [XmlArrayItem("Proxy", Type = typeof(ProxyXml))]
        public ProxyXml[] Proxies;
    }
}
