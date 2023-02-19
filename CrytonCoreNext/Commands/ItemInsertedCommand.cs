using CrytonCoreNext.ViewModels;
using System;
using System.Windows.Input;

namespace CrytonCoreNext.Commands
{
    public class ItemInsertedCommand : ICommand
    {
        private readonly FilesSelectorListingViewViewModel _viewModel;

        public ItemInsertedCommand(FilesSelectorListingViewViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _viewModel.InsertItem(_viewModel.InsertedItemViewModel, _viewModel.TargetItemViewModel);
        }
    }
}
