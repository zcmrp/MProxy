using MProxy.Data;
using MProxy.Protocols;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MProxy.Net.Proxies
{
    class ProxyOperative
    {
        private ConcurrentQueue<Octets> StreamRecvCli, StreamRecvSrv;
        private ConcurrentQueue<Protocol> StreamSendCli, StreamSendSrv;
        private bool CliSendWork, SrvSendWork;
        private bool CliRecvWork, SrvRecvWork;
        private Thread T1, T2, T3, T4;
        private ManualResetEvent CliSendRE, SrvSendRE;
        private ManualResetEvent CliRecvRE, SrvRecvRE;
        private Proxy Parent;
        public ProxyOperative(Proxy parent) 
        {
            Parent = parent;
            StreamRecvCli = new ConcurrentQueue<Octets>();
            StreamRecvSrv = new ConcurrentQueue<Octets>();
            StreamSendCli = new ConcurrentQueue<Protocol>();
            StreamSendSrv = new ConcurrentQueue<Protocol>();
            CliSendRE = new ManualResetEvent(false);
            CliRecvRE = new ManualResetEvent(false);
            SrvSendRE = new ManualResetEvent(false);
            SrvRecvRE = new ManualResetEvent(false);
            T1 = new Thread(() => ClientSendWork());
            T2 = new Thread(() => ServerSendWork());
            T3 = new Thread(() => ClientRecvWork());
            T4 = new Thread(() => ServerRecvWork());
            T1.IsBackground = true;
            T2.IsBackground = true;
            T3.IsBackground = true;
            T4.IsBackground = true;
        }

        public void Run()
        {
            CliSendWork = SrvSendWork = false;
            CliRecvWork = SrvRecvWork = false;
            T1.Start();
            T2.Start();
            T3.Start();
            T4.Start();
        }
        private void ClientRecvWork()
        {
            Octets os, last = null;
            while (true)
            {
                CliRecvRE.WaitOne();
                lock (CliRecvRE)
                    CliRecvRE.Reset();
                if (StreamRecvCli.Count == 0) break;
                while (true)
                {
                    if (!StreamRecvCli.TryDequeue(out os))
                        break;
                    if (last != null)
                    {
                        last.Write(os.RawData());
                        os = last;
                    }
                    os.Reset();
                    while (os.CanRead())
                        try
                        {
                            Protocol proto = Parent.DecodeCli(os);
                            last = null;
                            StreamSendSrv.Enqueue(proto);
                            lock (SrvSendRE)
                            {
                                if (!SrvSendWork)
                                {
                                    SrvSendWork = true;
                                    SrvSendRE.Set();
                                }
                            }
                        }
                        catch (ProtocolSizeExceedException)
                        {
                            os.ReadToEnd();
                            last = os;
                            continue;
                        }
                        catch (Exception)
                        {
                            Parent.Disconnect();
                        }
                }
                CliRecvWork = false;
            }
        }
        private void ServerRecvWork()
        {
            Octets os, last = null;
            while (true)
            {
                SrvRecvRE.WaitOne();
                lock(SrvRecvRE)
                    SrvRecvRE.Reset();
                if (StreamRecvSrv.Count == 0) break;
                while (true)
                {
                    if (!StreamRecvSrv.TryDequeue(out os))
                        break;
                    if (last != null)
                    {
                        last.Write(os.RawData());
                        os = last;
                    }
                    os.Reset();
                    while (os.CanRead())
                        try
                        {
                            Protocol proto = Parent.DecodeSrv(os);
                            last = null;
                            StreamSendCli.Enqueue(proto);
                            lock (CliSendRE)
                            {
                                if (!CliSendWork)
                                {
                                    CliSendWork = true;
                                    CliSendRE.Set();
                                }
                            }
                        }
                        catch (ProtocolSizeExceedException)
                        {
                            os.ReadToEnd();
                            last = os;
                            continue;
                        }
                        catch (Exception)
                        {
                            Parent.Disconnect();
                        }
                }
                SrvRecvWork = false;
            }
        }
        private void ClientSendWork()
        {
            Protocol p;
            while (true)
            {
                CliSendRE.WaitOne();
                lock (CliSendRE)
                    CliSendRE.Reset();
                if (StreamSendCli.Count == 0) break;
                while (true)
                {
                    if (!StreamSendCli.TryDequeue(out p))
                        break;
                    p.Process();
                }
                CliSendWork = false;
            }
        }
        private void ServerSendWork()
        {
            Protocol p;
            while (true)
            {
                SrvSendRE.WaitOne();
                lock (SrvSendRE)
                    SrvSendRE.Reset();
                if (StreamSendSrv.Count == 0) break;
                while (true)
                {
                    if (!StreamSendSrv.TryDequeue(out p))
                        break;
                    p.Process();
                }
                SrvSendWork = false;
            }
        }

        public void EnqueueCli(Octets os)
        {
            StreamRecvCli.Enqueue(os);
            lock (CliRecvRE)
            {
                if (!CliRecvWork)
                {
                    CliRecvWork = true;
                    CliRecvRE.Set();
                }
            }
        }

        public void EnqueueSrv(Octets os)
        {
            StreamRecvSrv.Enqueue(os);
            lock (SrvRecvRE)
            {
                if (!SrvRecvWork)
                {
                    SrvRecvWork = true;
                    SrvRecvRE.Set();
                }
            }
        }

        public void Abort()
        {
            CliSendRE.Set();
            CliRecvRE.Set();
            SrvSendRE.Set();
            SrvRecvRE.Set();
        }
    }
}
