using Docnet.Core;
using Docnet.Core.Models;
using ImageMagick;
using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace CrytonCoreNext.Models
{
    public class PDFManager
    {
        public WriteableBitmap GetImageFromPdf(PDFBase pdf, int num = 0)
        {
            MemoryStream memoryStream = new();
            MagickImage imgBackdrop;
            MagickColor backdropColor = MagickColors.White; // replace transparent pixels with this color 
            int pdfPageNum = num; // first page is 0
            var bitmap = new BitmapImage();

            try
            {
                using (IDocLib pdfLibrary = DocLib.Instance)
                {
                    var bytes = pdf.Bytes;
                    var dimensions = pdf.Dimensions;
                    var reader = string.Equals(string.Empty, default) ?
                        pdfLibrary.GetDocReader(bytes, new PageDimensions(dimensions)) :
                        pdfLibrary.GetDocReader(bytes, pdf.Password, new PageDimensions(dimensions));
                    using var docReader = reader;
                    using var pageReader = docReader.GetPageReader(pdfPageNum);
                    var rawBytes = pageReader.GetImage(); // Returns image bytes as B-G-R-A ordered list.
                    //rawBytes = RearrangeBytesToRGBA(rawBytes);
                    var width = pageReader.GetPageWidth();
                    var height = pageReader.GetPageHeight();

                    // specify that we are reading a byte array of colors in R-G-B-A order.
                    PixelReadSettings pixelReadSettings = new(width, height, StorageType.Char, PixelMapping.RGBA);
                    using MagickImage imgPdfOverlay = new(rawBytes, pixelReadSettings);
                    imgBackdrop = new MagickImage(backdropColor, width, height);
                    imgBackdrop.Composite(imgPdfOverlay, CompositeOperator.Over);
                }

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
    }
}
