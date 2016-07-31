using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Generic
{
    public static class StringExtension
    {
        /// <summary>
        /// 将指定格式的字符串转换成日期类型
        /// </summary>
        /// <param name="sValue"></param>
        /// <param name="sFormat"></param>
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

        /// <summary>
        /// 获取MD5串,使用UTF8编码
        /// </summary>
        /// <returns></returns>
        public static string ToMD5(this string sValue) 
        {
            return EncryptUtils.GetMD5String(sValue);
        }


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
        public static byte[] ToBytes(this string value)
        {
            return ToBytes(value, Encoding.UTF8);
        }
        public static byte[] ToBytes(this string value, Encoding encoding)
        {
            return encoding.GetBytes(value);
        }
        #endregion

    }
}
