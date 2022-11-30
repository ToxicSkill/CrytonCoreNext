using CrytonCoreNext.Models;
using System;
using System.Collections.Generic;

namespace CrytonCoreNext.Interfaces
{
    public interface IFilesView
    {
        event EventHandler FilesChanged;

        void UpdateFiles(List<File> newFiles);

        File? GetCurrentFile();

        List<File> GetFiles();

        int GetSelectedFileIndex();

        int GetFilesCount();
    }
}
