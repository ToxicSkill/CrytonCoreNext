using CrytonCoreNext.Interfaces.Serializers;
using Newtonsoft.Json;
using System; 

namespace CrytonCoreNext.Serializers
{
    public class JsonSerializer : IJsonSerializer
    {
        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        }

        public object? Deserialize(string str, Type type)
        {
            return JsonConvert.DeserializeObject(str, type);
        }
    }
}
