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

        private string _key;

        private string _iv;

        public ObservableCollection<string> BlockSizesComboBox { get; init; }

        public ObservableCollection<string> KeySizesComboBox { get; init; }

        public bool IsKeyAvailable { get; set; }

        public bool IsIVAvailable { get; set; }

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

        public int SelectedKeySize { get; set; }

        public int SelectedIVSize { get; set; }

        public string Error { get; private set; }

        public ICommand GenerateKeysCommand { get; init; }

        public ICommand ExportKeysCommand { get; init; }

        public ICommand ImportKeysCommand { get; init; }

        public AESViewModel(IJsonSerializer json, AesCng aes, string[] settingKeys, string pageName) : base(pageName)
        {
            _jsonSerializer = json;
            GenerateKeysCommand = new Command(GenerateRandomKeys, CanExecute);
            ExportKeysCommand = new Command(ExportKeys, CanExecute);
            ImportKeysCommand = new Command(ImportKeys, CanExecute);
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

        public override bool CanExecute()
        {
            return !IsBusy;
        }

        public override Dictionary<string, object> GetObjects()
        {
            return new()
            {
                { SettingsKeys[0], _key },
                { SettingsKeys[1], _iv },
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
                _key = Convert.ToHexString(key);
                _iv = Convert.ToHexString(iv);
                OnPropertyChanged(nameof(_key));
                OnPropertyChanged(nameof(_iv));
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


        private void ExportKeys()
        {
            var serialzieObjects = new Objects()
            {
                ToSerialzie = new ToSerialzieObjects()
                {
                    IV = this._iv,
                    SelectedKeySize = this.SelectedKey,
                    Key = this._key,
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

        private void ImportKeys()
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
                        _iv = castedObjects.ToSerialzie.IV;
                        _key = castedObjects.ToSerialzie.Key;
                        SelectedBlock = castedObjects.ToSerialzie.SelectedBlockSize;
                        SelectedKey = castedObjects.ToSerialzie.SelectedKeySize;
                        OnPropertyChanged(nameof(SelectedBlock));
                        OnPropertyChanged(nameof(SelectedKey));
                        OnPropertyChanged(nameof(IsKeyAvailable));
                        OnPropertyChanged(nameof(IsIVAvailable));
                    }
                }
            }
        }

        private void GenerateRandomKeys()
        {
            _iv = RandomCryptoGenerator.GetCryptoRandomBytesString(SelectedIVSize / 2);
            _key = RandomCryptoGenerator.GetCryptoRandomBytesString(SelectedKeySize / 2);
            IsKeyAvailable = true;
            IsIVAvailable = true;
            OnPropertyChanged(nameof(IsKeyAvailable));
            OnPropertyChanged(nameof(IsIVAvailable));
        }
    }
}
