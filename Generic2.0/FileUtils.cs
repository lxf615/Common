using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Generic
{
    public class FileUtils
    {
        public static byte[] ReadBytes(Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);
            List<byte> list = new List<byte>();

            byte[] buffer;
            int count = 1024;
            do
            {
                buffer = reader.ReadBytes(count);
                if (buffer == null || buffer.Length == 0)
                {
                    break;
                }
                list.AddRange(buffer);
                if (buffer.Length < count)
                {
                    break;
                }
            } while (true);

            return list.ToArray();
        }

        public static bool WriteFile(string filePath, byte[] data)
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                fileStream.Write(data, 0, data.Length);
                fileStream.Close();
            }

            return true;
        }

        public static bool WriteFile(string filePath, string data)
        {
            using (StreamWriter streamWriter = new StreamWriter(filePath,false,Encoding.Default))
            {
                streamWriter.Write(data);
                streamWriter.Close();
            }

            return true;
        }

        /// <summary>
        /// 检测目录是否存，如果不存在则创建
        /// </summary>
        /// <param name="dirPath"></param>
        public static bool CreateDirectory(string dirPath)
        {
            if (!Directory.Exists(dirPath))
            {   
                DirectoryInfo direcotryInfo = Directory.CreateDirectory(dirPath);
            }

            return true;
        }
    }
}
