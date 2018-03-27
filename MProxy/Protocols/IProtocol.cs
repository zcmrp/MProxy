using MProxy.Data;
using MProxy.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MProxy.Protocols
{
    interface IProtocol<T> where T: Octets
    {
        Octets Marshal();
        T Unmarshal();
        T Clone();
        bool SizePolicy();
        void Process();
    }
}
