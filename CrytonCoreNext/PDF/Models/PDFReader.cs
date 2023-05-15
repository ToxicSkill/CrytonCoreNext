﻿using CrytonCoreNext.Models;
using CrytonCoreNext.PDF.Enums;
using CrytonCoreNext.PDF.Interfaces;
using Docnet.Core;
using Docnet.Core.Exceptions;
using Docnet.Core.Models;
using Docnet.Core.Readers;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using iText.Kernel.Pdf;
using System.Runtime.InteropServices;
using System.Text;

namespace CrytonCoreNext.PDF.Models
{
    public class PDFReader : IPDFReader
    {
        private readonly double _dimensions = 1.0d;

        public PDFFile ReadPdf(File file, string password = "")
        {
            using IDocLib pdfLibrary = DocLib.Instance;
            IDocReader? reader = null;
            var status = EPdfStatus.Opened;
            var dimensions = _dimensions;

            if (file.Path.Equals(string.Empty))
            {
                return CreateNewPdfFile(file, reader, status);
            }

            try
            {
                reader = password.Equals(string.Empty, default) ?
                    pdfLibrary.GetDocReader(file.Bytes, new PageDimensions(dimensions)) :
                    pdfLibrary.GetDocReader(file.Bytes, password, new PageDimensions(dimensions));
            }
            catch (DocnetLoadDocumentException)
            {
                status = EPdfStatus.Protected;
            }
            catch (DocnetException)
            {
                status = EPdfStatus.Damaged;
            }

            var pdfFile = CreateNewPdfFile(file, reader, status);
            if (pdfFile.PdfStatus == EPdfStatus.Opened)
            {
                GetMetaInfo(ref pdfFile);
            }
            return pdfFile;
        }

        public void UpdatePdfFileInformations(ref PDFFile pdfFile)
        {
            using IDocLib pdfLibrary = DocLib.Instance;
            IDocReader? reader = null;
            pdfFile.Dimensions = _dimensions;
            try
            {
                reader = pdfFile.Password.Equals(string.Empty, default) ?
                    pdfLibrary.GetDocReader(pdfFile.Bytes, new PageDimensions(pdfFile.Dimensions)) :
                    pdfLibrary.GetDocReader(pdfFile.Bytes, pdfFile.Password, new PageDimensions(pdfFile.Dimensions));
            }
            catch (DocnetLoadDocumentException)
            {
                pdfFile.PdfStatus = EPdfStatus.Protected;
                return;
            }
            catch (DocnetException)
            {
                pdfFile.PdfStatus = EPdfStatus.Damaged;
                return;
            }

            pdfFile.Version = reader.GetPdfVersion();
            pdfFile.NumberOfPages = reader.GetPageCount();
            pdfFile.IsOpened = true;
            GetMetaInfo(ref pdfFile);
        }

        private void GetMetaInfo(ref PDFFile pdfFile)
        {
            try
            {
                var metaInfoDict = new Dictionary<string, string>();
                using (var pdfReader = new PdfReader(pdfFile.Path, new ReaderProperties().SetPassword(Encoding.Default.GetBytes(pdfFile.Password))))
                using (var pdfDocument = new PdfDocument(pdfReader))
                {
                    metaInfoDict["PDF.PageCount"] = $"{pdfDocument.GetNumberOfPages():D}";
                    metaInfoDict["PDF.Version"] = $"{pdfDocument.GetPdfVersion()}";

                    var pdfTrailer = pdfDocument.GetTrailer();
                    var pdfDictInfo = pdfTrailer.GetAsDictionary(PdfName.Info);
                    foreach (var pdfEntryPair in pdfDictInfo.EntrySet())
                    {
                        var key = "PDF." + pdfEntryPair.Key.ToString().Substring(1);
                        string value;
                        switch (pdfEntryPair.Value)
                        {
                            case PdfString pdfString:
                                value = pdfString.ToUnicodeString();
                                break;
                            default:
                                value = pdfEntryPair.Value.ToString();
                                break;
                        }
                        metaInfoDict[key] = value;
                    }
                    pdfFile.Metadata = metaInfoDict;
                }
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached) Debugger.Break();
                pdfFile.Metadata = new Dictionary<string, string>()
                { {"Error", ex.Message} };
            }
        }

        private PDFFile CreateNewPdfFile(File file, IDocReader? reader, EPdfStatus status)
        {
            return reader == null
                ? new PDFFile(file, status)
                : new PDFFile(
                file: file,
                version: reader.GetPdfVersion(),
                reader: reader,
                pdfStatus: status,
                password: string.Empty,
                dimensions: _dimensions,
                numberOfPages: reader.GetPageCount());
        }
    }
}
