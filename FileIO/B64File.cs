using System;
using System.IO;
using ESWToolbox.Utility;

namespace ESWToolbox.FileIO
{
    public static class B64File
    {
        public static bool EncodeFile(string location, object file)
        {
            try
            {
                File.WriteAllText(location, Convert.ToBase64String(Converter.ObjectToByteArray(file)));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static string GetFileString(string location)
        {
            return File.ReadAllText(location);
        }
        public static byte[] GetFileBytes(string location)
        {
            string file = File.ReadAllText(location);
            return Convert.FromBase64String(file);
        }
        public static object DecodeFile(string location)
        {
            string file = File.ReadAllText(location);
            byte[] bytes = Convert.FromBase64String(file);
            return Converter.ByteArrayToObject(bytes);
        }
    }
}
