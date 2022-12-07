using CrytonCoreNext.Abstract;
using CrytonCoreNext.Crypting.Helpers;
using CrytonCoreNext.Crypting.Interfaces;
using CrytonCoreNext.CryptingOptionsViewModels;
using CrytonCoreNext.Dictionaries;
using CrytonCoreNext.Enums;
using CrytonCoreNext.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace CrytonCoreNext.Crypting.Cryptors
{
    public class RSA : ICrypting
    {
        private static readonly string[] SettingsKeys = { "Keys", "KeySize" };

        private static readonly byte[] DefaultBytes = Array.Empty<byte>();

        private readonly RSAHelper _rsaHelper;

        private int _keysSize;

        private bool _useOAEP = false;

        private RSAParameters _keys;

        private RSACryptoServiceProvider _rsa;

        public string Name => nameof(RSA);

        public int ProgressCount => 2;

        public ViewModelBase ViewModel { get; init; }

        public RSA(IJsonSerializer jsonSerializer, IXmlSerializer xmlSerializer, IProgressView progressView)
        {
            _rsaHelper = new(_useOAEP);
            ViewModel = new RSAViewModel(jsonSerializer, xmlSerializer, progressView, _rsaHelper, Name, SettingsKeys);
            _keysSize = _rsaHelper.DefaultKeySize;
            _rsa = new(_keysSize);
        }

        public ViewModelBase GetViewModel() => ViewModel;

        public string GetName() => Name;

        public async Task<byte[]> Decrypt(byte[] data, IProgress<string> progress)
        {
            progress.Report(Language.Post("CollectingKeys"));

            if (await Task.Run(() => !ParseSettingsObjects(ViewModel.GetObjects(), data.Length, false)))
            {
                return DefaultBytes;
            }
            else if (!_rsaHelper.IsKeyPrivate(_keys))
            {
                ViewModel.Log(ELogLevel.Error, Language.Post("NoPrivateKeyError"));
                return DefaultBytes;
            }

            progress.Report(Language.Post("Decrypting"));
            return await Task.Run(() => _rsa.Decrypt(data, _useOAEP));
        }

        public async Task<byte[]> Encrypt(byte[] data, IProgress<string> progress)
        {
            progress.Report(Language.Post("CollectingKeys"));
            if (await Task.Run(() => !ParseSettingsObjects(ViewModel.GetObjects(), data.Length, true)))
                return DefaultBytes;

            progress.Report(Language.Post("Encrypting"));
            return await Task.Run(() => _rsa.Encrypt(data, _useOAEP));
        }

        public bool ParseSettingsObjects(Dictionary<string, object> objects, int dataLength, bool encryption)
        {
            foreach (var setting in SettingsKeys)
            {
                if (!objects.ContainsKey(setting))
                {
                    return false;
                }
            }

            var key = (RSAParameters)objects[SettingsKeys[0]];
            var keySize = Convert.ToInt32(objects[SettingsKeys[1]]);

            if (encryption && _rsaHelper.GetMaxNumberOfBytes(keySize) < dataLength)
            {
                ViewModel.Log(Enums.ELogLevel.Error, Language.Post("TooBigFile")); // -> the child process calls the static method and overwrites it
                return false;
            }

            if (keySize != _keysSize)
            {
                _keysSize = keySize;
                _rsa = new(_keysSize);
            }

            if (!_rsaHelper.IsKeyEmpty(key))
            {
                _keys = key;
                _rsa.ImportParameters(_keys);
            }
            else
            {
                _keys = _rsa.ExportParameters(true);
            }

            UpdateViewModel();
            return true;
        }

        private void UpdateViewModel()
        {
            //ViewModel.ParseObjects(new()
            //{
            //    { SettingsKeys[0], _keys },
            //    { SettingsKeys[1], _rsa.KeySize }
            //});
        }

        public bool UpdateKeys(bool encryption)
        {
            throw new NotImplementedException();
        }
    }
}
