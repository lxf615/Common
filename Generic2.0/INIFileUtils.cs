using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;

namespace Generic
{
    /// <summary>
    /// INI配置类
    /// </summary>
    public class INIFileUtils
    {
        #region INI File
        //声明读写INI文件的API函数
        [DllImport("kernel32")]
        private static extern bool WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, System.Text.StringBuilder retVal, int size, string filePath);
        #endregion

        /// <summary>
        ///写INI文件 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public bool WriteString(string filePath, string section, string key, string defaultValue)
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
        public static string ReadString(string filePath, string section, string key)
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
        public static int ReadInteger(string filePath, string section, string key)
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
        public static int ReadInteger(string filePath, string section, string key, int defaultValue)
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
        public static string ReadString(string filePath, string section, string key, string defaultValue)
        {
            System.Text.StringBuilder temp = new System.Text.StringBuilder(255);
            int len = GetPrivateProfileString(section, key, defaultValue, temp, temp.Capacity, filePath);
            return temp.ToString().Trim();
        }
    }
}
