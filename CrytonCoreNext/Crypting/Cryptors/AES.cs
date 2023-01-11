using CrytonCoreNext.Abstract;
using CrytonCoreNext.Crypting.Helpers;
using CrytonCoreNext.Crypting.Interfaces;
using CrytonCoreNext.CryptingOptionsViewModels;
using CrytonCoreNext.CryptingOptionsViews;
using CrytonCoreNext.Dictionaries;
using CrytonCoreNext.Interfaces;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Wpf.Ui.Common.Interfaces;
using Wpf.Ui.Mvvm.Contracts;

namespace CrytonCoreNext.Crypting.Cryptors
{
    public class AES : ICrypting
    {
        private readonly PaddingMode _paddingMode = PaddingMode.PKCS7;

        private readonly AesCng _aes;

        private readonly AESHelper _aesHelper;

        public string Name => nameof(AES);

        public string DescriptionName => $"{Name} - Symmetric alorithm";

        public int ProgressCount => 2;

        public INavigableView<ViewModelBase> ViewModel { get; init; }

        public AES(ISnackbarService snackbarService, IJsonSerializer jsonSerializer)
        {
            _aes = new();
            _aesHelper = new AESHelper(_aes, _paddingMode);
            ViewModel = new AESView(new AESViewModel(snackbarService, jsonSerializer, _aesHelper, Name));
        }

        public INavigableView<ViewModelBase> GetViewModel() => ViewModel;

        public string GetName() => Name;

        public async Task<byte[]> Encrypt(byte[] data, IProgress<string> progress)
        {
            using var encryptor = _aes.CreateEncryptor(_aes.Key, _aes.IV);
            return await Task.Run(() => PerformCryptography(data, encryptor, progress));
        }

        public async Task<byte[]> Decrypt(byte[] data, IProgress<string> progress)
        {
            using var decryptor = _aes.CreateDecryptor(_aes.Key, _aes.IV);
            return await Task.Run(() => PerformCryptography(data, decryptor, progress));
        }

        private byte[] PerformCryptography(byte[] data, ICryptoTransform cryptoTransform, IProgress<string> progress)
        {
            progress.Report(Language.Post("CollectingKeys"));
            using var ms = new MemoryStream();
            using var cryptoStream = new CryptoStream(ms, cryptoTransform, CryptoStreamMode.Write);
            cryptoStream.Write(data, 0, data.Length);
            try
            {
                cryptoStream.FlushFinalBlock();
            }
            catch (Exception)
            {
                progress.Report(Language.Post("Error"));
                return Array.Empty<byte>();
            }

            progress.Report(Language.Post("Success"));
            return ms.ToArray();
        }
    }
}
