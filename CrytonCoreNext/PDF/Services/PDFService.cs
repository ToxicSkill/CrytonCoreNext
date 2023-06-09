using CrytonCoreNext.Models;
using CrytonCoreNext.PDF.Interfaces;
using CrytonCoreNext.PDF.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace CrytonCoreNext.PDF.Services
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

        //public async IAsyncEnumerable<BitmapImage> LoadAllPDFImages(PDFFile pdfFile)
        //{
        //    await foreach (var image in _pdfManager.LoadAllPDFImages(pdfFile))
        //    {
        //        yield return image;
        //    }
        //}

        public WriteableBitmap LoadImage(PDFFile pdfFile)
        {
            return _pdfManager.LoadImage(pdfFile);
        }

        public async Task<PDFFile> Merge(List<PDFFile> pdfFiles)
        {
            return await _pdfManager.Merge(pdfFiles);
        }

        public PDFFile ReadPdf(File file, string password = "")
        {
            return _pdfReader.ReadPdf(file, password);
        }

        public async Task<PDFFile> Split(PDFFile pdfFile, int fromPage, int toPage, int newId)
        {
            return await _pdfManager.Split(pdfFile, fromPage, toPage, newId);
        }

        public PDFFile ImageToPdf(ImageFile image, int newId)
        {
           return _pdfManager.ImageToPdf(image, newId);
        }

        public void UpdatePdfFileInformations(ref PDFFile pdfFile)
        {
            _pdfReader.UpdatePdfFileInformations(ref pdfFile);
        }

        public PDFFile MergeAllImagesToPDF(List<ImageFile> images, int newId)
        {
            return _pdfManager.MergeAllImagesToPDF(images, newId);
        }
    }
}
