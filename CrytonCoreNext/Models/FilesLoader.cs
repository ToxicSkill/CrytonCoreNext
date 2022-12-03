using CrytonCoreNext.Crypting.Interfaces;
using CrytonCoreNext.Interfaces;
using System.Collections.Generic;
using System.IO;
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
