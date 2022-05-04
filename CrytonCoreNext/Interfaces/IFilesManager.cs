using CrytonCoreNext.ViewModels;
using System;
using System.Collections.Generic;

namespace CrytonCoreNext.Interfaces
{
    public interface IFilesManager
    {
        List<Models.File> AddFiles(string[] paths, int currentIndex = 0);

        bool DeleteItem(FilesViewViewModel filesViewViewModel, int index);

        bool DeleteItem(FilesViewViewModel filesViewViewModel, Guid guid);

        bool ClearAllFiles(FilesViewViewModel filesViewViewMode);
    }
}
