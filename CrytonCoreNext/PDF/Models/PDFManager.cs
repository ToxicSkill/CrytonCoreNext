using CrytonCoreNext.PDF.Interfaces;
using Docnet.Core;
using Docnet.Core.Models;
using Docnet.Core.Readers;
using MethodTimer;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace CrytonCoreNext.PDF.Models
{
    public class PDFManager : IPDFManager
    {
        //public async IAsyncEnumerable<BitmapImage> LoadAllPDFImages(PDFFile pdfFile)
        //{
        //    using IDocLib pdfLibrary = DocLib.Instance;
        //    var dimensions = pdfFile.Dimensions;
        //    var reader = pdfFile.Password.Equals(string.Empty) ?
        //        pdfLibrary.GetDocReader(pdfFile.Bytes, new PageDimensions(dimensions)) :
        //        pdfLibrary.GetDocReader(pdfFile.Bytes, pdfFile.Password, new PageDimensions(dimensions));
        //    using var docReader = reader;
        //    for (int i = 0; i <= pdfFile.NumberOfPages; i++)
        //    {
        //        yield return await Task.Run(() =>
        //        {
        //            using var pageReader = docReader.GetPageReader(i);
        //            return GetImage(pageReader);
        //        });
        //    }
        //}

        [Time]
        public WriteableBitmap LoadImage(PDFFile pdfFile)
        {
            using IDocLib pdfLibrary = DocLib.Instance;
            var dimensions = pdfFile.Dimensions;
            using var reader = pdfFile.Password.Equals(string.Empty) ?
                pdfLibrary.GetDocReader(pdfFile.Bytes, new PageDimensions(dimensions)) :
                pdfLibrary.GetDocReader(pdfFile.Bytes, pdfFile.Password, new PageDimensions(dimensions));
            using var docReader = reader;
            using var pageReader = docReader.GetPageReader(pdfFile.LastPage);
            return GetImage(pageReader);
        }

        public async Task<CrytonCoreNext.Models.File> Merge(List<PDFFile> pdfFiles)
        {
            using IDocLib pdfLibrary = DocLib.Instance;
            var bytes = pdfFiles.Select(x => x.Bytes).ToArray();
            var mergedFileBytes = await Task.Run(() => pdfLibrary.Merge(bytes));
            var templateFile = pdfFiles.First();
            return new CrytonCoreNext.Models.File(pdfFiles.First(), PrepareFileNameForMerge(pdfFiles), mergedFileBytes, pdfFiles.Count() + 1);
        }

        public async Task<CrytonCoreNext.Models.File> Split(PDFFile pdfFile, int fromPage, int toPage, int newId)
        {
            using IDocLib pdfLibrary = DocLib.Instance;
            var splittedFileBytes = await Task.Run(() => pdfLibrary.Split(pdfFile.Bytes, fromPage, toPage));
            return new CrytonCoreNext.Models.File(pdfFile, PrepareFileNameForSplit(pdfFile, fromPage, toPage), splittedFileBytes, newId);
        }

        private static string PrepareFileNameForMerge(List<PDFFile> pdfFiles)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(pdfFiles.Count);
            stringBuilder.Append("MergedFiles");
            foreach (var file in pdfFiles)
            {
                stringBuilder.Append(file.Name[0]);
            }

            return stringBuilder.ToString();
        }

        private static string PrepareFileNameForSplit(PDFFile pdfFile, int fromPage, int toPage)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("From");
            stringBuilder.Append(fromPage);
            stringBuilder.Append("To");
            stringBuilder.Append(toPage);
            stringBuilder.Append("SplittedFile");
            stringBuilder.Append(pdfFile.Name);
            return stringBuilder.ToString();
        }

        private static WriteableBitmap GetImage(IPageReader pageReader)
        {
            var bgrBytes = pageReader.GetImage(RenderFlags.LimitImageCacheSize);
            var width = pageReader.GetPageWidth();
            var height = pageReader.GetPageHeight();

            using var bgraMat = new Mat(height, width, MatType.CV_8UC4, bgrBytes);
            using var matOut = new Mat();

            var nativeSplitted = Cv2.Split(bgraMat);
            Cv2.Merge(new[] { nativeSplitted[3] }, matOut);
            using Mat inversed = new Scalar(255) - matOut;

            using var add = new Mat();
            Cv2.CvtColor(bgraMat, bgraMat, ColorConversionCodes.BGRA2BGR);
            Cv2.CvtColor(inversed, inversed, ColorConversionCodes.GRAY2BGR);
            Cv2.Add(bgraMat, inversed, add);
            return add.ToWriteableBitmap();
        }
    }
}
