using CrytonCoreNext.Abstract;
using CrytonCoreNext.PDF.Interfaces;

namespace CrytonCoreNext.PDF.ViewModels
{
    public class PdfSplitViewModel : ViewModelBase
    {
        private readonly IPDFService _pdfService;

        public PdfSplitViewModel(IPDFService pdfService)
        {
            _pdfService = pdfService;
            PageName = "Split pdf";
        }
    }
}
