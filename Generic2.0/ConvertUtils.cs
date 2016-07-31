using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Generic
{
    /// <summary>
    /// 通用数据转换工具
    /// </summary>
    public class ConvertUtils
    {
        /// <summary>
        /// 转为字符串
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        static public string ToString(object o)
        {
            if (o == null) return string.Empty;
            if (o.GetType().Equals(typeof(System.DBNull)))
                return string.Empty;

            return o.ToString();
        }

        /// <summary>
        /// 转为小数
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        static public decimal ToDecimal(object o)
        {
            return ToDecimal(o,  0);
        }

        /// <summary>
        /// 转为小数
        /// </summary>
        /// <param name="o"></param>
        /// <param name="defaults">默认值</param>
        /// <returns></returns>
        static public decimal ToDecimal(object o, decimal defaults)
        {
            if (o == null) return defaults;
            if (o.GetType().Equals(typeof(DBNull))) return defaults;

            if (o.GetType().Equals(typeof(float)))
                return (decimal)(float)o;

            if (o.GetType().Equals(typeof(double)))
                return (decimal)(double)o;
            try
            {
                return decimal.Parse(o.ToString(), System.Globalization.NumberStyles.Currency);
            }
            catch
            {
                return defaults;
            }

        }

        /// <summary>
        /// 转为整数
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        static public int ToInt(object o)
        {
            return ToInt(o, 0);  // if no default provided, we use 0 as defaut if exception occur or null value
        }
        
        /// <summary>
        /// 数据为整数
        /// </summary>
        /// <param name="o"></param>
        /// <param name="defaultInt">默认值</param>
        /// <returns></returns>
        static public int ToInt(object o, int defaultInt)
        {
            if (o == null)
                return defaultInt;

            if (o.GetType().Equals(typeof(bool)))
            {
                if (((bool)o) == true)
                    return 1;
                else
                    return 0;
            }

            if (o.GetType().Equals(typeof(int)))
                return (int)o;

            try
            {
                if (o.GetType().Equals(typeof(string)))
                {
                    if (string.IsNullOrEmpty(o.ToString().Trim()))
                    {
                        return defaultInt;
                    }
                    else
                    {
                        return int.Parse(o.ToString());
                    }
                }


                if (o.GetType().Equals(typeof(short)))
                    return (short)o;

                if (o.GetType().Equals(typeof(decimal)))
                    return decimal.ToInt32((decimal)o);

                if (o.GetType().Equals(typeof(byte)))
                    return ((byte)o);

                if (o.GetType().Equals(typeof(float)))
                    return (int)((float)o);

                if (o.GetType().Equals(typeof(DBNull)))
                    return defaultInt;

                if (o.GetType().Equals(typeof(double)))
                    return (int)((double)o);

                return int.Parse(o.ToString());
            }
            catch
            {
                return defaultInt;
            }
        }

    }
}
