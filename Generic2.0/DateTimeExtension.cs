using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Generic
{
    /// <summary>
    /// 日期类型扩展
    /// </summary>
    public static class DateTimeExtension
    {
        /// <summary>
        /// 生成时间戳
        /// </summary>
        /// <param name="value"></param>
        /// <param name="len">长度12或者13,默认12</param>
        /// <returns></returns>
        public static long ToTimestamp(this DateTime value, int len = 12)
        {
            if (len == 12)
            {
                DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                return (long)(value - startTime).TotalSeconds;                
            }
            else
            {
                DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                return (DateTime.Now.Ticks - startTime.Ticks) / 10000;
            }
        }
    }
}
