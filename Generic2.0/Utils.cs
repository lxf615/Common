using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Net;
using System.Net.NetworkInformation;
using System.Xml;
using System.Runtime.Serialization;

namespace Generic
{
    public class Utils
    {
        #region XML
        /// <summary>
        /// 将对象序列化成XML
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Serialize<T>(T obj)
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(T));
            MemoryStream stream = new MemoryStream();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;
            using (XmlWriter writer = XmlWriter.Create(stream, settings))
            {
                serializer.WriteObject(writer, obj);
            }

            return Encoding.UTF8.GetString(stream.ToArray());
        }
        #endregion

        #region Password
        /// <summary>
        /// 随机密码
        /// </summary>
        /// <param name="len">长度</param>
        /// <returns></returns>
        public static string GetRandomPassword(int len)
        {
            List<string> list = new List<string>() { "a", "b", "c", "d", "e", "f", "g", "h", "i", "g", "k", "l", "m", "n"
            , "o", "p", "q", "i", "s", "t", "u", "v", "w", "x", "y", "z","1","2","3","4","5","6","7","8","9",};
            //List<string> list = new List<string>() { "1","2","3","4","5","6","7","8","9",};
            int max = list.Count - 1;
            System.Random random = new Random();
            int index = 0;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < len; i++)
            {
                index = random.Next(0, max);
                sb.Append(list[index]);
            }
            return sb.ToString();
        }

        #endregion

        ///// <summary>
        /////  获取模块的完整路径，包括文件名。
        ///// </summary>
        ///// <returns></returns>
        //public static string GetCurrentDirectory() {
        //    return System.Environment.CurrentDirectory;
        //}

        /// <summary>
        /// 获取程序根据目录
        /// </summary>
        /// <returns></returns>
        public static string GetBaseDirectory()
        {
            return System.AppDomain.CurrentDomain.BaseDirectory;
        }

        /// <summary>
        /// 检测端口是否被占用
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public static bool CheckPort(int port)
        {
            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] ipEndPoints = ipProperties.GetActiveTcpListeners();

            foreach (IPEndPoint endPoint in ipEndPoints)
            {
                if (endPoint.Port == port)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
