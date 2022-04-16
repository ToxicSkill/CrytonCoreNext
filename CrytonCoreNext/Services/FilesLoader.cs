using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;

namespace CrytonCoreNext.Services
{
    public class FilesLoader
    {
        public static Models.File LoadFile(string path, int currentFilesCount = 0)
        {
            if (!File.Exists(path))
                return null;

            var byteArray = File.ReadAllBytes(path);
            return new Models.File()
            {
                Id = currentFilesCount++,
                Name = Path.GetFileName(path), 
                Path = path, 
                Bytes = byteArray 
            };
        }

        public static List<Models.File> LoadFiles(string[] paths, int currentFilesCount = 0)
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
                var newFile = new Models.File()
                {
                    Id = currentFilesCount,
                    Name = Path.GetFileName(path), 
                    Size = GetSizeString(byteArray.Length), 
                    Path = path, 
                    Bytes = byteArray 
                };
                files.Add(newFile);
            }

            return files;
        }


        private static string GetSizeString(int bytesCount)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytesCount;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                ++order;
                len /= 1024;
            }
            return string.Format("{0:0.## }{1}", len, sizes[order]);
        }
    }
}
