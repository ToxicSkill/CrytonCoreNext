using CrytonCoreNext.Interfaces.Serializers;
using Newtonsoft.Json;
using System;
using System.IO;

namespace CrytonCoreNext.Serializers
{
    public class JsonSerializer : IJsonSerializer
    {
        public void Serialize(object obj, string filePath)
        {
            var serializer = new Newtonsoft.Json.JsonSerializer();

            using (var sw = new StreamWriter(filePath))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, obj);
            }
        }

        public object Deserialize(string path, Type type)
        {
            var serializer = new Newtonsoft.Json.JsonSerializer();

            using (var sw = new StreamReader(path))
            using (var reader = new JsonTextReader(sw))
            {
                return serializer.Deserialize(reader, type);
            }
        }
    }
}
