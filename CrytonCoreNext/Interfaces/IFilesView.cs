using CrytonCoreNext.Models;
using System;
using System.Collections.Generic;

namespace CrytonCoreNext.Interfaces
{
    public interface IFilesView
    {
        event EventHandler FilesChanged;

        File? GetCurrentFile();

        File? GetFileByIndex(int index);

        int GetFilesCount();

        int GetSelectedFileIndex();

        bool AddNewFiles(List<File> files);

        bool AnyFiles();
    }
}
