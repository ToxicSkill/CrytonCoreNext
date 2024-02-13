using CrytonCoreNext.Extensions;
using CrytonCoreNext.PDF.Enums;
using CrytonCoreNext.PDF.Interfaces;
using CrytonCoreNext.PDF.Models;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Wpf.Ui.Controls;


namespace CrytonCoreNext.PDF.Services
{
    public class PDFReader : IPDFReader
    {
        private Dictionary<string, SymbolIcon> _symbolByPdfInformation;

        private readonly double _dimensions = 1.0d;

        public PDFReader()
        {
            LoadSymbols();
        }

        public PDFFile ReadPdf(CrytonCoreNext.Models.File file)
        {
            try
            {
                var memoryStream = new MemoryStream(file.Bytes);
                using var pdfReader = new PdfReader(memoryStream);
                using var pdfDocument = new PdfDocument(pdfReader);
                var pdfFile = new PDFFile(file, EPdfStatus.Opened);
                LoadMetadata(pdfFile, pdfDocument);
                return pdfFile;
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

        public void OpenProtectedPdf(PDFFile file)
        {
            try
            {
                var memoryStream = new MemoryStream(file.Bytes);
                using var pdfReader = new PdfReader(memoryStream, new ReaderProperties().SetPassword(Encoding.UTF8.GetBytes(file.Password)));
                using var pdfDocument = new PdfDocument(pdfReader);
                LoadMetadata(file, pdfDocument);
                file.SetPdfStatus(EPdfStatus.Opened | EPdfStatus.Protected);
            }
            catch (BadPasswordException)
            {
                file.SetPdfStatus(EPdfStatus.Protected);
            }
            catch (Exception)
            {
                file.SetPdfStatus(EPdfStatus.Damaged);
            }
        }

        public void LoadMetadata(PDFFile file)
        {
            if (!string.IsNullOrEmpty(file.Path))
            {
                var memoryStream = new MemoryStream(file.Bytes);
                using var pdfReader = new PdfReader(memoryStream, new ReaderProperties().SetPassword(Encoding.UTF8.GetBytes(file.Password)));
                using var pdfDocument = new PdfDocument(pdfReader);
                var fileInfo = new FileInfo(file.Path);
                file.NumberOfPages = pdfDocument.GetNumberOfPages();
                ReadPdfInformations(file, fileInfo, pdfDocument);
            }
        }

        private void LoadMetadata(PDFFile file, PdfDocument pdfDocument)
        {
            if (!string.IsNullOrEmpty(file.Path))
            {
                var fileInfo = new FileInfo(file.Path);
                file.NumberOfPages = pdfDocument.GetNumberOfPages();
                ReadPdfInformations(file, fileInfo, pdfDocument);
            }
        }

        private void ReadPdfInformations(PDFFile file, FileInfo fileInfo, PdfDocument pdfDocument)
        {
            var documentInfo = pdfDocument.GetDocumentInfo();
            var documentInfos = new List<(EPdfInfo, string)>()
            {
                (EPdfInfo.Author, documentInfo.GetAuthor()),
                (EPdfInfo.Creator, documentInfo.GetCreator()),
                (EPdfInfo.Keywords, documentInfo.GetKeywords()),
                (EPdfInfo.Producer, documentInfo.GetProducer()),
                (EPdfInfo.Subject, documentInfo.GetSubject()),
                (EPdfInfo.Title, documentInfo.GetTitle()),
                (EPdfInfo.NumberOfPages, pdfDocument.GetNumberOfPages().ToString()),
                (EPdfInfo.NumberOfPdfObjects, pdfDocument.GetNumberOfPdfObjects().ToString()),
                (EPdfInfo.PdfVersion, pdfDocument
                       .GetPdfVersion()
                       .ToPdfName()
                       .GetValue())
            };
            if (fileInfo.Exists)
            {
                var fileInfos = new List<(EPdfInfo, string)>()
                {
                    (EPdfInfo.Name, fileInfo.Name),
                    (EPdfInfo.FullName, fileInfo.FullName),
                    (EPdfInfo.LastWriteTime, fileInfo.LastWriteTime.ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture)),
                    (EPdfInfo.LastWriteTimeUtc, fileInfo.LastWriteTimeUtc.ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture)),
                    (EPdfInfo.Attributes, fileInfo.Attributes.ToString()),
                    (EPdfInfo.CreationTime, fileInfo.CreationTime.ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture)),
                    (EPdfInfo.CreationTimeUtc, fileInfo.CreationTimeUtc.ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture)),
                    (EPdfInfo.Length, fileInfo.Length.ToString())
                };
                documentInfos.AddRange(fileInfos);
            }
            file.Metadata = [];
            foreach (var (type, value) in documentInfos)
            {
                if (string.IsNullOrEmpty(value))
                {
                    continue;
                }
                var nameofInfoType = type.ToString();
                if (_symbolByPdfInformation.ContainsKey(nameofInfoType))
                {
                    var symbol = _symbolByPdfInformation[nameofInfoType];
                    symbol.ToolTip = nameofInfoType.ToPdfInformationString(false);
                    file.Metadata.Add(symbol, value);
                }
            }
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
            _symbolByPdfInformation = new()
            {
                { nameof(EPdfInfo.Author), new SymbolIcon() { Symbol = SymbolRegular.Person20 } },
                { nameof(EPdfInfo.Creator), new SymbolIcon() { Symbol = SymbolRegular.Person20, Filled=true  } },
                { nameof(EPdfInfo.CreationTime), new SymbolIcon() { Symbol = SymbolRegular.CalendarRtl20  } },
                { nameof(EPdfInfo.LastWriteTime), new SymbolIcon() { Symbol = SymbolRegular.CalendarRtl20, Filled=true  } },
                { nameof(EPdfInfo.Producer), new SymbolIcon() { Symbol = SymbolRegular.Production20  } },
                { nameof(EPdfInfo.Title), new SymbolIcon() { Symbol = SymbolRegular.TextCaseTitle20  } },
                { nameof(EPdfInfo.Subject), new SymbolIcon() { Symbol = SymbolRegular.Subtitles20  } },
                { nameof(EPdfInfo.Keywords), new SymbolIcon() { Symbol = SymbolRegular.Key20  } },
                { nameof(EPdfInfo.NumberOfPdfObjects), new SymbolIcon() { Symbol = SymbolRegular.Cube20  } },
                { nameof(EPdfInfo.PdfVersion), new SymbolIcon() { Symbol = SymbolRegular.Diversity20  } }
            };
        }
    }
}
