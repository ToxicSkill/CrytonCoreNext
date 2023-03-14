using CrytonCoreNext.Crypting.Cryptors;
using CrytonCoreNext.Crypting.Interfaces;
using CrytonCoreNext.Crypting.Models;
using CrytonCoreNext.Crypting.Services;
using CrytonCoreNext.Crypting.ViewModels;
using CrytonCoreNext.Crypting.Views;
using CrytonCoreNext.Interfaces;
using CrytonCoreNext.Models;
using CrytonCoreNext.PDF.Interfaces;
using CrytonCoreNext.PDF.Models;
using CrytonCoreNext.PDF.Services;
using CrytonCoreNext.PDF.ViewModels;
using CrytonCoreNext.PDF.Views;
using CrytonCoreNext.Serializers;
using CrytonCoreNext.Services;
using CrytonCoreNext.ViewModels;
using CrytonCoreNext.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            services.AddSingleton<ICustomPageService, PageService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<ISnackbarService, SnackbarService>();

            services.AddSingleton<Interfaces.IDialogService, Services.DialogService>();
            services.AddTransient<IProgressService, ProgressService>();
            services.AddSingleton<IFilesLoader, FilesLoader>();
            services.AddSingleton<IFilesSaver, FilesSaver>();
            services.AddSingleton<IFilesManager, FilesManager>();
            services.AddSingleton(CreateFileService);
            services.AddTransient(CreateFilesView);

            services.AddSingleton<IJsonSerializer, JsonSerializer>();
            services.AddSingleton<IXmlSerializer, XmlSerializer>();

            // crypting
            services.AddScoped(CreateCryptingRecognition);
            services.AddScoped<ICryptingReader, CryptingReader>();
            services.AddScoped<ICrypting, AES>();
            services.AddScoped<ICrypting, RSA>();
            services.AddScoped(CreateAESViewModel);
            services.AddScoped(CreateRSAViewModel);
            services.AddSingleton(CreateAESView);
            services.AddSingleton(CreateRSAView);
            services.AddSingleton(CreateCryptingService);
            services.AddSingleton<CryptingView>();
            services.AddSingleton(CreateCryptingViewModel);

            // pdf
            services.AddTransient(CreateFilesLeftView);
            services.AddTransient(CreateFilesSelectorListingViewViewModel);
            services.AddSingleton<IPDFManager, PDFManager>();
            services.AddSingleton<IPDFReader, PDFReader>();
            services.AddSingleton(CreatePDFService);
            services.AddScoped(CreateFileService);
            services.AddScoped<PdfMergeView>();
            services.AddScoped(CreatePdfMergeViewModel);
            services.AddScoped<PdfSplitView>();
            services.AddScoped(CreatePdfSplitViewModel);
            services.AddScoped<PdfImageToPdfView>();
            services.AddScoped(CreatePdfImageToPdfViewModel);

            services.AddScoped<Dashboard>();
            services.AddScoped<DashboardViewModel>();

            services.AddScoped<SettingsView>();
            services.AddScoped<SettingsViewModel>();

            services.AddScoped<NavigationPDFView>();
            services.AddScoped(CreateNavigationPDFViewModel);

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

        private void OnLoaded(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            InitializeDictionary();
        }

        private async void OnExit(object sender, ExitEventArgs e)
        {
            await _host.StopAsync();
            _host.Dispose();
        }

        private static FilesSelectorListingViewViewModel CreateFilesSelectorListingViewViewModel(IServiceProvider provider)
        {
            return new FilesSelectorListingViewViewModel();
        }

        private static PdfMergeViewModel CreatePdfMergeViewModel(IServiceProvider provider)
        {
            var pdfService = provider.GetRequiredService<IPDFService>();

            return new PdfMergeViewModel(pdfService);
        }
        private static PdfImageToPdfViewModel CreatePdfImageToPdfViewModel(IServiceProvider provider)
        {
            var pdfService = provider.GetRequiredService<IPDFService>();
            var filesSelectorView = provider.GetRequiredService<FilesSelectorViewViewModel>();

            return new PdfImageToPdfViewModel(pdfService, filesSelectorView);
        }

        private static PdfSplitViewModel CreatePdfSplitViewModel(IServiceProvider provider)
        {
            var pdfService = provider.GetRequiredService<IPDFService>();

            return new PdfSplitViewModel(pdfService);
        }

        private static MainViewModel CreateMainWindowViewModel(IServiceProvider provider)
        {
            var pageService = provider.GetRequiredService<ICustomPageService>();
            return new(pageService);
        }

        private static NavigationPDFViewViewModel CreateNavigationPDFViewModel(IServiceProvider provider)
        {
            var pdfMergeView = provider.GetRequiredService<PdfMergeView>();
            var pdfSplitView = provider.GetRequiredService<PdfSplitView>();
            var pdfImageView = provider.GetRequiredService<PdfImageToPdfView>();
            var navigationService = provider.GetRequiredService<INavigationService>();

            return new NavigationPDFViewViewModel(navigationService, pdfMergeView, pdfSplitView, pdfImageView);
        }

        private static IPDFService CreatePDFService(IServiceProvider provider)
        {
            var pdfManager = provider.GetRequiredService<IPDFManager>();
            var pdfReader = provider.GetRequiredService<IPDFReader>();

            return new PDFService(pdfManager, pdfReader);
        }

        private static CryptingViewModel CreateCryptingViewModel(IServiceProvider provider)
        {
            var fileService = provider.GetRequiredService<IFileService>();
            var dialogService = provider.GetRequiredService<Interfaces.IDialogService>();
            var cryptingService = provider.GetRequiredService<ICryptingService>();
            var filesView = provider.GetRequiredService<IFilesView>();
            var snackbar = provider.GetRequiredService<ISnackbarService>();
            var cryptors = provider.GetServices<ICryptingView<CryptingMethodViewModel>>();

            return new(fileService, dialogService, cryptingService, filesView, snackbar, cryptors.ToList());
        }

        public static ICryptingService CreateCryptingService(IServiceProvider provider)
        {
            var cryptingRecognition = provider.GetRequiredService<ICryptingRecognition>();
            var cryptingReader = provider.GetRequiredService<ICryptingReader>();
            return new CryptingService(cryptingRecognition, cryptingReader);
        }

        private static ICryptingView<CryptingMethodViewModel> CreateAESView(IServiceProvider provider)
        {
            var aesViewModel = provider.GetServices<CryptingMethodViewModel>().ToList().Where(x => x.PageName == Crypting.Enums.EMethod.AES.ToString()).First();
            return new AESView(aesViewModel);
        }

        private static ICryptingView<CryptingMethodViewModel> CreateRSAView(IServiceProvider provider)
        {
            var rsaViewModel = provider.GetServices<CryptingMethodViewModel>().ToList().Where(x => x.PageName == Crypting.Enums.EMethod.RSA.ToString()).First();
            return new RSAView(rsaViewModel);
        }

        private static CryptingMethodViewModel CreateAESViewModel(IServiceProvider provider)
        {
            var jsonSerialzer = provider.GetRequiredService<IJsonSerializer>();
            var snackbar = provider.GetRequiredService<ISnackbarService>();
            var aes = provider.GetServices<ICrypting>().ToList().Where(x => x.Method == Crypting.Enums.EMethod.AES).First();
            return new AESViewModel(aes, snackbar, jsonSerialzer, aes.Method.ToString());
        }

        private static CryptingMethodViewModel CreateRSAViewModel(IServiceProvider provider)
        {
            var jsonSerialzer = provider.GetRequiredService<IJsonSerializer>();
            var xmlSerialzer = provider.GetRequiredService<IXmlSerializer>();
            var snackbar = provider.GetRequiredService<ISnackbarService>();
            var rsa = provider.GetServices<ICrypting>().ToList().Where(x => x.Method == Crypting.Enums.EMethod.RSA).First();
            return new RSAViewModel(rsa, snackbar, jsonSerialzer, xmlSerialzer, rsa.Method.ToString());
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

        private void InitializeDictionary()
        {
            //this.Resources.MergedDictionaries.Clear();
            this.Resources.MergedDictionaries.Add(LanguagesDictionaries[0]);
        }
    }
}