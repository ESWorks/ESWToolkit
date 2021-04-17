using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

// ReSharper disable InconsistentNaming

namespace ESWToolbox.Utility
{
    public enum HashType
    {
        MD5,
        SHA1,
        SHA256,
        SHA358,
        SHA512
    }
    public static class Hash
    {
        public static string GetBufferedStreamHash(HashType type, string file)
        {
            using (var stream = new BufferedStream(File.OpenRead(file), 1200000))
            {
                byte[] checksum;
                switch (type)
                {
                    case HashType.MD5:
                        checksum = MD5H(stream);
                        break;
                    case HashType.SHA1:
                        checksum = SHA1H(stream);
                        break;
                    case HashType.SHA256:
                        checksum = SHA256H(stream);
                        break;
                    case HashType.SHA358:
                        checksum = SHA358H(stream);
                        break;
                    case HashType.SHA512:
                        checksum = SHA358H(stream);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }
                return StringHash(type,checksum);
            }
        }

        public static string GetCryptoStreamHash(HashType type, string file, Action<long, long> report = null)
        {
            
            using (FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (HashAlgorithm hash = GetAlgorithm(type))
            using (CryptoStream stream = new CryptoStream(fileStream,hash,CryptoStreamMode.Read))
            {
                byte[] bufferedBytes = new byte[8 * 1024];
                while (stream.Read(bufferedBytes, 0, bufferedBytes.Length) > 0) { }
                return Converter.ByteArrayToHexString(hash.Hash);
            }
            
        }

        public static byte[] ByteHash(HashType type, string value, string salt = "")
        {
            return ByteBlockHash(Encoding.UTF8.GetBytes(value + salt), type);
        }
        public static string StringHash(HashType type, string value, string salt = "")
        {
            return Converter.ByteArrayToHexString(ByteBlockHash(Encoding.UTF8.GetBytes(value + salt), type));
        }
        public static string StringHash(HashType type, byte[] arr)
        {
            return Converter.ByteArrayToHexString(ByteBlockHash(arr, type));
        }
        public static string StringHash(byte[] arr)
        {
            return Converter.ByteArrayToHexString(arr);
        }
        public static byte[] ByteBlockHash(byte[] file, HashType type)
        {
            switch (type)
            {
                case HashType.MD5:
                    return MD5H(file);
                case HashType.SHA1:
                    return SHA1H(file);
                case HashType.SHA256:
                    return SHA256H(file);
                case HashType.SHA358:
                    return SHA358H(file);
                case HashType.SHA512:
                    return SHA512H(file);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
        public static HashAlgorithm GetAlgorithm(HashType type)
        {
            switch (type)
            {
                case HashType.MD5:
                    return new MD5Cng();
                case HashType.SHA1:
                    return new SHA1Cng();
                case HashType.SHA256:
                    return new SHA256Cng();
                case HashType.SHA358:
                    return new SHA384Cng();
                case HashType.SHA512:
                    return new SHA512Cng();
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
        public static string ByteBlockUTF8(byte[] block)
        {
            return Encoding.UTF8.GetString(block).TrimEnd('\0');
        }

        public static byte[] MD5H(byte[] block)
        { 
            using (MD5 hash = MD5.Create())
            {
                return hash.ComputeHash(block);
            }
        }
        public static byte[] MD5H(Stream block)
        {
            using (MD5 hash = MD5.Create())
            {
                return hash.ComputeHash(block);
            }
        }
        public static byte[] SHA1H(byte[] block)
        {
            using (SHA1 hash = SHA1.Create())
            {
                return hash.ComputeHash(block);
            }
        }
        public static byte[] SHA1H(Stream block)
        {
            using (SHA1 hash = SHA1.Create())
            {
                return hash.ComputeHash(block);
            }
        }
        public static byte[] SHA256H(byte[] block)
        {
            using (SHA256 hash = SHA256.Create())
            {
                return hash.ComputeHash(block);
            }
        }
        public static byte[] SHA256H(Stream block)
        {
            using (SHA256 hash = SHA256.Create())
            {
                return hash.ComputeHash(block);
            }
        }
        public static byte[] SHA358H(byte[] block)
        {
            using (SHA384 hash = SHA384.Create())
            {
                return hash.ComputeHash(block);
            }
        }
        public static byte[] SHA358H(Stream block)
        {
            using (SHA384 hash = SHA384.Create())
            {
                return hash.ComputeHash(block);
            }
        }
        public static byte[] SHA512H(byte[] block)
        {
            using (SHA512 hash = SHA512.Create())
            {
                return hash.ComputeHash(block);
            }
        }
        public static byte[] SHA512H(Stream block)
        {
            using (SHA512 hash = SHA512.Create())
            {
                return hash.ComputeHash(block);
            }
        }
    }
}
