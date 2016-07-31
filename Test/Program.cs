using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Xml.Serialization;
using System.Xml;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;

using Newtonsoft.Json;
using AutoMapper;
using Generic;
namespace Test
{
    public abstract class A
    {

        public A()
        {
            PrintFields();
            short s1 = 1; s1 += 1;
        }

        public virtual void PrintFields() { }

    
    }

    public class B : A
    {

        public int x = 1;
        int y;
        public B()
        {
            y = -1;
        }

        public override void PrintFields()
        {
            Console.WriteLine("x={0},y={1}", x, y);
        }
    }


    public class C
    {
        public void Foo(B b)
        {

            lock (this)
            {

                if (b.x > 2)
                {
                    b.x--;
                    Foo(b);
                }
            }
        }
    }
   

    class Program
    {
        public static void Main(string[] args)
        {
            B b = new B();
            b.x = 5;
            C c = new C();
            c.Foo(b);
            Console.ReadLine();
        }

        

        public static int foo(int k)
        {
            if (k <= 0)
            {
                return 0;
            }

            if (k <= 2)
            {
                return 1;
            }

            int first = 0;
            int middle = 0;
            int last = 0;
            for (int i = 0; i <= k; i++)
            {
                if (i == 0)
                {
                    first = 0;
                    continue;
                }
                if (i == 1)
                {
                    middle = 1;
                    continue;
                }

                last = first + middle;
                first = middle;
                middle = last;
            }
            return last;
        }

        private static SymmetricAlgorithm mCSP;
        private static string txtKey = "12345ABCDE67890EFDCA1234567890EF";
        private static string txtIV = "0000000000000000";
        private static string EncryptString(string Value)
        {
            ICryptoTransform ct;
            MemoryStream ms;
            CryptoStream cs;
            byte[] byt;
            ct = mCSP.CreateEncryptor(mCSP.Key, mCSP.IV);
            byt = Encoding.UTF8.GetBytes(Value);
            ms = new MemoryStream();
            cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
            cs.Write(byt, 0, byt.Length);
            cs.FlushFinalBlock();
            cs.Close();
            return Convert.ToBase64String(ms.ToArray());
        }

        private static void btnKeyGen()
        {
            mCSP = SetEnc();
            byte[] byt2 = Convert.FromBase64String(txtKey);
            mCSP.Key = byt2;
        }
        private static void btnIVGen()
        {
            byte[] byt2 = Convert.FromBase64String(txtIV);
            mCSP.IV = byt2;
        }

        private static SymmetricAlgorithm SetEnc()
        {
            return new DESCryptoServiceProvider();
        }
    }
}
