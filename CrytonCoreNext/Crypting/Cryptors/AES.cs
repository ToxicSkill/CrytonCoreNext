using CrytonCoreNext.Abstract;
using CrytonCoreNext.Crypting.Helpers;
using CrytonCoreNext.Crypting.Interfaces;
using CrytonCoreNext.CryptingOptionsViewModels;
using CrytonCoreNext.Dictionaries;
using CrytonCoreNext.Extensions;
using CrytonCoreNext.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace CrytonCoreNext.Crypting.Cryptors
{
    public class AES : ICrypting
    {
        private static readonly string[] SettingsKeys = { "Key", "IV", "KeySize", "BlockSize" };

        private readonly PaddingMode _paddingMode = PaddingMode.PKCS7;

        private readonly AesCng _aes;

        private readonly AESHelper _aesHelper;

        public string Name => nameof(AES);

        public int ProgressCount => 5;

        public ViewModelBase ViewModel { get; init; }

        public AES(IJsonSerializer jsonSerializer)
        {
            _aes = new();
            _aesHelper = new AESHelper(_aes, _paddingMode);
            ViewModel = new AESViewModel(jsonSerializer, _aesHelper, SettingsKeys, Name);
        }

        public ViewModelBase GetViewModel() => ViewModel;

        public string GetName() => Name;

        public async Task<byte[]> Encrypt(byte[] data, IProgress<string> progress)
        {
            if (!ParseSettingsObjects(ViewModel.GetObjects(), data.Length, true))
                return Array.Empty<byte>();

            _aes.Padding = _paddingMode;

            using var encryptor = _aes.CreateEncryptor(_aes.Key, _aes.IV);
            var cryptographyResult = await Task.Run(() => PerformCryptography(data, encryptor, progress));
            return cryptographyResult;
        }

        public async Task<byte[]> Decrypt(byte[] data, IProgress<string> progress)
        {
            if (!ParseSettingsObjects(ViewModel.GetObjects(), data.Length, false))
                return Array.Empty<byte>();

            _aes.Padding = _paddingMode;

            using var decryptor = _aes.CreateDecryptor(_aes.Key, _aes.IV);
            return await Task.Run(() => PerformCryptography(data, decryptor, progress));
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
            var blockSize = Convert.ToInt32(objects[SettingsKeys[3]]);
            _aes.KeySize = Convert.ToInt32(keySize);
            _aes.BlockSize = Convert.ToInt32(objects[SettingsKeys[3]]);

            if (objects[SettingsKeys[0]] is string key &&
                objects[SettingsKeys[1]] is string iv)
            {
                if (Equals(key.Length, keySize / 4) &&
                    Equals(iv.Length, blockSize / 4))
                {
                    _aes.Key = key.Str2Bytes();
                    _aes.IV = iv.Str2Bytes();
                    return true;
                }
            }

            if (encryption)
            {
                _aes.GenerateKey();
                _aes.GenerateIV();
                UpdateViewModel();
                return true;
            }

            ViewModel.Log(Enums.ELogLevel.Error, Language.Post("IncorrectKeys"));
            return false;
        }

        private byte[] PerformCryptography(byte[] data, ICryptoTransform cryptoTransform, IProgress<string> progress)
        {
            progress.Report("Preparing");
            using var ms = new MemoryStream();
            using var cryptoStream = new CryptoStream(ms, cryptoTransform, CryptoStreamMode.Write);
            cryptoStream.Write(data, 0, data.Length);
            progress.Report("Starting");
            try
            {
                cryptoStream.FlushFinalBlock();
                progress.Report("Finished");
            }
            catch (Exception)
            {
                ViewModel.Log(Enums.ELogLevel.Error, Language.Post("IncorrectKeys"));
                progress.Report("Process failed");
                return Array.Empty<byte>();
            }

            progress.Report("Updating data");
            UpdateViewModel();
            progress.Report("Success");
            return ms.ToArray();
        }

        private void UpdateViewModel()
        {
            ViewModel.SetObjects(new()
            {
                { SettingsKeys[0], _aes.Key },
                { SettingsKeys[1], _aes.IV },
                { SettingsKeys[2], _aes.KeySize },
                { SettingsKeys[3], _aes.BlockSize }
            });
        }
    }
}
