using CrytonCoreNext.PDF.Interfaces;
using CrytonCoreNext.PDF.Models;
using Docnet.Core;
using Docnet.Core.Models;
using Docnet.Core.Readers;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace CrytonCoreNext.PDF.Services
{
    public class PDFImageLoader : IPDFImageLoader
    {
        public WriteableBitmap LoadImage(PDFFile pdfFile)
        {
            if (pdfFile.PdfStatus == Enums.EPdfStatus.Protected && string.IsNullOrEmpty(pdfFile.Password))
            {
                return default;
            }
            using IDocLib pdfLibrary = DocLib.Instance;
            var dimensions = pdfFile.Dimensions;
            using var reader = pdfFile.Password.Equals(string.Empty) ?
                pdfLibrary.GetDocReader(pdfFile.Bytes, new PageDimensions(dimensions)) :
                pdfLibrary.GetDocReader(pdfFile.Bytes, pdfFile.Password, new PageDimensions(dimensions));
            using var docReader = reader;
            using var pageReader = docReader.GetPageReader(pdfFile.LastPage);
            using var mat = GetImage(pageReader);
            return mat.ToWriteableBitmap();
        }

        public async IAsyncEnumerable<WriteableBitmap> LoadImages(PDFFile pdfFile)
        {
            if (pdfFile.PdfStatus == Enums.EPdfStatus.Protected && string.IsNullOrEmpty(pdfFile.Password))
            {
                yield return default;
            }
            using IDocLib pdfLibrary = DocLib.Instance;
            var dimensions = pdfFile.Dimensions;
            using var reader = pdfFile.Password.Equals(string.Empty) ?
                pdfLibrary.GetDocReader(pdfFile.Bytes, new PageDimensions(dimensions)) :
                pdfLibrary.GetDocReader(pdfFile.Bytes, pdfFile.Password, new PageDimensions(dimensions));
            using var docReader = reader;
            for (var i = 0; i < pdfFile.NumberOfPages; i++)
            {
                using var pageReader = docReader.GetPageReader(i);
                using var mat = await Task.Run(() => GetImage(pageReader));
                yield return mat.ToWriteableBitmap();
            }
        }

        private static Mat GetImage(IPageReader pageReader)
        {
            var bgrBytes = pageReader.GetImage(RenderFlags.LimitImageCacheSize);
            var width = pageReader.GetPageWidth();
            var height = pageReader.GetPageHeight();

            using var bgraMat = new Mat(height, width, MatType.CV_8UC4, bgrBytes);
            using var matOut = new Mat();

            var nativeSplitted = Cv2.Split(bgraMat);
            Cv2.Merge([nativeSplitted[3]], matOut);
            using Mat inversed = new Scalar(255) - matOut;
            var add = new Mat();
            Cv2.CvtColor(bgraMat, bgraMat, ColorConversionCodes.BGRA2BGR);
            Cv2.CvtColor(inversed, inversed, ColorConversionCodes.GRAY2BGR);
            Cv2.Add(bgraMat, inversed, add);
            return add;
        }
    }
}
