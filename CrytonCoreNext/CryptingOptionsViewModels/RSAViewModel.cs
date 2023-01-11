using CrytonCoreNext.Abstract;
using CrytonCoreNext.Commands;
using CrytonCoreNext.Crypting.Helpers;
using CrytonCoreNext.Dictionaries;
using CrytonCoreNext.Helpers;
using CrytonCoreNext.Interfaces;
using CrytonCoreNext.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Wpf.Ui.Mvvm.Contracts;

namespace CrytonCoreNext.CryptingOptionsViewModels
{
    public class RSAViewModel : ViewModelBase
    {
        private readonly ISnackbarService _snackbarService;

        private readonly IJsonSerializer _jsonSerializer;

        private readonly RSAHelper _rsaHelper;

        private int _selectedKey;

        public ObservableCollection<int> KeySizesComboBox { get; init; }

        public IProgressView ProgressView { get; init; }

        public bool IsPublicKeyAvailable { get; set; }

        public bool IsPrivateKeyAvailable { get; set; }

        public int SelectedKey
        {
            get => _selectedKey;
            set
            {
                if (_selectedKey != value && !IsBusy)
                {
                    _selectedKey = value;
                    GenerateKeys().ConfigureAwait(true);
                    OnPropertyChanged(nameof(SelectedKey));
                }
            }
        }

        public string MaxBytes { get; set; }

        public ICommand ExportPublicKeyCommand { get; init; }

        public ICommand ExportPrivateKeyCommand { get; init; }

        public ICommand ImportCryptorCommand { get; init; }

        public ICommand GenerateKeysCommand { get; init; }

        public RSAViewModel(ISnackbarService snackbarService, IJsonSerializer json, IXmlSerializer xml, IProgressView progressView, RSAHelper rsaHelper, string pageName) : base(pageName)
        {
            _snackbarService = snackbarService;
            _jsonSerializer = json;
            _rsaHelper = rsaHelper;
            ProgressView = progressView;
            ProgressView.ShowLabels(false);

            ExportPublicKeyCommand = new Command(ExportPublicKey, CanExecute);
            ExportPrivateKeyCommand = new Command(ExportPrivateKey, CanExecute);
            ImportCryptorCommand = new AsyncCommand(ImportCryptor, CanExecute);
            GenerateKeysCommand = new AsyncCommand(GenerateKeys, CanExecute);

            KeySizesComboBox = new ObservableCollection<int>(rsaHelper.LegalKeys);
            SelectedKey = _rsaHelper.GetKeySize();

            OnPropertyChanged(nameof(ProgressView));
            OnPropertyChanged(nameof(KeySizesComboBox));
            OnPropertyChanged(nameof(SelectedKey));
        }

        private struct ToSerialzieObjects
        {
            public string Keys;
            public int SelectedKeySize;
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
                await Task.Run(() => _rsaHelper.SetKeySize(_selectedKey));
                CombineMaxBytesMessage();
            }
            finally
            {
                await Task.Run(() => UpdateKeys());
                Log(Enums.ELogLevel.Information, Language.Post("KeysGenerated"));
                ProgressView.ClearProgress();
                Unlock();
            }
        }

        private void UpdateKeys()
        {
            IsPublicKeyAvailable = true;
            IsPrivateKeyAvailable = _rsaHelper.IsPrivateKeyAvailable();
            OnPropertyChanged(nameof(IsPublicKeyAvailable));
            OnPropertyChanged(nameof(IsPrivateKeyAvailable));
        }

        private void ExportPublicKey()
        {
            if (IsPublicKeyAvailable)
            {
                ExportKey(false);
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
                ExportKey(true);
            }
            else
            {
                NoKeyError();
            }
        }

        private void CombineMaxBytesMessage()
        {
            var prefix = Language.Post("MaximumFileSize");
            MaxBytes = $"{prefix}{_rsaHelper.MaxFileSize} B";
            OnPropertyChanged(nameof(MaxBytes));
        }

        private void NoKeyError()
        {
            Log(Enums.ELogLevel.Error, Language.Post("NoGeneratedKeys"));
            OnPropertyChanged(nameof(Logger));
            return;
        }

        private void ExportKey(bool includePrivate)
        {
            var serialzieObjects = new Objects()
            {
                ToSerialzie = new ToSerialzieObjects()
                {
                    Keys = _rsaHelper.ToXmlString(includePrivate),
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

        private async Task ImportCryptor()
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
                        _rsaHelper.FromXmlString(castedObjects.ToSerialzie.Keys);
                        _selectedKey = castedObjects.ToSerialzie.SelectedKeySize;
                        await Task.Run(() => UpdateKeys());
                        Log(Enums.ELogLevel.Information, Language.Post("Imported"));
                        OnPropertyChanged(nameof(SelectedKey));
                    }
                }
            }
        }
    }
}
