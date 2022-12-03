﻿using CrytonCoreNext.Abstract;
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

        private int _selectedItemIndex = -1;

        private static readonly (bool result, int newIndex) DefaultResult = new(false, -1);

        private readonly IFilesManager _filesManager;

        private Guid _deletedFileGuid = Guid.Empty;

        public ObservableCollection<File> FilesCollection { get; private set; }

        public event EventHandler CurrentFileChanged;

        public event EventHandler FileDeleted;

        public event EventHandler AllFilesDeleted;

        public Guid CurrentFileGuid { get; set; }

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
                if (_selectedItemIndex != value && !IsBusy)
                {
                    _selectedItemIndex = value;
                    UpdateCurrentFile();
                    ChangeShowFilesView();
                    OnPropertyChanged(nameof(SelectedItemIndex));
                }
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

        public Guid GetCurrentFileGuid()
        {
            return CurrentFileGuid;
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

        public List<Guid> GetFilesGuids()
        {
            return FilesCollection.Select(x => x.Guid).ToList();
        }

        public int GetSelectedFileIndex()
        {
            return SelectedItemIndex;
        }

        public void ClearAllFiles()
        {
            _deletedFileGuid = Guid.Empty;
            DoAction(_filesManager.ClearAllFiles);
            AllFilesDeleted.Invoke(null, null);
        }

        public void DeleteFile()
        {
            if (FilesCollection.Count == 1)
            {
                ClearAllFiles();
                return;
            }

            _deletedFileGuid = CurrentFileGuid;
            DoAction(_filesManager.DeleteItem);
            FileDeleted.Invoke(null, null);
        }

        public void SetFileAsFirst()
        {
            if (!IsItemFirst())
            {
                DoAction(_filesManager.SetItemAsFirst);
            }
        }

        public void MoveFileUp()
        {
            if (!IsItemFirst())
            {
                DoAction(_filesManager.MoveItemUp);
            }
        }

        public void SetFileAsLast()
        {
            if (!IsItemLast())
            {
                DoAction(_filesManager.SetItemAsLast);
            }
        }

        public void MoveFileDown()
        {
            if (!IsItemLast())
            {
                DoAction(_filesManager.MoveItemDown);
            }
        }

        private bool IsItemLast()
        {
            return SelectedItemIndex == FilesCollection.Count -1;
        }

        private bool IsItemFirst()
        {
            return SelectedItemIndex == 0;
        }

        private void NotifyCurrentFileChanged(object? o = null, EventArgs? s = null)
        {
            CurrentFileChanged?.Invoke(o, s);
        }

        private void UpdateCurrentFile()
        {
            if (SelectedItemIndex != -1 && FilesCollection.Count >= SelectedItemIndex + 1)
            {
                CurrentFileGuid = FilesCollection.ElementAt(SelectedItemIndex).Guid;
            }
            else
            {
                CurrentFileGuid = Guid.Empty;
            }

            NotifyCurrentFileChanged();
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

            Lock();
            var (result, newIndex) = !CurrentFileGuid.Equals(Guid.Empty) ? function(FilesCollection, CurrentFileGuid) : DefaultResult;
            Unlock();
            if (result)
            {
                SelectedItemIndex = newIndex;
                Lock();
                OnPropertyChanged(nameof(FilesCollection));
                Unlock();
            }

            UpdateCurrentFile();
        }
    }
}
