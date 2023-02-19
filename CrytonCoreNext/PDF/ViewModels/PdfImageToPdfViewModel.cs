using CrytonCoreNext.Abstract;
using CrytonCoreNext.PDF.Interfaces;
using CrytonCoreNext.ViewModels;

namespace CrytonCoreNext.PDF.ViewModels
{
    public class PdfImageToPdfViewModel : ViewModelBase
    {
        private readonly IPDFService _pdfService;

        private readonly FilesSelectorViewViewModel _filesSelectorViewViewModel;

        public PdfImageToPdfViewModel(IPDFService pdfService, FilesSelectorViewViewModel filesSelectorViewViewModel)
        {
            _pdfService = pdfService;
            _filesSelectorViewViewModel = filesSelectorViewViewModel;
            PageName = "Image to pdf";
        }
    }
}
