using System;

namespace CrytonCoreNext.Interfaces
{
    public interface IJsonSerializer
    {
        void Serialize(object obj, string filePath);

        object Deserialize(string path, Type type);
    }
}
