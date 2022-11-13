﻿using CrytonCoreNext.Abstract;
using CrytonCoreNext.Commands;
using CrytonCoreNext.Helpers;
using CrytonCoreNext.Interfaces;
using CrytonCoreNext.Logger;
using CrytonCoreNext.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Input;

namespace CrytonCoreNext.CryptingOptionsViewModels
{
    public class RSAViewModel : ViewModelBase
    {
        private const int LoggerSeconds = 4;

        private readonly string[] _settingsKeys;

        private readonly IJsonSerializer _jsonSerializer;

        private readonly IXmlSerializer _xmlSerializer;

        private readonly RSAHelper _rsaHelper;

        private RSAParameters _keys;

        private string _selectedKey;

        private Log _logger;

        public ObservableCollection<string> KeySizesComboBox { get; init; }

        public bool IsPublicKeyAvailable { get; set; }

        public bool IsPrivateKeyAvailable { get; set; }

        public string SelectedKey
        {
            get => _selectedKey;
            set
            {
                if (_selectedKey != value)
                {
                    _selectedKey = value;
                    CombineMaxBytesMessage();
                    OnPropertyChanged(nameof(SelectedKey));
                }
            }
        }

        public Log Logger
        {
            get => _logger;
            set
            {
                _logger = value;
                InvokeTimerActionForLogger();
            }
        }

        public string MaxBytes { get; set; }

        public ICommand ExportPublicKeyCommand { get; init; }

        public ICommand ExportPrivateKeyCommand { get; init; }

        public ICommand ImportCryptorCommand { get; init; }

        public ICommand GenerateKeysCommand { get; init; }

        public RSAViewModel(IJsonSerializer json, IXmlSerializer xml, RSAHelper rsaHelper, string pageName, string[] settingKeys) : base(pageName)
        {
            _settingsKeys = settingKeys;
            _xmlSerializer = xml;
            _jsonSerializer = json;
            _rsaHelper = rsaHelper;
            _logger = new();

            ExportPublicKeyCommand = new Command(ExportPublicKey, true);
            ExportPrivateKeyCommand = new Command(ExportPrivateKey, true);
            ImportCryptorCommand = new Command(ImportCryptor, true);
            GenerateKeysCommand = new Command(GenerateKeys, true);

            KeySizesComboBox = new ObservableCollection<string>(rsaHelper.LegalKeys);
            SelectedKey = rsaHelper.DefaultKeySize.ToString();


            OnPropertyChanged(nameof(KeySizesComboBox));
            OnPropertyChanged(nameof(SelectedKey));
        }

        public override Dictionary<string, object> GetObjects()
        {
            return new()
            {
                { _settingsKeys[0], _keys },
                { _settingsKeys[1], SelectedKey },
                { _settingsKeys[2], _logger }
            };
        }

        public override void SetObjects(Dictionary<string, object> objects)
        {
            foreach (var setting in _settingsKeys)
            {
                if (!objects.ContainsKey(setting))
                {
                    return;
                }
            }

            if (objects[_settingsKeys[2]] is Log logger)
            {
                Logger = logger;
                if (logger.LogLevel == Enums.ELogLevel.Fatal || logger.LogLevel == Enums.ELogLevel.Error)
                {
                    return;
                }
            }

            if (objects[_settingsKeys[0]] is RSAParameters keys)
            {
                if (!keys.Equals(null))
                {
                    _keys = keys;
                    UpdateKeys();
                }
            }

            if (objects[_settingsKeys[1]] is string size)
            {
                if (KeySizesComboBox.Contains(size))
                {
                    SelectedKey = size;
                }
            }
        }

        private struct ToSerialzieObjects
        {
            public string Keys;
            public string SelectedKeySize;
        }

        private struct Objects
        {
            public ToSerialzieObjects ToSerialzie;
            public string Name;
        }

        private void GenerateKeys()
        {
            var rsap = new RSACryptoServiceProvider(Convert.ToInt32(SelectedKey));
            _keys = rsap.ExportParameters(true);
            UpdateKeys();
            _logger.Set(Enums.ELogLevel.Information, Application.Current.Resources.MergedDictionaries[0]["KeysGenerated"].ToString() ?? string.Empty);
            InvokeTimerActionForLogger();
        }

        private void ExportPublicKey()
        {
            if (IsPublicKeyAvailable)
            {
                ExportKey(true);
            }
            else
            {
                NoKeyError();
            }
        }

        private void ExportPrivateKey()
        {
            if (IsPrivateKeyAvailable)
            {
                ExportKey(false);
            }
            else
            {
                NoKeyError();
            }
        }

        private void CombineMaxBytesMessage()
        {
            var prefix = Application.Current.Resources.MergedDictionaries[0]["MaximumFileSize"].ToString() ?? string.Empty;
            MaxBytes = $"{prefix}{_rsaHelper.GetMaxNumberOfBytes(Convert.ToInt32(_selectedKey))} B";
            OnPropertyChanged(nameof(MaxBytes));
        }

        private void NoKeyError()
        {
            _logger.Set(Enums.ELogLevel.Error, Application.Current.Resources.MergedDictionaries[0]["NoGeneratedKeys"].ToString() ?? string.Empty);
            OnPropertyChanged(nameof(Logger));
            return;
        }

        private void InvokeTimerActionForLogger()
        {
            ActionTimer.InitializeTimerWithAction(ClearLogger, LoggerSeconds);
            OnPropertyChanged(nameof(Logger));
        }

        private void ExportKey(bool publicKey)
        {
            var serialzieObjects = new Objects()
            {
                ToSerialzie = new ToSerialzieObjects()
                {
                    Keys = _xmlSerializer.RsaParameterKeyToString(publicKey ? _rsaHelper.GetOnlyPublicMembers(_keys) : _keys),
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

        private void ImportCryptor()
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
                        _logger.Set(Enums.ELogLevel.Error, Application.Current.Resources.MergedDictionaries[0]["IncorrectFile"].ToString() ?? string.Empty);
                        InvokeTimerActionForLogger();
                    }
                    else
                    {
                        _keys = _xmlSerializer.StringKeyToRsaParameter<RSAParameters>(castedObjects.ToSerialzie.Keys);
                        SelectedKey = castedObjects.ToSerialzie.SelectedKeySize;
                        UpdateKeys();
                        _logger.Set(Enums.ELogLevel.Information, Application.Current.Resources.MergedDictionaries[0]["KeysImported"].ToString() ?? string.Empty);
                        InvokeTimerActionForLogger();
                        OnPropertyChanged(nameof(SelectedKey));
                    }
                }
            }
        }

        private void UpdateKeys()
        {
            if (_rsaHelper.IsKeyEmpty(_keys))
            {
                return;
            }

            IsPublicKeyAvailable = true;
            IsPrivateKeyAvailable = _rsaHelper.IsKeyPrivate(_keys);
            OnPropertyChanged(nameof(IsPublicKeyAvailable));
            OnPropertyChanged(nameof(IsPrivateKeyAvailable));
        }

        private void ClearLogger(object sender, EventArgs e)
        {
            Logger.Reset();
            OnPropertyChanged(nameof(Logger));
        }
    }
}
