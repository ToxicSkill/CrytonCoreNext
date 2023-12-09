using CrytonCoreNext.PDF.Enums;
using CrytonCoreNext.PDF.Interfaces;
using CrytonCoreNext.PDF.Models;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Wpf.Ui.Controls;


namespace CrytonCoreNext.PDF.Services
{
    public class PDFReader : IPDFReader
    {
        private Dictionary<string, SymbolIcon> _symbolByPdfInformation;

        private readonly double _dimensions = 1.0d;

        public PDFFile ReadPdf(CrytonCoreNext.Models.File file)
        {
            try
            {
                var memoryStream = new MemoryStream(file.Bytes);
                using var pdfReader = new PdfReader(memoryStream);
                using var pdfDocument = new PdfDocument(pdfReader);
                if (!string.IsNullOrEmpty(file.Path))
                {
                    var fileInfo = new FileInfo(file.Path);
                    ReadPdfInformations(fileInfo, pdfDocument);
                }
                return new(file, EPdfStatus.Opened);
            }
            catch (BadPasswordException)
            {
                return new(file, EPdfStatus.Protected);
            }
            catch (Exception)
            {
                return new(file, EPdfStatus.Damaged);
            }
        }

        public void OpenProtectedPdf(ref PDFFile file)
        {
            try
            {
                var memoryStream = new MemoryStream(file.Bytes);
                using var pdfReader = new PdfReader(memoryStream, new ReaderProperties().SetPassword(Encoding.UTF8.GetBytes(file.Password)));
                using var pdfDocument = new PdfDocument(pdfReader);
                if (!string.IsNullOrEmpty(file.Path))
                {
                    var fileInfo = new FileInfo(file.Path);
                    ReadPdfInformations(fileInfo, pdfDocument);
                }
                file.PdfStatus = EPdfStatus.Opened;
            }
            catch (BadPasswordException)
            {
                file.PdfStatus = EPdfStatus.Protected;
            }
            catch (Exception)
            {
                file.PdfStatus = EPdfStatus.Damaged;
            }
        }

        private static void ReadPdfInformations(FileInfo fileInfo, PdfDocument pdfDocument)
        {
            var documentInfo = pdfDocument.GetDocumentInfo();

            var s = JsonUtil.SerializeToString(new
            {
                fileInfo.Name,
                fileInfo.FullName,
                fileInfo.LastWriteTime,
                fileInfo.LastWriteTimeUtc,
                Attributes = fileInfo.Attributes.ToString(),
                fileInfo.CreationTime,
                fileInfo.CreationTimeUtc,
                fileInfo.Length,
                Author = documentInfo.GetAuthor(),
                Creator = documentInfo.GetCreator(),
                Keywords = documentInfo.GetKeywords(),
                Producer = documentInfo.GetProducer(),
                Subject = documentInfo.GetSubject(),
                Title = documentInfo.GetTitle(),
                NumberOfPages = pdfDocument.GetNumberOfPages(),
                NumberOfPdfObjects = pdfDocument.GetNumberOfPdfObjects(),
                PdfVersion = pdfDocument
                       .GetPdfVersion()
                       .ToPdfName()
                       .GetValue()
            });
        }

        private void GetMetaInfo(PDFFile pdfFile)
        {
            if (!pdfFile.LoadMetadata)
            {
                return;
            }
            LoadSymbols();
            try
            {
                ParseNativeMetainfo(pdfFile);
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached) Debugger.Break();
                pdfFile.Metadata = new()
                { { new SymbolIcon() { Symbol = Wpf.Ui.Common.SymbolRegular.ErrorCircle20 }, ex.Message} };
            }
        }
        private void ParseNativeMetainfo(PDFFile pdfFile)
        {
            //pdfFile.Metadata = [];
            //var info = pdfFile.Informations;
            //foreach (var key in _symbolByPdfInformation.Keys)
            //{
            //    string? value;
            //    if (key.ToLowerInvariant().Contains("date"))
            //    {
            //        var date = (DateTime?)info.GetPropertyValue(key);
            //        var dateTime = date ?? default;
            //        if (dateTime != default)
            //        {
            //            value = dateTime.ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);
            //        }
            //        else
            //        {
            //            value = string.Empty;
            //        }
            //    }
            //    else
            //    {
            //        value = (string)info.GetPropertyValue(key);
            //    }
            //    if (!string.IsNullOrEmpty(value))
            //    {
            //        var symbol = _symbolByPdfInformation[key];
            //        symbol.ToolTip = key.ToPdfInformationString(false);
            //        pdfFile.Metadata.Add(symbol, value);
            //    }
            //}
        }

        private PDFFile CreateNewPdfFile(CrytonCoreNext.Models.File file, EPdfStatus status, int numberOfPages = 0)
        {
            return new PDFFile(
                file: file,
                pdfStatus: status,
                password: string.Empty,
                dimensions: _dimensions,
                numberOfPages: numberOfPages);
        }

        private void LoadSymbols()
        {
            //var fakeInfo = new PdfInformation();
            //_symbolByPdfInformation = new()
            //{
            //    { nameof(fakeInfo.Author), new SymbolIcon() { Symbol = Wpf.Ui.Common.SymbolRegular.Person20 } },
            //    { nameof(fakeInfo.Creator), new SymbolIcon() { Symbol = Wpf.Ui.Common.SymbolRegular.Person20, Filled=true  } },
            //    { nameof(fakeInfo.CreationDate), new SymbolIcon() { Symbol = Wpf.Ui.Common.SymbolRegular.CalendarRtl20  } },
            //    { nameof(fakeInfo.ModificationDate), new SymbolIcon() { Symbol = Wpf.Ui.Common.SymbolRegular.CalendarRtl20, Filled=true  } },
            //    { nameof(fakeInfo.Producer), new SymbolIcon() { Symbol = Wpf.Ui.Common.SymbolRegular.Production20  } },
            //    { nameof(fakeInfo.Title), new SymbolIcon() { Symbol = Wpf.Ui.Common.SymbolRegular.TextCaseTitle20  } },
            //    { nameof(fakeInfo.Subject), new SymbolIcon() { Symbol = Wpf.Ui.Common.SymbolRegular.Subtitles20  } },
            //    { nameof(fakeInfo.Keywords), new SymbolIcon() { Symbol = Wpf.Ui.Common.SymbolRegular.Key20  } }
            //};
        }
    }
}
