using CrytonCoreNext.Interfaces.Files;
using CrytonCoreNext.Models;
using CrytonCoreNext.PDF.Interfaces;
using CrytonCoreNext.PDF.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace CrytonCoreNextTests
{
    public class PdfFiles
    {
        private readonly IFilesLoader _filesLoader;

        private readonly IPDFReader _pdfReader;

        private readonly List<string> _pdfFilesToOpen = new() 
        { 
            "./TestingFiles/PDF NOT SECURED.pdf",
            "./TestingFiles/SECURED PDF 123456.pdf"
        };

        public readonly List<PDFFile> PDFFiles;

        public PdfFiles()
        {
            _pdfReader = new PDFReader();
            _filesLoader = new FilesLoader();
            PDFFiles = new List<PDFFile>();
            LoadPdfFiles();
        }

        private async Task LoadPdfFiles()
        {
            await foreach (var file in _filesLoader.LoadFiles(_pdfFilesToOpen))
            {
                PDFFiles.Add(_pdfReader.ReadPdf(file));
            }
        }
    }
    public class PDFReaderTest : IClassFixture<PdfFiles>
    {
        private readonly PdfFiles _files;

        public PDFReaderTest(PdfFiles filesFixture)
        {
            _files = filesFixture;
        }

        [Fact]
        public void ShouldNotEmptyPdfLoadedFiles()
        {
            var files = _files.PDFFiles;
            Assert.NotEmpty(files);
        }
    }
}
