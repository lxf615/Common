using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Generic
{
  
   public class StringUtil
   {
      public static string Left(string sSource, int iLength)
      {
         if (sSource == null)
            return string.Empty;
         return sSource.Substring(0, sSource.Length > iLength ? iLength : sSource.Length);
      }

      public static string Right(string sSource, int iLength)
      {
         if (sSource == null)
            return string.Empty;
         return sSource.Substring(iLength > sSource.Length ? 0 : sSource.Length - iLength);
      }

      public static string Mid(string sSource, int iStart, int iLength)
      {
         if (sSource == null)
            return string.Empty;
         int iStartPoint = iStart > sSource.Length ? sSource.Length : iStart;
         return sSource.Substring(iStartPoint, iStartPoint + iLength > sSource.Length ? sSource.Length - iStartPoint : iLength);
      }

      public static bool IsNumber(string strNumber)
      {
         Regex objNotNumberPattern = new Regex("[^0-9.-]");
         Regex objTwoDotPattern = new Regex("[0-9]*[.][0-9]*[.][0-9]*");
         Regex objTwoMinusPattern = new Regex("[0-9]*[-][0-9]*[-][0-9]*");
         string strValidRealPattern = "^([-]|[.]|[-.]|[0-9])[0-9]*[.]*[0-9]+$";
         string strValidIntegerPattern = "^([-]|[0-9])[0-9]*$";
         Regex objNumberPattern = new Regex("(" + strValidRealPattern + ")|(" + strValidIntegerPattern + ")");

         return !objNotNumberPattern.IsMatch(strNumber) &&
                !objTwoDotPattern.IsMatch(strNumber) &&
                !objTwoMinusPattern.IsMatch(strNumber) &&
                 objNumberPattern.IsMatch(strNumber);
      }
   }
}
