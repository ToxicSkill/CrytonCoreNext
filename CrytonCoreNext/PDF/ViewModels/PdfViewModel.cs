using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrytonCoreNext.Abstract;
using CrytonCoreNext.Crypting.Models;
using CrytonCoreNext.Interfaces.Files;
using CrytonCoreNext.Models;
using CrytonCoreNext.PDF.Interfaces;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Wpf.Ui.Mvvm.Contracts;
using IDialogService = CrytonCoreNext.Interfaces.IDialogService;

namespace CrytonCoreNext.PDF.ViewModels
{
    public partial class PdfViewModel : InteractiveViewBase
    {

        private readonly IFileService _fileService;

        private readonly IPDFService _pdfService;

        [ObservableProperty]
        public ObservableCollection<File> files;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Files))]
        public File selectedFile;

        public PdfViewModel(IPDFService pdfService,
            IFileService fileService,
            IDialogService dialogService,
            ISnackbarService snackbarService) : base(fileService, dialogService, snackbarService)
        {
            _pdfService = pdfService;
            files = new();
        }

        [RelayCommand]
        private async Task LoadFiles()
        {
            Lock();
            await foreach (var file in base.LoadFiles())
            {
                Files.Add(file);
                SelectedFile = Files.Last();
            }
            if (SelectedFile == null && Files.Any())
            {
                SelectedFile = Files.First();
            }
            Unlock();
        }
    }
}
