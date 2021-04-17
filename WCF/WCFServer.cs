
using System;
using System.ServiceModel;
using ESWToolbox.FileIO;

namespace ESWToolbox.WCF
{
    // ReSharper disable once InconsistentNaming
    public abstract class WCFServer<T> : IDisposable
    {
        public ServiceHost ServiceHost { get; }

        protected WCFServer(int port) : this(port, typeof(T).Namespace, typeof(T).GetInterfaces()[0], new NetTcpBinding()) { }

        protected WCFServer(int port, NetTcpBinding binding) : this(port, typeof(T).Namespace, typeof(T).GetInterfaces()[0], binding) { }

        protected WCFServer(int port, Type @interface, NetTcpBinding binding) : this(port, typeof(T).Namespace, @interface, binding) { }

        protected WCFServer(int port, Type @interface) : this(port, typeof(T).Namespace, @interface, new NetTcpBinding()) { }

        protected WCFServer(int port, string servicesName,Type @interface, NetTcpBinding binding)
        {
            ServiceHost = new ServiceHost(typeof(T));
            ServiceHost.AddServiceEndpoint(@interface, binding, $@"net.tcp://localhost:{port}/{servicesName}");
            ServiceHost.UnknownMessageReceived += ServiceHost_UnknownMessageReceived;
            ServiceHost.Closed += ServiceHost_Closed;
            ServiceHost.Closing += ServiceHost_Closing;
            ServiceHost.Faulted += ServiceHost_Faulted;
        }

        private void ServiceHost_Faulted(object sender, EventArgs e)
        {
            Close();
            Open();
        }

        public void Open()
        {
            ServiceHost.Open();
            Console.WriteLine(String.Format("The WCF server is ready at localhost"));
            Console.WriteLine();
        }

        public void Close()
        {
            ServiceHost.Close();
            Console.WriteLine(String.Format("The WCF server is closed at localhost"));
            Console.WriteLine();
        }

        

        public abstract void ServiceHost_Closing(object sender, EventArgs e);
        public abstract void ServiceHost_Closed(object sender, EventArgs e);
        public abstract void ServiceHost_UnknownMessageReceived(object sender, EventArgs e);
        public bool Running => ServiceHost != null && ServiceHost.State == CommunicationState.Opened;

        public void Dispose()
        {
            ServiceHost.Close();
            ((IDisposable) ServiceHost)?.Dispose();
        }
    }
   
}
