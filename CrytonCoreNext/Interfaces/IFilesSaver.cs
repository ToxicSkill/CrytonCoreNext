using CrytonCoreNext.Enums;

namespace CrytonCoreNext.Interfaces
{
    public interface IFilesSaver
    {
        bool SaveFile(EDialogFilters.DialogFilters filter, string title, Models.File file);
    }
}
