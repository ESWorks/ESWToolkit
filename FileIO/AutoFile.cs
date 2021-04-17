using System;
using System.IO;

namespace ESWToolbox.FileIO
{
    public static class AutoFile
    {
        public static bool SaveFile(string file, object value, Extentions.Type type)
        {
            FileInfo info = new FileInfo(file);
            return SaveFile(value, info.Name, info.DirectoryName, $".{type}");
        }

        private static object ReadFile(string file, Extentions.Type type)
        {
            FileInfo info = new FileInfo(file);
            return ReadFile(info.Name, info.DirectoryName, $".{type}");
        }
        private static bool SaveFile(object value, string filename, string location, string extention)
        {
            try
            {
                
                switch (extention)
                {
                    case Extentions.HEX_EXT:
                        return HexFile.EncodeObj(Path.Combine(location, filename + extention), value);
                    case Extentions.B64_EXT:
                        return B64File.EncodeFile(Path.Combine(location, filename + extention), value);
                    case Extentions.BIN_EXT:
                        return BinFile.EncodeObj(Path.Combine(location, filename + extention), value);
                    case Extentions.BYTE_EXT:
                        return ByteFile.WriteByte(Path.Combine(location, filename + extention), value);
                    default:
                        return ByteFile.WriteByte(Path.Combine(location, filename + extention), value);
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static object ReadFile(string filename, string location, string extention)
        {
            try
            {

                switch (extention)
                {
                    case Extentions.HEX_EXT:
                        return HexFile.DecodeFile(Path.Combine(location, filename + extention));
                    case Extentions.B64_EXT:
                        return B64File.DecodeFile(Path.Combine(location, filename + extention));
                    case Extentions.BIN_EXT:
                        return BinFile.DecodeFile(Path.Combine(location, filename + extention));
                    case Extentions.BYTE_EXT:
                        return ByteFile.OpenByte(Path.Combine(location, filename + extention));
                    default:
                        return ByteFile.OpenByte(Path.Combine(location, filename + extention));
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
