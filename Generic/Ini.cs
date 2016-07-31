using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Generic;
using System.Runtime.InteropServices;
namespace Generic
{
    public class Ini
    {
        // 声明INI文件的写操作函数 WritePrivateProfileString()
        [System.Runtime.InteropServices.DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        /// <summary>
        /// 读取INI文件中指定的Key的值  
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="def"></param>
        /// <param name="retVal"></param>
        /// <param name="size"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, System.Text.StringBuilder retVal, int size, string filePath);


        /// <summary>  
        /// 读取INI文件中指定的Key的值  
        /// </summary>  
        /// <param name="lpAppName">节点名称。如果为null,则读取INI中所有节点名称,每个节点名称之间用\0分隔</param>  
        /// <param name="lpKeyName">Key名称。如果为null,则读取INI中指定节点中的所有KEY,每个KEY之间用\0分隔</param>  
        /// <param name="lpDefault">读取失败时的默认值</param>  
        /// <param name="lpReturnedString">读取的内容缓冲区，读取之后，多余的地方使用\0填充</param>  
        /// <param name="nSize">内容缓冲区的长度</param>  
        /// <param name="lpFileName">INI文件名</param>  
        /// <returns>实际读取到的长度</returns>  
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern uint GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, [In, Out] char[] lpReturnedString, 
            uint nSize, string lpFileName);  

        private string sPath = System.Environment.CurrentDirectory + "\\config.ini";

        public Ini(string path)
        {
            this.sPath = path;
        }
        public Ini()
        {
           
        }
        public void Writue(string section, string key, string value)
        {
            // section=配置节，key=键名，value=键值，path=路径
            WritePrivateProfileString(section, key, value, sPath);
        }
        public string ReadValue(string section, string key)
        {
            // 每次从ini中读取多少字节
            System.Text.StringBuilder sBuilder = new System.Text.StringBuilder(255);
            
            // section=配置节，key=键名，temp=上面，path=路径
            GetPrivateProfileString(section, key, "", sBuilder, 255, sPath);

            return sBuilder.ToString();

        }
        public string[] ReadKeys(string section)
        {
            string[] value = new string[0];
            const int SIZE = 1024 * 10;

            char[] chars = new char[SIZE];
            uint result = GetPrivateProfileString(section, null, null, chars, SIZE, sPath);

            if (result != 0)
            {
                value = new string(chars).Split(new char[] { '\0' }, StringSplitOptions.RemoveEmptyEntries);
            }
            chars = null;

            return value;  
        }
    }
}
