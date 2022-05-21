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

        public bool DeleteItem(ObservableCollection<Models.File> files, Guid guid)
        {
            if (files.IsCollectionEmpty())
            {
                return false;
            }
            var fileToDelete = GetFileByGuid(files, guid);
            if (fileToDelete != null)
            {
                var fileId = fileToDelete?.Id;
                files.Remove(fileToDelete);
                foreach (var file in files)
                {
                   if (file.Id > fileId)
                   {
                        file.Id--;
                   }
                }
                return true;
            }
            return false;
        }

        public bool SetItemAsFirst(ObservableCollection<Models.File> files, Guid guid)
        {
            if (files.IsCollectionEmpty())
            {
                return false;
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

            return true;
        }

        public bool SetItemAsLast(ObservableCollection<Models.File> files, Guid guid)
        {
            if (files.IsCollectionEmpty())
            {
                return false;
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

            return true;
        }

        public bool MoveItemUp(ObservableCollection<Models.File> files, Guid guid)
        {
            if (files.IsCollectionEmpty())
            {
                return false;
            }
            var chosen = GetFileByGuid(files, guid);
            if (chosen != null && chosen.Id != 1)
            {
                files.Remove(chosen);
                files?.Insert(chosen.Id - 2, chosen);
                foreach (var file in files.Select((value, i) => new { i, value }))
                {
                    file.value.Id = file.i + 1;
                }
            }

            return true;
        }

        public bool MoveItemDown(ObservableCollection<Models.File> files, Guid guid)
        {
            if (files.IsCollectionEmpty())
            {
                return false;
            }
            var chosen = GetFileByGuid(files, guid);
            if (chosen != null && chosen.Id < files.Count)
            {
                files.Remove(chosen);
                files.Insert(chosen.Id, chosen);
                foreach (var file in files.Select((value, i) => new { i, value }))
                {
                    file.value.Id = file.i + 1;
                }
            }

            return true;
        }


        public bool DeleteItem(ObservableCollection<Models.File> files, int index)
        {
            if (!files.IsCollectionEmpty())
            {
                return files.Remove(files.ElementAt(index));
            }

            return false;                
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
    }
}
