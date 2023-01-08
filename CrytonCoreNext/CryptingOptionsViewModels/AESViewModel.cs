﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrytonCoreNext.Abstract;
using CrytonCoreNext.Crypting.Helpers;
using CrytonCoreNext.Dictionaries;
using CrytonCoreNext.Helpers;
using CrytonCoreNext.Interfaces;
using CrytonCoreNext.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace CrytonCoreNext.CryptingOptionsViewModels
{
    public partial class AESViewModel : ViewModelBase
    {
        private readonly IJsonSerializer _jsonSerializer;

        private readonly AESHelper _aesHelper;

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

        partial void OnSelectedBlockChanged(int value)
        {
            if (SelectedBlock != value)
            {
                SelectedBlock = value;
                OnPropertyChanged(nameof(SelectedBlock));
                if (!IsBusy)
                {
                    _aesHelper.SetBlockSize(value);
                    Log(Enums.ELogLevel.Information, Language.Post("RegenerateIV"));
                }
            }
        }

        partial void OnSelectedKeyChanged(int value)
        {
            if (SelectedKey != value)
            {
                SelectedKey = value;
                OnPropertyChanged(nameof(SelectedKey));
                if (!IsBusy)
                {
                    _aesHelper.SetKeySize(value);
                    Log(Enums.ELogLevel.Information, Language.Post("RegenerateKey"));
                }
            }
        }

        public AESViewModel(IJsonSerializer json, AESHelper aesHelper, string pageName) : base(pageName)
        {
            _jsonSerializer = json;
            _aesHelper = aesHelper;

            BlockSizesComboBox = new();
            KeySizesComboBox = new();

            KeySizesComboBox = new ObservableCollection<int>(_aesHelper.LegalKeys);
            BlockSizesComboBox = new ObservableCollection<int>(_aesHelper.LegalBlocks);
            SelectedBlock = _aesHelper.GetCurrentBlockSize();
            SelectedKey = _aesHelper.GetCurrentKeySize();
            UpdateKeyAvailability(true);
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
                Log(Enums.ELogLevel.Information, Language.Post("Exported"));
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
                        Log(Enums.ELogLevel.Warning, Language.Post("IncorrectFile"));
                        _aesHelper.GenerateNewKeys();
                        return;
                    }
                    else
                    {
                        if (!ValidateKeys(castedObjects.ToSerialzie.IV, castedObjects.ToSerialzie.Key))
                        {
                            Log(Enums.ELogLevel.Warning, Language.Post("IncorrectKeys"));
                            _aesHelper.GenerateNewKeys();
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
            OnPropertyChanged(nameof(SelectedBlock));
            OnPropertyChanged(nameof(SelectedKey));
        }

        private void UpdateKeyAvailability(bool available)
        {
            KeysAvailable = available;
            OnPropertyChanged(nameof(KeysAvailable));
        }

        [RelayCommand]
        private void GenerateRandomKeys()
        {
            _aesHelper.GenerateKey();
            _aesHelper.GenerateIV();
            UpdateKeyAvailability(true);
            Log(Enums.ELogLevel.Information, Language.Post("KeysGenerated"));
        }
    }
}
