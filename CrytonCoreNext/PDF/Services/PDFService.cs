﻿using CrytonCoreNext.Models;
using CrytonCoreNext.PDF.Interfaces;
using CrytonCoreNext.PDF.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace CrytonCoreNext.PDF.Services
{
    public class PDFService : IPDFService
    {
        private readonly IPDFManager _pdfManager;

        private readonly IPDFReader _pdfReader;

        public PDFService(IPDFManager pdfManager, IPDFReader pdfReader)
        {
            _pdfManager = pdfManager;
            _pdfReader = pdfReader;
        }

        public async IAsyncEnumerable<BitmapImage> LoadAllPDFImages(PDFFile pdfFile)
        {
            await foreach (var image in _pdfManager.LoadAllPDFImages(pdfFile))
            {
                yield return image;
            }
        }

        public async Task<File> Merge(List<PDFFile> pdfFiles)
        {
            return await _pdfManager.Merge(pdfFiles);
        }

        public PDFFile? ReadPdf(File file, string password = "")
        {
            return _pdfReader.ReadPdf(file, password);
        }
    }
}
