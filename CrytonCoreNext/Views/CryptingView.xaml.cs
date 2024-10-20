﻿using CrytonCoreNext.BackgroundUI;
using CrytonCoreNext.ViewModels;
using Wpf.Ui.Controls;

namespace CrytonCoreNext.Views
{
    public partial class CryptingView : INavigableView<CryptingViewModel>
    {
        public CryptingViewModel ViewModel
        {
            get;
        }

        public CryptingView(CryptingViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            DataContext = ViewModel;
            Background.Content = new FluentWaves();
        }

        private void ConfirmPassword_Button(object sender, System.Windows.RoutedEventArgs e)
        {
            UpdatePasswordBox();
        }

        private void UpdatePasswordBox()
        {
            ViewModel.FilePassword = FilePasswordBox.Password;
        }

        private void FilePasswordBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            ViewModel.FilePassword = FilePasswordBox.Password;
        }
    }
}
