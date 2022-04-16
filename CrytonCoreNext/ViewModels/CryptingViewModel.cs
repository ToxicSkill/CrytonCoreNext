using CrytonCoreNext.Abstract;
using CrytonCoreNext.Commands;
using CrytonCoreNext.Helpers;
using CrytonCoreNext.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace CrytonCoreNext.ViewModels
{
    public class CryptingViewModel : InteractiveViewBase
    {
        public string FileSize { get; set; }

        public ICommand PostFilesCommand { get; set; }

        public CryptingViewModel()
        {
            PostFilesCommand = new Command(UpdateFiles, true);
            FilesViewViewModel.SelectedFilesViewItemPropertyChanged += SelectedItem_PropertyChanged;
            //PostPopup("Hello world!", 5, EPopopColor.Information);
        }

        private void SelectedItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            FileSize = FilesViewViewModel.FilesView.ElementAt(FilesViewViewModel.SelectedItemIndex).Size;
            OnPropertyChanged(nameof(FileSize));
        }

        private void UpdateFiles()
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
                var newFiles = FilesLoader.LoadFiles(chosenPaths.ToArray(), filesCount);
                var newFilesCollection = filesCount > 0 ? 
                    FilesViewViewModel.FilesView?.ToList().Concat(newFiles) :
                    newFiles;

                UpdateFilesView(new ObservableCollection<Models.File>(newFilesCollection));
            }
        }
    }
}
