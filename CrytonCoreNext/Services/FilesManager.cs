using CrytonCoreNext.Abstract;
using CrytonCoreNext.Extensions;
using CrytonCoreNext.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CrytonCoreNext.Services
{
    public class FilesManager : ViewModelBase, IFilesManager
    {
        private readonly IFilesLoader _filesLoader;

        public FilesManager(IFilesLoader filesLoader)
        {
            _filesLoader = filesLoader;
        }

        public List<Models.File> AddFiles(string[] paths, int currentIndex = 0)
        {
            return _filesLoader.LoadFiles(paths, currentIndex);
        }

        public (bool result, int newIndex) DeleteItem(ObservableCollection<Models.File> files, Guid guid)
        {
            if (files.IsCollectionEmpty())
            {
                return new(false, -1);
            }
            var fileToDelete = GetFileByGuid(files, guid);
            if (fileToDelete != null)
            {
                var trigger = false;
                var fileId = fileToDelete?.Id;
                files.Remove(fileToDelete);
                foreach (var file in files)
                {
                   if (file.Id > fileId)
                   {
                        file.Id--;
                        trigger = true;
                   }
                }
                return new(true, trigger ? fileToDelete.Id - 1 : fileToDelete.Id);
            }
            ReorderFiles(files);

            return new(false, -1);
        }

        public (bool result, int newIndex) SetItemAsFirst(ObservableCollection<Models.File> files, Guid guid)
        {
            if (files.IsCollectionEmpty())
            {
                return new(false, -1);
            }
            var chosen = GetFileByGuid(files, guid);
            if (chosen != null && chosen.Id != 1)
            {
                files.Remove(chosen);
                files.Insert(0, chosen);
                foreach (var file in files.Select((value, i) => new { i, value }))
                {
                    file.value.Id = file.i + 1;
                }
            }
            ReorderFiles(files);

            return new(true, 0);
        }

        public (bool result, int newIndex) SetItemAsLast(ObservableCollection<Models.File> files, Guid guid)
        {
            if (files.IsCollectionEmpty())
            {
                return new(false, -1);
            }
            var chosen = GetFileByGuid(files, guid);
            if (chosen != null && chosen.Id != files.Count)
            {
                files.Remove(chosen);
                files.Insert(files.Count, chosen);
                foreach (var file in files.Select((value, i) => new { i, value }))
                {
                    file.value.Id = file.i + 1;
                }
            }
            ReorderFiles(files);

            return new(true, files.Count - 1);
        }

        public (bool result, int newIndex) MoveItemUp(ObservableCollection<Models.File> files, Guid guid)
        {
            if (files.IsCollectionEmpty())
            {
                return new(false, -1);
            }
            var chosen = GetFileByGuid(files, guid);
            var index = chosen.Id - 1;
            if (chosen != null && chosen.Id != 1)
            {
                files.Remove(chosen);
                files?.Insert(chosen.Id - 2, chosen);
                foreach (var file in files.Select((value, i) => new { i, value }))
                {
                    file.value.Id = file.i + 1;
                }
            }
            ReorderFiles(files);

            if (index <= 0)
            {
                return new(true, 0);
            }
            else
            {
                return new(true, index - 1);
            }
        }

        public (bool result, int newIndex) MoveItemDown(ObservableCollection<Models.File> files, Guid guid)
        {
            if (files.IsCollectionEmpty())
            {
                return new(false, -1);
            }
            var chosen = GetFileByGuid(files, guid);
            var index = chosen.Id - 1;
            if (chosen != null && chosen.Id < files.Count)
            {
                files.Remove(chosen);
                files.Insert(chosen.Id, chosen);
                foreach (var file in files.Select((value, i) => new { i, value }))
                {
                    file.value.Id = file.i + 1;
                }
            }
            ReorderFiles(files);

            if (index == files.Count - 1)
            {
                return new(true, index);
            }
            else
            {
                return new(true, index + 1);
            }
        }

        public bool ClearAllFiles(ObservableCollection<Models.File> files)
        {
            if (files.IsCollectionEmpty())
            {
                return false;
            }

            files.Clear();

            return true;
        }

        private Models.File? GetFileByGuid(ObservableCollection<Models.File> files, Guid guid)
        {
            return files.Where(x => x.Guid == guid).Select(x => x).FirstOrDefault();
        }

        private void ReorderFiles(ObservableCollection<Models.File> files)
        {
            foreach (var item in files.Select((file, i) => new { i, file }))
            {
                item.file.Id = item.i + 1;
            }
        }
    }
}
