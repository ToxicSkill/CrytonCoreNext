using CrytonCoreNext.Abstract;
using CrytonCoreNext.CryptingOptionsViewModels;
using CrytonCoreNext.Interfaces;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace CrytonCoreNext.Crypting
{
    public class RSA : ICrypting
    {
        private static readonly string[] SettingsKeys = { "Key", "IV", "KeySize", "BlockSize", "Error" };
        private const string Name = "RSA";

        public ViewModelBase ViewModel { get; set; }

        private readonly RSACng _rsa;

        public RSA()
        {
            _rsa = new RSACng();
            ViewModel = new RSAViewModel(_rsa, SettingsKeys, Name);
        }

        public ViewModelBase GetViewModel() => ViewModel;

        public string GetName() => Name;

        public byte[] Decrypt(byte[] data)
        {
            throw new System.NotImplementedException();
        }

        public byte[] Encrypt(byte[] data)
        {
            throw new System.NotImplementedException();
        }

        public void ParseSettingsObjects(Dictionary<string, object> objects)
        {
            throw new System.NotImplementedException();
        }

        public bool ParseSettingsObjects(Dictionary<string, object> objects, bool encryption)
        {
            throw new System.NotImplementedException();
        }
    }
}
