using CrytonCoreNext.Enums;
using CrytonCoreNext.Extensions;
using CrytonCoreNext.Models;
using CrytonCoreNext.PDF.Interfaces;
using CrytonCoreNext.Services;
using Docnet.Core;
using Docnet.Core.Editors;
using Docnet.Core.Models;
using Docnet.Core.Readers;
using iText.IO.Image;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using MethodTimer;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using File = CrytonCoreNext.Models.File;

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
            return GetImage(pageReader);
        }

        public async Task<PDFFile> Merge(List<PDFFile> pdfFiles)
        {
            using IDocLib pdfLibrary = DocLib.Instance;
            var bytes = pdfFiles.Select(x => x.Bytes).ToArray();
            var mergedFileBytes = await Task.Run(() => pdfLibrary.Merge(bytes));
            var templateFile = pdfFiles.First();
            var file = new File(pdfFiles.First(), PrepareFileNameForMerge(pdfFiles), mergedFileBytes, pdfFiles.Count() + 1);
            return new PDFFile(file, Enums.EPdfStatus.Opened);
        }

        public async Task<PDFFile> Split(PDFFile pdfFile, int fromPage, int toPage, int newId)
        {
            using IDocLib pdfLibrary = DocLib.Instance;
            var splittedFileBytes = await Task.Run(() => pdfLibrary.Split(pdfFile.Bytes, fromPage, toPage));
            var file = new File(pdfFile, PrepareFileNameForSplit(pdfFile, fromPage, toPage), splittedFileBytes, newId);
            return new PDFFile(file, Enums.EPdfStatus.Opened);
        }

        public PDFFile ImageToPdf(ImageFile image, int newId)
        {
            var converter = new ImageConverterService();
            byte[] bytes = default;
            var extension = image.Extension.ToLowerInvariant();
            if (extension == Enum.GetName(typeof(EImageExtensions), EImageExtensions.png) || 
                extension == Enum.GetName(typeof(EImageExtensions), EImageExtensions.gif) ||
                extension == Enum.GetName(typeof(EImageExtensions), EImageExtensions.bmp))
            {
                bytes = converter.ConvertImageToJpeg(image);
            }
            else
            {
                bytes = image.Bytes;
            }
            var jpegImage = new JpegImage
            {
                Bytes = bytes,
                Width = (int)image.Width,
                Height = (int)image.Height
            };

            using IDocLib pdfLibrary = DocLib.Instance;
            var pdfBbytes = pdfLibrary.JpegToPdf(new[] { jpegImage });
            var file = new File($"{image.Name}_Converted", string.Empty, bytes.GetSizeString(), DateTime.Now, "pdf", newId, pdfBbytes);
            return new PDFFile(file, Enums.EPdfStatus.Opened);
        }

        public PDFFile MergeAllImagesToPDF(List<ImageFile> images, int newId)
        {
            using var memStream = new MemoryStream();
            using var writer = new PdfWriter(memStream);
            using var document = new PdfDocument(writer);
            using var doc = new Document(document, PageSize.A4);
            var size = document.GetDefaultPageSize();
            foreach (var imageFile in images)
            {
                ImageData imageData = ImageDataFactory.Create(imageFile.Bytes);
                Image image = new Image(imageData);
                doc.Add(image);
                doc.Add(new AreaBreak(iText.Layout.Properties.AreaBreakType.NEXT_PAGE));
            }
            document.Close();
            var bytes = memStream.ToArray();
            var file = new File("MergedImages", string.Empty, bytes.GetSizeString(), DateTime.Now, "pdf", newId, bytes);
            return new PDFFile(file, Enums.EPdfStatus.Opened);
        }

        private static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
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
