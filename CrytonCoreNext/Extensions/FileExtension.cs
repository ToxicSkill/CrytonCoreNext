using CrytonCoreNext.ViewModels;
using System.Collections.ObjectModel;

namespace CrytonCoreNext.Extensions
{
    public static class FileExtension
    {
        public static ObservableCollection<Models.File> Copy(this ObservableCollection<Models.File> files)
        {
            var newFile = new ObservableCollection<Models.File>();
            foreach (var file in files)
            {
                newFile.Add(file);
            }

            return newFile;
        }

        public static bool IsNullOrEmpty(this FilesViewViewModel filesView)
        {
            if (filesView != null)
            {
                if (filesView.FilesView != null)
                {
                    if (!filesView.FilesView.IsCollectionEmpty())
                    {
                        return false;
                    }
                    return true;
                }
                return true;
            }
            return true;
        }

        public static bool IsCollectionEmpty(this ObservableCollection<Models.File> files)
        {
            return files.Count <= 0;
        }
    }
}
