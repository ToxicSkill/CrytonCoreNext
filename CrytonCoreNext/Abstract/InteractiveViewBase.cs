﻿using CrytonCoreNext.Enums;
using CrytonCoreNext.Helpers;
using CrytonCoreNext.Interfaces;
using CrytonCoreNext.Models;
using CrytonCoreNext.Services;
using CrytonCoreNext.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace CrytonCoreNext.Abstract
{
    public class InteractiveViewBase : ViewModelBase, IInteractiveFiles, IDisposable
    {
        private readonly IFilesManager _filesManager;

        private DispatcherTimer? _timer;

        protected int _lastSelectedItemIndex = 0;

        public InformationPopupViewModel PopupViewModel { get; set; }

        public FilesViewViewModel FilesViewViewModel { get; set; }

        public File? CurrentFile { get; set; }

        public Visibility FileInformationVisibility { get; set; } = Visibility.Hidden;

        public InteractiveViewBase(IFilesManager filesManager)
        {
            _filesManager = filesManager;
            PopupViewModel = new ();
            FilesViewViewModel = new ();

            NotifyObjectChangeByName(new List<string>()
            { 
                nameof(PopupViewModel),
                nameof(FilesViewViewModel)
            }.ToArray());
        }

        public void ClearAllFiles()
        {
            _filesManager.ClearAllFiles(FilesViewViewModel);
            UpdateFilesVisibility();
        }

        public void AddFiles()
        {
            WindowDialog.OpenDialog openDialog = new(new DialogHelper()
            {
                Filters = Enums.EDialogFilters.ExtensionToFilter(Enums.EDialogFilters.DialogFilters.All),
                Multiselect = true,
                Title = (string)(Application.Current as App).Resources.MergedDictionaries[0]["OpenFileDialog"]
            });
            var chosenPaths = openDialog.RunDialog();
            if (chosenPaths.Count > 0)
            {
                var filesCount = FilesViewViewModel.FilesView == null ? 0 : FilesViewViewModel.FilesView.Count;
                _lastSelectedItemIndex = FilesViewViewModel.SelectedItemIndex != -1 ? FilesViewViewModel.SelectedItemIndex : 0;
                var newFiles = _filesManager.AddFiles(chosenPaths.ToArray(), filesCount);
                var newFilesCollection = filesCount > 0 ?
                    FilesViewViewModel.FilesView?.ToList().Concat(newFiles) :
                    newFiles;

                UpdateFiles(newFilesCollection);
            }
        }

        public void PostPopup(string informationString, int seconds, Color color = default)
        {
            PopupViewModel = new (informationString, color);
            ShowInformationBar(true);
            OnPropertyChanged(nameof(PopupViewModel));
            InitializeTimerWithAction(CollapsePopup, seconds);
        }

        public void DeleteItem()
        {
            _filesManager.DeleteItem(FilesViewViewModel, CurrentFile.Guid);
            UpdateFilesView(FilesViewViewModel.FilesView);
        }

        public virtual void Dispose() { }

        private void UpdateFilesView(ObservableCollection<Models.File>? files = null)
        {
            FilesViewViewModel = new(files, FilesViewViewModel.ShowFilesView);
            FilesViewViewModel.PropertyChanged += SelectedItem_PropertyChanged;
            OnPropertyChanged(nameof(FilesViewViewModel));
            FilesViewViewModel.SelectedItemIndex = _lastSelectedItemIndex;
            UpdateFilesVisibility();
        }

        private void UpdateFiles(IEnumerable<File>? filesCollection)
        {
            if (filesCollection != null)
            {
                UpdateFilesView(new ObservableCollection<File>(filesCollection));
                PostPopup("File(s) where loaded successfuly", 5, EPopopColor.Information);
            }
            else
            {
                PostPopup("Error occured when loading file(s)", 5, EPopopColor.Error);
            }
        }

        private void UpdateFilesVisibility()
        {
            if (FilesViewViewModel.SelectedItemIndex != -1 && FilesViewViewModel != null)
            {
                if (FilesViewViewModel.FilesView.Count > 0)
                {
                    FileInformationVisibility = Visibility.Visible;
                }
                else
                {
                    FileInformationVisibility = Visibility.Hidden;
                    ShowFilesView(false);
                }
            }
            else
            {
                FileInformationVisibility = Visibility.Hidden;
            }
            OnPropertyChanged(nameof(FileInformationVisibility));
        }

        private void SelectedItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var tempSelectedItemIndex = FilesViewViewModel.SelectedItemIndex;
            if (tempSelectedItemIndex != -1 && FilesViewViewModel.FilesView.Count >= tempSelectedItemIndex + 1)
            {
                CurrentFile = FilesViewViewModel.FilesView.ElementAt(tempSelectedItemIndex);
            }
            OnPropertyChanged(nameof(CurrentFile));
        }

        private void ShowFilesView(bool show)
        {
            FilesViewViewModel.ShowFilesView = show;
            OnPropertyChanged(nameof(FilesViewViewModel.ShowFilesView));
        }

        private void ShowInformationBar(bool show)
        {
            PopupViewModel.ShowPopup = show;
            OnPropertyChanged(nameof(PopupViewModel.ShowPopup));
        }

        private void InitializeTimerWithAction(Action<object, EventArgs> obj, int seconds)
        {
            _timer = new();
            _timer.Tick += new EventHandler(obj);
            _timer.Interval = new TimeSpan(0, 0, seconds);
            _timer.Start();
        }

        private void CollapsePopup(object sender, EventArgs e)
        {
            ShowInformationBar(false);
            _timer?.Stop();
        }

        private void NotifyObjectChangeByName(string[] objects)
        {
            foreach (var obj in objects)
            {
                OnPropertyChanged(obj);
            }
        }
    }
}