using CrytonCoreNext.Abstract;
using DragDropDemo.Commands;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CrytonCoreNext.ViewModels
{
    public class FilesSelectorListingViewViewModel : ViewModelBase
    {
        private readonly ObservableCollection<Models.File> _ItemViewModels;

        public IEnumerable<Models.File> ItemViewModels => _ItemViewModels;

        private Models.File _incomingItemViewModel;

        public Models.File IncomingItemViewModel
        {
            get
            {
                return _incomingItemViewModel;
            }
            set
            {
                _incomingItemViewModel = value;
                OnPropertyChanged(nameof(IncomingItemViewModel));
            }
        }

        private Models.File _removedItemViewModel;

        public Models.File RemovedItemViewModel
        {
            get
            {
                return _removedItemViewModel;
            }
            set
            {
                _removedItemViewModel = value;
                OnPropertyChanged(nameof(RemovedItemViewModel));
            }
        }

        private Models.File _insertedItemViewModel;

        public Models.File InsertedItemViewModel
        {
            get
            {
                return _insertedItemViewModel;
            }
            set
            {
                _insertedItemViewModel = value;
                OnPropertyChanged(nameof(InsertedItemViewModel));
            }
        }

        private Models.File _targetItemViewModel;
        public Models.File TargetItemViewModel
        {
            get
            {
                return _targetItemViewModel;
            }
            set
            {
                _targetItemViewModel = value;
                OnPropertyChanged(nameof(TargetItemViewModel));
            }
        }

        public ICommand ItemReceivedCommand { get; }
        public ICommand ItemRemovedCommand { get; }
        public ICommand ItemInsertedCommand { get; }

        public FilesSelectorListingViewViewModel()
        {
            _ItemViewModels = new ObservableCollection<Models.File>();

            ItemReceivedCommand = new ItemReceivedCommand(this);
            ItemRemovedCommand = new ItemRemovedCommand(this);
            ItemInsertedCommand = new ItemInsertedCommand(this);
        }

        public void AddItem(Models.File item)
        {
            if (!_ItemViewModels.Contains(item))
            {
                _ItemViewModels.Add(item);
            }
            OnPropertyChanged(nameof(ItemViewModels));
        }

        public void InsertItem(Models.File insertedItem, Models.File targetItem)
        {
            if (insertedItem == targetItem)
            {
                return;
            }

            int oldIndex = _ItemViewModels.IndexOf(insertedItem);
            int nextIndex = _ItemViewModels.IndexOf(targetItem);

            if (oldIndex != -1 && nextIndex != -1)
            {
                _ItemViewModels.Move(oldIndex, nextIndex);
            }
        }

        public void RemoveItem(Models.File item)
        {
            _ItemViewModels.Remove(item);
        }
    }
}
