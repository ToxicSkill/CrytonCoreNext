using CrytonCoreNext.Interfaces;
using CrytonCoreNext.Models;
using CrytonCoreNext.Services;
using CrytonCoreNext.Static;
using CrytonCoreNext.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace CrytonCoreNext.Abstract
{
    public class InteractiveViewBase : ViewModelBase, IDisposable
    {
        private readonly IFileService _fileService;
            
        private readonly IDialogService _dialogService;

        private DispatcherTimer? _timer;

        public InformationPopupViewModel PopupViewModel { get; private set; }

        public FilesViewViewModel FilesViewViewModel { get; init; }

        public File? CurrentFile { get; private set; }

        public Visibility FileInformationVisibility { get; private set; } = Visibility.Hidden;

        public InteractiveViewBase(IFileService fileService, IDialogService dialogService)
        {
            _fileService = fileService;
            _dialogService = dialogService;
            PopupViewModel = new ();
            FilesViewViewModel = new (_fileService);
            FilesViewViewModel.FilesChanged += HandleFileChanged;
        }

        public void PostPopup(string informationString, int seconds, Color color = default)
        {
            PopupViewModel = new (informationString, color);
            ShowInformationBar(true);
            OnPropertyChanged(nameof(PopupViewModel));
            InitializeTimerWithAction(CollapsePopup, seconds);
        }

        public void HandleFileChanged(object? sender, EventArgs? e)
        {
            CurrentFile = FilesViewViewModel.CurrentFile;
            UpdateFilesVisibility();
            OnPropertyChanged(nameof(CurrentFile));
        }

        public void LoadFiles()
        {
            var filesCount = FilesViewViewModel.FilesView == null ? 0 : FilesViewViewModel.FilesView.Count;
            var filesPaths = _dialogService.GetFilesNamesToOpen(Static.Extensions.DialogFilters.All, "Open files", true);
            var newFiles = _fileService.LoadFiles(filesPaths, filesCount);
            if (newFiles != null)
            {
                var newFilesCollection = filesCount > 0 ?
                    FilesViewViewModel.FilesView?.ToList().Concat(newFiles) :
                    newFiles;

                FilesViewViewModel.Update(newFilesCollection);
                PostPopup("File(s) where loaded successfuly", 2, ColorStatus.Information);
            }
        }

        public void SaveFile()
        {
            var filePath = _dialogService.GetFilesNamesToSave(Static.Extensions.DialogFilters.All, "Save file", CurrentFile.Extension);
            var result = _fileService.SaveFile(filePath.First(), CurrentFile);
            if (result)
            {
                PostPopup("File has been saved successfuly", 2, ColorStatus.Information);
            }
            if (!result)
            {
                PostPopup("Error when saving file", 2, ColorStatus.Error);
            }
        }

        public bool ModifyFile(ObservableCollection<File> files, Guid guid, byte[] bytes, bool status, string? methodName)
        {
            var result = _fileService.ModifyFile(files, guid, bytes, status, methodName);
            OnPropertyChanged(nameof(FilesViewViewModel.FilesView));
            OnPropertyChanged(nameof(CurrentFile));
            return result.result;
        }

        public virtual void Dispose() { }


        private void UpdateFilesVisibility()
        {
            FileInformationVisibility = 
                FilesViewViewModel.SelectedItemIndex != -1 && FilesViewViewModel != null ? 
                Visibility.Visible : 
                Visibility.Hidden;
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
    }
}