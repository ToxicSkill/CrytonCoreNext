using Microsoft.Extensions.DependencyInjection;
using CrytonCoreNext.ViewModels;
using System;
using System.Windows;
using CrytonCoreNext.Interfaces;
using CrytonCoreNext.InformationsServices;
using System.Collections.Generic;

namespace CrytonCoreNext
{
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;

        private readonly List<ResourceDictionary> LanguagesDictionaries;

        public App()
        {
            LanguagesDictionaries = new List<ResourceDictionary>(){
                new ResourceDictionary() { Source = new Uri("..\\Dictionaries\\EnglishDictionary.xaml", UriKind.Relative) }
            };

            InitializeDictionary();

            IServiceCollection services = new ServiceCollection();

            _ = services
                .AddSingleton<IInternetConnection, InternetConnection>()
                .AddSingleton<ITimeDate, TimeDate>()
                .AddTransient<FilesViewViewModel>()
                .AddTransient<InformationPopupViewModel>()
                .AddSingleton(CreateHomeViewModel)
                .AddSingleton<CryptingViewModel>()
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

            return new (timeDate, internetConnection);
        }


        private MainViewModel CreateMainWindowViewModel(IServiceProvider provider)
        {
            var homeView = provider.GetService<HomeViewModel>();
            var cryptingView = provider.GetService<CryptingViewModel>();

            return new (homeView, cryptingView);
        }

        private void InitializeDictionary()
        {
            this.Resources.MergedDictionaries.Clear();
            this.Resources.MergedDictionaries.Add(LanguagesDictionaries[0]);
        }
    }
}