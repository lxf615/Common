using System;
using System.Diagnostics;
using System.Collections;
using System.ComponentModel;

using System.Drawing;
using Microsoft.Win32;
using Insignia;
using System.IO;
using System.Data;
using System.Text;
using System.Security.Cryptography;

namespace Insignia
{
  #region BasicDBInfo
  public class BasicDBInfo
  {
    public string ServerName;
    public string UserID;
    public string Password;
    public string DataBaseName;

    public string GetConnectionString()
    {
      return "Data Source=" + ServerName
        + "; Initial Catalog=" + DataBaseName
        + "; User ID = " + UserID
        + "; Password = " + Password;
    }
  }
  #endregion

	/// <summary>
	/// Summary description for Registry.
	/// </summary>
	public class RegEdit
	{
    #region Get Registry Information
    public static BasicDBInfo GetDatabaseInfo( int type)
    {
      string strInfo = null;
		if( type==0)	
		  strInfo = GetValue("SOFTWARE\\Insignia\\StudentInformationSystem", "DataLink");
     else if (type == 1)
        strInfo = GetValue("SOFTWARE\\Insignia\\LibrarySystem", "DataLink");
     else if (type == 2)
        strInfo = GetValue("SOFTWARE\\Insignia\\MediaManager", "DataLink");

      if(strInfo == null) 
			return null;

      strInfo =Encryption.EncryptDecrypt(strInfo);
      BasicDBInfo db = new BasicDBInfo();

      db.ServerName   = strInfo.Substring (0, 128).Trim ();
      db.UserID       = strInfo.Substring (256, 128).Trim ();
      db.DataBaseName = strInfo.Substring (128, 128).Trim ();
      db.Password     =strInfo.Substring  (384);
      return db;
    }

    public static string GetValue(string folder, string name)
    {
      object o = QueryRegValue(Registry.LocalMachine, folder, name);
      
      string str = o as String;
      if(str != null) return str;

      byte[] bValue = (byte[])o;
      if (bValue == null) return "";
      String sValue = Insignia.Global.GetByteString(bValue);
      sValue = sValue.Substring (0, sValue.Length - 1);
      return sValue;
    }

    public static Object QueryRegValue(RegistryKey RootKey, string KeyName, string ValueName)
    {
      RegistryKey Hkinteral;
      Object vValue;
      try
      {
        Hkinteral = RootKey.OpenSubKey(KeyName);
        vValue = Hkinteral.GetValue(ValueName);
        Hkinteral.Close ();
      }
      catch( Exception ex)
      {
			Debug.WriteLine(ex.ToString());
        return null ;
      }
      return vValue;
    }
    #endregion

    #region Set Registry Info ???
    public static bool SetRegValue(RegistryKey RootKey, String sKeyName,
      String sValueName, Object vValueSetting, int lValueType)
    {
      RegistryKey Hkinteral;
      try
      {
        try
        {
          Hkinteral = RootKey.OpenSubKey(sKeyName,true);
          if (Hkinteral==null)
            Hkinteral = RootKey.CreateSubKey(sKeyName);
        }
        catch
        {
          Hkinteral = RootKey.CreateSubKey(sKeyName);
        }
        if (lValueType== 3 && vValueSetting.GetType().Name  == "String")
        {
          byte[] bValue;
          bValue = Insignia.Global.ConvertStringToByte((String) vValueSetting);
          Hkinteral.SetValue (sValueName, bValue);
        }
        else
          Hkinteral.SetValue (sValueName, vValueSetting);
        Hkinteral.Close ();
        return true;
      }
      catch( Exception ex)
      {
			Debug.WriteLine(ex.ToString());
        return false ;
      }
    }
    #endregion
	}
}
