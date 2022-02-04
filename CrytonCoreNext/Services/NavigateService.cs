using CrytonCoreNext.Abstract;
using CrytonCoreNext.Interfaces;
using System;
using System.Windows.Input;

namespace CrytonCoreNext.Services
{
    public class NavigateService : ICommand
    {
        private readonly INavigate _navigator;
        private readonly ViewModelBase _viewModel;

        public NavigateService(INavigate navigator, ViewModelBase viewModel)
        {
            _navigator = navigator;
            _viewModel = viewModel;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter)
        {
            _navigator.Navigate(_viewModel);
        }
    }
}
