using CrytonCoreNext.ViewModels;
using System;
using System.Windows.Input;

namespace CrytonCoreNext.Commands
{
    public class ItemReceivedCommand : ICommand
    {
        private readonly FilesSelectorListingViewViewModel _ItemListingViewModel;

        public ItemReceivedCommand(FilesSelectorListingViewViewModel ItemListingViewModel)
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
            _ItemListingViewModel.AddItem(_ItemListingViewModel.IncomingItemViewModel);
        }
    }
}
