using CrytonCoreNext.Interfaces.Files;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CrytonCoreNext.Extensions;

namespace CrytonCoreNext.Models
{
    public class FilesLoader : IFilesLoader
    {

        public async IAsyncEnumerable<File> LoadFiles(List<string> filesNames, int currentIndex = 0)
        {
            for (int i = 0; i < filesNames.Count; i++)
            {
                currentIndex += 1;
                var newIndex = currentIndex;
                var fileName = filesNames[i];
                yield return await Task.Run(() =>
                {
                    var byteArray = System.IO.File.ReadAllBytes(fileName);
                    return InitializeNewFile(newIndex, fileName, byteArray);
                });
            }
        }

        private File InitializeNewFile(int currentFilesCount, string path, byte[] byteArray)
        {
            if (byteArray.Length == 0)
            {
                return default!;
            }
            var fileInfo = new FileInfo(path);
            var fileExtension = fileInfo.Extension.Contains('.') ? fileInfo.Extension.Substring(1) : "N/A";
            return new File(name: Path.GetFileNameWithoutExtension(fileInfo.FullName), path: path, date: fileInfo.CreationTimeUtc, extension: fileExtension, id: currentFilesCount, bytes: byteArray);
        }
    }
}
