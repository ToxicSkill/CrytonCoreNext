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
    }
}
