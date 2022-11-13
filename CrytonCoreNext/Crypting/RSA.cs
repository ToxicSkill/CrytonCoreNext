using CrytonCoreNext.Abstract;
using CrytonCoreNext.CryptingOptionsViewModels;
using CrytonCoreNext.Helpers;
using CrytonCoreNext.Interfaces;
using CrytonCoreNext.Logger;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows;

namespace CrytonCoreNext.Crypting
{
    public class RSA : ICrypting
    {
        private static readonly string[] SettingsKeys = { "Keys", "KeySize", "Logger" };

        private static readonly byte[] DefaultBytes = Array.Empty<byte>();

        private readonly RSAHelper _rsaHelper;

        private int _keysSize;

        private bool _useOAEP = false;

        private RSAParameters _keys;

        private RSACryptoServiceProvider _rsa;

        public string Name => nameof(RSA);

        public int ProgressCount => 2;

        public ViewModelBase ViewModel { get; init; }

        public RSA(IJsonSerializer jsonSerializer, IXmlSerializer xmlSerializer)
        {
            _rsaHelper = new(_useOAEP);
            ViewModel = new RSAViewModel(jsonSerializer, xmlSerializer, _rsaHelper, Name, SettingsKeys);
            _keysSize = _rsaHelper.DefaultKeySize;
            _rsa = new(_keysSize);
        }

        public ViewModelBase GetViewModel() => ViewModel;

        public string GetName() => Name;

        public async Task<byte[]> Decrypt(byte[] data, IProgress<string> progress)
        {
            progress.Report(Application.Current.Resources.MergedDictionaries[0]["CollectingKeys"].ToString() ?? string.Empty);

            if (await Task.Run(() => !ParseSettingsObjects(ViewModel.GetObjects(), data.Length, false)))
            {
                return DefaultBytes;
            }
            else if (!_rsaHelper.IsKeyPrivate(_keys))
            {
                UpdateViewModel(message: Application.Current.Resources.MergedDictionaries[0]["NoPrivateKeyError"].ToString() ?? string.Empty);
                return DefaultBytes;
            }

            progress.Report(Application.Current.Resources.MergedDictionaries[0]["Decrypting"].ToString() ?? string.Empty);
            return await Task.Run(() => _rsa.Decrypt(data, _useOAEP));
        }

        public async Task<byte[]> Encrypt(byte[] data, IProgress<string> progress)
        {
            progress.Report(Application.Current.Resources.MergedDictionaries[0]["CollectingKeys"].ToString() ?? string.Empty);

            if (await Task.Run(() => !ParseSettingsObjects(ViewModel.GetObjects(), data.Length, true)))
                return DefaultBytes;

            progress.Report(Application.Current.Resources.MergedDictionaries[0]["Encrypting"].ToString() ?? string.Empty);
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
                UpdateViewModel(message: Application.Current.Resources.MergedDictionaries[0]["TooBigFile"].ToString() ?? string.Empty);
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

        private void UpdateViewModel(string message = "")
        {
            ViewModel.SetObjects(new()
            {
                { SettingsKeys[0], _keys },
                { SettingsKeys[1], _rsa.KeySize },
                { SettingsKeys[2], message == string.Empty ? new Log() : new Log(Enums.ELogLevel.Error, message) }
            });
        }
    }
}
