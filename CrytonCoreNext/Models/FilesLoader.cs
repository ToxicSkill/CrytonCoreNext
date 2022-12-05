using CrytonCoreNext.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CrytonCoreNext.Models
{
    public class FilesLoader : IFilesLoader
    {
        private static readonly string[] Sizes = { "B", "KB", "MB", "GB", "TB" };

        public List<File>? LoadFiles(List<string> filesNames, int currentIndex = 0)
        {
            var tasks = new List<Task<File>>();

            for (int i = 0; i < filesNames.Count; i++)
            {
                currentIndex += 1;
                var newIndex = currentIndex;
                var fileName = filesNames[i];
                tasks.Add(Task.Run(() =>
                {
                    var byteArray = System.IO.File.ReadAllBytes(fileName);
                    return InitializeNewFile(newIndex, fileName, byteArray);
                }));
            }

            Task.WaitAll(tasks.ToArray());
            return tasks.Select(x => x.Result).ToList();
        }

        private File InitializeNewFile(int currentFilesCount, string path, byte[] byteArray)
        {
            var fileInfo = new FileInfo(path);
            var fileExtension = fileInfo.Extension.Contains('.') ? fileInfo.Extension.Substring(1) : "N/A";
            return new File(name: Path.GetFileNameWithoutExtension(fileInfo.FullName), nameWithExtension: fileInfo.Name, path: path, size: GetSizeString(fileInfo.Length), date: fileInfo.CreationTimeUtc, extension: fileExtension, id: currentFilesCount, bytes: byteArray);
        }

        private static string GetSizeString(long bytesCount)
        {
            double len = bytesCount;
            int order = 0;
            while (len >= 1024 && order < Sizes.Length - 1)
            {
                ++order;
                len /= 1024;
            }
            return string.Format("{0:0.## }{1}", len, Sizes[order]);
        }
    }
}
