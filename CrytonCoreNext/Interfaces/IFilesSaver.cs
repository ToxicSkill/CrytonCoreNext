using CrytonCoreNext.Enums;

namespace CrytonCoreNext.Interfaces
{
    public interface IFilesSaver
    {
        bool SaveFile(string fileName, Models.File file);
    }
}
