using CrytonCoreNext.Helpers;
using CrytonCoreNext.PDF.Interfaces;
using Docnet.Core;
using Docnet.Core.Models;
using Docnet.Core.Readers;
using ImageMagick;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace CrytonCoreNext.PDF.Models
{
    public class PDFManager : IPDFManager
    {
        public async IAsyncEnumerable<BitmapImage> LoadAllPDFImages(PDFFile pdfFile)
        {
            using IDocLib pdfLibrary = DocLib.Instance;
            var dimensions = pdfFile.Dimensions;
            var reader = pdfFile.Password.Equals(string.Empty) ?
                pdfLibrary.GetDocReader(pdfFile.Bytes, new PageDimensions(dimensions)) :
                pdfLibrary.GetDocReader(pdfFile.Bytes, pdfFile.Password, new PageDimensions(dimensions));
            using var docReader = reader;
            for (int i = 0; i <= pdfFile.NumberOfPages; i++)
            {
                yield return await Task.Run(() =>
                {
                    using var pageReader = docReader.GetPageReader(i);
                    return GetImage(pageReader);
                });
            }
        }

        public async Task<CrytonCoreNext.Models.File> Merge(List<PDFFile> pdfFiles)
        {
            using IDocLib pdfLibrary = DocLib.Instance;
            var bytes = pdfFiles.Select(x => x.Bytes).ToArray();
            var mergedFileBytes = await Task.Run(() => pdfLibrary.Merge(bytes));
            var templateFile = pdfFiles.First();
            var name = $"{pdfFiles.Count}MergedFiles";
            return new CrytonCoreNext.Models.File(pdfFiles.First(), name, mergedFileBytes, pdfFiles.Count() + 1);
        }

        private static BitmapImage GetImage(IPageReader pageReader)
        {
            var memoryStream = new MemoryStream();
            var bitmap = new BitmapImage();
            var bgrBytes = pageReader.GetImage(RenderFlags.LimitImageCacheSize); // Returns image bytes as B-G-R-A ordered list.
            var rgbaBytes = RearrangeBytesToRGBA(bgrBytes);
            ClearArray(bgrBytes);
            var width = pageReader.GetPageWidth();
            var height = pageReader.GetPageHeight();
            var pixelReadSettings = new PixelReadSettings(width, height, StorageType.Char, PixelMapping.RGBA);
            using var imgOverlay = new MagickImage(rgbaBytes, pixelReadSettings);
            ClearArray(rgbaBytes);
            using var imgBackdrop = new MagickImage(MagickColors.White, width, height);
            imgBackdrop.Composite(imgOverlay, CompositeOperator.Over);
            imgBackdrop.Write(memoryStream, MagickFormat.Bmp);
            imgBackdrop.Dispose();
            imgOverlay.Dispose();

            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            memoryStream.Seek(0, SeekOrigin.Begin);
            bitmap.StreamSource = memoryStream;
            bitmap.EndInit();
            bitmap.Freeze();
            memoryStream.Dispose();
            memoryStream = null;
            return bitmap;
        }

        private static void ClearArray(byte[]? array)
        {
            if (array == null)
            {
                return;
            }
            Array.Clear(array, 0, array.Length);
            array = null;
            GCHelper.Collect();
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
