﻿using CrytonCoreNext.Enums;
using CrytonCoreNext.Helpers;
using CrytonCoreNext.Interfaces;
using CrytonCoreNext.Models;
using System.Collections.Generic;
using System.Windows.Controls.Primitives;

namespace CrytonCoreNext.Services
{
    public class DialogService : IDialogService
    {
        private const string DefaultFileName = "file.";

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

        public List<string> GetFilesNamesToSave(Static.Extensions.DialogFilters filter, string title, string extension)
        {
            WindowDialog.SaveDialog saveDialog = new(new DialogHelper()
            {
                Filters = Static.Extensions.FilterToPrompt(filter),
                Multiselect = false,
                Title = title,
                FileName = DefaultFileName + extension
            });

            return saveDialog.RunDialog();
        }
    }
}