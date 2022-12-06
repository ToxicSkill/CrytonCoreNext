﻿using CrytonCoreNext.Models;
using System;
using System.Collections.Generic;

namespace CrytonCoreNext.Interfaces
{
    public interface IFilesView
    {
        event EventHandler CurrentFileChanged;

        event EventHandler FileDeleted;

        event EventHandler AllFilesDeleted;

        void UpdateFiles();

        void AddFile(File newFile);

        Guid GetCurrentFileGuid();

        Guid GetDeletedFileGuid();

        List<Guid> GetFilesGuids();

        int GetSelectedFileIndex();

        int GetFilesCount();
    }
}
