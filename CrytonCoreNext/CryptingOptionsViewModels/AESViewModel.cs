using CrytonCoreNext.Abstract;
using CrytonCoreNext.Commands;
using CrytonCoreNext.Crypting;
using CrytonCoreNext.Helpers;
using CrytonCoreNext.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;

namespace CrytonCoreNext.CryptingOptionsViewModels
{
    public class AESViewModel : ViewModelBase
    {
        private readonly string[] SettingsKeys;
        private string _selectedKey;
        private string _selectedBlock;

        public ObservableCollection<string> BlockSizesComboBox { get; init; }

        public ObservableCollection<string> KeySizesComboBox { get; init; }

        public string SelectedBlockSize
        {
            get => _selectedBlock;
            set
            {
                if (_selectedBlock != value)
                {
                    _selectedBlock = value;
                    SelectedIVSize = Convert.ToInt32(_selectedBlock) / 4;
                    OnPropertyChanged(nameof(SelectedBlockSize));
                    OnPropertyChanged(nameof(SelectedIVSize));
                }
            }
        }

        public string SelectedKey 
        {
            get => _selectedKey;
            set
            {
                if (_selectedKey != value)
                {
                    _selectedKey = value;
                    SelectedKeySize = Convert.ToInt32(_selectedKey) / 4;
                    OnPropertyChanged(nameof(SelectedKey));
                    OnPropertyChanged(nameof(SelectedKeySize));
                }
            }
        }

        public string BlocksSizeName { get; init; }

        public string KeyName { get; init; }

        public string IVName { get; init; }

        public string KeysName { get; init; }

        public string GenerateRandomKeyName { get; init; }

        public string GenerateRandomIVName { get; init; }

        public string SaveCryptorName { get; init; }

        public int SelectedKeySize { get; set; }

        public int SelectedIVSize { get; set; }

        public string Key { get; set; }

        public string IV { get; set; }

        public string Error { get; private set; }

        public ICommand GenerateRandomKeyCommand { get; init; }

        public ICommand GenerateRandomIVCommand { get; init; }

        public ICommand SaveCryptorCommand { get; init; }

        public AESViewModel(AesCng aes, string[] settingKeys, string pageName) : base(pageName)
        {
            GenerateRandomKeyCommand = new Command(GenerateRandomKey, true);
            GenerateRandomIVCommand = new Command(GenerateRandomIV, true);
            SaveCryptorCommand = new Command(SaveCryptor, true);

            SettingsKeys = settingKeys;
            BlocksSizeName = "Block size";
            KeysName = "Key size";
            KeyName = "Key";
            IVName = "IV";
            GenerateRandomKeyName = "Generate random Key";
            GenerateRandomIVName = "Generate random IV";
            SaveCryptorName = "Save";

            BlockSizesComboBox = new ();
            KeySizesComboBox = new ();

            var legalKeys = aes.LegalKeySizes[0];
            var legalBlocks = aes.LegalBlockSizes[0];

            if (legalKeys.SkipSize != 0)
            {
                for (var i = legalKeys.MinSize; i <= legalKeys.MaxSize; i+= legalKeys.SkipSize)
                {
                    KeySizesComboBox.Add(i.ToString());
                }
            }
            else
            {
                KeySizesComboBox.Add(legalKeys.MinSize.ToString());
            }
            if (legalBlocks.SkipSize != 0)
            {
                for (var i = legalBlocks.MinSize; i <= legalBlocks.MaxSize; i += legalBlocks.SkipSize)
                {
                    BlockSizesComboBox.Add(i.ToString());
                }
            }
            else
            {
                BlockSizesComboBox.Add(legalBlocks.MinSize.ToString());
            }

            SelectedBlockSize = BlockSizesComboBox.First();
            SelectedKey = KeySizesComboBox.First();
            OnPropertyChanged(nameof(BlockSizesComboBox));
            OnPropertyChanged(nameof(KeySizesComboBox));
        }

        public override Dictionary<string, object> GetObjects()
        {
            return new ()
            {
                { SettingsKeys[0], Key },
                { SettingsKeys[1], IV },
                { SettingsKeys[2], SelectedKey },
                { SettingsKeys[3], SelectedBlockSize },
                { SettingsKeys[4], string.Empty }
            };
        }

        public override void SetObjects(Dictionary<string, object> objects)
        {
            foreach (var setting in SettingsKeys)
            {
                if (!objects.ContainsKey(setting))
                {
                    return;
                }
            }

            if (objects[SettingsKeys[4]] is string error)
            {
                Error = error;
                OnPropertyChanged(nameof(Error));
                if (!string.IsNullOrEmpty(error))
                {
                    return;
                }
            }

            var key = objects[SettingsKeys[0]] as byte[];
            var iv = objects[SettingsKeys[1]] as byte[];
            if (key != null &&
                iv != null)
            {
                Key = Convert.ToHexString(key);
                IV = Convert.ToHexString(iv);
                OnPropertyChanged(nameof(Key));
                OnPropertyChanged(nameof(IV));
            }
        }

        private void SaveCryptor()
        {
            var cryptorOutput = CrytonCoreNext.Serializers.JsonSerializer.SerializeList(new List<string>()
            {
                SelectedKeySize.ToString(), Key, SelectedBlockSize.ToString(), IV
            });
            WindowDialog.SaveDialog saveDialog = new(new DialogHelper()
            {
                Filters = Enums.EDialogFilters.ExtensionToFilter(Enums.EDialogFilters.DialogFilters.All),
                Multiselect = false,
                Title = (string)(Application.Current as App).Resources.MergedDictionaries[0]["OpenFileDialog"]
            });

            var saveDestination = saveDialog.RunDialog();
            if (saveDestination != null)
            {
                TextWriter tw = new StreamWriter(saveDestination.First());

                foreach (var s in cryptorOutput)
                    tw.WriteLine(s);

                tw.Close();
            }
        }

        private void GenerateRandomKey()
        {
            Key = RandomCryptoGenerator.GetCryptoRandomBytesString(SelectedKeySize / 2);
            OnPropertyChanged(nameof(Key));
        }

        private void GenerateRandomIV()
        {
            IV = RandomCryptoGenerator.GetCryptoRandomBytesString(SelectedIVSize / 2);
            OnPropertyChanged(nameof(IV));
        }
    }
}
