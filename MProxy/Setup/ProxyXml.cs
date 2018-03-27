using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MProxy.Setup
{
    [XmlType("Proxy")]
    public class ProxyXml
    {
        [XmlElementAttribute("UserProxyAddr")]
        public string UserProxyAddr { get; set; }
        [XmlElementAttribute("LinkServerAddr")]
        public string LinkServerAddr { get; set; }

        [XmlElementAttribute("GsProxyAddr")]
        public string GsProxyAddr { get; set; }
        [XmlElementAttribute("ProviderServerAddr")]
        public string ProviderServerAddr { get; set; }
    }
}
