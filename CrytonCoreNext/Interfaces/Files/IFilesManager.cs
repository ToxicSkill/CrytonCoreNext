using CrytonCoreNext.Models;
using CrytonCoreNext.Static;
using System;
using System.Collections.ObjectModel;

namespace CrytonCoreNext.Interfaces.Files
{
    public interface IFilesManager
    {
        (bool result, int newIndex) DeleteItem(ObservableCollection<File> files, Guid guid);

        (bool result, int newIndex) ClearAllFiles(ObservableCollection<File> files, Guid guid);

        (bool result, int newIndex) SetItemAsFirst(ObservableCollection<File> files, Guid guid);

        (bool result, int newIndex) SetItemAsLast(ObservableCollection<File> files, Guid guid);

        (bool result, int newIndex) MoveItemUp(ObservableCollection<File> files, Guid guid);

        (bool result, int newIndex) MoveItemDown(ObservableCollection<File> files, Guid guid);

        (bool result, int newIndex) ModifyFile(ObservableCollection<File> files, Guid guid, byte[] bytes, CryptingStatus.Status status, string? methodName);

        bool HasBytes(File? file);

        void ReorderFiles(ObservableCollection<File> files);
    }
}
