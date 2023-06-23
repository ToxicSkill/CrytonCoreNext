using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrytonCoreNext.Crypting.Helpers;
using CrytonCoreNext.Crypting.Interfaces;
using CrytonCoreNext.Crypting.Models;
using CrytonCoreNext.Dictionaries;
using CrytonCoreNext.Helpers;
using CrytonCoreNext.Interfaces.Serializers;
using CrytonCoreNext.Models;
using System.Collections.ObjectModel;
using System.Linq;
using Wpf.Ui.Common;
using Wpf.Ui.Mvvm.Contracts;

namespace CrytonCoreNext.Crypting.ViewModels
{
    public partial class AESViewModel : CryptingMethodViewModel
    {
        private readonly ISnackbarService _snackbarService;

        private readonly IJsonSerializer _jsonSerializer;

        private AESHelper _aesHelper;

        [ObservableProperty]
        public ObservableCollection<int> blockSizesComboBox;

        [ObservableProperty]
        public ObservableCollection<int> keySizesComboBox;

        [ObservableProperty]
        public bool keysAvailable;

        [ObservableProperty]
        public int selectedBlock;

        [ObservableProperty]
        public int selectedKey;

        public AESViewModel(ICrypting crypting,
            ISnackbarService snackbarService,
            IJsonSerializer json,
            string pageName) : base(pageName)
        {
            Crypting = crypting;

            _snackbarService = snackbarService;
            _jsonSerializer = json;

            BlockSizesComboBox = new();
            KeySizesComboBox = new();
            UpdateKeyAvailability(true);
            InitializeHelper((AESHelper)Crypting.GetHelper());
        }

        private void InitializeHelper(AESHelper aesHelper)
        {
            _aesHelper = aesHelper;

            KeySizesComboBox = new(_aesHelper.LegalKeys);
            BlockSizesComboBox = new(_aesHelper.LegalBlocks);

            SelectedBlock = _aesHelper.GetCurrentBlockSize();
            SelectedKey = _aesHelper.GetCurrentKeySize();
        }

        [RelayCommand]
        private void ExportKeys()
        {
            var serialzieObjects = new Objects()
            {
                ToSerialzie = new ToSerialzieObjects()
                {
                    IV = _aesHelper.GetIVString(),
                    Key = _aesHelper.GetKeyString()
                },
                Name = PageName
            };

            WindowDialog.SaveDialog saveDialog = new(new DialogHelper()
            {
                Filters = Static.Extensions.FilterToPrompt(Static.Extensions.DialogFilters.Json),
                Multiselect = false,
                Title = Language.Post("OpenFileDialog")
            });

            var saveDestination = saveDialog.RunDialog();
            if (saveDestination != null)
            {
                _jsonSerializer.Serialize(serialzieObjects, saveDestination.First());
                _snackbarService.Show(Language.Post("Information"), Language.Post("Exported"), SymbolRegular.Checkmark20, ControlAppearance.Success);
            }
        }

        [RelayCommand]
        private void ImportKeys()
        {
            WindowDialog.OpenDialog openDialog = new(new DialogHelper()
            {
                Filters = Static.Extensions.FilterToPrompt(Static.Extensions.DialogFilters.Json),
                Multiselect = false,
                Title = Language.Post("OpenFileDialog")
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
                        _snackbarService.Show(Language.Post("Warning"), Language.Post("IncorrectFile"), SymbolRegular.Warning20, ControlAppearance.Caution);
                        _aesHelper.GenerateNewKeys();
                        return;
                    }
                    else
                    {
                        if (!ValidateKeys(castedObjects.ToSerialzie.IV, castedObjects.ToSerialzie.Key))
                        {
                            _snackbarService.Show(Language.Post("Warning"), Language.Post("IncorrectKeys"), SymbolRegular.Warning20, ControlAppearance.Caution);
                            _aesHelper.GenerateNewKeys();
                            return;
                        }

                        _snackbarService.Show(Language.Post("Success"), Language.Post("Imported"), SymbolRegular.Checkmark20, ControlAppearance.Success);
                    }
                }
            }
        }

        private struct ToSerialzieObjects
        {
            public string Key;
            public string IV;
        }

        private struct Objects
        {
            public ToSerialzieObjects ToSerialzie;
            public string Name;
        }

        private bool ValidateKeys(string iv, string key)
        {
            Lock();
            var keysCorrect = _aesHelper.ValidateKeys(iv, key);
            UpdateKeyAvailability(keysCorrect);
            UpdateSelectedKeys();
            Unlock();

            return keysCorrect;
        }

        private void UpdateSelectedKeys()
        {
            SelectedBlock = _aesHelper.GetCurrentBlockSize();
            SelectedKey = _aesHelper.GetCurrentKeySize();
        }

        private void UpdateKeyAvailability(bool available)
        {
            KeysAvailable = available;
        }

        [RelayCommand]
        private void GenerateRandomKeys()
        {
            _aesHelper.GenerateKey();
            _aesHelper.GenerateIV();
            UpdateKeyAvailability(true);
            _snackbarService.Show(Language.Post("Success"), Language.Post("KeysGenerated"), SymbolRegular.Checkmark20, ControlAppearance.Success);
        }
    }
}
