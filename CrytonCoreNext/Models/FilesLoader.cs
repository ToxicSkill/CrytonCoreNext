using CrytonCoreNext.Interfaces.Files;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrytonCoreNext.Models
{
    public class FilesLoader : IFilesLoader
    {
        public async IAsyncEnumerable<File> LoadFiles(List<string> filesNames, IProgress<double> progress, int currentIndex = 0)
        {
            for (var i = 0; i < filesNames.Count; i++)
            {
                progress.Report(i + 1 / filesNames.Count);
                currentIndex += 1;
                var newIndex = currentIndex;
                var fileName = filesNames[i];
                yield return await Task.Run(() =>
                {
                    var byteArray = System.IO.File.ReadAllBytes(fileName);
                    return CreateNewFile(newIndex, fileName, byteArray);
                });
            }
        }

        private static File CreateNewFile(int currentFilesCount, string path, byte[] byteArray)
        {
            if (byteArray.Length == 0)
            {
                return default!;
            }
            return new File(path: path, id: currentFilesCount, bytes: byteArray);
        }
    }
}
