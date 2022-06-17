using CrytonCoreNext.Abstract;
using CrytonCoreNext.Interfaces;
using System.IO;
using System.Security.Cryptography;

namespace CrytonCoreNext.Crypting
{
    public class RSA : ICrypting
    {
        private const string Name = "RSA";

        public ViewModelBase ViewModel { get; set; }

        public RSA(ViewModelBase viewModel)
        {
            ViewModel = viewModel;
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
    }
}
