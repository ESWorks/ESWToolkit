using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ESWToolbox.WCF
{
    public class BasicWCFServer<T> : WCFServer<T>
    {
        public BasicWCFServer(int port) : base(port)
        {
        }

        public BasicWCFServer(int port, NetTcpBinding binding) : base(port, binding)
        {
        }

        public BasicWCFServer(int port, Type @interface, NetTcpBinding binding) : base(port, @interface, binding)
        {
        }

        public BasicWCFServer(int port, Type @interface) : base(port, @interface)
        {
        }

        public BasicWCFServer(int port, string servicesName, Type @interface, NetTcpBinding binding) : base(port, servicesName, @interface, binding)
        {
        }

        public override void ServiceHost_Closing(object sender, EventArgs e)
        {
            Console.WriteLine("Service Closing...");
        }

        public override void ServiceHost_Closed(object sender, EventArgs e)
        {
            Console.WriteLine("Service Closed...");
        }

        public override void ServiceHost_UnknownMessageReceived(object sender, EventArgs e)
        {
            Console.WriteLine("Service received foreign information...");
        }

    }
}
