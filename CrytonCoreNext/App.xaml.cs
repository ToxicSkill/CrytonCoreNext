using CrytonCoreNext.AI;
using CrytonCoreNext.AI.Interfaces;
using CrytonCoreNext.Crypting.Cryptors;
using CrytonCoreNext.Crypting.Interfaces;
using CrytonCoreNext.Crypting.Models;
using CrytonCoreNext.Crypting.Services;
using CrytonCoreNext.Crypting.ViewModels;
using CrytonCoreNext.Crypting.Views;
using CrytonCoreNext.Drawers;
using CrytonCoreNext.Interfaces;
using CrytonCoreNext.Interfaces.Files;
using CrytonCoreNext.Interfaces.Serializers;
using CrytonCoreNext.Models;
using CrytonCoreNext.PDF.Interfaces;
using CrytonCoreNext.PDF.Services;
using CrytonCoreNext.Providers;
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
using System.Threading.Tasks;
using System.Windows;
using Wpf.Ui.Mvvm.Contracts;
using Wpf.Ui.Mvvm.Services;
using DialogService = CrytonCoreNext.Services.DialogService;

namespace CrytonCoreNext
{
    public partial class App
    {
        public static readonly Guid AppKey = new("adae2137-dead-beef-6666-3eb841121af8");

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
            services.AddSingleton<DialogService>();

            services.AddTransient<IProgressService, ProgressService>();
            services.AddSingleton<IFilesLoader, FilesLoader>();
            services.AddSingleton<IFilesSaver, FilesSaver>();
            services.AddSingleton<IFilesManager, FilesManager>();
            services.AddSingleton(CreateFileService);

            services.AddSingleton<IJsonSerializer, JsonSerializer>();
            services.AddSingleton<IXmlSerializer, XmlSerializer>();

            // crypting
            services.AddScoped<ICryptingRecognition, CryptingRecognition>();
            services.AddScoped<ICryptingReader, CryptingReader>();
            services.AddScoped<ICryptingReader, CryptingReader>();
            services.AddScoped<IPasswordProvider, PasswordProvider>();
            services.AddScoped<ICrypting, AES>();
            services.AddScoped<ICrypting, RSA>();
            services.AddScoped<ICrypting, _3DES>();
            services.AddScoped(CreateAESViewModel);
            services.AddScoped(CreateRSAViewModel);
            services.AddSingleton(CreateAESView);
            services.AddSingleton(CreateRSAView);
            services.AddSingleton(CreateCryptingService);
            services.AddSingleton<CryptingView>();
            services.AddSingleton(CreateCryptingViewModel);

            // pdf
            services.AddSingleton<IPDFManager, PDFManager>();
            services.AddSingleton<IPDFReader, PDFReader>();
            services.AddSingleton<IPDFImageLoader, PDFImageLoader>();
            services.AddScoped(CreateFileService);
            services.AddScoped<PdfView>();
            services.AddScoped<PdfViewModel>();

            //AI viewer

            services.AddSingleton<ImageDrawer>();
            services.AddSingleton<IYoloModelService, YoloModelService>();
            services.AddScoped<AIViewerView>();
            services.AddScoped<AIViewerViewModel>();

            services.AddSingleton<ICameraService, CameraService>();
            services.AddSingleton<CameraViewModel>();
            services.AddScoped<CameraView>();

            services.AddScoped<Dashboard>();
            services.AddScoped<DashboardViewModel>();

            services.AddScoped<SettingsView>();
            services.AddScoped<SettingsViewModel>();

            services.AddScoped<INavigationWindow, MainWindow>();
            services.AddScoped<MainViewModel>();
        }).Build();

        private readonly List<ResourceDictionary> LanguagesDictionaries;

        public App()
        {
            LanguagesDictionaries = [
                new() { Source = new Uri("..\\Dictionaries\\EnglishDictionary.xaml", UriKind.Relative) }
            ];
        }

        public static T GetService<T>()
        where T : class
        {
            return _host.Services.GetService(typeof(T)) as T;
        }

        private async void OnStartup(object sender, StartupEventArgs e)
        {
            await _host.StartAsync();
            await Task.Delay(200);
            ConfigureServices();
        }

        private static void ConfigureServices()
        {
            var mw = _host.Services.GetService<MainViewModel>();
            var sw = _host.Services.GetService<SettingsViewModel>();
            if (sw != null && mw != null)
            {
                sw.ThemeStyleChanged += mw.InvokeThemeChanged;
            }
            var pv = _host.Services.GetService<PdfView>();
            var cv = _host.Services.GetService<CryptingView>();
            var av = _host.Services.GetService<AIViewerView>();
            var camerav = _host.Services.GetService<CameraView>();

            sw?.OnStartup();
        }

        private void OnLoaded(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            InitializeDictionary();
        }

        private async void OnExit(object sender, ExitEventArgs e)
        {
            await _host.StopAsync();
            CrytonCoreNext.Properties.Settings.Default.Save();
            _host.Dispose();
        }

        private static CryptingViewModel CreateCryptingViewModel(IServiceProvider provider)
        {
            var fileService = provider.GetRequiredService<IFileService>();
            var dialogService = provider.GetRequiredService<DialogService>();
            var cryptingService = provider.GetRequiredService<ICryptingService>();
            var snackbar = provider.GetRequiredService<ISnackbarService>();
            var cryptors = provider.GetServices<ICryptingView<CryptingMethodViewModel>>();
            var passwordProvider = provider.GetRequiredService<IPasswordProvider>();

            return new(fileService, cryptingService, snackbar, cryptors.ToList(), passwordProvider, dialogService);
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

        private static IFileService CreateFileService(IServiceProvider provider)
        {
            var fileLoader = provider.GetRequiredService<IFilesLoader>();
            var fileSaver = provider.GetRequiredService<IFilesSaver>();
            var fileManager = provider.GetRequiredService<IFilesManager>();
            return new FileService(fileSaver, fileLoader, fileManager);
        }

        private void InitializeDictionary()
        {
            //this.Resources.MergedDictionaries.Clear();
            this.Resources.MergedDictionaries.Add(LanguagesDictionaries[0]);
        }
    }
}