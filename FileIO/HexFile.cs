using System;
using System.IO;
using ESWToolbox.Utility;

namespace ESWToolbox.FileIO
{
    public static class HexFile
    {
        public static bool EncodeHStr(string file, string value)
        {
            try
            {
                File.WriteAllText(file,value);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public static object DecodeHStr(string value)
        {
            try
            {
                return Converter.ByteArrayToObject(Converter.StringToByteArray(value));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static bool EncodeObj(string file, object value)
        {
            try
            {
                return EncodeHStr(file, Converter.ByteArrayToHexString(Converter.ObjectToByteArray(value)));
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static object DecodeFile(string file)
        {
            try
            {
                return DecodeHStr(File.ReadAllText(file));
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
