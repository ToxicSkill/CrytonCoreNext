using CrytonCoreNext.Crypting.Enums;
using CrytonCoreNext.Crypting.Helpers;
using CrytonCoreNext.Crypting.Interfaces;
using CrytonCoreNext.Dictionaries;
using System;
using System.Threading.Tasks;

namespace CrytonCoreNext.Crypting.Cryptors
{
    public class RSA : ICrypting
    {
        private readonly RSAHelper _rsaHelper;

        private bool _useOAEP = false;

        public EMethod Method => EMethod.RSA;

        public string DescriptionName => $"{Method} - Asymmetric alorithm";

        public int ProgressCount => 3;

        public RSA()
        {
            _rsaHelper = new(_useOAEP);
        }

        public object GetHelper()
        {
            return _rsaHelper;
        }

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
