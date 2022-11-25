using CrytonCoreNext.Models;

namespace CrytonCoreNext.Interfaces
{
    public interface IPDFReader
    {
        PDFBase? ReadPdf(File file, string password = "");
    }
}
