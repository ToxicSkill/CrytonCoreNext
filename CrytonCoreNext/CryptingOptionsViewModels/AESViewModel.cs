using CrytonCoreNext.Abstract;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CrytonCoreNext.CryptingOptionsViewModels
{
    public class AESViewModel : ViewModelBase
    {
        private readonly string[] SettingsKeys;
        private string _selectedKey;
        private string _selectedBlock;

        public ObservableCollection<string> BlockSizesComboBox { get; init; }

        public ObservableCollection<string> KeySizesComboBox { get; init; }

        public string SelectedBlockSize
        {
            get => _selectedBlock;
            set
            {
                if (_selectedBlock != value)
                {
                    _selectedBlock = value;
                    SelectedIVSize = Convert.ToInt32(_selectedBlock) / 4;
                    OnPropertyChanged(nameof(SelectedBlockSize));
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

        public string CryptorName { get; init; }

        public string BlocksSizeName { get; init; }

        public string KeyName { get; init; }

        public string IVName { get; init; }

        public string KeysName { get; init; }

        public string Key { get; set; }

        public string IV { get; set; }

        public string Error { get; private set; }

        public AESViewModel(AesCng aes, string[] settingKeys)
        {
            SettingsKeys = settingKeys;
            CryptorName = "AES";
            BlocksSizeName = "Block size";
            KeysName = "Key size";
            KeyName = "Key";
            IVName = "IV";

            BlockSizesComboBox = new ();
            KeySizesComboBox = new();

            var legalKeys = aes.LegalKeySizes[0];
            var legalBlocks = aes.LegalBlockSizes[0];

            if (legalKeys.SkipSize != 0)
            {
                for (var i = legalKeys.MinSize; i <= legalKeys.MaxSize; i+= legalKeys.SkipSize)
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

            SelectedBlockSize = BlockSizesComboBox.First();
            SelectedKey = KeySizesComboBox.First();
            OnPropertyChanged(nameof(BlockSizesComboBox));
            OnPropertyChanged(nameof(KeySizesComboBox));
        }

        public override Dictionary<string, object> GetObjects()
        {
            return new ()
            {
                { SettingsKeys[0], Key },
                { SettingsKeys[1], IV },
                { SettingsKeys[2], SelectedKey },
                { SettingsKeys[3], SelectedBlockSize },
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
                Key = Convert.ToHexString(key);
                IV = Convert.ToHexString(iv);
                OnPropertyChanged(nameof(Key));
                OnPropertyChanged(nameof(IV));
            }
        }
    }
}
