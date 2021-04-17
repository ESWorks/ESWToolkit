using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ESWToolbox.WCF
{
    public class BasicWCFClient<T> : WCFClient<T>
    {
        public BasicWCFClient(CallbackHandler callback, string host, int port) : base(callback, host, port)
        {
        }

        public BasicWCFClient(CallbackHandler callback, string host, int port, NetTcpBinding tcp) : base(callback, host, port, tcp)
        {
        }

        public BasicWCFClient(CallbackHandler callback, string host, int port, string servicesName, string contractName, string nameSpace) : base(callback, host, port, servicesName, contractName, nameSpace)
        {
        }

        public BasicWCFClient(object callback, string host, int port) : base(callback, host, port)
        {
        }

        public BasicWCFClient(object callback, string host, int port, NetTcpBinding tcp) : base(callback, host, port, tcp)
        {
        }

        public BasicWCFClient(object callback, string host, int port, string servicesName, string contractName, string nameSpace, NetTcpBinding tcp) : base(callback, host, port, servicesName, contractName, nameSpace, tcp)
        {
        }
    }
}
