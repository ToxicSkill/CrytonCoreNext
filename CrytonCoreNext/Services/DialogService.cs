using CrytonCoreNext.Helpers;
using CrytonCoreNext.Interfaces;
using CrytonCoreNext.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using static CrytonCoreNext.Models.WindowDialog;

namespace CrytonCoreNext.Services
{
    public class DialogService : IDialogService
    {
        private const string Dot = ".";

        private const string Empty = "";

        private const string DefaultFileName = "file";

        public List<string> GetFilesNamesToOpen(Static.Extensions.DialogFilters filter, string title, bool multiselect = false)
        {
            var openDialog = new OpenFileDialog()
            { 
                Filter = Static.Extensions.FilterToPrompt(filter),
                Multiselect = multiselect,
                Title = title
            };
            var result = openDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                if (multiselect)
                {
                    return [.. openDialog.FileNames];
                }
                else
                {
                    return [openDialog.FileName];
                }
            }
            return [];
        }

        public List<string> GetFilesNamesToSave(Static.Extensions.DialogFilters filter, string title, File file)
        {
            if (file == null)
            {
                return (List<string>)Enumerable.Empty<string>();
            }
            var saveDialog = new SaveFileDialog()
            {
                Filter = Static.Extensions.FilterToPrompt(filter),
                Title = title,
                FileName = file.Name + file.Suffix + (!file.Extension.Equals(Empty) ? Dot + file.Extension : Empty)
            };

            var result = saveDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                return [saveDialog.FileName];
            }
            return [];
        }
    }
}