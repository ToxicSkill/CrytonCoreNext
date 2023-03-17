using CrytonCoreNext.Abstract;
using CrytonCoreNext.Interfaces.Files;
using CrytonCoreNext.PDF.Interfaces;
using Wpf.Ui.Mvvm.Contracts;
using IDialogService = CrytonCoreNext.Interfaces.IDialogService;

namespace CrytonCoreNext.PDF.ViewModels
{
    public class PdfViewModel : InteractiveViewBase
    {
        private readonly IPDFService _pdfService;

        public PdfViewModel(IPDFService pdfService,
            IFileService fileService,
            IDialogService dialogService,
            ISnackbarService snackbarService) : base(fileService, dialogService, snackbarService)
        {
            _pdfService = pdfService;
        }
    }
}
