using System;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using ESWToolbox.Utility;
using ESWToolbox.Websocket.Clients;
using ESWToolbox.Websocket.Objects;

namespace ESWToolbox.Websocket.Services
{
    public class ESWServerService
    {
        private readonly bool _authenticate;
        private readonly WorksServer _worksServer;
        private Socket _serverSocket;
        private byte[] _byteData = new byte[65535];
        public int Port { get; }

        public ESWServerService(int port, bool authenticate, WorksServer worksServer)
        {
            _authenticate = authenticate;
            _worksServer = worksServer;
            Port = port;
        }

        public void Listen()
        {
            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, Port);
            _serverSocket.Bind(localEndPoint);
            _serverSocket.Listen(100);
        }
        protected void OnAccept(IAsyncResult ar)
        {
            try
            {

                Socket clientSocket = _serverSocket.EndAccept(ar);
                //Start listening for more clients
                _serverSocket.BeginAccept(new AsyncCallback(OnAccept), null);

                
                
                //Once the client connects then start receiving the commands from her
                clientSocket.BeginReceive(_byteData, 0, _byteData.Length, SocketFlags.None,
                    new AsyncCallback(OnReceive), clientSocket);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        protected void OnReceive(IAsyncResult ar)
        {
            try
            {
                Socket clientSocket = (Socket)ar.AsyncState;
                Console.WriteLine("SERVER --> RECEIVING...");
                clientSocket.EndReceive(ar);
                //Transform the array of bytes received from the user into an
                //intelligent form of object Data
                WorksPacket packet = (WorksPacket) Converter.ByteArrayToObject(_byteData);

                _byteData = new byte[65535];
                bool authResult = true;

                if (packet.Header == "AUTHMODE")
                {
                    SendClient(new WorksPacket() { ClientName = "REQUESTED", Header = "AUTHMODE", Payload = _authenticate ? "AUTHORIZED" : "UNAUTHORIZED" }, clientSocket);
                }
                else if (_authenticate)
                {
                    UserAuth client = (UserAuth) packet.Payload;
                    if (packet.Header == "AUTH")
                    {
                        bool result = _worksServer.AuthenticateClientConnection(true, client.Username, client.Password);
                        SendClient(
                            result
                                ? new WorksPacket() { ClientName = packet.ClientName, Header = "AUTH", Payload = "TRUE" }
                                : new WorksPacket() { ClientName = packet.ClientName, Header = "AUTH", Payload = "FALSE" },
                            clientSocket);
                        if (result)
                        {
                            _worksServer.SetClientSocket(clientSocket, client.Username);
                        }
                    }
                    else if (packet.Header == "UPDT")
                    {
                        bool result = _worksServer.CheckCredentials(client.Username, client.Password);
                        if (result)
                        {
                            _worksServer.RegenerateToken(client.Username, client.NewPassword);

                        }
                        SendClient(
                            result
                                ? new WorksPacket() { ClientName = packet.ClientName, Header = "UPDT", Payload = "TRUE" }
                                : new WorksPacket() { ClientName = packet.ClientName, Header = "UPDT", Payload = "FALSE" },
                            clientSocket);
                    }
                    else if (packet.Header == "DISC")
                    {
                        bool result = _worksServer.AuthenticateClientConnection(false, client.Username, client.Password);
                        authResult = result;
                        SendClient(
                            result
                                ? new WorksPacket() { ClientName = packet.ClientName, Header = "DISC", Payload = "TRUE" }
                                : new WorksPacket() { ClientName = packet.ClientName, Header = "DISC", Payload = "FALSE" },
                            clientSocket);
                    }
                    else
                    {
                        if(_worksServer.IsAuthenticated(packet.ClientName))
                            _worksServer.ExReceivedPackets(clientSocket, packet);
                    }
                }
                else
                {
                    _worksServer.ExReceivedPackets(clientSocket, packet);
                }

                if (packet.Header != "DISC" || packet.Header == "DISC" && authResult)
                {
                    //Start listening to the message sent by the client
                    clientSocket.BeginReceive(_byteData, 0, _byteData.Length, SocketFlags.None,
                        new AsyncCallback(OnReceive), clientSocket);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"serverTCP", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void SendClient(WorksPacket packet, Socket Client)
        {
            //send to client
            byte[] seal = Converter.ObjectToByteArray(packet);
            Client.BeginSend(seal, 0, seal.Length,
                SocketFlags.None,
                new AsyncCallback(OnSend), Client);
        }

        protected void OnSend(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;
                Console.WriteLine(@"SERVER --> SENDING TO {" + ((IPEndPoint)client.LocalEndPoint).Address + @"}");
                client.EndSend(ar);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"serverTCP", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public bool Status()
        {
            return _serverSocket.IsBound;
        }
    }
}
