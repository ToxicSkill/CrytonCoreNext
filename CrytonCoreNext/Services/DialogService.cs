using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;

namespace CrytonCoreNext.Services
{
    public class DialogService
    {
        public string GetFileNameToSave(string filename, string extension, Environment.SpecialFolder specialFolder, Static.Extensions.DialogFilters filters = Static.Extensions.DialogFilters.All)
        {
            var fileDialog = new SaveFileDialog()
            {
                Title = "Save file",
                FileName = filename,
                InitialDirectory = Environment.GetFolderPath(specialFolder),
                AddExtension = true,
                Filter = Static.Extensions.FilterToPrompt(filters)
            };
            if (fileDialog.ShowDialog() == true)
            {
                var outputFilePath = fileDialog.FileName;
                if (outputFilePath == null)
                {
                    return string.Empty;
                }
                return Path.ChangeExtension(outputFilePath, extension);
            }
            return string.Empty;
        }

        public List<string> GetFilesNamesToOpen(Static.Extensions.DialogFilters filters, Environment.SpecialFolder specialFolder)
        {
            var filesDialog = new OpenFileDialog()
            {
                Title = "Select files",
                Multiselect = true,
                Filter = Static.Extensions.FilterToPrompt(filters),
                InitialDirectory = Environment.GetFolderPath(specialFolder)
            };

            if (filesDialog.ShowDialog() == true)
            {
                return [.. filesDialog.FileNames];
            }
            return [];
        }

        public string GetFileNameToOpen(Static.Extensions.DialogFilters filters, Environment.SpecialFolder specialFolder)
        {
            var fileDialog = new OpenFileDialog()
            {
                Title = "Select file",
                Multiselect = false,
                Filter = Static.Extensions.FilterToPrompt(filters),
                InitialDirectory = Environment.GetFolderPath(specialFolder)
            };

            if (fileDialog.ShowDialog() == true)
            {
                return fileDialog.FileName;
            }
            return string.Empty;
        }
    }
}
