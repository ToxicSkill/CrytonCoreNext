using CrytonCoreNext.Enums;
using CrytonCoreNext.Helpers;
using CrytonCoreNext.Interfaces;
using System;
using System.Linq;

namespace CrytonCoreNext.Services
{
    public class FilesSaver : IFilesSaver
    {
        public bool SaveFile(EDialogFilters.DialogFilters filter, string title, Models.File file)
        {
            WindowDialog.SaveDialog saveDialog = new(new DialogHelper()
            {
                Filters = EDialogFilters.ExtensionToFilter(filter),
                Multiselect = false,
                Title = title
            });
            var chosenPath = saveDialog.RunDialog();
            if (chosenPath.Count == 1)
            {
                try
                {
                    ByteArrayToFile(chosenPath.First(), file.Bytes);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return false;
                }
                return true;
            }
            return false;
        }

        private void ByteArrayToFile(string fileName, byte[] bytes) => System.IO.File.WriteAllBytes(fileName, bytes);
    }
}
