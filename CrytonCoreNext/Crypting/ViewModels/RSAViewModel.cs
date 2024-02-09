using CommunityToolkit.Mvvm.ComponentModel;
using CrytonCoreNext.Crypting.Helpers;
using CrytonCoreNext.Crypting.Interfaces;
using CrytonCoreNext.Crypting.Models;
using CrytonCoreNext.Dictionaries;
using CrytonCoreNext.Interfaces.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Wpf.Ui;
using Wpf.Ui.Controls;


namespace CrytonCoreNext.Crypting.ViewModels
{
    public partial class RSAViewModel : CryptingMethodViewModel, ICryptingViewModel
    {
        private readonly ISnackbarService _snackbarService;

        private readonly IJsonSerializer _jsonSerializer;

        private List<int> _legalKeyValues;

        private RSAHelper _rsaHelper;

        private int _selectedFileKeySize = 0;

        [ObservableProperty]
        public bool isPublicKeyAvailable;

        [ObservableProperty]
        public bool isPrivateKeyAvailable;

        [ObservableProperty]
        public int selectedKeySize;

        [ObservableProperty]
        public int sliderFrequency;

        [ObservableProperty]
        public int sliderMinimum;

        [ObservableProperty]
        public int sliderMaximum;

        [ObservableProperty]
        public bool includePrivateKey;

        [ObservableProperty]
        public string maxBytesMessage;

        [ObservableProperty]
        public Brush fileTooBigCautionColor = new SolidColorBrush(Colors.White);

        public RSAViewModel(ICrypting crypting,
            ISnackbarService snackbarService,
            IJsonSerializer json,
            IXmlSerializer xml,
            string pageName) : base(pageName)
        {
            Crypting = crypting;

            _snackbarService = snackbarService;
            _jsonSerializer = json;
            InitializeHelper((RSAHelper)Crypting.GetHelper());
        }

        private void InitializeHelper(RSAHelper rsaHelper)
        {
            _rsaHelper = rsaHelper;

            _legalKeyValues = rsaHelper.LegalKeys;
            SelectedKeySize = _rsaHelper.GetKeySize();
            InitializeSlider();
        }

        private void InitializeSlider()
        {
            SliderMinimum = _legalKeyValues.First();
            SliderMaximum = _legalKeyValues.Last();
            SliderFrequency = _rsaHelper.GetKeyValueStep();
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

        partial void OnSelectedKeySizeChanged(int value)
        {
            CombineMaxBytesMessage();
            _rsaHelper.SetKeySize(value);
        }

        private void UpdateKeys()
        {
            IsPublicKeyAvailable = true;
            IsPrivateKeyAvailable = _rsaHelper.IsPrivateKeyAvailable();
        }


        private void CombineMaxBytesMessage()
        {
            var prefix = Language.Post("MaximumFileSize");
            var maxNumberOfBytes = _rsaHelper.GetMaxNumberOfBytes(selectedKeySize);
            MaxBytesMessage = $"{prefix}{maxNumberOfBytes} B";
            if (maxNumberOfBytes < _selectedFileKeySize)
            {
                FileTooBigCautionColor = new SolidColorBrush(Colors.Orange);
            }
            else
            {
                FileTooBigCautionColor = new SolidColorBrush(Colors.White);
            }
        }

        public override void HandleFileChanged(CryptFile file)
        {
            if (file == null)
            {
                return;
            }
            _selectedFileKeySize = file.Bytes.Length;
            CombineMaxBytesMessage();
        }

        public byte[] ExportObjects()
        {
            var serialzieObjects = new Objects()
            {
                ToSerialzie = new ToSerialzieObjects()
                {
                    Keys = _rsaHelper.ToXmlString(includePrivateKey),
                    SelectedKeySize = selectedKeySize
                },
                Name = PageName
            };
            return Encoding.ASCII.GetBytes(_jsonSerializer.Serialize(serialzieObjects));
        }

        public bool ImportObjects(string str)
        {
            var objects = _jsonSerializer.Deserialize(str, typeof(Objects));
            if (objects is not null)
            {
                var castedObjects = (Objects)objects;
                if (castedObjects.Name != PageName)
                {
                    _snackbarService.Show(Language.Post("Error"), Language.Post("IncorrectFile"), ControlAppearance.Danger, new SymbolIcon(SymbolRegular.ErrorCircle20), TimeSpan.FromSeconds(2));
                    return false;
                }
                else
                {
                    _rsaHelper.FromXmlString(castedObjects.ToSerialzie.Keys);
                    selectedKeySize = castedObjects.ToSerialzie.SelectedKeySize;
                    UpdateKeys();
                    _snackbarService.Show(Language.Post("Information"), Language.Post("Imported"), ControlAppearance.Info, new SymbolIcon(SymbolRegular.Check20), TimeSpan.FromSeconds(2));
                    return true;
                }
            }
            return false;
        }

    }
}
