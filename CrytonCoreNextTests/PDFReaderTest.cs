using CrytonCoreNext.Interfaces;
using CrytonCoreNext.Models;
using Xunit;

namespace CrytonCoreNextTests
{
    public class PDFReaderTest
    {

        private readonly IPDFReader _pdfReader = new PDFReader();

        public PDFReaderTest()
        {

        }

        [Fact]
        public void CanReadShouldNotNull()
        {
            var path = "";
            var result = _pdfReader.ReadPdf(path);
            Assert.NotNull(result);
        }

        [Fact]
        public void CanReadShouldNull()
        {
            var path = "";
            var result = _pdfReader.ReadPdf(path);
            Assert.Null(result);
        }
    }
}
