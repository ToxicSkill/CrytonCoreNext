using System.Collections.Generic;

namespace CrytonCoreNext.Serializers
{
    public static class JsonSerializer
    {
        public static string SerializeList(List<string> objects)
        {
           return System.Text.Json.JsonSerializer.Serialize(objects);
        }
    }
}
