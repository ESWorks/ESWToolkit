
using System;
using System.Drawing;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace ESWToolbox.WCF
{
    // ReSharper disable once InconsistentNaming
    public class WCFClient<T> : IDisposable
    {
        public readonly ServiceEndpoint Endpoint;
        public bool Registered => _cf?.State == CommunicationState.Opened;
        public T Service { get; private set; }

        private readonly DuplexChannelFactory<T> _cf;

        private bool _manualClose;

        protected WCFClient(CallbackHandler callback, string host, int port) : this(callback,host,port, typeof(T).Namespace, typeof(T).Name, typeof(T).FullName) { }

        protected WCFClient(CallbackHandler callback, string host, int port, NetTcpBinding tcp) : this(callback, host, port, typeof(T).Namespace, typeof(T).Name, typeof(T).FullName, tcp) { }

        protected WCFClient(CallbackHandler callback, string host, int port, string servicesName, string contractName, string nameSpace) : this (callback, host,port,servicesName,contractName,nameSpace, new NetTcpBinding()) { }

        protected WCFClient(object callback, string host, int port) : this(callback, host, port, typeof(T).Namespace, typeof(T).Name, typeof(T).FullName, new NetTcpBinding()) { }

        protected WCFClient(object callback, string host, int port, NetTcpBinding tcp) : this(callback, host, port, typeof(T).Namespace, typeof(T).Name, typeof(T).FullName, tcp) { }

        protected WCFClient(object callback, string host, int port, string servicesName, string contractName, string nameSpace, NetTcpBinding tcp)
        {
            EndpointAddress end = new EndpointAddress($@"net.tcp://{host}:{port}/{servicesName}");
            Endpoint = new ServiceEndpoint(new ContractDescription(contractName, nameSpace), tcp, end);
            _cf = new DuplexChannelFactory<T>(callback, tcp, end);
            _manualClose = false;
            _cf.Closed += Cf_Closed;
            _cf.Faulted += Cf_Faulted;
        }

        private void Cf_Faulted(object sender, EventArgs e)
        {
            Create();
        }

        private void Cf_Closed(object sender, EventArgs e)
        {
            if (!_manualClose)
            {
                Service = _cf.CreateChannel();
            }
        }

        public void Create()
        {
            Service = _cf.CreateChannel();
        }
       
        public void Reconnect()
        {

            Service = _cf.CreateChannel();
        }

        public void Dispose()
        {
            _manualClose = true;
            _cf.Close();
            Service = default;
            ((IDisposable) _cf)?.Dispose();
        }
    }
}
