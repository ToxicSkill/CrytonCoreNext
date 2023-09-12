using System;

namespace CrytonCoreNext.Interfaces.Serializers
{
    public interface IJsonSerializer
    {
        string Serialize(object obj);

        object? Deserialize(string str, Type type);
    }
}
