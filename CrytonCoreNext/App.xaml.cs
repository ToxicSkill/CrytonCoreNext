using CrytonCoreNext.Crypting.Cryptors;
using CrytonCoreNext.Crypting.Interfaces;
using CrytonCoreNext.Crypting.Models;
using CrytonCoreNext.Crypting.Services;
using CrytonCoreNext.Interfaces;
using CrytonCoreNext.Models;
using CrytonCoreNext.PDF.Interfaces;
using CrytonCoreNext.PDF.Services;
using CrytonCoreNext.PDF.ViewModels;
using CrytonCoreNext.Services;
using CrytonCoreNext.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using Wpf.Ui.Mvvm.Contracts;
using Wpf.Ui.Mvvm.Services;

namespace CrytonCoreNext
{
    public partial class App
    {
        private static readonly Guid AppKey = new("adae2137-dead-beef-6666-3eb841121af8");

        private static readonly IHost _host = Host
        .CreateDefaultBuilder()
        .ConfigureAppConfiguration(c => { c.SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)); })
        .ConfigureServices((context, services) =>
        {
            services.AddHostedService<ApplicationHostService>();
            services.AddSingleton<IThemeService, ThemeService>();
            services.AddSingleton<IPageService, PageService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddScoped<INavigationWindow, MainWindow>();
            services.AddScoped<MainViewModel>();


            //services.AddSingleton<IInternetConnection, InternetConnection>();
            //services.AddSingleton<ITimeDate, TimeDate>();
            //services.AddSingleton(CreateCryptingRecognition);
            //services.AddSingleton<IFilesLoader, FilesLoader>();
            //services.AddSingleton<IFilesSaver, FilesSaver>();
            //services.AddSingleton<IFilesManager, FilesManager>(); ;
            //services.AddSingleton<IPDFManager, PDFManager>();
            //services.AddSingleton<IPDFReader, PDFReader>();
            //services.AddSingleton<ICryptingReader, CryptingReader>();
            //services.AddSingleton(CreateFileService);
            //services.AddTransient(CreatePDFService);
            //services.AddTransient(CreateFilesView);
            //services.AddTransient(CreateFilesLeftView);
            //services.AddTransient(CreateImagesView);
            //services.AddTransient(CreateFilesSelectorListingViewViewModel);
            //services.AddSingleton<IDialogService, DialogService>();
            //services.AddTransient<IProgressService, ProgressService>();
            //services.AddSingleton<IJsonSerializer, JsonSerializer>();
            //services.AddSingleton<IXmlSerializer, XmlSerializer>();
            //services.AddTransient(CreateProgressViewModel);
            //services.AddTransient(CreateAES);
            //services.AddTransient(CreateRSA);
            //services.AddTransient(CreateCryptingService);
            //services.AddTransient<InformationPopupViewModel>();
            //services.AddSingleton(CreateCryptingViewModel);
            //services.AddSingleton(CreatePdfManagerViewModel);
            //services.AddSingleton(CreatePdfMergeViewModel);
            //services.AddSingleton(CreatePdfSplitViewModel);
            //services.AddSingleton(CreatePdfImageToPdfViewModel);
            //services.AddSingleton(CreateMainWindowViewModel);
            //services.Configure<AppConfig>(context.Configuration.GetSection(nameof(AppConfig)));
        }).Build();

        private readonly List<ResourceDictionary> LanguagesDictionaries;

        public App()
        {
            LanguagesDictionaries = new List<ResourceDictionary>(){
                new ResourceDictionary() { Source = new Uri("..\\Dictionaries\\EnglishDictionary.xaml", UriKind.Relative) }
            };
        }
        public static T GetService<T>()
        where T : class
        {
            return _host.Services.GetService(typeof(T)) as T;
        }

        private async void OnStartup(object sender, StartupEventArgs e)
        {
            await _host.StartAsync();
        }

        private async void OnExit(object sender, ExitEventArgs e)
        {
            await _host.StopAsync();

            _host.Dispose();
        }


        private static PdfImageToPdfViewModel CreatePdfImageToPdfViewModel(IServiceProvider provider)
        {
            var pdfManager = provider.GetRequiredService<PdfManagerViewModel>();
            var pdfService = provider.GetRequiredService<IPDFService>();
            var filesSelectorView = provider.GetRequiredService<FilesSelectorViewViewModel>();

            return new PdfImageToPdfViewModel(pdfManager, filesSelectorView, pdfService);
        }

        private static FilesSelectorListingViewViewModel CreateFilesSelectorListingViewViewModel(IServiceProvider provider)
        {
            return new FilesSelectorListingViewViewModel();
        }

        private static PdfMergeViewModel CreatePdfMergeViewModel(IServiceProvider provider)
        {
            var pdfManager = provider.GetRequiredService<PdfManagerViewModel>();
            var pdfService = provider.GetRequiredService<IPDFService>();

            return new PdfMergeViewModel(pdfManager, pdfService);
        }

        private static PdfSplitViewModel CreatePdfSplitViewModel(IServiceProvider provider)
        {
            var pdfManager = provider.GetRequiredService<PdfManagerViewModel>();
            var pdfService = provider.GetRequiredService<IPDFService>();

            return new PdfSplitViewModel(pdfManager, pdfService);
        }

        private static MainViewModel CreateMainWindowViewModel(IServiceProvider provider)
        {
            return new();
        }

        private static CryptingViewModel CreateCryptingViewModel(IServiceProvider provider)
        {
            var fileService = provider.GetRequiredService<IFileService>();
            var dialogService = provider.GetRequiredService<Interfaces.IDialogService>();
            var cryptingService = provider.GetRequiredService<ICryptingService>();
            var filesView = provider.GetRequiredService<IFilesView>();
            var progressView = provider.GetRequiredService<IProgressView>();

            return new(fileService, dialogService, cryptingService, filesView, progressView);
        }

        private static PdfManagerViewModel CreatePdfManagerViewModel(IServiceProvider provider)
        {
            var fileService = provider.GetRequiredService<IFileService>();
            var dialogService = provider.GetRequiredService<Interfaces.IDialogService>();
            var filesView = provider.GetRequiredService<IFilesView>();
            var progressView = provider.GetRequiredService<IProgressView>();
            var pdfService = provider.GetRequiredService<IPDFService>();

            return new(fileService, dialogService, filesView, progressView, pdfService);
        }

        private static IPDFService CreatePDFService(IServiceProvider provider)
        {
            var pdfManager = provider.GetRequiredService<IPDFManager>();
            var pdfReader = provider.GetRequiredService<IPDFReader>();

            return new PDFService(pdfManager, pdfReader);
        }

        private static ICrypting CreateAES(IServiceProvider provider)
        {
            var jsonSerialzer = provider.GetRequiredService<IJsonSerializer>();
            return new AES(jsonSerialzer);
        }

        private static ICrypting CreateRSA(IServiceProvider provider)
        {
            var jsonSerialzer = provider.GetRequiredService<IJsonSerializer>();
            var xmlSerialzer = provider.GetRequiredService<IXmlSerializer>();
            var progressView = provider.GetRequiredService<IProgressView>();
            return new RSA(jsonSerialzer, xmlSerialzer, progressView);
        }

        private static ICryptingRecognition CreateCryptingRecognition(IServiceProvider provider)
        {
            var recognitionValues = new RecognitionValues(AppKey);
            return new CryptingRecognition(recognitionValues);
        }

        private static IFileService CreateFileService(IServiceProvider provider)
        {
            var fileLoader = provider.GetRequiredService<IFilesLoader>();
            var fileSaver = provider.GetRequiredService<IFilesSaver>();
            var fileManager = provider.GetRequiredService<IFilesManager>();
            return new FileService(fileSaver, fileLoader, fileManager);
        }

        public static ICryptingService CreateCryptingService(IServiceProvider provider)
        {
            var cryptingRecognition = provider.GetRequiredService<ICryptingRecognition>();
            var cryptingReader = provider.GetRequiredService<ICryptingReader>();
            var cryptors = provider.GetServices<ICrypting>();
            return new CryptingService(cryptingRecognition, cryptingReader, cryptors);
        }

        public static IFilesView CreateFilesView(IServiceProvider provider)
        {
            var fileService = provider.GetRequiredService<IFileService>();
            return new FilesViewViewModel(fileService);
        }

        public static FilesSelectorViewViewModel CreateFilesLeftView(IServiceProvider provider)
        {
            var progressItemViewViewModel = provider.GetRequiredService<FilesSelectorListingViewViewModel>();
            var completedItemViewViewModel = provider.GetRequiredService<FilesSelectorListingViewViewModel>();
            return new FilesSelectorViewViewModel(progressItemViewViewModel, completedItemViewViewModel);
        }

        public static IImageView CreateImagesView(IServiceProvider provider)
        {
            return new ImageViewerViewModel();
        }

        public static IProgressView CreateProgressViewModel(IServiceProvider provider)
        {
            var progressService = provider.GetRequiredService<IProgressService>();
            return new ProgressViewModel(progressService);
        }

        private void InitializeDictionary()
        {
            this.Resources.MergedDictionaries.Clear();
            this.Resources.MergedDictionaries.Add(LanguagesDictionaries[0]);
        }
    }
}