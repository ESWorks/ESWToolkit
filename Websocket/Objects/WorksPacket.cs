using System;

namespace ESWToolbox.Websocket.Objects
{
    [Serializable]
    public class WorksPacket
    {
        public string Header;
        public string ClientName;
        public object Payload;
    }
}
