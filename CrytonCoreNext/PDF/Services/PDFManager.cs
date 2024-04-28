using CrytonCoreNext.Enums;
using CrytonCoreNext.Models;
using CrytonCoreNext.PDF.Interfaces;
using CrytonCoreNext.PDF.Models;
using CrytonCoreNext.Services;
using Docnet.Core;
using Docnet.Core.Editors;
using iText.Kernel.Pdf;
using iText.Layout;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using File = CrytonCoreNext.Models.File;

namespace CrytonCoreNext.PDF.Services
{
    public class PDFManager : IPDFManager
    {

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

        public bool ProtectFile(PDFFile pdfFile, int permissions, int encryption)
        {
            try
            {
                using var pdfReader = new PdfReader(new MemoryStream(pdfFile.Bytes));
                using var pdfDocument = new PdfDocument(pdfReader);
                using var stream = new MemoryStream();
                var password = Encoding.UTF8.GetBytes(pdfFile.Password);
                PdfEncryptor.Encrypt(new PdfReader(new MemoryStream(pdfFile.Bytes)), stream, new EncryptionProperties().SetStandardEncryption(
                    password, password,
                    permissions,
                    encryption));
                pdfFile.Bytes = stream.ToArray();
                pdfFile.SetPdfStatus(Enums.EPdfStatus.Protected | Enums.EPdfStatus.Opened);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<PDFFile> Merge(List<PDFFile> pdfFiles)
        {
            using IDocLib pdfLibrary = DocLib.Instance;
            var bytes = pdfFiles.Select(x => x.Bytes).ToArray();
            var mergedFileBytes = await Task.Run(() => pdfLibrary.Merge(bytes));
            var templateFile = pdfFiles.First();
            var file = new File(pdfFiles.First(), PrepareFileNameForMerge(pdfFiles), mergedFileBytes, pdfFiles.Count() + 1);
            return new PDFFile(file, Enums.EPdfStatus.Opened, string.Empty, 1.0, pdfFiles.Sum(x => x.NumberOfPages));
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
            using var exportMat = image.Bitmap.ToMat();
            if (image.SelectedPdfFormat != Enums.EPDFFormat.Original)
            {
                Cv2.Resize(exportMat, exportMat, image.ExportSize, interpolation: InterpolationFlags.Lanczos4);
            }
            byte[] bytes;
            if (extension == Enum.GetName(typeof(EImageExtensions), EImageExtensions.png) ||
                extension == Enum.GetName(typeof(EImageExtensions), EImageExtensions.gif) ||
                extension == Enum.GetName(typeof(EImageExtensions), EImageExtensions.bmp))
            {
                bytes = converter.ConvertImageToJpeg(exportMat.ToBytes());
            }
            else
            {
                bytes = exportMat.ToBytes(ext: ".jpeg");
            }
            var jpegImage = new JpegImage
            {
                Bytes = bytes,
                Width = exportMat.Width,
                Height = exportMat.Height
            };
            using IDocLib pdfLibrary = DocLib.Instance;
            var pdfBbytes = pdfLibrary.JpegToPdf(new[] { jpegImage });
            var file = new File($"{image.Name}_Converted", newId, pdfBbytes);
            return new PDFFile(file, Enums.EPdfStatus.Opened);
        }

        public async Task<PDFFile> MergeAllImagesToPDF(List<ImageFile> images, int newId)
        {
            var pdfFiles = new List<PDFFile>();
            var index = 0;
            foreach (var imageFile in images)
            {
                var pdfFile = ImageToPdf(imageFile, index);
                pdfFile.NumberOfPages = 1;
                if (pdfFile != null)
                {
                    pdfFiles.Add(pdfFile);
                }
                index++;
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
    }
}
