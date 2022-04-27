using CrytonCoreNext.Abstract;
using CrytonCoreNext.Commands;
using CrytonCoreNext.Enums;
using CrytonCoreNext.Helpers;
using CrytonCoreNext.Interfaces;
using CrytonCoreNext.Models;
using CrytonCoreNext.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace CrytonCoreNext.ViewModels
{
    public class CryptingViewModel : InteractiveViewBase, IInteractiveFiles
    {
        public ICommand PostFilesCommand { get; set; }

        public CryptingViewModel()
        {
            PostFilesCommand = new Command(AddFiles, true);
        }

        public void UpdateFiles(IEnumerable<File>? filesCollection)
        {
            if (filesCollection != null)
            {
                UpdateFilesView(new ObservableCollection<File>(filesCollection));
                PostPopup("File(s) where loaded successfuly", 5, EPopopColor.Information);
            }
            else
                PostPopup("Error occured when loading file(s)", 5, EPopopColor.Error);
        }

        public void DeleteFile()
        {
            throw new System.NotImplementedException();
        }

        public void AddFile()
        {
            throw new System.NotImplementedException();
        }

        public void AddFiles()
        {
            WindowDialog.OpenDialog openDialog = new(new DialogHelper()
            {
                Filters = Enums.EDialogFilters.ExtensionToFilter(Enums.EDialogFilters.DialogFilters.All),
                Multiselect = true
                //Title = (string)(App.Current as App).Resources.MergedDictionaries[0]["openFile"]
            });
            var chosenPaths = openDialog.RunDialog();
            if (chosenPaths.Count > 0)
            {
                var filesCount = FilesViewViewModel.FilesView == null ? 0 : FilesViewViewModel.FilesView.Count;
                _lastSelectedItemIndex = FilesViewViewModel.SelectedItemIndex != -1 ? FilesViewViewModel.SelectedItemIndex : 0;
                var newFiles = FilesLoader.LoadFiles(chosenPaths.ToArray(), filesCount);
                var newFilesCollection = filesCount > 0 ?
                    FilesViewViewModel.FilesView?.ToList().Concat(newFiles) :
                    newFiles;

                UpdateFiles(newFilesCollection);
            }
        }
    }
}
