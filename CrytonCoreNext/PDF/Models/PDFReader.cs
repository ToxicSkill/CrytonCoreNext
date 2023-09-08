using CrytonCoreNext.Models;
using CrytonCoreNext.PDF.Enums;
using CrytonCoreNext.PDF.Interfaces;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using iText.Kernel.Pdf;
using System.Text;
using Wpf.Ui.Controls;
using CrytonCoreNext.Extensions;
using PdfiumViewer;
using System.Globalization;

namespace CrytonCoreNext.PDF.Models
{
    public class PDFReader : IPDFReader
    {
        private Dictionary<string, SymbolIcon> _symbolByPdfInformation;

        private readonly double _dimensions = 1.0d;

        public PDFFile ReadPdf(File file, string password = "")
        {
            var status = EPdfStatus.Opened;

            if (file.Path.Equals(string.Empty))
            {
                return CreateNewPdfFile(file, null, status);
            }
            PdfiumViewer.PdfDocument? document = null;
            try
            {
                document = password.Equals(string.Empty, default) ?
                    PdfiumViewer.PdfDocument.Load(file.Path) :
                    PdfiumViewer.PdfDocument.Load(file.Path, password);
            }
            catch (PdfiumViewer.PdfException)
            {
                status = EPdfStatus.Protected;
            }
            catch (Exception)
            {
                status = EPdfStatus.Damaged;
            }

            var pdfFile = CreateNewPdfFile(file, document, status);
            if (pdfFile.PdfStatus == EPdfStatus.Opened)
            {
                GetMetaInfo(ref pdfFile);
            }
            else if (pdfFile.PdfStatus == EPdfStatus.Protected)
            {
                pdfFile.HasPassword = true;
            }
            return pdfFile;
        }

        public void UpdatePdfFileInformations(ref PDFFile file)
        { 
            try
            {
                file.Document = file.Password.Equals(string.Empty, default) ?
                    PdfiumViewer.PdfDocument.Load(file.Path) :
                    PdfiumViewer.PdfDocument.Load(file.Path, file.Password);
            }
            catch (PdfiumViewer.PdfException)
            {
                file.PdfStatus = EPdfStatus.Protected;
                return;
            }
            catch (Exception)
            {
                file.PdfStatus = EPdfStatus.Damaged;
                return;
            }
             
            file.NumberOfPages = file.Document.PageCount;
            file.IsOpened = true;
            GetMetaInfo(ref file);
        }

        private void GetMetaInfo(ref PDFFile pdfFile)
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
                pdfFile.Metadata = new ()
                { { new SymbolIcon() { Symbol = Wpf.Ui.Common.SymbolRegular.ErrorCircle20 }, ex.Message} };
            }
        }

        private void ParseNativeMetainfo(PDFFile pdfFile)
        {
            var info = pdfFile.Document.GetInformation();
            foreach (var key in _symbolByPdfInformation.Keys)
            {
                string? value;
                if (key.ToLowerInvariant().Contains("date"))
                {
                    var date = (DateTime?)info.GetPropertyValue(key);
                    var dateTime = date ?? default;
                    if (dateTime != default)
                    {
                        value = dateTime.ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        value = string.Empty;
                    }
                }
                else
                {
                    value = (string)info.GetPropertyValue(key);
                }
                if (!string.IsNullOrEmpty(value))
                {
                    var symbol = _symbolByPdfInformation[key];
                    symbol.ToolTip = key.ToPdfInformationString(false);
                    pdfFile.Metadata.Add(symbol, value);
                }
            }
        }

        private PDFFile CreateNewPdfFile(File file, PdfiumViewer.PdfDocument? pdfDocument, EPdfStatus status)
        {
            return pdfDocument == null
                ? new PDFFile(file, status)
                : new PDFFile(
                file: file,
                document: pdfDocument, 
                pdfStatus: status,
                password: string.Empty,
                dimensions: _dimensions,
                numberOfPages: pdfDocument.PageCount);
        }

        private void LoadSymbols()
        {
            var fakeInfo = new PdfInformation();
            _symbolByPdfInformation = new ()
            {
                { nameof(fakeInfo.Author), new SymbolIcon() { Symbol = Wpf.Ui.Common.SymbolRegular.Person20 } },
                { nameof(fakeInfo.Creator), new SymbolIcon() { Symbol = Wpf.Ui.Common.SymbolRegular.Person20, Filled=true  } },
                { nameof(fakeInfo.CreationDate), new SymbolIcon() { Symbol = Wpf.Ui.Common.SymbolRegular.CalendarRtl20  } },
                { nameof(fakeInfo.ModificationDate), new SymbolIcon() { Symbol = Wpf.Ui.Common.SymbolRegular.CalendarRtl20, Filled=true  } },
                { nameof(fakeInfo.Producer), new SymbolIcon() { Symbol = Wpf.Ui.Common.SymbolRegular.Production20  } },
                { nameof(fakeInfo.Title), new SymbolIcon() { Symbol = Wpf.Ui.Common.SymbolRegular.TextCaseTitle20  } },
                { nameof(fakeInfo.Subject), new SymbolIcon() { Symbol = Wpf.Ui.Common.SymbolRegular.Subtitles20  } },
                { nameof(fakeInfo.Keywords), new SymbolIcon() { Symbol = Wpf.Ui.Common.SymbolRegular.Key20  } }
            };
        }
    }
}
