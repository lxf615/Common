using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;


namespace Generic
{
    public class Logs
    {
        private static readonly object obj = new object();

        public static string gLogDirectory = string.Format(@"{0}\Logs", EnvironmentUtils.GetBaseDirectory());

        public static void Console(string msg) 
        {
#if DEBUG
            Log(msg, gLogDirectory);       
            System.Console.WriteLine(msg);
#else
            Log(msg, gLogDirectory);       
#endif
        }

        #region Web Log

        /// <summary>
        /// 记录Web日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        public static void WebLog(string msg)
        {
            string url = HttpContext.Current.Request.Url.LocalPath;
            StringBuilder message = new StringBuilder();
            message.Append(Environment.NewLine);
            message.Append(string.Format(@"日志URL：{0}", url));
            message.Append(Environment.NewLine);
            message.Append(string.Format(@"日志内容：{0}",msg));
            Log(msg, url);
        }

        /// <summary>
        /// 记录Web异常
        /// </summary>
        /// <param name="ex">异常对象</param>
        public static void WebLog(Exception ex)
        {

            string path = HttpContext.Current.Request.Url.LocalPath;
            WebLog(ex, path);
        }
        /// <summary>
        /// 记录Web异常
        /// </summary>
        /// <param name="ex">异常对象</param>
        /// <param name="url">异常请求的Web Url</param>
        public static void WebLog(Exception ex, string url)
        {
            StringBuilder message = new StringBuilder();
            message.Append(Environment.NewLine);
            message.Append(string.Format(@"异常类型：{0}", ex.GetType().Name));
            message.Append(Environment.NewLine);
            message.Append(string.Format(@"异常内容：{0}", ex.Message));
            message.Append(Environment.NewLine);
            message.Append(string.Format(@"引发异常的方法：{0}", ex.TargetSite));
            message.Append(Environment.NewLine);
            message.Append(string.Format(@"异常URL：{0}", url));
            message.Append(Environment.NewLine);
            message.Append(string.Format(@"异常详情：{0}", ex.StackTrace.ToString()));

            Log(message.ToString());
        }
        #endregion

        /// <summary>
        /// 记录异常
        /// </summary>
        /// <param name="msg"></param>
        public static void Log(string msg)
        {
            Log(msg, gLogDirectory);
        }

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        /// <param name="directory">日志记录路径</param>
        public static void Log(string msg, string directory)
        {
            try
            {
                //Monitor.Enter(obj);
                DateTime dt = DateTime.Now;
                string message = "\r\n" + dt.ToString("yyyy-MM-dd HH:mm:ss") + ": " + msg + "\r\n";
                directory = string.Format("{0}\\{1}", directory, dt.ToString("yyyyMMdd"));
                if (!System.IO.Directory.Exists(directory))
                    System.IO.Directory.CreateDirectory(directory);
                string filePath = string.Format("{0}\\{1}.log", directory, dt.Hour);
                using (System.IO.FileStream fileStream = new System.IO.FileStream(filePath, System.IO.FileMode.Append))
                {
                    byte[] byte_arr = Encoding.Default.GetBytes(message);
                    fileStream.Write(byte_arr, 0, byte_arr.Length);
                    fileStream.Close();
                }
            }
            catch(Exception ex)
            {
                
            }
            finally
            {
                //Monitor.Pulse(obj);
                //Monitor.Exit(obj);
            }
        }
    }
}
