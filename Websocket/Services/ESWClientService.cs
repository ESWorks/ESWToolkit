using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using ESWToolbox.Utility;
using ESWToolbox.Websocket.Clients;
using ESWToolbox.Websocket.Objects;

namespace ESWToolbox.Websocket.Services
{
    public class ESWClientService
    {
        private readonly WorksClient _worksClient;
        public int Port { get; }
        private bool ServerAuthMode = false;
        private bool UserAuthored = false;
        private Socket ClientSocket; //The main client socket
        private byte[] ByteData = new byte[1024];

        public Socket Client => ClientSocket;

        public ESWClientService(int port, WorksClient worksClient)
        {
            _worksClient = worksClient;
            Port = port;
        }

        public void ConnectBlockingCall(string server, int timeout)
        {
            ConnectToServer(server);
            
            while (true)
            {
                bool connected = RunConnectionWait(timeout);
                _worksClient.ReportStatus(connected);
                Thread.Sleep(1000);
            }
        }

        public bool RunConnectionWait(int timeout)
        {
            int milliseconds = 0;
            while (!ClientSocket.Connected && milliseconds < timeout)
            {
                Thread.Sleep(10);
                milliseconds += 10;
                Console.Write(milliseconds % 1000 == 0 ? "." : "");
            }
            Console.WriteLine();
            return ClientSocket.Connected;
        }

        public bool Status()
        {
            return true;
        }
        internal void ConnectToServer(string serverIp)
        {
            try
            {
                ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ipAddress = IPAddress.Parse(serverIp);
                //Server is listening on port 1000
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, Port);
                Console.WriteLine("CLIENT --> BEGINNING CONNECT");
                //Connect to the server
                ClientSocket.BeginConnect(ipEndPoint, new AsyncCallback(OnConnect), null);

            }
            catch (ObjectDisposedException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        internal void OnConnect(IAsyncResult ar)
        {
            try
            {
                Console.WriteLine("CLIENT --> CONNECTED");
                ClientSocket.EndConnect(ar);
            }
            catch (ObjectDisposedException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        internal void Send(WorksPacket packet)
        {
            try
            {
                ByteData = Converter.ObjectToByteArray(packet);
                if (ServerAuthMode)
                {
                    if (UserAuthored)
                    {
                        ClientSocket.BeginSend(ByteData, 0, ByteData.Length, SocketFlags.None, OnSend, null);
                    }
                }
                else
                {
                    //Send it to the server
                    ClientSocket.BeginSend(ByteData, 0, ByteData.Length, SocketFlags.None, OnSend, null);
                }
                

            }
            catch (Exception)
            {
                MessageBox.Show("Unable to send message to the server.", "Client TCP", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        internal  void OnSend(IAsyncResult ar)
        {
            try
            {
                Console.WriteLine("CLIENT --> Sending Cork {" + ((IPEndPoint)ClientSocket.RemoteEndPoint).Address + "}");
                ClientSocket.EndSend(ar);
            }
            catch (ObjectDisposedException)
            { }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        internal void OnReceive(IAsyncResult ar)
        {
            try
            {
                Console.WriteLine("CLIENT --> RECEIVING");
                ClientSocket.EndReceive(ar);

                WorksPacket msgReceived = (WorksPacket) Converter.ByteArrayToObject(ByteData);

                Console.WriteLine("CLIENT --> {" + msgReceived.Header + "}");
                
                ByteData = new byte[1024];

                if (msgReceived.Header == "AUTHMODE")
                {
                    ServerAuthMode = (string) msgReceived.Payload == "AUTHORIZED";
                    if (ServerAuthMode)
                    {
                        _worksClient.SendCredentials();
                    }
                }
                else
                {
                    if (ServerAuthMode)
                    {
                        if (msgReceived.Header == "AUTH")
                        {
                            UserAuthored = (string) msgReceived.Payload == "TRUE";
                        }
                        else if (msgReceived.Header == "UPDT")
                        {
                            if ((string) msgReceived.Payload == "TRUE")
                            {
                                _worksClient.UpdatePassword();
                            }
                        }
                        else if (msgReceived.Header == "DISC")
                        {
                            UserAuthored = (string)msgReceived.Payload != "TRUE";
                        }
                        else
                        {
                            if (UserAuthored)
                            {
                                _worksClient.ReceivedPackets(_worksClient, msgReceived);
                            }
                        }
                    }
                    else
                    {
                        _worksClient.ReceivedPackets(_worksClient, msgReceived);
                    }
                }
                ClientSocket.BeginReceive(ByteData,
                    0,
                    ByteData.Length,
                    SocketFlags.None,
                    new AsyncCallback(OnReceive),
                    null);

            }
            catch (ObjectDisposedException)
            { }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Client TCP: ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
