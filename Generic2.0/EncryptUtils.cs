using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Generic
{
    public class EncryptUtils
    {
        #region AES
        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="plainStr">明文字符串</param>
        /// <param name="key">加密密钥128,192,256bit</param>
        /// <param name="iv">加密向量128bit</param>
        /// <returns>密文</returns>
        public static string AESEncrypt(string plainStr, string key, string iv)
        {
            if (string.IsNullOrEmpty(key))
            {
                return string.Empty;
            }

            byte[] bKey = Encoding.UTF8.GetBytes(key);
            byte[] bIV = string.IsNullOrEmpty(iv)?null:Encoding.UTF8.GetBytes(iv);
            byte[] byteArray = Encoding.UTF8.GetBytes(plainStr);

            string encrypt = null;
            Rijndael aes = Rijndael.Create();
            try
            {
                using (MemoryStream mStream = new MemoryStream())
                {
                    using (CryptoStream cStream = new CryptoStream(mStream, aes.CreateEncryptor(bKey, bIV), CryptoStreamMode.Write))
                    {
                        cStream.Write(byteArray, 0, byteArray.Length);
                        cStream.FlushFinalBlock();
                        encrypt = Convert.ToBase64String(mStream.ToArray());
                    }
                }
            }
            catch 
            {
                
            }
            finally 
            {
                aes.Clear();
            }
            

            return encrypt;
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="encryptStr">密文字符串</param>
        /// <param name="key">加密密钥128,192,256bit</param>
        /// <param name="iv">加密向量128bit</param>
        /// <returns>明文</returns>
        public static string AESDecrypt(string encryptStr, string key, string iv)
        {
            if (string.IsNullOrEmpty(encryptStr))
            {
                return string.Empty;
            }

            byte[] bKey = Encoding.UTF8.GetBytes(key);
            byte[] bIV = string.IsNullOrEmpty(iv) ? null : Encoding.UTF8.GetBytes(iv);
            byte[] byteArray = Convert.FromBase64String(encryptStr);

            string decrypt = null;
            Rijndael aes = Rijndael.Create();
            try
            {
                using (MemoryStream mStream = new MemoryStream())
                {
                    using (CryptoStream cStream = new CryptoStream(mStream, aes.CreateDecryptor(bKey, bIV), CryptoStreamMode.Write))
                    {
                        cStream.Write(byteArray, 0, byteArray.Length);
                        cStream.FlushFinalBlock();
                        decrypt = Encoding.UTF8.GetString(mStream.ToArray());
                    }
                }
            }
            catch { }
            aes.Clear();

            return decrypt;
        } 

        #endregion


        #region DEC
        /// <summary>
        /// DEC 解密过程
        /// </summary>
        /// <param name="pToDecrypt"></param>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public static string Decrypt(string value, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = new byte[value.Length / 2];
            for (int x = 0; x < value.Length / 2; x++)
            {
                int i = (Convert.ToInt32(value.Substring(x * 2, 2), 16));
                inputByteArray[x] = (byte)i;
            }
            des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);  //建立加密对象的密钥和偏移量，此值重要，不能修改  
            des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();  //建立StringBuild对象，CreateDecrypt使用的是流对象，必须把解密后的文本变成流对象     
            return System.Text.Encoding.UTF8.GetString(ms.ToArray());
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
    }
}
