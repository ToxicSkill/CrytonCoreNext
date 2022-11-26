using CrytonCoreNext.Models;

namespace CrytonCoreNext.Interfaces
{
    public interface IPDFReader
    {
        PDFFile? ReadPdf(File file, string password = "");
    }
}
