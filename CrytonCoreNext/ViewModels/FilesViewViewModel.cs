﻿using CrytonCoreNext.Abstract;
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
    public class FilesViewViewModel : ViewModelBase
    {
        private bool _showFilesView = false;

        private int _selectedItemIndex = 0;

        private static readonly (bool result, int newIndex) DefaultResult = new (false, -1);

        private readonly IFilesManager _filesManager;

        private bool _fileChangeBlocker = false;

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
                if ((_selectedItemIndex != value || value == 0) && !_fileChangeBlocker && value != -1)
                {
                    _selectedItemIndex = value;
                    UpdateCurrentFile();
                    NotifyFilesView();
                    OnPropertyChanged(nameof(SelectedItemIndex));
                }
                ChangeShowFilesView();
            }
        }

        public ObservableCollection<File>? FilesView { get; private set; }

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
            ClearFilesCommand = new Command(ClearAllFiles, true);
            DeleteCurrentFileCommand = new Command(DeleteFile, true);
            SetFileAsFirstCommand = new Command(SetFileAsFirst, true);
            SetFileAsLastCommand = new Command(SetFileAsLast, true);
            MoveFileUpCommand = new Command(MoveFileUp, true);
            MoveFileDownCommand = new Command(MoveFileDown, true);
            _filesManager = filesManager;
        }

        public void Update(IEnumerable<File>? files = null, bool showFilesView = false)
        {
            ShowFilesView = showFilesView;
            FilesView = new ObservableCollection<File>(files)?.Copy();
            InitializeFiles();
        }

        public void ClearAllFiles()
        {
            _ = _filesManager.ClearAllFiles(FilesView);
            SelectedItemIndex = -1;
            OnPropertyChanged(nameof(FilesView));
        }

        public void DeleteFile()
        {
            _fileChangeBlocker = true;
            var results = CurrentFile != null ? _filesManager.DeleteItem(FilesView, CurrentFile.Guid) : DefaultResult;
            _fileChangeBlocker = false;
            SelectedItemIndex = results.newIndex;
            OnPropertyChanged(nameof(FilesView));
        }

        public void SetFileAsFirst()
        {
            _fileChangeBlocker = true;
            var results = CurrentFile != null ? _filesManager.SetItemAsFirst(FilesView, CurrentFile.Guid) : DefaultResult;
            _fileChangeBlocker = false;
            SelectedItemIndex = results.newIndex;
            OnPropertyChanged(nameof(FilesView));
            
        }

        public void SetFileAsLast()
        {
            _fileChangeBlocker = true;
            var results = CurrentFile != null ? _filesManager.SetItemAsLast(FilesView, CurrentFile.Guid) : DefaultResult;
            _fileChangeBlocker = false;
            SelectedItemIndex = results.newIndex;
            OnPropertyChanged(nameof(FilesView));
        }

        public void MoveFileUp()
        {
            _fileChangeBlocker = true;
            var results = CurrentFile != null ? _filesManager.MoveItemUp(FilesView, CurrentFile.Guid) : DefaultResult;
            _fileChangeBlocker = false;
            SelectedItemIndex = results.newIndex;
            OnPropertyChanged(nameof(FilesView));
            
        }

        public void MoveFileDown()
        {
            _fileChangeBlocker = true;
            var results = CurrentFile != null ? _filesManager.MoveItemDown(FilesView, CurrentFile.Guid) : DefaultResult;
            _fileChangeBlocker = false;
            SelectedItemIndex = results.newIndex;
            OnPropertyChanged(nameof(FilesView));            
        }

        private void NotifyFilesView(object o = null, EventArgs s = null)
        {
            FilesChanged?.Invoke(o, s);
        }


        private void InitializeFiles()
        {
            if (FilesView != null && FilesView.Count > 0)
            {
                SelectedItemIndex = 0;
            }
            OnPropertyChanged(nameof(FilesView));
        }

        private void UpdateCurrentFile()
        {
            if (SelectedItemIndex != -1 && FilesView.Count >= SelectedItemIndex + 1)
            {
                CurrentFile = FilesView.ElementAt(SelectedItemIndex);
            }
            else
            {
                CurrentFile = null;
            }
        }

        private void ChangeShowFilesView()
        {            
            if (FilesView?.Count == 0)
            {
                ShowFilesView = false;
            }
            OnPropertyChanged(nameof(ShowFilesView));
        }
    }
}
