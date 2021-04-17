using System;
using ESWToolbox.Websocket.Objects;
using ESWToolbox.Websocket.Services;

namespace ESWToolbox.Websocket.Clients
{
    public class WorksClient
    {
        private readonly string _username;
        private string _password;
        private object _tempObj;
        private readonly ESWClientService _service;

        public WorksClient(string username = "default", string password = "default", int port = 46700)
        {
            _username = username;
            _password = password;
            _service = new ESWClientService(port, this);
        }

        private void DefaultMethod(object sender, WorksPacket e)
        {
            //ignore
        }

        public int Port => _service.Port;

        //Run in a seperate thread, Connects to the server and holds it.
        public void RunBlockingProcess(string server = "127.0.0.1", int timeout = 5000)
            => _service.ConnectBlockingCall(server, timeout);

        public void SendPacket(WorksPacket packet)
        {

            _service.Send(packet);
            SentPackets(this, packet);
        }

        //Event to handle received data
        public EventHandler<WorksPacket> ReceivedPackets;
        //Event to handle received data
        public EventHandler<bool> ExternalStatus;
        //Addition Event when sending packets
        public EventHandler<WorksPacket> SentPackets;

        public void SendCredentials()
        {
            _service.Send(new WorksPacket() {ClientName = _username, Header = "AUTH", Payload = new UserAuth() {Username = _username , Password = _password} });
        }

        public void SendPasswordUpdate(string password)
        {
            _tempObj = password;
            _service.Send(new WorksPacket() {ClientName = _username, Header = "UPDT", Payload = new UserAuth() {NewPassword = password,Password = _password, Username = _username} });
        }

        public void ReportStatus(bool connected)
        {
            ExternalStatus(this, connected);
        }

        public void UpdatePassword()
        {
            _password = (string) _tempObj;
            _tempObj = null;
        }
    }
}
