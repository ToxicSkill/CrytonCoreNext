using CrytonCoreNext.Enums;
using System.Collections.Generic;

namespace CrytonCoreNext.Interfaces
{
    public interface IDialogService
    {
        List<string> GetFilesNamesToOpen(EDialogFilters.DialogFilters filter, string title, bool multiselect = false);

        List<string> GetFilesNamesToSave(EDialogFilters.DialogFilters filter, string title, string extension);
    }
}
