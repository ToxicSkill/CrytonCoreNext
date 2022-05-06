using CrytonCoreNext.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CrytonCoreNext.Interfaces
{
    public interface IFilesManager
    {
        List<Models.File> AddFiles(string[] paths, int currentIndex = 0);

        bool DeleteItem(ObservableCollection<Models.File> files, int index);

        bool DeleteItem(ObservableCollection<Models.File> files, Guid guid);

        bool ClearAllFiles(ObservableCollection<Models.File> files);

        bool SetItemAsFirst(ObservableCollection<Models.File> files, Guid guid);

        bool SetItemAsLast(ObservableCollection<Models.File> files, Guid guid);

        bool MoveItemUp(ObservableCollection<Models.File> files, Guid guid);

        bool MoveItemDown(ObservableCollection<Models.File> files, Guid guid);
    }
}
