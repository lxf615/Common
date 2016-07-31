using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Insignia
{
	/// <summary>
	/// Summary description for Encryption.
	/// </summary>
	public class Encryption
	{
		public Encryption()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		public static string EncryptDecrypt(string strDataIn ) 
		{

			if ( strDataIn == null || strDataIn =="")
										return strDataIn;
			short intXORValue1;
			short intXORValue2;
			string strDataOut ="";
			//string strCodeKey;
			char[] DataIn = strDataIn.ToCharArray() ;

			char[] CodeKey = {'I','n','s','i','g','n','i','a','S','W'} ; //"InsigniaSW"																																																 strCodeKey = "InsigniaSW"
			//' strDataOut = ""
			for ( int i=0; i < DataIn.Length;i++) 
			{
				intXORValue1 = (short) DataIn[i]; //AscW(Mid(strDataIn, lonDataPtr, 1))
				intXORValue2 = (short) CodeKey[(i+1) % CodeKey.Length]; // AscW(Mid(strCodeKey, (lonDataPtr Mod Len(strCodeKey)) + 1, 1))
				strDataOut = strDataOut + (char)(intXORValue1 ^ intXORValue2);
			}													
																		
			return strDataOut;
			
		}
		public static string EncryptText(string textToEncrypt)
		{
			try
			{
				ASCIIEncoding x = new ASCIIEncoding();
				byte[] key = x.GetBytes("keykkeykkeykkeykkeykkeykkeykkeyk");
				byte[] iv = x.GetBytes("iviviviviviviviv");

				byte[] bytes = Encoding.Unicode.GetBytes(textToEncrypt);
				MemoryStream ms = new MemoryStream();

				SymmetricAlgorithm sa = new RijndaelManaged();
				CryptoStream crypto = new CryptoStream(ms, sa.CreateEncryptor(key, iv),
					CryptoStreamMode.Write);
				crypto.Write(bytes, 0, bytes.Length);
				crypto.FlushFinalBlock();
				crypto.Close();
				byte[] bytOut = ms.GetBuffer();
				int i = 0;
				for (i = 0; i < bytOut.Length; i++)
					if (bytOut[i] == 0)
						break;
				return System.Convert.ToBase64String(bytOut, 0, i);
			}
			catch
			{
				return "";
			}
		}
      static string Mapping(string strIn, bool encrypt, int seed)
      {
         string src = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
         string des = "LASK0HDJFQZPMWER8CXVBNOYI9saljghdktqzpmwerfcxvuoniy1bU36T7G245";
         src = src.Substring(seed * 2) + src.Substring(0, seed * 2);

         if (strIn.Length == 0)
            return strIn;
         else
         {
            if (encrypt)
            {
               int index = src.IndexOf(strIn);
               if (index >= 0)
                  return des.Substring(index, 1);
               else
                  return strIn;
            }
            else
            {
               int index = des.IndexOf(strIn);
               if (index >= 0)
                  return src.Substring(index, 1);
               else
                  return strIn;
            }
            //            string strOut = strIn.Substring(0,1);
            //            switch  (strOut)
            //            {
            //               case "a":
            //                  return "L";
            //               case "b":
            //                  return "A";
            //               case "c":
            //                  return "S";
            //               case "d":
            //                  return "K";
            //               case "e":
            //                  return "G";
            //               case "f":
            //                  return "H";
            //               case "g":
            //                  return "D";
            //               case "h":
            //                  return "J";
            //               case "i":
            //                  return "F";
            //               case "j":
            //                  return "Q";
            //               case "k":
            //                  return "Z";
            //               case "l":
            //                  return "P";
            //               case "m":
            //                  return "M";
            //               case "n":
            //                  return "W";
            //               case "o":
            //                  return "E";
            //               case "p":
            //                  return "R";
            //               case "q":
            //                  return "T";
            //               case "r":
            //                  return "C";
            //               case "s":
            //                  return "X";
            //               case "t":
            //                  return "V";
            //               case "u":
            //                  return "B";
            //               case "v":
            //                  return "N";
            //               case "w":
            //                  return "O";
            //               case "x":
            //                  return "Y";
            //               case "y":
            //                  return "I";
            //               case "z":
            //                  return "U";
            //               case "A":
            //                  return "s";
            //               case "B":
            //                  return "a";
            //               case "C":
            //                  return "l";
            //               case "D":
            //                  return "j";
            //               case "E":
            //                  return "g";
            //               case "F":
            //                  return "h";
            //               case "G":
            //                  return "d";
            //               case "H":
            //                  return "k";
            //               case "I":
            //                  return "t";
            //               case "J":
            //                  return "q";
            //               case "K":
            //                  return "z";
            //               case "L":
            //                  return "p";
            //               case "M":
            //                  return "m";
            //               case "N":
            //                  return "w";
            //               case "O":
            //                  return "e";
            //               case "P":
            //                  return "r";
            //               case "Q":
            //                  return "f";
            //               case "R":
            //                  return "c";
            //               case "S":
            //                  return "x";
            //               case "T":
            //                  return "v";
            //               case "U":
            //                  return "u";
            //               case "V":
            //                  return "o";
            //               case "W":
            //                  return "n";
            //               case "X":
            //                  return "i";
            //               case "Y":
            //                  return "y";
            //               case "Z":
            //                  return "b";
            //               case "0":
            //                  return "1";
            //               case "1":
            //                  return "9";
            //               case "2":
            //                  return "3";
            //               case "3":
            //                  return "6";
            //               case "4":
            //                  return "8";
            //               case "5":
            //                  return "7";
            //               case "6":
            //                  return "0";
            //               case "7":
            //                  return "2";
            //               case "8":
            //                  return "4";
            //               case "9":
            //                  return "5";
            //               default:
            //                  return strOut;
            //            }
         }
      }
      public static string MappingInfo(string strDataIn, bool encrypt, int seed)
      {

         if (strDataIn == null || strDataIn == "")
            return strDataIn;
         string strDataOut = "";
         for (int i = 0; i < strDataIn.Length; i++)
         {
            strDataOut = strDataOut + Mapping(strDataIn.Substring(i, 1), encrypt, seed);
         }

         return strDataOut;

      }
      public static void VerifyRenewalDate(string CustomerCode, string strIn, DateTime today, ref int errorCode, ref string errorMessage, ref DateTime validDate)
      {
         int year = 1990;
         int month = 1;
         int day = 1;
         errorCode = 0;
         errorMessage = "";
         //code = 1: expired, 2: will expired in 30 days, 3: code not valid, 4: it is for the user
         try
         {
            int seed = int.Parse(strIn.Substring(0, 1));
            strIn = strIn.Substring(1);
            strIn = MappingInfo(strIn, false, seed);
            string customer = "";
            if (strIn.Length < 16)
            {
               customer = strIn.Substring(8);
               strIn = strIn.Substring(0, 8);
            }
            else
            {
               customer = strIn.Substring(1, 1) + strIn.Substring(3, 1) + strIn.Substring(5, 1) + strIn.Substring(7, 1) + strIn.Substring(9, 1) + strIn.Substring(11, 1) + strIn.Substring(13, 1) + strIn.Substring(15);
               strIn = strIn.Substring(0, 1) + strIn.Substring(2, 1) + strIn.Substring(4, 1) + strIn.Substring(6, 1) + strIn.Substring(8, 1) + strIn.Substring(10, 1) + strIn.Substring(12, 1) + strIn.Substring(14, 1);
            }
            string newCustomer = "";
            for (int i = 1; i <= customer.Length; i++)
            {
               newCustomer = newCustomer + customer.Substring(customer.Length - i, 1);
            }
            customer = newCustomer;
            month = int.Parse(strIn.Substring(0, 2));
            day = int.Parse(strIn.Substring(2, 2));
            year = int.Parse(strIn.Substring(4));
            DateTime d = new DateTime(year, month, day);
            validDate = d;
            if (d.CompareTo(today) > 0)
            {
               if (customer != CustomerCode)
               {
                  errorMessage = @"Renewal code doesn't match for current customer and access has been disabled, please renew your subscription. If you do not have renewal code, please call 866-428-3997 or 780-428-3997 x.227 for renewal code.";
                  errorCode = 4;
                  return;
               }
               if (d.CompareTo(today.AddDays(30)) < 0)
               {
                  errorCode = 2;
                  errorMessage = @"ILS license will expire in " + d.Subtract(today).Days.ToString()
                     + " days, please renew your subscription. If you do not have renewal code, please call 866-428-3997 or 780-428-3997 x.227 for renewal code.";
               }
               return;
            }
            else
            {
               if (d.CompareTo(today.AddDays(-7)) < 0)
               {
                  errorCode = 3;
                  errorMessage = @"ILS license has expired and access has been disabled, please renew your subscription. If you do not have renewal code, please call 866-428-3997 or 780-428-3997 x.227 for renewal code.";
               }
               else
               {
                  errorMessage = @"ILS license has expired and access will be disabled in " + (7 - today.Subtract(d).Days).ToString() + " days, If you do not have renewal code, please call 866-428-3997 or 780-428-3997 x.227 for renewal code.";
                  errorCode = 1;
                  return;
               }
            }
         }
         catch
         {
            errorCode = 3;
            errorMessage = @"ILS license has expired and access has been disabled, please renew your subscription. If you do not have renewal code, please call 866-428-3997 or 780-428-3997 x.227 for renewal code.";
            return;
         }
      }
		public static string DecryptText(string txtToDecode)
		{
			try
			{
				ASCIIEncoding x = new ASCIIEncoding();
				byte[] key = x.GetBytes("keykkeykkeykkeykkeykkeykkeykkeyk");
				byte[] iv = x.GetBytes("iviviviviviviviv");

				byte[] bytes = Convert.FromBase64String(txtToDecode);
				MemoryStream ms = new System.IO.MemoryStream(bytes, 0, bytes.Length);
				SymmetricAlgorithm sa = new RijndaelManaged();
				CryptoStream crypto = new CryptoStream(ms, sa.CreateDecryptor(key, iv),
					CryptoStreamMode.Read);
				System.IO.StreamReader sr = new System.IO.StreamReader( crypto );
				string str = sr.ReadToEnd();
				str = str.Replace("\0","");
				return str;
			}
			catch
			{
				return "";
			}
		}

	}
}
