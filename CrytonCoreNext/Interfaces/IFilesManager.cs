using CrytonCoreNext.Static;
using System;
using System.Collections.ObjectModel;

namespace CrytonCoreNext.Interfaces
{
    public interface IFilesManager
    {
        (bool result, int newIndex) DeleteItem(ObservableCollection<Models.File> files, Guid guid);

        (bool result, int newIndex) ClearAllFiles(ObservableCollection<Models.File> files, Guid guid);

        (bool result, int newIndex) SetItemAsFirst(ObservableCollection<Models.File> files, Guid guid);

        (bool result, int newIndex) SetItemAsLast(ObservableCollection<Models.File> files, Guid guid);

        (bool result, int newIndex) MoveItemUp(ObservableCollection<Models.File> files, Guid guid);

        (bool result, int newIndex) MoveItemDown(ObservableCollection<Models.File> files, Guid guid);

        (bool result, int newIndex) ModifyFile(Models.File file, byte[] bytes, CryptingStatus.Status status, string? methodName);

        (bool result, int newIndex) ModifyFile(ObservableCollection<Models.File> files, Guid guid, byte[] bytes, CryptingStatus.Status status, string? methodName);
    }
}
