using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Collections.Generic;


namespace Generic
{
    /// <summary>
    /// 文件IO类扩展
    /// </summary>
    public static class IOExtension
    {
        #region 文件
        /// <summary>
        /// 检测目录是否存，如果不存在则创建
        /// </summary>
        /// <param name="dirPath"></param>
        public static bool CreateDirectory(this string dirPath)
        {
            if (!Directory.Exists(dirPath))
            {
                DirectoryInfo direcotryInfo = Directory.CreateDirectory(dirPath);
            }

            return true;
        }

        /// <summary>
        /// 读取文字
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static byte[] ReadBytes(this Stream stream)
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

        /// <summary>
        /// 在指定的路径文件中写数据
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool WriteFile(this string filePath, byte[] data)
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                fileStream.Write(data, 0, data.Length);
                fileStream.Close();
            }

            return true;
        }

        /// <summary>
        /// 在指定的路径文件中写数据
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool WriteFile(this string filePath, string data)
        {
            using (StreamWriter streamWriter = new StreamWriter(filePath, false, Encoding.Default))
            {
                streamWriter.Write(data);
                streamWriter.Close();
            }

            return true;
        }
        #endregion
    }
}
