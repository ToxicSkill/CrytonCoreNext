using System.Collections.Generic;

namespace CrytonCoreNext.Interfaces
{
    public interface IDialogService
    {
        List<string> GetFilesNamesToOpen(Static.Extensions.DialogFilters filter, string title, bool multiselect = false);

        List<string> GetFilesNamesToSave(Static.Extensions.DialogFilters filter, string title, string extension);
    }
}
