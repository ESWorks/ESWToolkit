using System;
using System.Net.Sockets;

namespace ESWToolbox.Websocket.Objects
{
    [Serializable]
    internal class WorksSrvClient
    {
        public Socket ClientSocket;
        public string ClientName;
        public string ClientToken;
        public int Permissions;
        public string ClientSalt;
    }
}
