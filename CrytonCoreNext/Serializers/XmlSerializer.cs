using CrytonCoreNext.Interfaces.Serializers;
using System.IO;

namespace CrytonCoreNext.Serializers
{
    public class XmlSerializer : IXmlSerializer
    {
        public string RsaParameterKeyToString<T>(T parameter)
        {
            var stringWriter = new StringWriter();
            var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            xmlSerializer.Serialize(stringWriter, parameter);
            return stringWriter.ToString();
        }

        public T StringKeyToRsaParameter<T>(string key)
        {
            var stringReader = new StringReader(key);
            var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            return (T)xmlSerializer.Deserialize(stringReader);
        }
    }
}
