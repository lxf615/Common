using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Generic
{
    public class WebUtil
    {
        public static string ParserToFullPath(string relativePath)
        {
            return HttpContext.Current.Server.MapPath(relativePath);
        }
    }
}
