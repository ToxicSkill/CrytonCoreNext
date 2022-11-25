using CrytonCoreNext.Interfaces;
using CrytonCoreNext.Static;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

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

        public List<File>? LoadFiles(List<string> filesNames, int currentIndex = 0)
        {
            var files = new List<File>();
            var count = filesNames.Count;
            ManualResetEvent[] resetEvents = new ManualResetEvent[count];

            for (int i = 0; i < count; i++)
            {
                currentIndex += 1;
                var newIndex = currentIndex;
                var fileName = filesNames[i];
                resetEvents[i] = new ManualResetEvent(false);
                ThreadPool.QueueUserWorkItem(new WaitCallback((object index) =>
                {
                    var byteArray = System.IO.File.ReadAllBytes(fileName);
                    var newFile = InitializeNewFile(newIndex, fileName, byteArray);
                    lock (files)
                    {
                        files.Add(newFile);
                    }

                    resetEvents[(int)index].Set();
                }), i);
            }

            foreach (var reset in resetEvents)
            {
                reset.WaitOne();
            }

            return files;
        }

        private File InitializeNewFile(int currentFilesCount, string path, byte[] byteArray)
        {
            var fileInfo = new FileInfo(path);
            var fileExtension = fileInfo.Extension.Contains('.') ? fileInfo.Extension.Substring(1) : "N/A";
            var recognitionResults = _cryptingRecognition.RecognizeBytes(byteArray);
            return new File()
            {
                Id = currentFilesCount,
                Name = System.IO.Path.GetFileNameWithoutExtension(fileInfo.FullName),
                NameWithExtension = fileInfo.Name,
                Extension = recognitionResults.succes ? recognitionResults.Item2.extension : fileExtension,
                Date = fileInfo.CreationTimeUtc,
                Size = GetSizeString(fileInfo.Length),
                Path = path,
                Bytes = recognitionResults.succes ? byteArray.Skip(64).ToArray() : byteArray,
                Status = recognitionResults.succes ? CryptingStatus.Status.Encrypted : CryptingStatus.Status.Decrypted,
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
