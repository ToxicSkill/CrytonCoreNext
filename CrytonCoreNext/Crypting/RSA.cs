using CrytonCoreNext.Abstract;
using CrytonCoreNext.CryptingOptionsViewModels;
using CrytonCoreNext.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;

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

        public async Task<byte[]> Decrypt(byte[] data, IProgress<string> progress)
        {
            return await Task.FromResult<byte[]>(Array.Empty<byte>());
        }

        public async Task<byte[]> Encrypt(byte[] data, IProgress<string> progress)
        {
            return await Task.FromResult<byte[]>(Array.Empty<byte>());
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
