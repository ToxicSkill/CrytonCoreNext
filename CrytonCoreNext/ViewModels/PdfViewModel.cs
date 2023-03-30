using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrytonCoreNext.Abstract;
using CrytonCoreNext.Interfaces.Files;
using CrytonCoreNext.Models;
using CrytonCoreNext.PDF.Interfaces;
using CrytonCoreNext.PDF.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Wpf.Ui.Mvvm.Contracts;
using IDialogService = CrytonCoreNext.Interfaces.IDialogService;

namespace CrytonCoreNext.ViewModels
{
    public partial class PdfViewModel : InteractiveViewBase
    {
        private readonly IFileService _fileService;

        private readonly ISnackbarService _snackbarService;

        private readonly IPDFService _pdfService;

        [ObservableProperty]
        public ObservableCollection<PDFFile> files;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Files))]
        public PDFFile selectedFile;

        public PdfViewModel(IPDFService pdfService,
            IFileService fileService,
            IDialogService dialogService,
            ISnackbarService snackbarService) : base(fileService, dialogService, snackbarService)
        {
            _snackbarService = snackbarService;
            _pdfService = pdfService;
            files = new();
        }

        partial void OnSelectedFileChanged(PDFFile value)
        {
            value.PageImage = new WriteableBitmap(_pdfService.LoadImage(value));
        }

        [RelayCommand]
        private new async Task LoadFiles()
        {
            Lock();
            var protectedFile = new List<File>();
            await foreach (var file in base.LoadFiles(Static.Extensions.DialogFilters.Pdf))
            {
                var pdfFile = _pdfService.ReadPdf(file);
                if (pdfFile != null)
                {
                    Files.Add(pdfFile);
                }
                else
                {
                    protectedFile.Add(file);
                }
                SelectedFile = Files.Last();
            }
            if (SelectedFile == null && Files.Any())
            {
                SelectedFile = Files.First();
            }
            if (protectedFile.Any())
            {
                _snackbarService.Show("Warning",
                    (protectedFile.Count > 1 ?
                    $"{protectedFile.Count} of {protectedFile.Count + files.Count} loaded files" :
                    "One file") + " require password",
                    Wpf.Ui.Common.SymbolRegular.Warning20,
                    Wpf.Ui.Common.ControlAppearance.Caution);
            }
            Unlock();
        }
    }
}
