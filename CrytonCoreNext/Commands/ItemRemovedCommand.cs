using CrytonCoreNext.ViewModels;
using System;
using System.Windows.Input;

namespace DragDropDemo.Commands
{
    public class ItemRemovedCommand : ICommand
    {
        private readonly FilesSelectorListingViewViewModel _ItemListingViewModel;

        public ItemRemovedCommand(FilesSelectorListingViewViewModel ItemListingViewModel)
        {
            _ItemListingViewModel = ItemListingViewModel;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _ItemListingViewModel.RemoveItem(_ItemListingViewModel.RemovedItemViewModel);
        }
    }
}
