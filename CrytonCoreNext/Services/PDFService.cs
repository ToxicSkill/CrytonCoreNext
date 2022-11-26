using CrytonCoreNext.Interfaces;
using CrytonCoreNext.Models;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace CrytonCoreNext.Services
{
    public class PDFService : IPDFService
    {
        private readonly IPDFManager _pdfManager;

        private readonly IPDFReader _pdfReader;

        public PDFService(IPDFManager pdfManager, IPDFReader pdfReader)
        {
            _pdfManager = pdfManager;
            _pdfReader = pdfReader;
        }

        public List<BitmapImage> GetAllPdfImages(PDFFile pdf)
        {
            return _pdfManager.GetAllPdfImages(pdf);
        }

        public WriteableBitmap GetImage(PDFFile pdf)
        {
            return _pdfManager.GetImage(pdf);
        }

        public WriteableBitmap GetImageFromPdf(PDFFile pdf, int pageNumber)
        {
            return _pdfManager.GetImageFromPdf(pdf, pageNumber);
        }

        public PDFFile? ReadPdf(File file, string password = "")
        {
            return _pdfReader.ReadPdf(file, password);
        }
    }
}
