namespace CrytonCoreNext.Interfaces
{
    public interface IXmlSerializer
    {
        string RsaParameterKeyToString<T>(T parameter);

        T StringKeyToRsaParameter<T>(string key);
    }
}
