using CrytonCoreNext.Abstract;
using CrytonCoreNext.Commands;
using CrytonCoreNext.Interfaces;
using CrytonCoreNext.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace CrytonCoreNext.ViewModels
{
    public class FilesViewViewModel : ViewModelBase, IFilesView
    {
        private bool _showFilesView = false;

        private int _selectedItemIndex = 0;

        private static readonly (bool result, int newIndex) DefaultResult = new(false, -1);

        private readonly IFilesManager _filesManager;

        private bool _fileChangeBlocker = false;

        private Guid _deletedFileGuid = Guid.Empty;

        public ObservableCollection<File> FilesCollection { get; private set; }

        public event EventHandler CurrentFileChanged;

        public event EventHandler FileDeleted;

        public event EventHandler AllFilesDeleted;

        public File? CurrentFile { get; set; }

        public ICommand ClearFilesCommand { get; set; }

        public ICommand DeleteCurrentFileCommand { get; set; }

        public ICommand SetFileAsFirstCommand { get; set; }

        public ICommand SetFileAsLastCommand { get; set; }

        public ICommand MoveFileUpCommand { get; set; }

        public ICommand MoveFileDownCommand { get; set; }

        public int SelectedItemIndex
        {
            get => _selectedItemIndex;
            set
            {
                if ((_selectedItemIndex != value || value == 0) && !_fileChangeBlocker)
                {
                    _selectedItemIndex = value;
                    UpdateCurrentFile();
                    if (FilesCollection.Any() && _selectedItemIndex != -1)
                    {
                        NotifyFilesView();
                    }
                    OnPropertyChanged(nameof(SelectedItemIndex));
                }
                ChangeShowFilesView();
            }
        }

        public bool ShowFilesView
        {
            get => _showFilesView;
            set
            {
                _showFilesView = value;
                OnPropertyChanged(nameof(ShowFilesView));
            }
        }

        public FilesViewViewModel(IFilesManager filesManager)
        {
            FilesCollection = new ObservableCollection<File>();
            ClearFilesCommand = new Command(ClearAllFiles, CanExecute);
            DeleteCurrentFileCommand = new Command(DeleteFile, CanExecute);
            SetFileAsFirstCommand = new Command(SetFileAsFirst, CanExecute);
            SetFileAsLastCommand = new Command(SetFileAsLast, CanExecute);
            MoveFileUpCommand = new Command(MoveFileUp, CanExecute);
            MoveFileDownCommand = new Command(MoveFileDown, CanExecute);
            _filesManager = filesManager;
        }

        public override bool CanExecute()
        {
            return !IsBusy;
        }

        public int GetFilesCount()
        {
            return FilesCollection.Count;
        }

        public File? GetCurrentFile()
        {
            return CurrentFile;
        }

        public void UpdateFiles(List<File> newFiles)
        {
            if (newFiles == null)
            {
                return;
            }

            FilesCollection = new(FilesCollection.ToList().Concat(newFiles));
            OnPropertyChanged(nameof(FilesCollection));

            if (FilesCollection != null && FilesCollection.Count > 0)
            {
                SelectedItemIndex = 0;
            }

            _filesManager.ReorderFiles(FilesCollection);
        }

        public Guid GetDeletedFileGuid()
        {
            return _deletedFileGuid;
        }
        public List<File> GetFiles()
        {
            return FilesCollection.ToList();
        }

        public int GetSelectedFileIndex()
        {
            return SelectedItemIndex;
        }

        public void ClearAllFiles()
        {
            DoAction(_filesManager.ClearAllFiles);
            AllFilesDeleted.Invoke(null, null);
        }

        public void DeleteFile()
        {
            _deletedFileGuid = CurrentFile.Guid;
            DoAction(_filesManager.DeleteItem);
            FileDeleted.Invoke(null, null);
        }

        public void SetFileAsFirst() => DoAction(_filesManager.SetItemAsFirst);

        public void SetFileAsLast() => DoAction(_filesManager.SetItemAsLast);

        public void MoveFileUp() => DoAction(_filesManager.MoveItemUp);

        public void MoveFileDown() => DoAction(_filesManager.MoveItemDown);

        private void NotifyFilesView(object o = null, EventArgs s = null)
        {
            CurrentFileChanged?.Invoke(o, s);
        }

        private void UpdateCurrentFile()
        {
            if (SelectedItemIndex != -1 && FilesCollection.Count >= SelectedItemIndex + 1)
            {
                CurrentFile = FilesCollection.ElementAt(SelectedItemIndex);
            }
            else
            {
                CurrentFile = null;
            }
        }

        private void ChangeShowFilesView()
        {
            if (FilesCollection?.Count == 0)
            {
                ShowFilesView = false;
            }

            OnPropertyChanged(nameof(ShowFilesView));
        }

        private void DoAction(Func<ObservableCollection<File>, Guid, (bool result, int newIndex)> function)
        {
            if (FilesCollection == null)
                return;

            _fileChangeBlocker = true;
            var (result, newIndex) = CurrentFile != null ? function(FilesCollection, CurrentFile.Guid) : DefaultResult;
            _fileChangeBlocker = false;
            if (result)
            {
                SelectedItemIndex = newIndex;
                OnPropertyChanged(nameof(FilesCollection));
            }
        }
    }
}
