using Microsoft.Extensions.DependencyInjection;
using CrytonCoreNext.ViewModels;
using CrytonCoreNext.CryptingOptionsViewModels;
using System;
using System.Windows;
using CrytonCoreNext.Interfaces;
using CrytonCoreNext.InformationsServices;
using System.Collections.Generic;
using CrytonCoreNext.Services;
using CrytonCoreNext.Crypting;

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
                .AddSingleton<IFilesLoader, FilesLoader>()
                .AddSingleton<IFilesManager>(CreateFilesManager)
                .AddTransient<FilesViewViewModel>()
                .AddTransient<ICrypting>(CreateAES)
                .AddTransient<CryptingOptionsViewModel>()
                .AddTransient<InformationPopupViewModel>()
                .AddSingleton(CreateHomeViewModel)
                .AddSingleton(CreateCryptingViewModel)
                .AddSingleton(CreatePdfManagerViewModel)
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
            var pdfManagerView = provider.GetService<PdfManagerViewModel>();

            return new (homeView, cryptingView, pdfManagerView);
        }

        private FilesManager CreateFilesManager(IServiceProvider provider)
        {
            var filesLoader = provider.GetRequiredService<IFilesLoader>();
            return new FilesManager(filesLoader);
        }

        private CryptingViewModel CreateCryptingViewModel(IServiceProvider provider)
        {
            var filesManager = provider.GetRequiredService<IFilesManager>();
            var cryptors = provider.GetServices<ICrypting>();
            return new (filesManager, cryptors);
        }

        private PdfManagerViewModel CreatePdfManagerViewModel(IServiceProvider provider)
        {
            var filesManager = provider.GetRequiredService<IFilesManager>();
            return new(filesManager);
        }

        private ICrypting CreateAES(IServiceProvider provider)
        {
            var viewModel = new AESViewModel();
            return new AES(viewModel);
        }

        private void InitializeDictionary()
        {
            this.Resources.MergedDictionaries.Clear();
            this.Resources.MergedDictionaries.Add(LanguagesDictionaries[0]);
        }
    }
}