using CrytonCoreNext.Abstract;
using CrytonCoreNext.Commands;
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
    public class RSAViewModel : ViewModelBase
    {
        private readonly string[] SettingsKeys;

        private readonly IJsonSerializer _jsonSerializer;

        private readonly IXmlSerializer _xmlSerializer;

        private string _selectedKey;

        private RSAParameters _privateKey;

        private RSAParameters _publicKey;

        private readonly Func<int, int> _restrictionFunction;

        public ObservableCollection<string> KeySizesComboBox { get; init; } = new();

        public string SelectedKey
        {
            get => _selectedKey;
            set
            {
                if (_selectedKey != value)
                {
                    _selectedKey = value;
                    SelectedKeysSize = Convert.ToInt32(_selectedKey);
                    MaxBytes = $"Maximum file size: {_restrictionFunction(SelectedKeysSize)} bytes";
                    OnPropertyChanged(nameof(MaxBytes));
                    OnPropertyChanged(nameof(SelectedKey));
                    OnPropertyChanged(nameof(SelectedKeysSize));
                }
            }
        }

        public bool EnablePrivateKeys { get; set; } = true;

        public string MaxBytes { get; set; }

        public int SelectedKeysSize { get; set; }

        public string Error { get; private set; }

        public ICommand SaveCryptorCommand { get; init; }

        public ICommand LoadCryptorCommand { get; init; }

        public RSAViewModel(IJsonSerializer json, IXmlSerializer xml, Func<int, int> restrictionFunction, KeySizes keySizes, int defaultKeySize, string[] settingKeys, string pageName) : base(pageName)
        {
            _xmlSerializer = xml;
            _jsonSerializer = json;
            _restrictionFunction = restrictionFunction;
            SettingsKeys = settingKeys;

            SaveCryptorCommand = new Command(SaveCryptor, true);
            LoadCryptorCommand = new Command(LoadCryptor, true);

            if (keySizes.SkipSize != 0)
            {
                for (var i = keySizes.MinSize; i <= keySizes.MaxSize; i += keySizes.SkipSize * 16)
                {
                    KeySizesComboBox.Add(i.ToString());
                }
            }
            else
            {
                KeySizesComboBox.Add(keySizes.MinSize.ToString());
            }

            SelectedKey = defaultKeySize.ToString();
            OnPropertyChanged(nameof(KeySizesComboBox));
            OnPropertyChanged(nameof(SelectedKey));
        }

        public override Dictionary<string, object> GetObjects()
        {
            return new()
            {
                { SettingsKeys[0], _publicKey },
                { SettingsKeys[1], _privateKey },
                { SettingsKeys[2], SelectedKey },
                { SettingsKeys[3], string.Empty }
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

            if (objects[SettingsKeys[3]] is string error)
            {
                Error = error;
                OnPropertyChanged(nameof(Error));
                if (!string.IsNullOrEmpty(error))
                {
                    return;
                }
            }

            if (objects[SettingsKeys[0]] is RSAParameters publicKey && objects[SettingsKeys[1]] is RSAParameters privateKey)
            {
                if (!publicKey.Equals(string.Empty) && !privateKey.Equals(string.Empty))
                {
                    _publicKey = publicKey;
                    _privateKey = privateKey;
                }
            }

            if (objects[SettingsKeys[2]] is string size)
            {
                if (KeySizesComboBox.Contains(size))
                {
                    SelectedKey = size;
                    OnPropertyChanged(nameof(SelectedKeysSize));
                }
            }
        }

        private struct ToSerialzieObjects
        {
            public string PublicKey;
            public string PrivateKey;
            public string SelectedKeySize;
        }

        private struct Objects
        {
            public ToSerialzieObjects ToSerialzie;
            public string Name;
        }

        private void SaveCryptor()
        {
            if (_publicKey.Modulus == null)
            {
                Error = "No generated or imported keys available";
                OnPropertyChanged(nameof(Error));
                return;
            }

            var serialzieObjects = new Objects()
            {
                ToSerialzie = new ToSerialzieObjects()
                {
                    PublicKey = _xmlSerializer.RsaParameterKeyToString(_publicKey),
                    PrivateKey = EnablePrivateKeys ? _xmlSerializer.RsaParameterKeyToString(_privateKey) : string.Empty,
                    SelectedKeySize = this.SelectedKey
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
                        _publicKey = _xmlSerializer.StringKeyToRsaParameter<RSAParameters>(castedObjects.ToSerialzie.PublicKey);
                        _privateKey = _xmlSerializer.StringKeyToRsaParameter<RSAParameters>(castedObjects.ToSerialzie.PrivateKey);
                        SelectedKey = castedObjects.ToSerialzie.SelectedKeySize;
                        OnPropertyChanged(nameof(SelectedKey));
                    }
                }
            }
        }
    }
}
