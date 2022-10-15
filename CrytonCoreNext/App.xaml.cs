using Microsoft.Extensions.DependencyInjection;
using CrytonCoreNext.ViewModels;
using System;
using System.Windows;
using CrytonCoreNext.Interfaces;
using CrytonCoreNext.InformationsServices;
using System.Collections.Generic;
using CrytonCoreNext.Services;
using CrytonCoreNext.Crypting;
using CrytonCoreNext.Serializers;
using CrytonCoreNext.Models;

namespace CrytonCoreNext
{
    public partial class App : Application
    {
        private static readonly Guid AppKey = new ("adae2137-dead-beef-6666-3eb841121af8");

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
                .AddSingleton(CreateCryptingRecognition)
                .AddSingleton(CreateFilesLoader)
                .AddSingleton(CreateFilesSaver)
                .AddSingleton<IFilesManager, FilesManager>()
                .AddSingleton(CreateFileService)
                .AddSingleton<IDialogService, DialogService>()
                .AddSingleton<IJsonSerializer, JsonSerializer>()
                .AddTransient<FilesViewViewModel>()
                .AddTransient(CreateAES)
                .AddTransient(CreateRSA)
                .AddTransient(CreateCryptingService)
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

        private CryptingViewModel CreateCryptingViewModel(IServiceProvider provider)
        {
            var fileService = provider.GetRequiredService<IFileService>();
            var dialogService = provider.GetRequiredService<IDialogService>();
            var cryptingService = provider.GetRequiredService<ICryptingService>();
            return new (fileService, dialogService, cryptingService);
        }

        private PdfManagerViewModel CreatePdfManagerViewModel(IServiceProvider provider)
        {
            var fileService = provider.GetRequiredService<IFileService>();
            var dialogService = provider.GetRequiredService<IDialogService>();
            return new(fileService, dialogService);
        }

        private ICrypting CreateAES(IServiceProvider provider)
        {
            var serialzer = provider.GetRequiredService<IJsonSerializer>();
            return new AES(serialzer);
        }

        private ICrypting CreateRSA(IServiceProvider provider)
        {
            return new RSA();
        }

        private ICryptingRecognition CreateCryptingRecognition(IServiceProvider provider)
        {
            var recognitionValues = new RecognitionValues(AppKey);
            return new CryptingRecognition(recognitionValues);
        }

        private IFilesLoader CreateFilesLoader(IServiceProvider provider)
        {
            var cryptingRecognition = provider.GetRequiredService<ICryptingRecognition>();
            return new FilesLoader(cryptingRecognition);
        }
        
        private IFilesSaver CreateFilesSaver(IServiceProvider provider)
        {
            var cryptingRecognition = provider.GetRequiredService<ICryptingRecognition>();
            return new FilesSaver(cryptingRecognition);
        }

        private IFileService CreateFileService(IServiceProvider provider)
        {
            var fileLoader = provider.GetRequiredService<IFilesLoader>();
            var fileSaver = provider.GetRequiredService<IFilesSaver>();
            var fileManager = provider.GetRequiredService<IFilesManager>();
            return new FileService(fileSaver, fileLoader, fileManager);
        }

        public ICryptingService CreateCryptingService(IServiceProvider provider)
        {
            var cryptors = provider.GetServices<ICrypting>();
            return new CryptingService(cryptors);
        }

        private void InitializeDictionary()
        {
            this.Resources.MergedDictionaries.Clear();
            this.Resources.MergedDictionaries.Add(LanguagesDictionaries[0]);
        }
    }
}