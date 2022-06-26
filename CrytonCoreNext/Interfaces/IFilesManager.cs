using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CrytonCoreNext.Interfaces
{
    public interface IFilesManager
    {
        List<Models.File> AddFiles(string[] paths, int currentIndex = 0);

        (bool result, int newIndex) DeleteItem(ObservableCollection<Models.File> files, Guid guid);

        (bool result, int newIndex) ClearAllFiles(ObservableCollection<Models.File> files, Guid guid);

        (bool result, int newIndex) SetItemAsFirst(ObservableCollection<Models.File> files, Guid guid);

        (bool result, int newIndex) SetItemAsLast(ObservableCollection<Models.File> files, Guid guid);

        (bool result, int newIndex) MoveItemUp(ObservableCollection<Models.File> files, Guid guid);

        (bool result, int newIndex) MoveItemDown(ObservableCollection<Models.File> files, Guid guid);

        (bool result, int newIndex) ModifyFile(ObservableCollection<Models.File> files, Guid guid, byte[] bytes, bool status);
    }
}
