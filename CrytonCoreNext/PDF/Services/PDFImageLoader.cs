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
        private const int MaximalDimensionSize = 1500;

        public WriteableBitmap LoadImage(PDFFile pdfFile)
        {
            if (pdfFile.PdfStatus == Enums.EPdfStatus.Protected && string.IsNullOrEmpty(pdfFile.Password))
            {
                return default;
            }
            using IDocLib pdfLibrary = DocLib.Instance;
            using var reader = pdfLibrary.GetDocReader(pdfFile.Bytes, pdfFile.Password, new PageDimensions(pdfFile.Dimensions));
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
            using var reader = pdfLibrary.GetDocReader(pdfFile.Bytes, pdfFile.Password, new PageDimensions(pdfFile.Dimensions));
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
            var bgrBytes = pageReader.GetImage(RenderFlags.OptimizeTextForLcd | RenderFlags.LimitImageCacheSize);
            var width = pageReader.GetPageWidth();
            var height = pageReader.GetPageHeight();

            using var bgraMat = new Mat(height, width, MatType.CV_8UC4, bgrBytes);
            using var alphaMat = new Mat();
            var newSize = new Size(width, height);
            if (width > MaximalDimensionSize && width > height)
            {
                var ratio = (double)MaximalDimensionSize / (double)width;
                var newHeight = height * ratio;
                newSize = new Size(MaximalDimensionSize, newHeight);
            }
            else if (height > MaximalDimensionSize && width < height)
            {
                var ratio = (double)MaximalDimensionSize / (double)height;
                var newWidth = width * ratio;
                newSize = new Size(newWidth, MaximalDimensionSize);
            }
            var nativeSplitted = Cv2.Split(bgraMat);
            Cv2.Merge([nativeSplitted[3]], alphaMat);
            using Mat inversedAlphaMat = new Scalar(255) - alphaMat;
            var resultMat = new Mat();
            Cv2.CvtColor(bgraMat, bgraMat, ColorConversionCodes.BGRA2BGR);
            Cv2.CvtColor(inversedAlphaMat, inversedAlphaMat, ColorConversionCodes.GRAY2BGR);
            Cv2.Add(bgraMat, inversedAlphaMat, resultMat);
            Cv2.Resize(resultMat, resultMat, newSize);
            return resultMat;
        }
    }
}
