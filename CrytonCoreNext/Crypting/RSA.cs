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
        private static readonly string[] SettingsKeys = { "Keys", "KeySize", "Error" };

        private int _keySize = 2048;

        private bool _useOAEP = false;

        private RSAParameters _keys;

        public string Name => nameof(RSA);

        public int ProgressCount => 3;

        public ViewModelBase ViewModel { get; init; }

        private RSACryptoServiceProvider _rsa;

        public RSA(IJsonSerializer jsonSerializer, IXmlSerializer xmlSerializer)
        {
            ViewModel = new RSAViewModel(jsonSerializer, xmlSerializer, GetMaxNumberOfBytes, new RSACryptoServiceProvider().LegalKeySizes[0], _keySize, SettingsKeys, Name);
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
            progress.Report("Encrypting");
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

            //if ((!AreObjectsValid(key, keySize) && IsKeyEmpty(_keys)) || (encryption && IsFileTooBig(keySize, dataLength)))
            //{
            //    return false;
            //}

            if (encryption)
            {
                // Two options:
                // keys are imported
                // no keys imported - generate new

                if (!IsKeyEmpty(key)) // keys imported
                {
                    _keys = key;
                    _rsa.ImportParameters(_keys);
                }
                else // no keys - generate new
                {
                    if (keySize != _keySize)
                    {
                        _keySize = keySize;
                    }

                    _rsa = new(_keySize);
                    _keys = _rsa.ExportParameters(true);
                }

                UpdateViewModel();
                return true;
            }
            else
            {
                if (!IsKeyEmpty(key) && IsKeyPrivate(key))
                {
                    _rsa = new();
                    _rsa.ImportParameters(key);
                }
                else
                {
                    _rsa = new(_keySize);
                }

                UpdateViewModel();
                return true;
            }

            UpdateViewModel("Unable to create RSA cryptor");
            return false;
        }
        private bool AreObjectsValid(RSAParameters parameters, int keySize)
        {
            return !IsKeyEmpty(parameters) && IsKeyValid(keySize);
        }

        private bool IsFileTooBig(int keySize, int dataLength)
        {
            if (GetMaxNumberOfBytes(keySize) < dataLength)
            {
                UpdateViewModel("File is too big");
                return false;
            }

            return true;
        }

        private void UpdateViewModel(string error = "")
        {
            ViewModel.SetObjects(new()
            {
                { SettingsKeys[0], _keys },
                { SettingsKeys[1], _rsa.KeySize },
                { SettingsKeys[2], error }
            });
        }

        private int GetMaxNumberOfBytes(int keySize)
        {
            return _useOAEP ? ((keySize - 384) / 8) + 37 : ((keySize - 384) / 8) + 7;
        }

        private bool IsKeyEmpty(RSAParameters parameters)
        {
            return parameters.Modulus == null;
        }
        private bool IsKeyPrivate(RSAParameters parameters)
        {
            return parameters.D != null;
        }

        private bool IsKeyValid(int keySize)
        {
            return _keySize != keySize &&
                keySize % 128 == 0 &&
                keySize <= _rsa.LegalKeySizes[0].MaxSize &&
                keySize >= _rsa.LegalKeySizes[0].MinSize;
        }
    }
}
