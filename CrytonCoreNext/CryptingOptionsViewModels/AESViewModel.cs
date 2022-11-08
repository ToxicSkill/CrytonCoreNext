using CrytonCoreNext.Abstract;
using CrytonCoreNext.Commands;
using CrytonCoreNext.Crypting;
using CrytonCoreNext.Helpers;
using CrytonCoreNext.Interfaces;
using CrytonCoreNext.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Input;

namespace CrytonCoreNext.CryptingOptionsViewModels
{
    public class AESViewModel : ViewModelBase
    {
        private readonly string[] SettingsKeys;

        private string _selectedKey;

        private string _selectedBlock;

        private readonly IJsonSerializer _jsonSerializer;

        public ObservableCollection<string> BlockSizesComboBox { get; init; }

        public ObservableCollection<string> KeySizesComboBox { get; init; }

        public string SelectedBlock
        {
            get => _selectedBlock;
            set
            {
                if (_selectedBlock != value)
                {
                    _selectedBlock = value;
                    SelectedIVSize = Convert.ToInt32(_selectedBlock) / 4;
                    OnPropertyChanged(nameof(SelectedBlock));
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

        public string Key { get; set; }

        public string IV { get; set; }

        public int SelectedKeySize { get; set; }

        public int SelectedIVSize { get; set; }

        public string Error { get; private set; }

        public ICommand GenerateRandomKeyCommand { get; init; }

        public ICommand GenerateRandomIVCommand { get; init; }

        public ICommand SaveCryptorCommand { get; init; }

        public ICommand LoadCryptorCommand { get; init; }

        public AESViewModel(IJsonSerializer json, AesCng aes, string[] settingKeys, string pageName) : base(pageName)
        {
            _jsonSerializer = json;
            GenerateRandomKeyCommand = new Command(GenerateRandomKey, true);
            GenerateRandomIVCommand = new Command(GenerateRandomIV, true);
            SaveCryptorCommand = new Command(SaveCryptor, true);
            LoadCryptorCommand = new Command(LoadCryptor, true);

            SettingsKeys = settingKeys;

            BlockSizesComboBox = new();
            KeySizesComboBox = new();

            var legalKeys = aes.LegalKeySizes[0];
            var legalBlocks = aes.LegalBlockSizes[0];

            if (legalKeys.SkipSize != 0)
            {
                for (var i = legalKeys.MinSize; i <= legalKeys.MaxSize; i += legalKeys.SkipSize)
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

            SelectedBlock = BlockSizesComboBox.First();
            SelectedKey = KeySizesComboBox.First();
            OnPropertyChanged(nameof(BlockSizesComboBox));
            OnPropertyChanged(nameof(KeySizesComboBox));
        }

        public override Dictionary<string, object> GetObjects()
        {
            return new()
            {
                { SettingsKeys[0], Key },
                { SettingsKeys[1], IV },
                { SettingsKeys[2], SelectedKey },
                { SettingsKeys[3], SelectedBlock },
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

        private struct ToSerialzieObjects
        {
            public string Key;
            public string IV;
            public string SelectedKeySize;
            public string SelectedBlockSize;
        }

        private struct Objects
        {
            public ToSerialzieObjects ToSerialzie;
            public string Name;
        }


        private void SaveCryptor()
        {
            var serialzieObjects = new Objects()
            {
                ToSerialzie = new ToSerialzieObjects()
                {
                    IV = this.IV,
                    SelectedKeySize = this.SelectedKey,
                    Key = this.Key,
                    SelectedBlockSize = this.SelectedBlock
                },
                Name = PageName
            };

            WindowDialog.SaveDialog saveDialog = new(new DialogHelper()
            {
                Filters = Static.Extensions.FilterToPrompt(Static.Extensions.DialogFilters.Json),
                Multiselect = false,
                //Title = (string)(Application.Current as App).Resources.MergedDictionaries[0]["OpenFileDialog"]
            });

            var saveDestination = saveDialog.RunDialog();
            if (saveDestination != null)
            {
                _jsonSerializer.Serialize(serialzieObjects, saveDestination.First());
            }
        }

        private void LoadCryptor()
        {
            WindowDialog.OpenDialog openDialog = new(new DialogHelper()
            {
                Filters = Static.Extensions.FilterToPrompt(Static.Extensions.DialogFilters.Json),
                Multiselect = false,
                //Title = (string)(Application.Current as App).Resources.MergedDictionaries[0]["OpenFileDialog"]
            });

            var saveDestination = openDialog.RunDialog();
            if (saveDestination.Count != 0)
            {
                var objects = _jsonSerializer.Deserialize(saveDestination.First(), typeof(Objects));
                if (objects is not null)
                {
                    var castedObjects = (Objects)objects;
                    if (castedObjects.Name != PageName)
                    {
                        Error = "Incorrect file";
                        OnPropertyChanged(nameof(Error));
                    }
                    else
                    {
                        IV = castedObjects.ToSerialzie.IV;
                        Key = castedObjects.ToSerialzie.Key;
                        SelectedBlock = castedObjects.ToSerialzie.SelectedBlockSize;
                        SelectedKey = castedObjects.ToSerialzie.SelectedKeySize;
                        OnPropertyChanged(nameof(IV));
                        OnPropertyChanged(nameof(Key));
                        OnPropertyChanged(nameof(SelectedBlock));
                        OnPropertyChanged(nameof(SelectedKey));
                    }
                }
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
