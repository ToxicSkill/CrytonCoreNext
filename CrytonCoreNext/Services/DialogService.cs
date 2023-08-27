using CrytonCoreNext.Helpers;
using CrytonCoreNext.Interfaces;
using CrytonCoreNext.Models;
using System.Collections.Generic;
using System.Linq;

namespace CrytonCoreNext.Services
{
    public class DialogService : IDialogService
    {
        private const string Dot = ".";

        private const string Empty = "";

        private const string DefaultFileName = "file";

        public List<string> GetFilesNamesToOpen(Static.Extensions.DialogFilters filter, string title, bool multiselect = false)
        {
            WindowDialog.OpenDialog openDialog = new(new DialogHelper()
            {
                Filters = Static.Extensions.FilterToPrompt(filter),
                Multiselect = multiselect,
                Title = title
            });

            return openDialog.RunDialog();
        }

        public List<string> GetFilesNamesToSave(Static.Extensions.DialogFilters filter, string title, File file)
        {
            if (file == null)
            {
                return (List<string>)Enumerable.Empty<string>();
            }
            WindowDialog.SaveDialog saveDialog = new(new DialogHelper()
            {
                Filters = Static.Extensions.FilterToPrompt(filter),
                Multiselect = false,
                Title = title,
                FileName = file.Name + file.Suffix + (!file.Extension.Equals(Empty) ? Dot + file.Extension : Empty)
            });

            return saveDialog.RunDialog();
        }
    }
}