using CrytonCoreNext.Abstract;
using CrytonCoreNext.Interfaces;
using CrytonCoreNext.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        public bool DeleteItem(FilesViewViewModel filesViewViewModel, Guid guid)
        {
            if (filesViewViewModel == null)
            {
            }
            var fileToDelete = filesViewViewModel?.FilesView?.Where(x => x.Guid == guid).Select(x => x).FirstOrDefault();
            if (fileToDelete != null)
            {
                var fileId = fileToDelete?.Id;
                filesViewViewModel?.FilesView?.Remove(fileToDelete);
                foreach (var file in filesViewViewModel?.FilesView)
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

        public bool DeleteItem(FilesViewViewModel filesViewViewModel, int index)
        {
            return filesViewViewModel == null ?
                false :
                filesViewViewModel.FilesView.Remove(filesViewViewModel.FilesView.ElementAt(index));
        }

        public bool ClearAllFiles(FilesViewViewModel filesViewViewModel)
        {
            if (filesViewViewModel == null)
            {
                return false;
            }
            if (filesViewViewModel.FilesView.Count == 0)
            {
                return false;
            }


            filesViewViewModel.FilesView.Clear();
            filesViewViewModel.SelectedItemIndex = -1;

            OnPropertyChanged(nameof(filesViewViewModel.SelectedItemIndex));
            OnPropertyChanged(nameof(filesViewViewModel.FilesView));

            return true;
        }
    }
}
