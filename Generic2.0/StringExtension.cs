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
    /// 字符串类扩展
    /// </summary>
    public static class StringExtension
    {

        /// <summary>
        /// 检测是否是有效的手机号
        /// </summary>
        /// <param name="sValue"></param>
        /// <returns></returns>
        public static bool IsMobile(this string sValue)
        {
            if (!string.IsNullOrEmpty(sValue))
            {
                Regex reg = new Regex(@"^\d{11}$");
                return reg.IsMatch(sValue);
            }
            else
            {
                return false;
            }
        }

        #region 日期处理

        /// <summary>
        /// 将指定格式的字符串转换成日期类型
        /// </summary>
        /// <param name="sValue">日期字符串</param>
        /// <param name="sFormat">格式</param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string sValue, string sFormat)
        {
            try
            {
                if (string.IsNullOrEmpty(sValue))
                {
                    return DateTime.MinValue;
                }
                return DateTime.ParseExact(sValue, sFormat, null);
            }
            catch (Exception)
            {

                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// 字符串转换成日期,字符串格式必须是yyyy-MM-dd HH:mm:ss
        /// 如果转换失败返回 DateTime.MinValue
        /// </summary>
        /// <param name="sValue"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string sValue)
        {
            try
            {
                if (string.IsNullOrEmpty(sValue))
                {
                    return DateTime.MinValue;
                }
                return DateTime.ParseExact(sValue, "yyyy-MM-dd HH:mm:ss", null);
            }
            catch (Exception)
            {

                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// 字符串转换成日期,字符串格式必须是yyyy-MM-dd
        /// 如果转换失败返回 DateTime.MinValue
        /// </summary>
        /// <param name="sValue"></param>
        /// <returns></returns>
        public static DateTime ToDate(this string sValue)
        {
            try
            {
                if (string.IsNullOrEmpty(sValue))
                {
                    return DateTime.MinValue;
                }
                return DateTime.ParseExact(sValue, "yyyy-MM-dd", null);
            }
            catch (Exception)
            {

                return DateTime.MinValue;
            }
        }


        #endregion

        #region MD5加密

        /// <summary>
        /// 获取MD5串,使用UTF8编码
        /// </summary>
        /// <returns></returns>
        public static string ToMD5(this string sValue)
        {
            return sValue.ToMD5(Encoding.UTF8);
        }


        /// 取得字符串的md5加密串 
        /// <summary>
        /// 取得字符串的md5加密串
        /// </summary>
        /// <param name="sValue">原字符串</param>
        /// <param name="encoding">编码集,如GB2312等</param>
        /// <returns></returns>
        public static string ToMD5(this string sValue, string encoding)
        {
            return sValue.ToMD5(Encoding.GetEncoding(encoding));
        }

        /// <summary>
        /// 取得字符串的md5加密串
        /// </summary>
        /// <param name="sValue">原字符串</param>
        /// <param name="encoding">编码集,如GB2312等</param>
        /// <returns></returns>
        public static string ToMD5(this string sValue, Encoding encoding)
        {
            if (string.IsNullOrEmpty(sValue))
            {
                return string.Empty;
            }

            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] t = md5.ComputeHash(encoding.GetBytes(sValue));
            StringBuilder reString = new StringBuilder(32);
            for (int i = 0; i < t.Length; i++)
            {
                reString.Append(t[i].ToString("x").PadLeft(2, '0'));
            }
            return reString.ToString();
        }
        #endregion

        #region TDES/3DES加解密
        /// <summary>
        /// TDES/3DES加密,UTF8编码
        /// Key24字节,IV的前8个字节
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <returns>Base64字符串</returns>
        public static string DES3Encrypt(this string data, string key)
        {
            if (string.IsNullOrEmpty(data))
            {
                return data;
            }

            byte[] buffer = Encoding.UTF8.GetBytes(data);
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] ivBytes = new byte[8];
            Array.Copy(keyBytes,ivBytes,8);

            return Convert.ToBase64String(buffer.DES3Encrypt(keyBytes, ivBytes));
        }

        /// <summary>
        /// TDES/3DES加密,UTF8编码
        /// </summary>
        /// <param name="data">明文</param>
        /// <param name="key">Key24字节,UTF8编码</param>
        /// <param name="iv">8个字节,UTF8编码</param>
        /// <returns>结果Base64编码</returns>
        public static string DES3Encrypt(this string data, string key,string iv)
        {
            if (string.IsNullOrEmpty(data))
            {
                return data;
            }

            byte[] buffer = Encoding.UTF8.GetBytes(data);
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] ivBytes = Encoding.UTF8.GetBytes(iv);

            return Convert.ToBase64String(buffer.DES3Encrypt(keyBytes, ivBytes));
        }

        /// <summary>
        /// TDES/3DES加密
        /// Key24字节,IV是Key前8个字节
        /// </summary>
        /// <param name="buffer">明文</param>
        /// <param name="key">私钥</param>
        /// <param name="iv">偏移量</param>
        /// <returns></returns>
        public static byte[] DES3Encrypt(this byte[] buffer, byte[] key,byte[] iv)
        {
            TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();

            DES.Key = key;
            DES.IV = iv;
            DES.Mode = CipherMode.CBC;
            DES.Padding = PaddingMode.PKCS7;

            ICryptoTransform DESEncrypt = DES.CreateEncryptor();

            return DESEncrypt.TransformFinalBlock(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// TDES/3DES加密,UFT8编码
        /// Key24字节,IV是Key前8个字节
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string DES3Decrypt(this string data, string key)
        {
            if (string.IsNullOrEmpty(data))
            {
                return data;
            }

            TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();

            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] ivBytes = new byte[8];
            Array.Copy(keyBytes, ivBytes, ivBytes.Length);
            byte[] buffer = Convert.FromBase64String(data);

            byte[] result = buffer.DES3Decrypt(keyBytes, ivBytes);
            if (result != null)
            {
                return Encoding.UTF8.GetString(result);
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// TDES/3DES加密,
        /// Key24字节,IV是Key前8个字节
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static byte[] DES3Decrypt(this byte[] buffer, byte[] key,byte[] iv)
        {
            TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();

            DES.Key = key;
            DES.IV = iv;
            DES.Mode = CipherMode.CBC;
            DES.Padding = PaddingMode.PKCS7;

            ICryptoTransform desDecrypt = DES.CreateDecryptor();

            try
            {
                return desDecrypt.TransformFinalBlock(buffer, 0, buffer.Length);
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region Base64
        /// <summary>
        /// Base64编码,UTF8编码
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ToBase64(this string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Base64解码,UTF8编码
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string FromBase64(this string value)
        {
            return FromBase64(value,Encoding.UTF8);
        }

        /// <summary>
        /// Base64解码
        /// </summary>
        /// <param name="encoding"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string FromBase64(this string value,Encoding encoding)
        {
            byte[] bytes = Convert.FromBase64String(value);
            return encoding.GetString(bytes);
        }
        #endregion

        #region byte
        /// <summary>
        /// 转换成字节编码，UTF8
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this string value)
        {
            return ToBytes(value, Encoding.UTF8);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this string value, Encoding encoding)
        {
            return encoding.GetBytes(value);
        }
        #endregion

        #region AES
        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="plainStr">明文字符串</param>
        /// <param name="key">加密密钥128,192,256bit</param>
        /// <param name="iv">加密向量128bit</param>
        /// <returns>密文</returns>
        public static string AESEncrypt(this string plainStr, string key, string iv)
        {
            if (string.IsNullOrEmpty(key))
            {
                return string.Empty;
            }

            byte[] bKey = Encoding.UTF8.GetBytes(key);
            byte[] bIV = string.IsNullOrEmpty(iv) ? null : Encoding.UTF8.GetBytes(iv);
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
        public static string AESDecrypt(this string encryptStr, string key, string iv)
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

        #region SQL转换
        /// <summary>
        /// SQL转换.添加单引号,防SQL注入
        /// </summary>
        /// <param name="sValue"></param>
        /// <returns></returns>
        public static string Q(this string sValue)
        {
            if (string.IsNullOrEmpty(sValue))
            {
                return "''";
            }

            sValue = sValue.Replace("'","''");
            return string.Format("'{0}'",sValue);
        }
        #endregion

        #region HMAC-SHA1加密
        /// <summary>
        /// HMAC-SHA1加密,UTF8编码
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Sha1Encode(this string data, string key)
        {
            byte[] bytesKey = Encoding.UTF8.GetBytes(key);
            HMACSHA1 myhmacsha1 = new HMACSHA1(bytesKey);
            byte[] byteArray = Encoding.UTF8.GetBytes(data);
            MemoryStream stream = new MemoryStream(byteArray);
            return Convert.ToBase64String(myhmacsha1.ComputeHash(stream));
        }
        #endregion
    }
}
