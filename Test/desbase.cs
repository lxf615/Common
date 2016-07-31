using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace Test
{
    public class Desbase
    {


        private static Encoding CurrentEncoding = Encoding.UTF8;//Encoding.GetEncoding("GB2312");

        public static string Encrypt2(string toEncrypt, string key, string ivv)
        {
            try
            {
                Encoding code = Encoding.Default;
                byte[] keyArray;
                byte[] toEncryptArray = GetdesDate(toEncrypt);

                keyArray = GetdesDate(key);
                string ivcode = ivv;
                byte[] iv = GetdesDate(ivcode);
                byte[] date = new byte[100];
                date = _3DES2Encrypt(toEncryptArray, keyArray, iv);
                return byteToHexStr(date);
            }
            catch (Exception ee)
            {

                return string.Empty;
            }
        }

        /// <summary>
        /// 3des将密文解密以16进制形式表示
        /// </summary>
        /// <param name="toDecrypt"></param>
        /// <param name="key"></param>
        /// <param name="ivv"></param>
        /// <returns></returns>
        public static string Decrypt2(string toDecrypt,string key,string ivv)
        {
            try
            {
                Encoding code = Encoding.Default;
                byte[] keyArray;
                byte[] toEncryptArray = GetdesDate(toDecrypt);
                
                keyArray = GetdesDate(key);
                string ivcode = ivv;
                byte[] iv = GetdesDate(ivcode);
                byte[] date = new byte[100];
                date = _3DES2Descrypt(toEncryptArray, keyArray, iv);
                return byteToHexStr(date);
            }
            catch (Exception ee)
            {

                return string.Empty;
            }

        }

        public static string XOR(string str1,string str2)
        {
            byte[] data1 = Desbase.GetdesDate(str1);
            byte[] data2 = Desbase.GetdesDate(str2);
            byte[] data3 = new byte[data1.Length];
            for (int i = 0; i < data1.Length; i++)
            {
                data3[i] = (byte)(data1[i] ^ data2[i]);
            }
            return byteToHexStr(data3);
        }

        public static string byteToHexStr(byte[] bytes)
        {
            string returnStr = ""; 
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            } return returnStr;
        }

        /// ECB解密
        /// </summary>
        /// <param name="sourceDataBytes"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static string Decrypt(string toDecrypt)
        {
            //return Desbase.TripleDesbase64Decode(toDecrypt);
            try
            {
                Encoding code = Encoding.Default;
                byte[] keyArray;
                byte[] toEncryptArray = GetdesDate(toDecrypt);

                //MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                //keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes("111122225555666688889999AAAACCCC"));
                keyArray = GetdesDate("111122225555666688889999AAAACCCC");
                string ivcode = "0000000000000000";
                byte[] iv = GetdesDate(ivcode);
                byte[] date = new byte[100];
                date = _3DES2Descrypt(toEncryptArray, keyArray, iv);
                string ss = code.GetString(date);
                return ss.Replace("\0","").Trim().ToString();
            }
            catch (Exception ee)
            {
                
                 return string.Empty;
            }
           
        }

        public static byte[] GetdesDate(string toDecrypt)
        {
            int length = (toDecrypt.Length / 2);
            byte[] inputByte = new byte[length];
            for (int index = 0; index < length; index++)
            {
                string substring = toDecrypt.Substring(index * 2, 2);
                inputByte[index] = Convert.ToByte(substring, 16);
            }
            return inputByte;

        }

        public static byte[] _3DES2Encrypt(byte[] data, byte[] key, byte[] iv)
        {

            byte[] key1 = new byte[8];
            Array.Copy(key, 0, key1, 0, 8);
            byte[] key2 = new byte[8];
            Array.Copy(key, 8, key2, 0, 8);

            byte[] data1 = EncryptECB(data, key1, iv);
            data1 = DecryptECB(data1, key2, iv);
            data1 = EncryptECB(data1, key1, iv);
            return data1;
        }

        public static byte[] _3DES2Descrypt(byte[] data, byte[] key, byte[] iv)
        {

            byte[] key1 = new byte[8];
            Array.Copy(key, 0, key1, 0, 8);
            byte[] key2 = new byte[8];
            Array.Copy(key, 8, key2, 0, 8);

            byte[] data1 = DecryptECB(data, key1, iv);
            data1 = EncryptECB(data1, key2, iv);
            data1 = DecryptECB(data1, key1, iv);
            return data1;
        }

        public static byte[] DecryptECB(byte[] encryptedDataBytes, byte[] keys, Byte[] iv)
        {
            MemoryStream tempStream = new MemoryStream(encryptedDataBytes, 0, encryptedDataBytes.Length);
            DESCryptoServiceProvider decryptor = new DESCryptoServiceProvider();
            decryptor.Mode = CipherMode.ECB;
            decryptor.Padding = PaddingMode.None;
            CryptoStream decryptionStream = new CryptoStream(tempStream, decryptor.CreateDecryptor(keys, iv), CryptoStreamMode.Read);
            StreamReader allDataReader = new StreamReader(decryptionStream);
            byte[] data = new byte[encryptedDataBytes.Length];
            decryptionStream.Read(data, 0, data.Length);
            decryptionStream.Close();
            tempStream.Close();
            return data;

        }

        /// ECB加密
        /// </summary>
        /// <param name="sourceDataBytes"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static byte[] EncryptECB(byte[] sourceDataBytes, byte[] keys, Byte[] iv)
        {

            //Byte[] iv = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            MemoryStream tempStream = new MemoryStream();
            //get encryptor and encryption stream
            DESCryptoServiceProvider encryptor = new DESCryptoServiceProvider();
            encryptor.Mode = CipherMode.ECB;
            encryptor.Padding = PaddingMode.None;
            CryptoStream encryptionStream = new CryptoStream(tempStream, encryptor.CreateEncryptor(keys, iv), CryptoStreamMode.Write);
            encryptionStream.Write(sourceDataBytes, 0, sourceDataBytes.Length);
            encryptionStream.FlushFinalBlock();
            encryptionStream.Close();
            byte[] encryptedDataBytes = tempStream.ToArray();
            tempStream.Close();
            return encryptedDataBytes;
        }
    }
}