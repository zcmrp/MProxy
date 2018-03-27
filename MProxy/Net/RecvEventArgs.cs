using MProxy.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MProxy.Net
{
    class RecvEventArgs : EventArgs
    {
        public Octets Data { get; private set; }
        public Octets Res { get; set; }
        public RecvEventArgs(Octets os)
        {
            this.Data = os;
            this.Data.Reset();
        }
    }
}
