using System;
using System.Collections.Generic;
using System.Net.Sockets;
using ESWToolbox.Utility;
using ESWToolbox.Websocket.Objects;
using ESWToolbox.Websocket.Services;

namespace ESWToolbox.Websocket.Clients
{
    public class WorksServer
    {
        private readonly ESWServerService _service;
        private readonly Dictionary<string, WorksSrvClient> _srvClients = new Dictionary<string, WorksSrvClient>();
        private readonly List<string> _srvAuthenticated = new List<string>();
        
        public WorksServer(int port = 46700, bool authenticate = false)
        {
            _service = new ESWServerService(port, authenticate, this);
        }

        public int Port => _service.Port;

        //Run in a seperate thread, Runs to the server and holds it.
        public void RunBlockingProcess()
        {
            _service.Listen();
            while (true)
            {
                if (!_service.Status())
                {

                }
            }
        }

        public void SendPacket(string clientname, string header, object payload)
        {
            WorksPacket packet = new WorksPacket() {ClientName = clientname, Header = header, Payload = payload};
            _service.SendClient(packet, _srvClients[clientname].ClientSocket);
            SentPackets(this, packet);
        }

        public void CreateClient(string clientname, string password, int permissions = 0)
        {
            string salt = Hash.StringHash(HashType.MD5, clientname);
            _srvClients[clientname] = new WorksSrvClient() {ClientName = clientname, Permissions = permissions, ClientToken = Hash.StringHash(HashType.SHA512, password, salt), ClientSalt = salt };
        }

        public void RegenerateToken(string clientname, string password)
        {
            if (_srvClients.ContainsKey(clientname))
            {
                _srvClients[clientname].ClientToken = Hash.StringHash(HashType.SHA512, password, _srvClients[clientname].ClientSalt);
            }
        }

        public bool CheckCredentials(string clientname, string password)
        {
            return _srvClients.ContainsKey(clientname) && _srvClients[clientname].ClientToken == Hash.StringHash(HashType.SHA512, password, _srvClients[clientname].ClientSalt);
        }

        public bool AuthenticateClientConnection(bool isConnecting, string clientname, string password)
        {
            if (!CheckCredentials(clientname, password)) return false;
            if (isConnecting)
            {
                if (_srvAuthenticated.Contains(clientname)) return false;
                _srvAuthenticated.Add(clientname);
                return true;
            }
            if (!_srvAuthenticated.Contains(clientname)) return false;
            _srvAuthenticated.Remove(clientname);
            return true;
        }

        public bool IsAuthenticated(string clientname)
        {
            return _srvAuthenticated.Contains(clientname);
        }

        //Event to handle received data
        public EventHandler<WorksPacket> ExReceivedPackets;

        //Additional Event when sending packets
        public EventHandler<WorksPacket> SentPackets;

        public void SetClientSocket(Socket clientSocket, string clientUsername)
        {
            _srvClients[clientUsername].ClientSocket = clientSocket;
        }

        public int GetPermission(string clientname)
        {
            return _srvClients[clientname].Permissions;
        }

        public void SetPermission(string clientname, int permission)
        {
            _srvClients[clientname].Permissions = permission;
        }
    }
}
