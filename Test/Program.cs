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
    
    class Program
    {
        public static void Main(string[] args)
        {
            var pan = "2168890010081238";
            var a = pan.DES3Encrypt("d3d3Lnl1ZnUuY253d3cuZnVr");
            Console.WriteLine(a);
            Console.WriteLine(a.DES3Decrypt("d3d3Lnl1ZnUuY253d3cuZnVr"));
            Console.ReadLine();
        }

    }
}
