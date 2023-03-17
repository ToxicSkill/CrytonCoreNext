using CrytonCoreNext.Interfaces.Files;
using CrytonCoreNext.Models;
using CrytonCoreNext.Static;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CrytonCoreNext.Services
{
    public class FileService : IFileService
    {
        private readonly IFilesSaver _filesSaver;

        private readonly IFilesLoader _filesLoader;

        private readonly IFilesManager _filesManager;

        public FileService(IFilesSaver filesSaver, IFilesLoader filesLoader, IFilesManager filesManager)
        {
            _filesSaver = filesSaver;
            _filesLoader = filesLoader;
            _filesManager = filesManager;
        }

        public bool HasBytes(File? file)
        {
            return file != null && file.Bytes.Length > 0;
        }

        public async IAsyncEnumerable<File> LoadFiles(List<string> filesNames, int currentIndex = 0)
        {
            await foreach (var file in _filesLoader.LoadFiles(filesNames, currentIndex))
            {
                yield return file;
            }
        }

        public bool SaveFile(string fileName, File file)
        {
            return _filesSaver.SaveFile(fileName, file);
        }

        public (bool result, int newIndex) MoveItemDown(ObservableCollection<File> files, Guid guid)
        {
            return _filesManager.MoveItemDown(files, guid);
        }

        public (bool result, int newIndex) MoveItemUp(ObservableCollection<File> files, Guid guid)
        {
            return _filesManager.MoveItemUp(files, guid);
        }

        public (bool result, int newIndex) SetItemAsFirst(ObservableCollection<File> files, Guid guid)
        {
            return _filesManager.SetItemAsFirst(files, guid);
        }

        public (bool result, int newIndex) SetItemAsLast(ObservableCollection<File> files, Guid guid)
        {
            return _filesManager.SetItemAsLast(files, guid);
        }

        public (bool result, int newIndex) ClearAllFiles(ObservableCollection<File> files, Guid guid)
        {
            return _filesManager.ClearAllFiles(files, guid);
        }

        public (bool result, int newIndex) DeleteItem(ObservableCollection<File> files, Guid guid)
        {
            return _filesManager.DeleteItem(files, guid);
        }

        public (bool result, int newIndex) ModifyFile(ObservableCollection<File> files, Guid guid, byte[] bytes, CryptingStatus.Status status, string? methodName)
        {
            return _filesManager.ModifyFile(files, guid, bytes, status, methodName);
        }

        public void ReorderFiles(ObservableCollection<File> files)
        {
            _filesManager.ReorderFiles(files);
        }
    }
}
