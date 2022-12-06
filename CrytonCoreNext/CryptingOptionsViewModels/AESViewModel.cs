using CrytonCoreNext.Abstract;
using CrytonCoreNext.Commands;
using CrytonCoreNext.Crypting.Helpers;
using CrytonCoreNext.Crypting.Models;
using CrytonCoreNext.Dictionaries;
using CrytonCoreNext.Helpers;
using CrytonCoreNext.Interfaces;
using CrytonCoreNext.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace CrytonCoreNext.CryptingOptionsViewModels
{
    public class AESViewModel : ViewModelBase
    {
        private readonly IJsonSerializer _jsonSerializer;

        private readonly AESHelper _aesHelper;

        private readonly string[] SettingsKeys;

        private string _selectedKey;

        private string _selectedBlock;

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
                    OnPropertyChanged(nameof(SelectedBlock));
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
                    OnPropertyChanged(nameof(SelectedKey));
                }
            }
        }

        public ICommand GenerateKeysCommand { get; init; }

        public ICommand ExportKeysCommand { get; init; }

        public ICommand ImportKeysCommand { get; init; }

        public AESViewModel(IJsonSerializer json, AESHelper aesHelper, string[] settingKeys, string pageName) : base(pageName)
        {
            _jsonSerializer = json;
            _aesHelper = aesHelper;
            GenerateKeysCommand = new Command(GenerateRandomKeys, CanExecute);
            ExportKeysCommand = new Command(ExportKeys, CanExecute);
            ImportKeysCommand = new Command(ImportKeys, CanExecute);
            SettingsKeys = settingKeys;

            BlockSizesComboBox = new();
            KeySizesComboBox = new();

            KeySizesComboBox = new ObservableCollection<string>(_aesHelper.LegalKeys);
            BlockSizesComboBox = new ObservableCollection<string>(_aesHelper.LegalBlocks);
            SelectedBlock = _aesHelper.DefaultBlockSize;
            SelectedKey = _aesHelper.DefaultKeySize;

            OnPropertyChanged(nameof(SelectedBlock));
            OnPropertyChanged(nameof(SelectedKey));
            OnPropertyChanged(nameof(BlockSizesComboBox));
            OnPropertyChanged(nameof(KeySizesComboBox));
        }

        public override Dictionary<string, object> GetObjects()
        {
            return new()
            {
                { SettingsKeys[0], _key },
                { SettingsKeys[1], _iv },
                { SettingsKeys[2], SelectedKey },
                { SettingsKeys[3], SelectedBlock }
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

            var key = objects[SettingsKeys[0]] as byte[];
            var iv = objects[SettingsKeys[1]] as byte[];
            ValidateKeys(iv, key);
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
                Log(Enums.ELogLevel.Information, Language.Post("Exported"));
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
                        Log(Enums.ELogLevel.Error, Language.Post("IncorrectFile"));
                        return;
                    }
                    else
                    {
                        var iv = castedObjects.ToSerialzie.IV;
                        var key = castedObjects.ToSerialzie.Key;
                        var selectedBlock = castedObjects.ToSerialzie.SelectedBlockSize;
                        var selectedKey = castedObjects.ToSerialzie.SelectedKeySize;
                        var keysCorrect = ValidateKeys(iv, key);
                        var sizesCorrect = ValidateSizes(selectedBlock, selectedKey);
                        if (!keysCorrect || !sizesCorrect)
                        {
                            if (keysCorrect && !sizesCorrect)
                            {
                                Log(Enums.ELogLevel.Warning, Language.Post("IncorrectSizes"));
                            }
                            else
                            {
                                Log(Enums.ELogLevel.Error, Language.Post("IncorrectKeys"));
                            }

                            return;
                        }

                        Log(Enums.ELogLevel.Information, Language.Post("Imported"));
                    }
                }
            }
        }

        private bool ValidateKeys(byte[]? ivArray, byte[]? keyArray)
        {
            if (ivArray == null || keyArray == null)
            {
                return false;
            }
            try
            {
                var iv = Convert.ToHexString(ivArray);
                var key = Convert.ToHexString(keyArray);
                return ValidateKeys(iv, key);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool ValidateKeys(string iv, string key)
        {
            var keysCorrect = _aesHelper.IsKeyValid(iv) && _aesHelper.IsIVValid(key);

            IsKeyAvailable = keysCorrect;
            IsIVAvailable = keysCorrect;

            if (keysCorrect)
            {
                _iv = iv;
                _key = key;
            }

            OnPropertyChanged(nameof(IsKeyAvailable));
            OnPropertyChanged(nameof(IsIVAvailable));

            return keysCorrect;
        }

        private bool ValidateSizes(string selectedBlock, string selectedKey)
        {
            var sizesCorrect = _aesHelper.IsSizeValid(selectedKey) && _aesHelper.IsSizeValid(selectedBlock);

            if (sizesCorrect)
            {
                SelectedBlock = selectedBlock;
                SelectedKey = selectedKey;
            }

            OnPropertyChanged(nameof(SelectedBlock));
            OnPropertyChanged(nameof(SelectedKey));

            return sizesCorrect;
        }

        private void GenerateRandomKeys()
        {
            var iv = RandomCryptoGenerator.GetCryptoRandomBytesString(Convert.ToInt32(SelectedBlock) / 8);
            var key = RandomCryptoGenerator.GetCryptoRandomBytesString(Convert.ToInt32(SelectedKey) / 8);
            if (ValidateKeys(iv, key))
            {
                Log(Enums.ELogLevel.Information, Language.Post("KeysGenerated"));
            }
            else
            {
                Log(Enums.ELogLevel.Error, Language.Post("KeysGenerationFail"));
            }
        }
    }
}
