using System;
using System.Diagnostics;
using System.Collections;
using System.IO;
namespace Insignia
{
    /// <summary>
    ///  Global variable for application
    /// </summary>
    public class Global
    {
        public static string ApplicationName = "Insignia Library System";
        public static string gsvServerType;
        public static string gsvDataBaseName = "";
        public static string gsvServerName = "";
        public static string gsvLibraryPatronsLDAPPath = "";
        public static string gsvADDomainControllerIP = "";
        public static string gsvADDomainname = "";
        public static string gsvADPassword = "";
        public static string gsvServerUserID = "";
        public static string gsvServerPassword = "";
        public static string gsvNTSecurity = "";
        public static string gsvServerConn = "";
        public static string AppRegFolder = "SOFTWARE\\Insignia\\LibrarySystem";
        public static string AppRegIRMFolder = "SOFTWARE\\Insignia\\MediaManager";
        public static string AppDefaultFolder = "C:\\Program Files\\Insignia\\Library\\";
        public static string gsvConnectionInfoSource = "";
        public static string APPShort = "ILS";
        public static string gsvAppVersion = "";
        public static string userName = "";
        public static string ComputerName = "";
        public static string DivisionID = "";
        public static string CustomerCode = "";
        public static string RenewalCode = "";
        public static int RenewalErrorCode = 0;
        public static string RenewalErrorMessage = "";
        public static DateTime RenewalDate = new DateTime(2000, 1, 1);

        public static bool isHB = false;

        public static string gsvCenterServerType;
        public static string gsvCenterDataBaseName = "";
        public static string gsvCenterServerName = "";
        public static string gsvCenterServerUserID = "";
        public static string gsvCenterServerPassword = "";
        public static bool bIsILSWeb = false;
        public static string gsvInfo = ""; //Jimmy add 2009.10.12
        public static bool bIsInfo = false; //Jimmy add 2009.10.12


        public static string GetByteString(byte[] bValue)
        {
            string sValue = "";
            int i;
            for (i = 0; i < bValue.Length; i++)
                sValue = sValue + (char)(bValue[i]);
            return sValue;
        }
        public static void GetINIInfo(String strFileName)
        {
            String strData;
            StreamReader fs = new StreamReader(strFileName);

            strData = fs.ReadToEnd();
            fs.Close();
            strData = Insignia.Encryption.EncryptDecrypt(strData);

            Global.gsvServerName = GetLineValue("Server", strData);
            Global.gsvDataBaseName = GetLineValue("DatabaseName", strData);
            Global.gsvServerUserID = GetLineValue("UserID", strData);
            Global.gsvServerPassword = GetLineValue("Password", strData);
            Global.gsvNTSecurity = GetLineValue("NTSecurity", strData);
        }
        public static String GetLineValue(String strKey, String strData)
        {
            int lStart, lEnd;
            String vbNewLine = "\n";    // "\n\r";

            lStart = strData.IndexOf(vbNewLine + strKey + "=", 1);
            if (lStart == 0)
                return "";
            lStart = lStart + (vbNewLine + strKey + "=").Length;
            lEnd = strData.IndexOf(vbNewLine, lStart);
            if (lEnd != 0)
                return strData.Substring(lStart, lEnd - lStart - 1);
            else
                return strData.Substring(lStart);
        }
        public static byte[] ConvertStringToByte(String sValue)
        {
            byte[] bValue;
            int i;
            bValue = new byte[sValue.Length + 1];
            char[] cValue;
            try
            {
                cValue = sValue.ToCharArray(0, sValue.Length);
                for (i = 0; i < cValue.Length; i++)
                {
                    bValue[i] = (byte)cValue[i];
                }
                bValue[bValue.Length - 1] = 0;
            }
            catch (Exception ex)
            {
            }
            return bValue;
        }

        public static string GetDBVersion()
        {
            DBOperation dbop;
            System.Data.DataTable dt;
            String sDBVersion = "";
            try
            {
                dbop = new DBOperation("Select t96Major, t96Minor, t96Revision from Version_t96 Order by t96Major desc, "
                    + "t96Minor Desc, t96Revision desc");
                dt = dbop.GetDataTable();
                if (dt.Rows.Count > 0)
                    sDBVersion = dt.Rows[0]["t96Major"].ToString() + "."
                        + dt.Rows[0]["t96Minor"] + "." + dt.Rows[0]["t96Revision"];
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            return sDBVersion;
        }

    }

}
