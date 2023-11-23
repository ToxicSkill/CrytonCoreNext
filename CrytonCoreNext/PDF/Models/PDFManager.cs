using CrytonCoreNext.Enums;
using CrytonCoreNext.Models;
using CrytonCoreNext.PDF.Interfaces;
using CrytonCoreNext.Services;
using Docnet.Core;
using Docnet.Core.Editors; 
using iText.Kernel.Pdf;
using iText.Layout;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenCvSharp.WpfExtensions;
using System;
using System.Collections.Generic;
using System.Drawing;
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
        public WriteableBitmap LoadImage(PDFFile pdfFile)
        {
            if (pdfFile.PdfStatus == Enums.EPdfStatus.Protected && string.IsNullOrEmpty(pdfFile.Password))
            {
                return default;
            } 
            using var image = GetPageImage(pdfFile.LastPage, GetPageSize(pdfFile.Document, pdfFile.LastPage), pdfFile.Document, Properties.Settings.Default.PdfRenderDpi);
            using var bitmap = new Bitmap(image);
            using var mat = new Mat(bitmap.Height, bitmap.Width, MatType.CV_8UC3);
            BitmapConverter.ToMat(bitmap, mat);
            return mat.ToWriteableBitmap();
        } 

        public List<string> GetAvailableEncryptionOptions()
        {
            return
            [
                nameof(EncryptionConstants.ENCRYPTION_AES_256),
                nameof(EncryptionConstants.ENCRYPTION_AES_128),
                nameof(EncryptionConstants.STANDARD_ENCRYPTION_40),
                nameof(EncryptionConstants.STANDARD_ENCRYPTION_128)
            ];
        }

        public List<string> GetAvailableEncryptionAllowOptions()
        {
            return
            [
                nameof(EncryptionConstants.ALLOW_ASSEMBLY),
                nameof(EncryptionConstants.ALLOW_MODIFY_ANNOTATIONS),
                nameof(EncryptionConstants.ALLOW_MODIFY_CONTENTS),
                nameof(EncryptionConstants.ALLOW_COPY),
                nameof(EncryptionConstants.ALLOW_FILL_IN),
                nameof(EncryptionConstants.ALLOW_PRINTING),
                nameof(EncryptionConstants.ALLOW_DEGRADED_PRINTING),
                nameof(EncryptionConstants.ALLOW_SCREENREADERS)
            ];
        }

        public void ProtectFile(PDFFile pdfFile, int permissions, int encryption)
        {
            using var pdfReader = new PdfReader(new MemoryStream(pdfFile.Bytes));
            using var pdfDocument = new PdfDocument(pdfReader);
            using var stream = new MemoryStream();
            var password = Encoding.UTF8.GetBytes(pdfFile.Password);
            try
            {
                PdfEncryptor.Encrypt(new PdfReader(new MemoryStream(pdfFile.Bytes)), stream, new EncryptionProperties().SetStandardEncryption(
                    password, password,
                    permissions,
                    encryption));
            }
            catch (Exception)
            {
            }
            pdfFile.Bytes = stream.ToArray();
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
            var extension = image.Extension.ToLowerInvariant();
            byte[] bytes;
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
            var file = new File($"{image.Name}_Converted", string.Empty, DateTime.Now, "pdf", newId, pdfBbytes);
            return new PDFFile(file, Enums.EPdfStatus.Opened);
        }

        public async Task<PDFFile> MergeAllImagesToPDF(List<ImageFile> images, int newId)
        {
            var pdfFiles = new List<PDFFile>();
            foreach (var imageFile in images)
            {
                pdfFiles.Add(ImageToPdf(imageFile, 0));
            }
            using IDocLib pdfLibrary = DocLib.Instance;
            return await Merge(pdfFiles);
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

        private static Image GetPageImage(int lastPage, System.Drawing.Size size, PdfiumViewer.PdfDocument document, int dpi)
        {
            return document.Render(lastPage, size.Width, size.Height, dpi, dpi, PdfiumViewer.PdfRenderFlags.None);
        }

        private static System.Drawing.Size GetPageSize(PdfiumViewer.PdfDocument document, int lastPage)
        {
            var size = document.PageSizes[lastPage];
            return new System.Drawing.Size((int)(size.Width), (int)(size.Height));
        }
    }
}
