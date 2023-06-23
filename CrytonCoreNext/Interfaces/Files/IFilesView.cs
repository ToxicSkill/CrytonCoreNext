using CrytonCoreNext.Models;
using System;
using System.Collections.Generic;

namespace CrytonCoreNext.Interfaces.Files
{
    public interface IFilesView
    {
        event EventHandler CurrentFileChanged;

        event EventHandler FileDeleted;

        event EventHandler AllFilesDeleted;

        event EventHandler FilesReordered;

        void UpdateFiles();

        void AddFile(File newFile);

        Guid GetCurrentFileGuid();

        Guid GetDeletedFileGuid();

        List<Guid> GetFilesGuids();

        int GetSelectedFileIndex();

        int GetFilesCount();

        Dictionary<Guid, int> GetFilesOrder();
    }
}
