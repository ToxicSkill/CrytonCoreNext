using CrytonCoreNext.Models;
using System;
using System.Collections.Generic;

namespace CrytonCoreNext.Interfaces
{
    public interface IFilesView
    {
        event EventHandler CurrentFileChanged;

        event EventHandler FileDeleted;

        event EventHandler AllFilesDeleted;

        void UpdateFiles(List<File> newFiles);

        File? GetCurrentFile();

        Guid GetDeletedFileGuid();

        List<File> GetFiles();

        int GetSelectedFileIndex();

        int GetFilesCount();
    }
}
