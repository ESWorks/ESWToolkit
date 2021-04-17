using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ESWork.Net.Objects
{
    internal static class WorksConverter
    {
        public static WorksPacket Release(byte[] stream)
        {
            BinaryFormatter binForm = new BinaryFormatter();
            WorksPacket obj = null;
            using (MemoryStream memStream = new MemoryStream())
            {
                memStream.Write(stream, 0, stream.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                obj = (WorksPacket)binForm.Deserialize(memStream);
            }
            return obj;
        }
        public static byte[] Seal(WorksPacket wrapper)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, wrapper);
                return ms.ToArray();
            }
        }
    }
}
