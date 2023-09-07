using CrytonCoreNext.Models;
using CrytonCoreNext.PDF.Enums;
using CrytonCoreNext.PDF.Interfaces;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using iText.Kernel.Pdf;
using System.Text;
using Wpf.Ui.Controls;
using System.Linq;
using CrytonCoreNext.Extensions;
using System.Globalization;

namespace CrytonCoreNext.PDF.Models
{
    public class PDFReader : IPDFReader
    {
        private Dictionary<EPdfMetainfo, SymbolIcon> _symbolByPDFKey;

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
                var metaInfoDict = new Dictionary<string, string>();
                using var pdfReader = new PdfReader(pdfFile.Path, new ReaderProperties().SetPassword(Encoding.Default.GetBytes(pdfFile.Password)));
                using var pdfDocument = new PdfDocument(pdfReader);
                metaInfoDict["PDF.PageCount"] = $"{pdfDocument.GetNumberOfPages():D}";
                metaInfoDict["PDF.Version"] = $"{pdfDocument.GetPdfVersion()}";

                var pdfTrailer = pdfDocument.GetTrailer();
                var pdfDictInfo = pdfTrailer.GetAsDictionary(PdfName.Info);
                if (pdfTrailer != null && pdfDictInfo != null)
                {
                    foreach (var pdfEntryPair in pdfDictInfo.EntrySet())
                    {
                        var key = "PDF." + pdfEntryPair.Key.ToString()[1..];
                        string value = pdfEntryPair.Value switch
                        {
                            PdfString pdfString => pdfString.ToUnicodeString(),
                            _ => pdfEntryPair.Value.ToString(),
                        };
                        metaInfoDict[key] = value;
                    }
                }
                ParseNativeMetainfo(pdfFile, metaInfoDict);
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached) Debugger.Break();
                pdfFile.Metadata = new ()
                { { new SymbolIcon() { Symbol = Wpf.Ui.Common.SymbolRegular.ErrorCircle20 }, ex.Message} };
            }
        }

        private void ParseNativeMetainfo(PDFFile pdfFile, Dictionary<string, string> metaInfoDict)
        {
            var indirectDict = new Dictionary<EPdfMetainfo, string>();
            foreach (var metaEnum in Enum.GetValues(typeof(EPdfMetainfo)).Cast<EPdfMetainfo>())
            {
                var metaKey = metaEnum.ToString(true);
                if (metaInfoDict.ContainsKey(metaKey))
                {
                    indirectDict.Add(metaEnum, metaInfoDict[metaKey]);
                }
            }
            pdfFile.Metadata = new Dictionary<SymbolIcon, string>();
            foreach (var key in indirectDict.Keys)
            {
                if (_symbolByPDFKey.ContainsKey(key))
                {
                    var symbol = _symbolByPDFKey[key];
                    var text = indirectDict[key];
                    if (key == EPdfMetainfo.CreationDate ||
                        key == EPdfMetainfo.ModDate)
                    {
                        text = text[2..16];
                        DateTime.TryParseExact(text, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime);
                        if (dateTime != DateTime.MinValue)
                        {
                            text = dateTime.ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);
                        }
                    }
                    symbol.ToolTip = key.ToString(false);
                    pdfFile.Metadata.Add(symbol, text);
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
            _symbolByPDFKey = new Dictionary<EPdfMetainfo, SymbolIcon>()
            {
                { EPdfMetainfo.Author, new SymbolIcon() { Symbol = Wpf.Ui.Common.SymbolRegular.Person20 } },
                { EPdfMetainfo.Creator, new SymbolIcon() { Symbol = Wpf.Ui.Common.SymbolRegular.Person20, Filled=true  } },
                { EPdfMetainfo.CreationDate, new SymbolIcon() { Symbol = Wpf.Ui.Common.SymbolRegular.CalendarRtl20  } },
                { EPdfMetainfo.ModDate, new SymbolIcon() { Symbol = Wpf.Ui.Common.SymbolRegular.CalendarRtl20, Filled=true  } },
                { EPdfMetainfo.PageCount, new SymbolIcon() { Symbol = Wpf.Ui.Common.SymbolRegular.DocumentMultiple20  } },
                { EPdfMetainfo.Version, new SymbolIcon() { Symbol = Wpf.Ui.Common.SymbolRegular.Box20  } }
            };
        }
    }
}
