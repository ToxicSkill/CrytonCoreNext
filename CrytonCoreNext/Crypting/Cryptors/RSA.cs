using CrytonCoreNext.Abstract;
using CrytonCoreNext.Crypting.Helpers;
using CrytonCoreNext.Crypting.Interfaces;
using CrytonCoreNext.CryptingOptionsViewModels;
using CrytonCoreNext.Dictionaries;
using CrytonCoreNext.Interfaces;
using System;
using System.Threading.Tasks;

namespace CrytonCoreNext.Crypting.Cryptors
{
    public class RSA : ICrypting
    {
        private readonly RSAHelper _rsaHelper;

        private bool _useOAEP = false;

        public string Name => nameof(RSA);

        public int ProgressCount => 3;

        public ViewModelBase ViewModel { get; init; }

        public RSA(IJsonSerializer jsonSerializer, IXmlSerializer xmlSerializer, IProgressView progressView)
        {
            _rsaHelper = new(_useOAEP);
            ViewModel = new RSAViewModel(jsonSerializer, xmlSerializer, progressView, _rsaHelper, Name);
        }

        public ViewModelBase GetViewModel() => ViewModel;

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
            progress.Report(Language.Post("CollectingKeys"));

            if (ViewModel.IsBusy)
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
                progress.Report(Language.Post("Error"));
                return emptyArray;
            }

            progress.Report(Language.Post("Success"));
            return result;
        }
    }
}
