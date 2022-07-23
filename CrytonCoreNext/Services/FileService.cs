﻿using CrytonCoreNext.Enums;
using CrytonCoreNext.Interfaces;
using CrytonCoreNext.Models;
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

        public List<File>? LoadFiles(EDialogFilters.DialogFilters filter, string title, bool multiselect = false, int currentIndex = 0)
        {
            return _filesLoader.LoadFiles(filter, title, multiselect, currentIndex);
        }

        public bool SaveFile(EDialogFilters.DialogFilters filter, string title, File file)
        {
            return _filesSaver.SaveFile(filter, title, file);
        }

        public (bool result, int newIndex) ModifyFile(ObservableCollection<File> files, Guid guid, byte[] bytes, bool status, string? methodName)
        {
            return _filesManager.ModifyFile(files, guid, bytes, status, methodName);
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
    }
}