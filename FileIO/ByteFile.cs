using System;
using System.IO;
using ESWToolbox.Utility;

namespace ESWToolbox.FileIO
{
    public static class ByteFile
    {
        public static object OpenByte(string file)
        {
            try
            {
                return Converter.ByteArrayToObject(File.ReadAllBytes(file));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static bool WriteByte(string file, object value)
        {
            try
            {
                File.WriteAllBytes(file,Converter.ObjectToByteArray(value));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
