using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Generic
{
    public class FileUtil
    {
        public static byte[] ReadBytes(Stream stream) 
        {
            BinaryReader reader = new BinaryReader(stream);
            List<byte> list = new List<byte>();

            byte[] buffer;
            int count=1024;
            do
            {
                buffer = reader.ReadBytes(count);
                if (buffer==null||buffer.Length==0)
                {
                    break;
                }
                list.AddRange(buffer);
                if (buffer.Length<count)
                {
                    break;
                }
            } while (true);

            return list.ToArray();
        }
    }
}
