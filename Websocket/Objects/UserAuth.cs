using System;

namespace ESWToolbox.Websocket.Objects
{
    [Serializable]
    public class UserAuth
    {
        public string Username;
        public string Password;
        public string NewPassword;
    }
}
