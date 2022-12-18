using CrytonCoreNext.Crypting.Cryptors;
using CrytonCoreNext.Crypting.Interfaces;
using CrytonCoreNext.Crypting.Models;
using CrytonCoreNext.Crypting.Services;
using CrytonCoreNext.InformationsServices;
using CrytonCoreNext.Interfaces;
using CrytonCoreNext.Models;
using CrytonCoreNext.PDF.Interfaces;
using CrytonCoreNext.PDF.Models;
using CrytonCoreNext.PDF.Services;
using CrytonCoreNext.PDF.ViewModels;
using CrytonCoreNext.Serializers;
using CrytonCoreNext.Services;
using CrytonCoreNext.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Windows;

namespace CrytonCoreNext
{
    public partial class App : Application
    {
        private static readonly Guid AppKey = new("adae2137-dead-beef-6666-3eb841121af8");

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
                .AddSingleton<IFilesLoader, FilesLoader>()
                .AddSingleton<IFilesSaver, FilesSaver>()
                .AddSingleton<IFilesManager, FilesManager>()
                .AddSingleton<IPDFManager, PDFManager>()
                .AddSingleton<IPDFReader, PDFReader>()
                .AddSingleton<ICryptingReader, CryptingReader>()
                .AddSingleton(CreateFileService)
                .AddTransient(CreatePDFService)
                .AddTransient(CreateFilesView)
                .AddTransient(CreateFilesLeftView)
                .AddTransient(CreateImagesView)
                .AddTransient(CreateFilesSelectorListingViewViewModel)
                .AddSingleton<IDialogService, DialogService>()
                .AddTransient<IProgressService, ProgressService>()
                .AddSingleton<IJsonSerializer, JsonSerializer>()
                .AddSingleton<IXmlSerializer, XmlSerializer>()
                .AddTransient(CreateProgressViewModel)
                .AddTransient(CreateAES)
                .AddTransient(CreateRSA)
                .AddTransient(CreateCryptingService)
                .AddTransient<InformationPopupViewModel>()
                .AddSingleton(CreateHomeViewModel)
                .AddSingleton(CreateCryptingViewModel)
                .AddSingleton(CreatePdfManagerViewModel)
                .AddSingleton(CreatePdfMergeViewModel)
                .AddSingleton(CreatePdfSplitViewModel)
                .AddSingleton(CreatePdfImageToPdfViewModel)
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

            return new(timeDate, internetConnection);
        }

        private PdfImageToPdfViewModel CreatePdfImageToPdfViewModel(IServiceProvider provider)
        {
            var pdfManager = provider.GetRequiredService<PdfManagerViewModel>();
            var pdfService = provider.GetRequiredService<IPDFService>();
            var filesSelectorView = provider.GetRequiredService<FilesSelectorViewViewModel>();

            return new PdfImageToPdfViewModel(pdfManager, filesSelectorView, pdfService);
        }

        private FilesSelectorListingViewViewModel CreateFilesSelectorListingViewViewModel(IServiceProvider provider)
        {
            return new FilesSelectorListingViewViewModel();
        }

        private PdfMergeViewModel CreatePdfMergeViewModel(IServiceProvider provider)
        {
            var pdfManager = provider.GetRequiredService<PdfManagerViewModel>();
            var pdfService = provider.GetRequiredService<IPDFService>();

            return new PdfMergeViewModel(pdfManager, pdfService);
        }

        private PdfSplitViewModel CreatePdfSplitViewModel(IServiceProvider provider)
        {
            var pdfManager = provider.GetRequiredService<PdfManagerViewModel>();
            var pdfService = provider.GetRequiredService<IPDFService>();

            return new PdfSplitViewModel(pdfManager, pdfService);
        }

        private MainViewModel CreateMainWindowViewModel(IServiceProvider provider)
        {
            var homeView = provider.GetRequiredService<HomeViewModel>();
            var cryptingView = provider.GetRequiredService<CryptingViewModel>();
            var pdfManagerView = provider.GetRequiredService<PdfManagerViewModel>();
            var pdfMergeView = provider.GetRequiredService<PdfMergeViewModel>();
            var pdfSplitView = provider.GetRequiredService<PdfSplitViewModel>();
            var pdfImageToPdfView = provider.GetRequiredService<PdfImageToPdfViewModel>();

            return new(homeView, cryptingView, pdfManagerView, pdfMergeView, pdfSplitView, pdfImageToPdfView);
        }

        private CryptingViewModel CreateCryptingViewModel(IServiceProvider provider)
        {
            var fileService = provider.GetRequiredService<IFileService>();
            var dialogService = provider.GetRequiredService<IDialogService>();
            var cryptingService = provider.GetRequiredService<ICryptingService>();
            var filesView = provider.GetRequiredService<IFilesView>();
            var progressView = provider.GetRequiredService<IProgressView>();

            return new(fileService, dialogService, cryptingService, filesView, progressView);
        }

        private PdfManagerViewModel CreatePdfManagerViewModel(IServiceProvider provider)
        {
            var fileService = provider.GetRequiredService<IFileService>();
            var dialogService = provider.GetRequiredService<IDialogService>();
            var filesView = provider.GetRequiredService<IFilesView>();
            var progressView = provider.GetRequiredService<IProgressView>();
            var pdfService = provider.GetRequiredService<IPDFService>();

            return new(fileService, dialogService, filesView, progressView, pdfService);
        }

        private IPDFService CreatePDFService(IServiceProvider provider)
        {
            var pdfManager = provider.GetRequiredService<IPDFManager>();
            var pdfReader = provider.GetRequiredService<IPDFReader>();

            return new PDFService(pdfManager, pdfReader);
        }

        private ICrypting CreateAES(IServiceProvider provider)
        {
            var jsonSerialzer = provider.GetRequiredService<IJsonSerializer>();
            return new AES(jsonSerialzer);
        }

        private ICrypting CreateRSA(IServiceProvider provider)
        {
            var jsonSerialzer = provider.GetRequiredService<IJsonSerializer>();
            var xmlSerialzer = provider.GetRequiredService<IXmlSerializer>();
            var progressView = provider.GetRequiredService<IProgressView>();
            return new RSA(jsonSerialzer, xmlSerialzer, progressView);
        }

        private ICryptingRecognition CreateCryptingRecognition(IServiceProvider provider)
        {
            var recognitionValues = new RecognitionValues(AppKey);
            return new CryptingRecognition(recognitionValues);
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
            var cryptingRecognition = provider.GetRequiredService<ICryptingRecognition>();
            var cryptingReader = provider.GetRequiredService<ICryptingReader>();
            var cryptors = provider.GetServices<ICrypting>();
            return new CryptingService(cryptingRecognition, cryptingReader, cryptors);
        }

        public IFilesView CreateFilesView(IServiceProvider provider)
        {
            var fileService = provider.GetRequiredService<IFileService>();
            return new FilesViewViewModel(fileService);
        }

        public FilesSelectorViewViewModel CreateFilesLeftView(IServiceProvider provider)
        {
            var progressItemViewViewModel = provider.GetRequiredService<FilesSelectorListingViewViewModel>();
            var completedItemViewViewModel = provider.GetRequiredService<FilesSelectorListingViewViewModel>();
            return new FilesSelectorViewViewModel(progressItemViewViewModel, completedItemViewViewModel);
        }

        public IImageView CreateImagesView(IServiceProvider provider)
        {
            return new ImageViewerViewModel();
        }

        public IProgressView CreateProgressViewModel(IServiceProvider provider)
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