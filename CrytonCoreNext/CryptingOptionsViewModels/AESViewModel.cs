using CrytonCoreNext.Abstract;
using CrytonCoreNext.Commands;
using CrytonCoreNext.Crypting.Helpers;
using CrytonCoreNext.Dictionaries;
using CrytonCoreNext.Helpers;
using CrytonCoreNext.Interfaces;
using CrytonCoreNext.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace CrytonCoreNext.CryptingOptionsViewModels
{
    public class AESViewModel : ViewModelBase
    {
        private readonly IJsonSerializer _jsonSerializer;

        private readonly AESHelper _aesHelper;

        private int _selectedKey;

        private int _selectedBlock;

        public ObservableCollection<int> BlockSizesComboBox { get; init; }

        public ObservableCollection<int> KeySizesComboBox { get; init; }

        public bool IsKeyAvailable { get; set; }

        public bool IsIVAvailable { get; set; }

        public int SelectedBlock
        {
            get => _selectedBlock;
            set
            {
                if (_selectedBlock != value && _aesHelper.SetBlockSize(value))
                {
                    _selectedBlock = value;
                    OnPropertyChanged(nameof(SelectedBlock));
                }
            }
        }

        public int SelectedKey
        {
            get => _selectedKey;
            set
            {
                if (_selectedKey != value && _aesHelper.SetKeySize(value))
                {
                    _selectedKey = value;
                    OnPropertyChanged(nameof(SelectedKey));
                }
            }
        }

        public ICommand GenerateKeysCommand { get; init; }

        public ICommand ExportKeysCommand { get; init; }

        public ICommand ImportKeysCommand { get; init; }

        public AESViewModel(IJsonSerializer json, AESHelper aesHelper, string pageName) : base(pageName)
        {
            _jsonSerializer = json;
            _aesHelper = aesHelper;
            GenerateKeysCommand = new Command(GenerateRandomKeys, CanExecute);
            ExportKeysCommand = new Command(ExportKeys, CanExecute);
            ImportKeysCommand = new Command(ImportKeys, CanExecute);

            BlockSizesComboBox = new();
            KeySizesComboBox = new();

            KeySizesComboBox = new ObservableCollection<int>(_aesHelper.LegalKeys);
            BlockSizesComboBox = new ObservableCollection<int>(_aesHelper.LegalBlocks);
            SelectedBlock = _aesHelper.GetCurrentBlockSize();
            SelectedKey = _aesHelper.GetCurrentKeySize();

            OnPropertyChanged(nameof(SelectedBlock));
            OnPropertyChanged(nameof(SelectedKey));
            OnPropertyChanged(nameof(BlockSizesComboBox));
            OnPropertyChanged(nameof(KeySizesComboBox));
        }

        private void ExportKeys()
        {
            var serialzieObjects = new Objects()
            {
                ToSerialzie = new ToSerialzieObjects()
                {
                    IV = _aesHelper.GetIVString(),
                    SelectedKeySize = SelectedKey.ToString(),
                    Key = _aesHelper.GetKeyString(),
                    SelectedBlockSize = SelectedBlock.ToString()
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
                        var keysCorrect = ValidateKeys(iv, key, selectedBlock, selectedKey);
                        if (!keysCorrect)
                        {
                            if (keysCorrect)
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

        private bool ValidateKeys(string iv, string key, string selectedBlock, string selectedKey)
        {
            var keysCorrect = _aesHelper.KeysCorrect(iv, key, selectedBlock, selectedKey);

            UpdateKeyAvailability(keysCorrect);
            UpdateSelectedKeys();

            return keysCorrect;
        }

        private void UpdateSelectedKeys()
        {
            SelectedBlock = _aesHelper.GetCurrentBlockSize();
            SelectedKey = _aesHelper.GetCurrentKeySize();
            OnPropertyChanged(nameof(SelectedBlock));
            OnPropertyChanged(nameof(SelectedKey));
        }

        private void UpdateKeyAvailability(bool available)
        {
            IsKeyAvailable = available;
            IsIVAvailable = available;

            OnPropertyChanged(nameof(IsKeyAvailable));
            OnPropertyChanged(nameof(IsIVAvailable));
        }

        private void GenerateRandomKeys()
        {
            _aesHelper.GenerateKey();
            _aesHelper.GenerateIV();
            UpdateKeyAvailability(true);
            Log(Enums.ELogLevel.Information, Language.Post("KeysGenerated"));
        }
    }
}
