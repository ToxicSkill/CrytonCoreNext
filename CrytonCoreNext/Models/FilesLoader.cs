using CrytonCoreNext.Enums;
using CrytonCoreNext.Helpers;
using CrytonCoreNext.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CrytonCoreNext.Models
{
    public class FilesLoader : IFilesLoader
    {
        private readonly ICryptingRecognition _cryptingRecognition;

        public FilesLoader(ICryptingRecognition cryptingRecognition)
        {
            _cryptingRecognition = cryptingRecognition;
        }

        private static readonly string[] Sizes = { "B", "KB", "MB", "GB", "TB" };

        public List<Models.File>? LoadFiles(EDialogFilters.DialogFilters filter, string title, bool multiselect = false, int currentIndex = 0)
        {
            WindowDialog.OpenDialog openDialog = new(new DialogHelper()
            {
                Filters = EDialogFilters.ExtensionToFilter(filter),
                Multiselect = multiselect,
                Title = title
            });
            var chosenPaths = openDialog.RunDialog();
            if (chosenPaths.Count > 0)
            {
                var validPaths = new List<string>();

                foreach (var path in chosenPaths)
                {
                    if (System.IO.File.Exists(path))
                        validPaths.Add(path);
                }

                if (validPaths.Count == 0)
                {
                    return new ();
                }

                var files = new List<Models.File>();

                foreach (var path in validPaths)
                {
                    currentIndex += 1;
                    var byteArray = System.IO.File.ReadAllBytes(path);
                    Models.File newFile = InitializeNewFile(currentIndex, path, byteArray);
                    files.Add(newFile);
                }

                return files;
            }

            return null;
        }

        private Models.File InitializeNewFile(int currentFilesCount, string path, byte[] byteArray)
        {
            var fileInfo = new FileInfo(path);
            var fileExtension = fileInfo.Extension.Contains('.') ? fileInfo.Extension.Substring(1) : "N/A";
            var recognitionResults = _cryptingRecognition.RecognizeBytes(byteArray);
            return new Models.File()
            {
                Id = currentFilesCount,
                Name = Path.GetFileNameWithoutExtension(fileInfo.FullName),
                NameWithExtension = fileInfo.Name,
                Extension = recognitionResults.succes ? recognitionResults.Item2.extension : fileExtension,
                Date = fileInfo.CreationTimeUtc,
                Size = GetSizeString(fileInfo.Length),
                Path = path,
                Bytes = recognitionResults.succes ? byteArray.Skip(64).ToArray() : byteArray,
                Status = recognitionResults.succes ? true : false,
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
