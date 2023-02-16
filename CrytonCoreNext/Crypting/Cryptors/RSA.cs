using CrytonCoreNext.Crypting.Helpers;
using CrytonCoreNext.Crypting.Interfaces;
using CrytonCoreNext.Crypting.Models;
using CrytonCoreNext.Crypting.ViewModels;
using CrytonCoreNext.Crypting.Views;
using CrytonCoreNext.Dictionaries;
using CrytonCoreNext.Interfaces;
using System;
using System.Threading.Tasks;
using Wpf.Ui.Mvvm.Contracts;

namespace CrytonCoreNext.Crypting.Cryptors
{
    public class RSA : ICrypting
    {
        private readonly RSAHelper _rsaHelper;

        private bool _useOAEP = false;

        public string Name => nameof(RSA);

        public string DescriptionName => $"{Name} - Asymmetric alorithm";

        public int ProgressCount => 3;

        public ICryptingView<CryptingMethodViewModel> ViewModel { get; init; }

        public RSA(ISnackbarService snackbarService, IJsonSerializer jsonSerializer, IXmlSerializer xmlSerializer)
        {
            _rsaHelper = new(_useOAEP);
            ViewModel = new RSAView(new RSAViewModel(snackbarService, jsonSerializer, xmlSerializer, _rsaHelper, Name));
        }

        public ICryptingView<CryptingMethodViewModel> GetViewModel() => ViewModel;

        public string GetName() => Name;

        public async Task<byte[]> Decrypt(byte[] data, IProgress<string> progress)
        {
            return await Task.Run(() => PerformCryptography(data, progress, false));
        }

        public async Task<byte[]> Encrypt(byte[] data, IProgress<string> progress)
        {
            return await Task.Run(() => PerformCryptography(data, progress, true));
        }

        private byte[] PerformCryptography(byte[] data, IProgress<string> progress, bool encryption)
        {
            var emptyArray = Array.Empty<byte>();
            progress.Report(Language.Post("GeneratingKeys"));
            if (encryption)
            {
                _rsaHelper.GenerateKey();
            }

            progress.Report(Language.Post("CollectingKeys"));

            if (ViewModel.ViewModel.IsBusy)
            {
                progress.Report(Language.Post("Busy"));
                return emptyArray;
            }

            var rsaCryptoService = _rsaHelper.GetRSACryptoServiceProvider();

            if (data.Length > _rsaHelper.MaxFileSize && encryption)
            {
                progress.Report(Language.Post("TooBigFile"));
                return emptyArray;
            }

            progress.Report(Language.Post(encryption ? "Encrypting" : "Decrypting"));
            byte[]? result;
            try
            {
                result = encryption ?
                    rsaCryptoService.Encrypt(data, _useOAEP) :
                    rsaCryptoService.Decrypt(data, _useOAEP);
            }
            catch (Exception)
            {
                return emptyArray;
            }
            return result;
        }
    }
}
