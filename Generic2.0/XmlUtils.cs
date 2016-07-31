using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Runtime.Serialization;

using System.IO;

namespace Generic
{
    public class XmlUtils
    {
        public static string Serialize<T>(T input)
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(T));
            MemoryStream stream = new MemoryStream();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;
            using (XmlWriter writer = XmlWriter.Create(stream, settings))
            {
                serializer.WriteObject(writer, input);
            }

            return Encoding.UTF8.GetString(stream.ToArray());
        }
    }
}
