using CrytonCoreNext.Abstract;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;

namespace CrytonCoreNext.CryptingOptionsViewModels
{
    public class AESViewModel : ViewModelBase
    {
        private string _selectedKey;
        private string _selectedBlock;

        public ObservableCollection<string> BlockSizesComboBox { get; init; }

        public string SelectedBlockSize { get; set; }

        public ObservableCollection<string> KeySizesComboBox { get; init; }

        public string SelectedKey 
        {
            get => _selectedKey;
            set
            {
                if (_selectedKey != value)
                {
                    _selectedKey = value;
                    SelectedKeySize = Convert.ToInt32(_selectedKey);
                    OnPropertyChanged(nameof(SelectedKey));
                    OnPropertyChanged(nameof(SelectedKeySize));
                }
            }
        }

        public int SelectedKeySize { get; set; }

        public string CryptorName { get; init; }

        public string BlocksSizeName { get; init; }

        public string KeyName { get; init; }

        public string IVName { get; init; }

        public string KeysName { get; init; }

        public AESViewModel(AesCng aes)
        {
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
                { "Key", SelectedKey },
                { "Block", SelectedBlockSize }
            };
        }
    }
}
