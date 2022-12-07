using CrytonCoreNext.Abstract;
using CrytonCoreNext.Commands;
using CrytonCoreNext.Crypting.Helpers;
using CrytonCoreNext.Dictionaries;
using CrytonCoreNext.Helpers;
using CrytonCoreNext.Interfaces;
using CrytonCoreNext.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CrytonCoreNext.CryptingOptionsViewModels
{
    public class RSAViewModel : ViewModelBase
    {
        private readonly string[] _settingsKeys;

        private readonly IJsonSerializer _jsonSerializer;

        private readonly IXmlSerializer _xmlSerializer;

        private readonly RSAHelper _rsaHelper;

        private RSAParameters _keys;

        private string _selectedKey;

        public ObservableCollection<string> KeySizesComboBox { get; init; }

        public IProgressView ProgressView { get; init; }

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

        public string MaxBytes { get; set; }

        public ICommand ExportPublicKeyCommand { get; init; }

        public ICommand ExportPrivateKeyCommand { get; init; }

        public ICommand ImportCryptorCommand { get; init; }

        public ICommand GenerateKeysCommand { get; init; }

        public RSAViewModel(IJsonSerializer json, IXmlSerializer xml, IProgressView progressView, RSAHelper rsaHelper, string pageName, string[] settingKeys) : base(pageName)
        {
            _settingsKeys = settingKeys;
            _xmlSerializer = xml;
            _jsonSerializer = json;
            _rsaHelper = rsaHelper;
            ProgressView = progressView;
            ProgressView.ChangeProgressType(BusyIndicator.IndicatorType.Cupertino);
            ProgressView.ShowLabels(false);

            ExportPublicKeyCommand = new Command(ExportPublicKey, CanExecute);
            ExportPrivateKeyCommand = new Command(ExportPrivateKey, CanExecute);
            ImportCryptorCommand = new Command(ImportCryptor, CanExecute);
            GenerateKeysCommand = new AsyncCommand(GenerateKeys, CanExecute);

            KeySizesComboBox = new ObservableCollection<string>(rsaHelper.LegalKeys);
            SelectedKey = rsaHelper.DefaultKeySize.ToString();

            OnPropertyChanged(nameof(ProgressView));
            OnPropertyChanged(nameof(KeySizesComboBox));
            OnPropertyChanged(nameof(SelectedKey));
        }

        public override bool CanExecute()
        {
            return !IsBusy;
        }

        public override Dictionary<string, object> GetObjects()
        {
            return new()
            {
                { _settingsKeys[0], _keys },
                { _settingsKeys[1], SelectedKey }
            };
        }

        public void ParseObjects(Dictionary<string, object> objects)
        {
            foreach (var setting in _settingsKeys)
            {
                if (!objects.ContainsKey(setting))
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

        private async Task GenerateKeys()
        {
            try
            {
                Lock();
                ProgressView.InitializeProgress<int>(0);
                var rsap = new RSACryptoServiceProvider(Convert.ToInt32(SelectedKey));
                _keys = await Task.Run(() => rsap.ExportParameters(true));
            }
            finally
            {
                UpdateKeys();
                Log(Enums.ELogLevel.Information, Language.Post("KeysGenerated"));
                ProgressView.ClearProgress();
                Unlock();
            }
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
            var prefix = Language.Post("MaximumFileSize");
            MaxBytes = $"{prefix}{_rsaHelper.GetMaxNumberOfBytes(Convert.ToInt32(_selectedKey))} B";
            OnPropertyChanged(nameof(MaxBytes));
        }

        private void NoKeyError()
        {
            Log(Enums.ELogLevel.Error, Language.Post("NoGeneratedKeys"));
            OnPropertyChanged(nameof(Logger));
            return;
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
                Log(Enums.ELogLevel.Information, Language.Post("Exported"));
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
                        Log(Enums.ELogLevel.Error, Language.Post("IncorrectFile"));
                    }
                    else
                    {
                        _keys = _xmlSerializer.StringKeyToRsaParameter<RSAParameters>(castedObjects.ToSerialzie.Keys);
                        SelectedKey = castedObjects.ToSerialzie.SelectedKeySize;
                        UpdateKeys();
                        Log(Enums.ELogLevel.Information, Language.Post("Imported"));
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
    }
}
