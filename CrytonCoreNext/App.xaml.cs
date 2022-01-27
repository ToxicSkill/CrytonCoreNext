using Microsoft.Extensions.DependencyInjection;
using CrytonCoreNext.Services;
using CrytonCoreNext.Stores;
using CrytonCoreNext.ViewModels;
using System;
using System.Windows;

namespace CrytonCoreNext
{
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;

        public App()
        {
            IServiceCollection services = new ServiceCollection();

            _ = services
                .AddSingleton<NavigationStore>()
                .AddSingleton<ModalNavigationStore>()
                .AddSingleton<INavigationService>(s => CreateHomeNavigationService(s))
                .AddSingleton<CloseModalNavigationService>()
                .AddTransient<HomeViewModel>()      
                .AddTransient<CryptingViewModel>()
                .AddTransient<PdfManagerViewModel>()
                .AddTransient(CreateNavigationBarViewModel)
                .AddSingleton<MainViewModel>()
                .AddSingleton(s => new MainWindow()
                {
                    DataContext = s.GetRequiredService<MainViewModel>()
                });

            _serviceProvider = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            INavigationService initialNavigationService = _serviceProvider.GetRequiredService<INavigationService>();
            initialNavigationService.Navigate();

            MainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            MainWindow.Show();

            base.OnStartup(e);
        }

        private INavigationService CreatePdfManagerNavigationService(IServiceProvider serviceProvider)
        {
            return new LayoutNavigationService<CryptingViewModel>(
                serviceProvider.GetRequiredService<NavigationStore>(),
                () => serviceProvider.GetRequiredService<CryptingViewModel>(),
                () => serviceProvider.GetRequiredService<NavigationBarViewModel>());
        }

        private INavigationService CreateHelpNavigationService(IServiceProvider serviceProvider)
        {
            return new ModalNavigationService<CryptingViewModel>(
                serviceProvider.GetRequiredService<ModalNavigationStore>(),
                () => serviceProvider.GetRequiredService<CryptingViewModel>());
        }

        private INavigationService CreateSettingsNavigationService(IServiceProvider serviceProvider)
        {
            return new ModalNavigationService<CryptingViewModel>(
                serviceProvider.GetRequiredService<ModalNavigationStore>(),
                () => serviceProvider.GetRequiredService<CryptingViewModel>());
        }

        private INavigationService CreateCryptingNavigationService(IServiceProvider serviceProvider)
        {
            return new LayoutNavigationService<CryptingViewModel>(
                serviceProvider.GetRequiredService<NavigationStore>(),
                () => serviceProvider.GetRequiredService<CryptingViewModel>(),
                () => serviceProvider.GetRequiredService<NavigationBarViewModel>());
        }

        private INavigationService CreateHomeNavigationService(IServiceProvider serviceProvider)
        {
            return new LayoutNavigationService<HomeViewModel>(
                serviceProvider.GetRequiredService<NavigationStore>(),
                () => serviceProvider.GetRequiredService<HomeViewModel>(),
                () => serviceProvider.GetRequiredService<NavigationBarViewModel>());
        }

        private NavigationBarViewModel CreateNavigationBarViewModel(IServiceProvider serviceProvider)
        {
            return new NavigationBarViewModel(
                CreateHomeNavigationService(serviceProvider),
                CreateCryptingNavigationService(serviceProvider),
                CreateSettingsNavigationService(serviceProvider),
                CreatePdfManagerNavigationService(serviceProvider),
                CreateHelpNavigationService(serviceProvider)
                );
        }
    }
}
