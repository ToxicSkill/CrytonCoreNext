using CrytonCoreNext.Abstract;
using CrytonCoreNext.PDF.Interfaces;

namespace CrytonCoreNext.PDF.ViewModels
{
    public class PdfMergeViewModel : ViewModelBase
    {
        private readonly IPDFService _pdfService;

        public PdfMergeViewModel(IPDFService pdfService)
        {
            _pdfService = pdfService;
            PageName = "Merge pdf";
        }
    }
}
