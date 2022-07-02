using System.Collections.Generic;

namespace CrytonCoreNext.Interfaces
{
    public interface IFilesLoader
    {
        List<Models.File>? LoadFiles(Enums.EDialogFilters.DialogFilters filter, string title, bool multiselect = false, int currentIndex = 0);
    }
}
