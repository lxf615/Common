using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.NetworkInformation;

namespace Generic
{
    public class EnvironmentUtils
    {
        ///// <summary>
        /////  获取模块的完整路径，包括文件名。
        ///// </summary>
        ///// <returns></returns>
        //public static string GetCurrentDirectory() {
        //    return System.Environment.CurrentDirectory;
        //}

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
