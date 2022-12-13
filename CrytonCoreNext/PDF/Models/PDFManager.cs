using CrytonCoreNext.PDF.Interfaces;
using Docnet.Core;
using Docnet.Core.Models;
using Docnet.Core.Readers;
using ImageMagick;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace CrytonCoreNext.PDF.Models
{
    public class PDFManager : IPDFManager
    {
        public WriteableBitmap GetImageFromPdf(PDFFile pdf, int num = 0)
        {
            MemoryStream memoryStream = new();
            MagickImage imgBackdrop;
            MagickColor backdropColor = MagickColors.White; // replace transparent pixels with this color 
            int pdfPageNum = num; // first page is 0
            var bitmap = new BitmapImage();

            try
            {
                using var pageReader = pdf.Reader.GetPageReader(pdfPageNum);
                var rawBytes = pageReader.GetImage(); // Returns image bytes as B-G-R-A ordered list.
                rawBytes = RearrangeBytesToRGBA(rawBytes);
                var width = pageReader.GetPageWidth();
                var height = pageReader.GetPageHeight();

                // specify that we are reading a byte array of colors in R-G-B-A order.
                PixelReadSettings pixelReadSettings = new(width, height, StorageType.Char, PixelMapping.RGBA);
                using MagickImage imgPdfOverlay = new(rawBytes, pixelReadSettings);
                imgBackdrop = new MagickImage(backdropColor, width, height);
                imgBackdrop.Composite(imgPdfOverlay, CompositeOperator.Over);


                imgBackdrop.Write(memoryStream, MagickFormat.Bmp);
                imgBackdrop.Dispose();

                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                memoryStream.Seek(0, SeekOrigin.Begin);
                bitmap.StreamSource = memoryStream;
                bitmap.EndInit();
            }
            catch (Exception)
            {
                throw;
            }

            return new(bitmap);
        }

        private BitmapImage GetImage(IPageReader pageReader)
        {
            MemoryStream memoryStream = new();
            MagickImage imgBackdrop;
            MagickColor backdropColor = MagickColors.White;
            var bitmap = new BitmapImage();
            var rawBytes = pageReader.GetImage(); // Returns image bytes as B-G-R-A ordered list.
            rawBytes = RearrangeBytesToRGBA(rawBytes);
            var width = pageReader.GetPageWidth();
            var height = pageReader.GetPageHeight();

            // specify that we are reading a byte array of colors in R-G-B-A order.
            PixelReadSettings pixelReadSettings = new(width, height, StorageType.Char, PixelMapping.RGBA);
            using MagickImage imgPdfOverlay = new(rawBytes, pixelReadSettings);
            imgBackdrop = new MagickImage(backdropColor, width, height);
            imgBackdrop.Composite(imgPdfOverlay, CompositeOperator.Over);


            imgBackdrop.Write(memoryStream, MagickFormat.Bmp);
            imgBackdrop.Dispose();

            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            memoryStream.Seek(0, SeekOrigin.Begin);
            bitmap.StreamSource = memoryStream;
            bitmap.EndInit();
            bitmap.Freeze();
            return bitmap;
        }

        private static byte[] RearrangeBytesToRGBA(byte[] BGRABytes)
        {
            var max = BGRABytes.Length;
            var RGBABytes = new byte[max];
            var idx = 0;
            byte r;
            byte g;
            byte b;
            byte a;
            while (idx < max)
            {
                // get colors in original order: B G R A
                b = BGRABytes[idx];
                g = BGRABytes[idx + 1];
                r = BGRABytes[idx + 2];
                a = BGRABytes[idx + 3];

                // re-arrange to be in new order: R G B A
                RGBABytes[idx] = r;
                RGBABytes[idx + 1] = g;
                RGBABytes[idx + 2] = b;
                RGBABytes[idx + 3] = a;

                idx += 4;
            }
            return RGBABytes;
        }

        public async IAsyncEnumerable<BitmapImage> LoadAllPDFImages(PDFFile pdfFile)
        {
            for (int i = 0; i < pdfFile.NumberOfPages; i++)
            {
                yield return await Task.Run(() =>
                {
                    using IDocLib pdfLibrary = DocLib.Instance;
                    var dimensions = pdfFile.Dimensions;
                    var reader = pdfFile.Password.Equals(string.Empty) ?
                        pdfLibrary.GetDocReader(pdfFile.Bytes, new PageDimensions(dimensions)) :
                        pdfLibrary.GetDocReader(pdfFile.Bytes, pdfFile.Password, new PageDimensions(dimensions));

                    using var docReader = reader;
                    using var pageReader = docReader.GetPageReader(i);
                    return GetImage(pageReader);
                });
            }
        }

        public WriteableBitmap GetImage(PDFFile pdf)
        {
            using (IDocLib pdfLibrary = DocLib.Instance)
            {
                var dimensions = pdf.Dimensions;
                var reader = pdf.Password.Equals(string.Empty) ?
                    pdfLibrary.GetDocReader(pdf.Bytes, new PageDimensions(dimensions)) :
                    pdfLibrary.GetDocReader(pdf.Bytes, pdf.Password, new PageDimensions(dimensions));

                using var docReader = reader;
                using var pageReader = docReader.GetPageReader(pdf.LastPage);
                return new(GetImage(pageReader));
            }
        }

        public WriteableBitmap ConvertBitmapToBitmapImage(Bitmap bitmap)
        {
            // Create a BitmapImage and set its source to the Bitmap
            var bitmapImage = new BitmapImage();
            using (var memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream, ImageFormat.Png);
                memoryStream.Position = 0;

                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.EndInit();
            }

            // Create a WritableBitmap and set its source to the BitmapImage
            var writableBitmap = new WriteableBitmap(bitmapImage);

            return writableBitmap;
        }

        public List<BitmapImage> GetAllPdfImages(PDFFile pdf)
        {
            var images = new List<BitmapImage>();
            using (IDocLib pdfLibrary = DocLib.Instance)
            {
                var dimensions = pdf.Dimensions;
                var reader = pdf.Password.Equals(string.Empty) ?
                    pdfLibrary.GetDocReader(pdf.Bytes, new PageDimensions(dimensions)) :
                    pdfLibrary.GetDocReader(pdf.Bytes, pdf.Password, new PageDimensions(dimensions));

                using var docReader = reader;

                for (int i = 0; i < pdf.NumberOfPages; i++)
                {
                    using var pageReader = docReader.GetPageReader(i);
                    images.Add(GetImage(pageReader));
                }
            }

            return images;
        }

        public void ExtractPages(PDFFile pdfFile)
        {
            PdfReader reader = null;
            Document document = null;
            PdfCopy pdfCopyProvider = null;
            PdfImportedPage importedPage = null;
            try
            {
                // Intialize a new PdfReader instance with the contents of the source Pdf file:
                reader = new PdfReader("E:\\Code\\C#\\CrytonCoreNext\\CrytonCoreNext\\Sdevorg22121209161.pdf");

                // Capture the correct size and orientation for the page:
                document = new Document(reader.GetPageSizeWithRotation(1));

                // Initialize an instance of the PdfCopyClass with the source
                // document and an output file stream:
                pdfCopyProvider = new PdfCopy(document,
                    new System.IO.FileStream("E:\\Code\\C#\\CrytonCoreNext\\CrytonCoreNext\\merge.pdf", System.IO.FileMode.Create));

                document.Open();

                // Extract the desired page number:
                for (int i = 1; i < 4; i++)
                {
                    importedPage = pdfCopyProvider.GetImportedPage(reader, i);
                    pdfCopyProvider.AddPage(importedPage);
                }
                document.Close();
                reader.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
