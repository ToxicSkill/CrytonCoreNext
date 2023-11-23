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
        public IPDFReader PdfReader { get; set; }

        public List<string> PdfFilesToOpen =
        [
            "./TestingFiles/PDF NOT SECURED.pdf",
            "./TestingFiles/SECURED PDF 123456.pdf"
        ];

        public PdfFiles()
        {
            PdfReader = new PDFReader();
        }
    }
      
    public class PDFReaderTest : IClassFixture<PdfFiles>
    {
        private readonly IFilesLoader _filesLoader;

        private readonly PdfFiles _files;

        public List<PDFFile> PDFFiles;

        public PDFReaderTest(PdfFiles filesFixture)
        {
            _files = filesFixture;
            _filesLoader = new FilesLoader();
            PDFFiles = [];
        }

        [StaFact]
        public async Task ShouldNotEmptyPdfLoadedFiles()
        {
            var expected = _files.PdfFilesToOpen.Count;
            await foreach (var file in _filesLoader.LoadFiles(_files.PdfFilesToOpen))
            {
                file.LoadMetadata = false;
                PDFFiles.Add(_files.PdfReader.ReadPdf(file));
            }
            Assert.Equal(expected, PDFFiles.Count);
        }
    }
}
