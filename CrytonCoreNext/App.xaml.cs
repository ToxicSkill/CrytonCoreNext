using Microsoft.Extensions.DependencyInjection;
using CrytonCoreNext.ViewModels;
using System;
using System.Windows;
using CrytonCoreNext.Interfaces;
using CrytonCoreNext.InformationsServices;

namespace CrytonCoreNext
{
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;

        public App()
        {
            IServiceCollection services = new ServiceCollection();

            _ = services
                .AddSingleton<IInternetConnection, InternetConnection>()
                .AddSingleton<ITimeDate, TimeDate>()
                .AddSingleton(CreateHomeViewModel)
                .AddSingleton(CreateCrytpingViewModel)
                .AddSingleton(CreateMainWindowViewModel)
                .AddSingleton(s => new MainWindow()
                {
                    DataContext = s.GetRequiredService<MainViewModel>()
                });

            _serviceProvider = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            MainWindow.Show();

            base.OnStartup(e);
        }

        private HomeViewModel CreateHomeViewModel(IServiceProvider provider)
        {
            var timeDate = provider.GetService<ITimeDate>();
            var internetConnection = provider.GetService<IInternetConnection>();

            return new HomeViewModel(timeDate, internetConnection);
        }

        private CryptingViewModel CreateCrytpingViewModel(IServiceProvider provider)
        {
            return new CryptingViewModel();
        }

        private MainViewModel CreateMainWindowViewModel(IServiceProvider provider)
        {
            var homeView = provider.GetService<HomeViewModel>();
            var cryptingView = provider.GetService<CryptingViewModel>();

            return new MainViewModel(homeView, cryptingView);
        }
    }
}