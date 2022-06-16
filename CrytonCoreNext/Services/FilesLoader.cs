using CrytonCoreNext.Interfaces;
using System.Collections.Generic;
using System.IO;

namespace CrytonCoreNext.Services
{
    public class FilesLoader : IFilesLoader
    {
        private static string[] Sizes = { "B", "KB", "MB", "GB", "TB" };

        public List<Models.File> LoadFiles(string[] paths, int currentFilesCount = 0)
        {
            var validPaths = new List<string>();

            foreach (var path in paths)
            {
                if (File.Exists(path))
                    validPaths.Add(path);
            }

            if (validPaths.Count == 0)
            {
                return new List<Models.File>();
            }

            var files = new List<Models.File>();

            foreach (var path in validPaths)
            {
                currentFilesCount += 1;
                var byteArray = File.ReadAllBytes(path);
                Models.File newFile = InitializeNewFile(currentFilesCount, path, byteArray);
                files.Add(newFile);
            }

            return files;
        }

        private static Models.File InitializeNewFile(int currentFilesCount, string path, byte[] byteArray)
        {
            var fileInfo = new FileInfo(path);
            return new Models.File()
            {
                Id = currentFilesCount,
                Name = Path.GetFileNameWithoutExtension(fileInfo.FullName),
                NameWithExtension = fileInfo.Name,
                Extension = fileInfo.Extension.Substring(1),
                Date = fileInfo.CreationTimeUtc,
                Size = GetSizeString(fileInfo.Length),
                Path = path,
                Bytes = byteArray,
                //Text = Parsers.FileContentParser.GetStringFromBytes(byteArray),
                Guid = System.Guid.NewGuid()
            };
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
