using CommunityToolkit.Mvvm.ComponentModel;
using System;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace CrytonCoreNext.ViewModels
{
    [ObservableObject]
    public partial class CameraViewModel
    {
        private readonly INavigationService _navigationService;

        public CameraViewModel(IServiceProvider serviceProvider)
        {
            _navigationService = new NavigationService(serviceProvider);
        }

        public void SetNavigationControl(INavigationView navigation)
        {
            _navigationService.SetNavigationControl(navigation);
        }
    }
}
