using System;

namespace CrytonCoreNext.Interfaces.Serializers
{
    public interface IJsonSerializer
    {
        void Serialize(object obj, string filePath);

        object Deserialize(string path, Type type);
    }
}
