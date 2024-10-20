﻿using CrytonCoreNext.Static;
using System;
using System.Collections.ObjectModel;

namespace CrytonCoreNext.Interfaces.Files
{
    public interface IFilesManager
    {
        (bool result, int newIndex) DeleteItem(ObservableCollection<Models.File> files, Guid guid);

        (bool result, int newIndex) ClearAllFiles(ObservableCollection<Models.File> files, Guid guid);

        (bool result, int newIndex) SetItemAsFirst(ObservableCollection<Models.File> files, Guid guid);

        (bool result, int newIndex) SetItemAsLast(ObservableCollection<Models.File> files, Guid guid);

        (bool result, int newIndex) MoveItemUp(ObservableCollection<Models.File> files, Guid guid);

        (bool result, int newIndex) MoveItemDown(ObservableCollection<Models.File> files, Guid guid);

        (bool result, int newIndex) ModifyFile(ObservableCollection<Models.File> files, Guid guid, byte[] bytes, CryptingStatus.Status status, string? methodName);

        void ReorderFiles(ObservableCollection<Models.File> files);
    }
}
