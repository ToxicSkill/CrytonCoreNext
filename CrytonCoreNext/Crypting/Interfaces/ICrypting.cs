using CrytonCoreNext.Abstract;
using System;
using System.Threading.Tasks;
using Wpf.Ui.Common.Interfaces;

namespace CrytonCoreNext.Crypting.Interfaces
{
    public interface ICrypting
    {
        public string Name { get; }

        public string DescriptionName { get; }

        public int ProgressCount { get; }

        Task<byte[]> Encrypt(byte[] data, IProgress<string> progress);

        Task<byte[]> Decrypt(byte[] data, IProgress<string> progress);

        INavigableView<ViewModelBase> GetViewModel();

        string GetName();
    }
}
