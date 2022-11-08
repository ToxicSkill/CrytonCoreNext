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
        private static readonly string[] SettingsKeys = { "PublicKey", "PrivateKey", "KeySize", "Error" };

        private int _keySize = 2048;

        private bool _useOAEP = false;

        private RSAParameters _publicKey;

        private RSAParameters _privateKey;

        public string Name => nameof(RSA);

        public int ProgressCount => 3;

        public ViewModelBase ViewModel { get; init; }

        private RSACryptoServiceProvider _rsa;

        public RSA(IJsonSerializer jsonSerializer, IXmlSerializer xmlSerializer)
        {
            _rsa = new(_keySize);
            ViewModel = new RSAViewModel(jsonSerializer, xmlSerializer, GetMaxNumberOfBytes, _rsa.LegalKeySizes[0], _keySize, SettingsKeys, Name);
        }

        public ViewModelBase GetViewModel() => ViewModel;

        public string GetName() => Name;

        public async Task<byte[]> Decrypt(byte[] data, IProgress<string> progress)
        {
            progress.Report("Collecting keys");

            if (await Task.Run(() => !ParseSettingsObjects(ViewModel.GetObjects(), data.Length, false)))
                return Array.Empty<byte>();

            progress.Report("Preparing keys");
            var privateKey = _rsa.ExportParameters(true);
            var csp = new RSACryptoServiceProvider();
            csp.ImportParameters(privateKey);
            progress.Report("Decrypting");
            return await Task.Run(() => csp.Decrypt(data, _useOAEP));
        }

        public async Task<byte[]> Encrypt(byte[] data, IProgress<string> progress)
        {
            progress.Report("Collecting keys");

            if (await Task.Run(() => !ParseSettingsObjects(ViewModel.GetObjects(), data.Length, true)))
                return Array.Empty<byte>();

            progress.Report("Preparing keys");
            var csp = new RSACryptoServiceProvider();
            csp.ImportParameters(_publicKey);
            progress.Report("Encrypting");
            return await Task.Run(() => csp.Encrypt(data, _useOAEP));
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

            var keySize = Convert.ToInt32(objects[SettingsKeys[2]]);
            if (_keySize != keySize &&
                keySize % 128 == 0 &&
                keySize <= _rsa.LegalKeySizes[0].MaxSize &&
                keySize >= _rsa.LegalKeySizes[0].MinSize &&
                _publicKey.Modulus == null &&
                _privateKey.Modulus == null)
            {
                _keySize = keySize;
                _rsa = new(_keySize);
            }

            if (encryption)
            {
                if (GetMaxNumberOfBytes(_keySize) < dataLength)
                {
                    UpdateViewModel("File is too big");
                    return false;
                }

                _privateKey = _rsa.ExportParameters(true);
                _publicKey = _rsa.ExportParameters(false);
                UpdateViewModel();
                return true;
            }
            else
            {
                if (objects[SettingsKeys[0]] is RSAParameters publicKey &&
                    objects[SettingsKeys[1]] is RSAParameters privateKey)
                {
                    _publicKey = publicKey;
                    _privateKey = privateKey;
                    _rsa.ImportParameters(_privateKey);
                    UpdateViewModel();
                    return true;
                }
            }

            UpdateViewModel("Unable to create RSA cryptor");
            return false;
        }
        private void UpdateViewModel(string error = "")
        {
            ViewModel.SetObjects(new()
            {
                { SettingsKeys[0], _publicKey },
                { SettingsKeys[1], _privateKey },
                { SettingsKeys[2], _rsa.KeySize },
                { SettingsKeys[3], error }
            });
        }

        private int GetMaxNumberOfBytes(int keySize)
        {
            return _useOAEP ? ((keySize - 384) / 8) + 37 : ((keySize - 384) / 8) + 7;
        }
    }
}
