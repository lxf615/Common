using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Generic
{
    [Obsolete]
    public class ConvertUtils
    {
        #region DateTime
        public static DateTime ToDate(string sValue)
        {
            try
            {
                return DateTime.Parse(sValue);
            }
            catch (Exception ex)
            {
                return DateTime.MinValue;
            }
             
        }
        #endregion

        static public string ToString(object o)
        {
            if (o == null) return string.Empty;
            if (o.GetType().Equals(typeof(System.DBNull)))
                return string.Empty;

            return o.ToString();
        }

        static public Decimal ToDecimal(object o)
        {
            return ToDecimal(o, (System.Decimal)0);
        }
        static public Decimal ToDecimal(object o, decimal defaults)
        {
            if (o == null) return defaults;
            if (o.GetType().Equals(typeof(System.DBNull))) return defaults;

            if (o.GetType().Equals(typeof(System.Single)))
                return (System.Decimal)(System.Single)o;

            if (o.GetType().Equals(typeof(System.Double)))
                return (System.Decimal)(System.Double)o;
            try
            {
                return Decimal.Parse(o.ToString(), System.Globalization.NumberStyles.Currency);
            }
            catch
            {
                return defaults;
            }

        }

        // Converts any of the database value types (int32, tiny int, byte, etc.) to
        //  an integer.
        static public int ToInt(object o)
        {
            return ToInt(o, 0);  // if no default provided, we use 0 as defaut if exception occur or null value
        }
        // Converts any of the database value types (int32, tiny int, byte, etc.) to
        //  an integer.
        static public int ToInt(object o, int defaultInt)
        {
            if (o == null)
                return defaultInt;

            if (o.GetType().Equals(typeof(System.Boolean)))
            {
                if (((bool)o) == true)
                    return 1;
                else
                    return 0;
            }

            if (o.GetType().Equals(typeof(System.Int32)))
                return (int)o;

            try
            {
                if (o.GetType().Equals(typeof(System.String)))
                {
                    if (o.ToString().Trim().Length <= 0 || o.ToString().Trim() == string.Empty)
                    {
                        return defaultInt;
                    }
                    else
                    {
                        return (int)Int32.Parse(o.ToString());
                    }
                }

            }
            catch (ArgumentNullException)
            {
                return defaultInt;
            }
            catch (FormatException)
            {
                return defaultInt;
            }
            catch (OverflowException)
            {
                return defaultInt;
            }
            catch
            {
                return defaultInt;
            }

            if (o.GetType().Equals(typeof(System.Int16)))
                return (Int16)o;

            if (o.GetType().Equals(typeof(System.Decimal)))
                return System.Decimal.ToInt32((System.Decimal)o);

            if (o.GetType().Equals(typeof(System.Byte)))
                return (int)((Byte)o);

            if (o.GetType().Equals(typeof(System.Single)))
                return (int)((Single)o);

            if (o.GetType().Equals(typeof(System.DBNull)))
                return defaultInt;

            if (o.GetType().Equals(typeof(double)))
                return (int)((double)o);
            try
            {
                return (int)Int32.Parse(o.ToString());
            }
            catch
            {
                // Why THROW ERROR!!!
                throw (new Exception(String.Format("SQL.ToInt: Can't handle {0}.",
                   o.GetType().ToString())));
            }
        }

    }
}
