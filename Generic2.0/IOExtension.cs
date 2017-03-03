using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Generic
{
    /// <summary>
    /// 文件IO类扩展
    /// </summary>
    public static class IOExtension
    {
        #region Common File
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

        #region INI File
        //声明读写INI文件的API函数
        [DllImport("kernel32")]
        private static extern bool WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, System.Text.StringBuilder retVal, int size, string filePath);


        /// <summary>
        ///写INI文件 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static bool WriteString(this string filePath, string section, string key, string defaultValue)
        {
            return WritePrivateProfileString(section, key, defaultValue, filePath);
        }


        /// <summary>
        /// 读取INI文件指定
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string ReadString(this string filePath, string section, string key)
        {
            return ReadString(filePath, section, key, string.Empty);
        }

        /// <summary>
        /// 读整数
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int ReadInteger(this string filePath, string section, string key)
        {
            return ReadInteger(filePath, section, key, 0);
        }

        /// <summary>
        /// 读整数
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int ReadInteger(this string filePath, string section, string key, int defaultValue)
        {
            string retValue = ReadString(filePath, section, key, Convert.ToString(defaultValue));
            try
            {
                return retValue.ToInt();

            }
            catch
            {
                return defaultValue;
            }
        }


        /// <summary>
        ///  读取INI文件指定
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string ReadString(this string filePath, string section, string key, string defaultValue)
        {
            StringBuilder temp = new StringBuilder(255);
            int len = GetPrivateProfileString(section, key, defaultValue, temp, temp.Capacity, filePath);
            return temp.ToString().Trim();
        }

        #endregion
    }


}
