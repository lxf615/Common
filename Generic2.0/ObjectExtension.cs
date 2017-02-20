using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Generic
{
    /// <summary>
    /// Object对象类通用扩展方法
    /// </summary>
    public static class ObjectExtension
    {
        /// <summary>
        /// 转为字符串
        /// 如果value为null,返回string.empty
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToString2(this object value)
        {
            if (value == null) return string.Empty;
            if (value.GetType().Equals(typeof(System.DBNull)))
                return string.Empty;

            return value.ToString();
        }

        /// <summary>
        /// 转为小数
        /// 转换失败，默认返回0
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static decimal ToDecimal(this object value)
        {
            return ToDecimal(value, 0);
        }

        /// <summary>
        /// 转为小数
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaults">转换失败，返回一个默认值</param>
        /// <returns></returns>
        public static decimal ToDecimal(this object value, decimal defaults)
        {
            if (value == null)
                return defaults;

            if (value.GetType().Equals(typeof(DBNull)))
                return defaults;

            if (value.GetType().Equals(typeof(float)))
                return (decimal)(float)value;

            if (value.GetType().Equals(typeof(double)))
                return (decimal)(double)value;

            try
            {
                return decimal.Parse(value.ToString(), System.Globalization.NumberStyles.Currency);
            }
            catch
            {
                return defaults;
            }

        }


        /// <summary>
        /// 转为整数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ToInt(this object value)
        {
            return ToInt(value, 0);  
        }

        /// <summary>
        /// 转为整数
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultInt">默认值</param>
        /// <returns></returns>
        public static int ToInt(this object value, int defaultInt)
        {
            if (value == null)
                return defaultInt;

            if (value.GetType().Equals(typeof(bool)))
            {
                if (((bool)value) == true)
                    return 1;
                else
                    return 0;
            }

            if (value.GetType().Equals(typeof(int)))
                return (int)value;

            try
            {
                if (value.GetType().Equals(typeof(string)))
                {
                    if (string.IsNullOrEmpty(value.ToString().Trim()))
                    {
                        return defaultInt;
                    }
                    else
                    {
                        return int.Parse(value.ToString());
                    }
                }


                if (value.GetType().Equals(typeof(short)))
                    return (short)value;

                if (value.GetType().Equals(typeof(decimal)))
                    return decimal.ToInt32((decimal)value);

                if (value.GetType().Equals(typeof(byte)))
                    return ((byte)value);

                if (value.GetType().Equals(typeof(float)))
                    return (int)((float)value);

                if (value.GetType().Equals(typeof(DBNull)))
                    return defaultInt;

                if (value.GetType().Equals(typeof(double)))
                    return (int)((double)value);

                return int.Parse(value.ToString());
            }
            catch
            {
                return defaultInt;
            }
        }
    }
}
