using CrytonCoreNext.Abstract;
using CrytonCoreNext.Commands;
using CrytonCoreNext.Extensions;
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

        private readonly IFileService _filesService;

        private bool _fileChangeBlocker = false;

        public ObservableCollection<File> FilesCollection { get; private set; }

        public event EventHandler FilesChanged;

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
                    NotifyFilesView();
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

        public FilesViewViewModel(IFileService filesService)
        {
            FilesCollection = new ObservableCollection<File>();
            ClearFilesCommand = new Command(ClearAllFiles, CanExecute);
            DeleteCurrentFileCommand = new Command(DeleteFile, CanExecute);
            SetFileAsFirstCommand = new Command(SetFileAsFirst, CanExecute);
            SetFileAsLastCommand = new Command(SetFileAsLast, CanExecute);
            MoveFileUpCommand = new Command(MoveFileUp, CanExecute);
            MoveFileDownCommand = new Command(MoveFileDown, CanExecute);
            _filesService = filesService;
        }

        public override bool CanExecute()
        {
            return !IsBusy;
        }

        public File? GetCurrentFile()
        {
            return CurrentFile;
        }

        public File? GetFileByIndex(int index)
        {
            if (GetFilesCount() < index)
            {
                return null;
            }

            return FilesCollection[index];
        }

        public int GetFilesCount()
        {
            if (FilesCollection == null)
            {
                return 0;
            }

            return FilesCollection.Any() ? FilesCollection.Count() : 0;
        }

        public int GetSelectedFileIndex()
        {
            return SelectedItemIndex;
        }

        public bool AddNewFiles(List<File> files)
        {
            if (files == null)
            {
                return false;
            }

            FilesCollection = new(FilesCollection.ToList().Concat(files));
            OnPropertyChanged(nameof(FilesCollection));

            if (FilesCollection != null && FilesCollection.Count > 0)
            {
                SelectedItemIndex = 0;
            }

            return true;
        }
        public bool AnyFiles()
        {
            return FilesCollection.Any();
        }

        public void Update(IEnumerable<File>? files = null, bool showFilesView = false)
        {
            ShowFilesView = showFilesView;
            FilesCollection = new ObservableCollection<File>(files)?.Copy();
            InitializeFiles();
        }

        public void ClearAllFiles() => DoAction(_filesService.ClearAllFiles);

        public void DeleteFile() => DoAction(_filesService.DeleteItem);

        public void SetFileAsFirst() => DoAction(_filesService.SetItemAsFirst);

        public void SetFileAsLast() => DoAction(_filesService.SetItemAsLast);

        public void MoveFileUp() => DoAction(_filesService.MoveItemUp);

        public void MoveFileDown() => DoAction(_filesService.MoveItemDown);

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

        private void NotifyFilesView(object o = null, EventArgs s = null)
        {
            FilesChanged?.Invoke(o, s);
        }

        private void InitializeFiles()
        {
            if (FilesCollection != null && FilesCollection.Count > 0)
            {
                SelectedItemIndex = 0;
            }

            OnPropertyChanged(nameof(FilesCollection));
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
    }
}
