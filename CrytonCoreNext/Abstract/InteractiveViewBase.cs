using CrytonCoreNext.Helpers;
using CrytonCoreNext.Interfaces;
using CrytonCoreNext.Models;
using CrytonCoreNext.Static;
using CrytonCoreNext.ViewModels;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace CrytonCoreNext.Abstract
{
    public class InteractiveViewBase : ViewModelBase, IDisposable
    {
        protected readonly IFileService _fileService;

        protected readonly IDialogService _dialogService;

        public InformationPopupViewModel PopupViewModel { get; private set; }

        public IProgressView ProgressViewModel { get; init; }

        public IFilesView FilesViewModel { get; init; }

        public File? CurrentFile { get; private set; }

        public Visibility FileInformationVisibility { get; private set; } = Visibility.Hidden;

        public InteractiveViewBase(IFileService fileService, IDialogService dialogService, IFilesView filesView, IProgressView progressView)
        {
            _fileService = fileService;
            _dialogService = dialogService;
            ProgressViewModel = progressView;
            FilesViewModel = filesView;
            PopupViewModel = new();
            FilesViewModel.FilesChanged += HandleFileChanged;
        }

        public void PostPopup(string informationString, int seconds, Color color = default)
        {
            PopupViewModel = new(informationString, color);
            ShowInformationBar(true);
            OnPropertyChanged(nameof(PopupViewModel));
            ActionTimer.InitializeTimerWithAction(CollapsePopup, seconds);
        }

        public void HandleFileChanged(object? sender, EventArgs? e)
        {
            CurrentFile = FilesViewModel.GetCurrentFile();
            UpdateFilesVisibility();
            OnPropertyChanged(nameof(CurrentFile));
        }

        public void LoadFiles()
        {
            var filesCount = FilesViewModel.GetFilesCount();
            var filesPaths = _dialogService.GetFilesNamesToOpen(Static.Extensions.DialogFilters.All, Application.Current.Resources.MergedDictionaries[0]["OpenFiles"].ToString() ?? string.Empty, true);
            var newFiles = _fileService.LoadFiles(filesPaths, filesCount);
            if (FilesViewModel.AddNewFiles(newFiles))
            {
                PostPopup(Application.Current.Resources.MergedDictionaries[0]["FilesLoaded"].ToString() ?? string.Empty, 2, ColorStatus.Information);
            }
        }

        public void SaveFile()
        {
            var filePath = _dialogService.GetFilesNamesToSave(Static.Extensions.DialogFilters.All, Application.Current.Resources.MergedDictionaries[0]["SaveFile"].ToString() ?? string.Empty, CurrentFile.Extension);
            var result = _fileService.SaveFile(filePath.First(), CurrentFile);
            if (result)
            {
                PostPopup(Application.Current.Resources.MergedDictionaries[0]["FilesSaved"].ToString() ?? string.Empty, 2, ColorStatus.Information);
            }
            if (!result)
            {
                PostPopup(Application.Current.Resources.MergedDictionaries[0]["FilesError"].ToString() ?? string.Empty, 2, ColorStatus.Error);
            }
        }

        public bool ModifyFile(File file, byte[] bytes, CryptingStatus.Status status, string? methodName)
        {
            var result = _fileService.ModifyFile(file, bytes, status, methodName);
            OnPropertyChanged(nameof(CurrentFile));
            return result.result;
        }

        private void UpdateFilesVisibility()
        {
            FileInformationVisibility =
                FilesViewModel.GetSelectedFileIndex() != -1 && FilesViewModel != null ?
                Visibility.Visible :
                Visibility.Hidden;
            OnPropertyChanged(nameof(FileInformationVisibility));
        }

        private void SelectedItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var tempSelectedItemIndex = FilesViewModel.GetSelectedFileIndex();
            if (tempSelectedItemIndex != -1 && FilesViewModel.GetFilesCount() >= tempSelectedItemIndex + 1)
            {
                CurrentFile = FilesViewModel.GetFileByIndex(tempSelectedItemIndex);
            }
            OnPropertyChanged(nameof(CurrentFile));
        }

        private void ShowInformationBar(bool show)
        {
            PopupViewModel.ShowPopup = show;
            OnPropertyChanged(nameof(PopupViewModel.ShowPopup));
        }

        private void CollapsePopup(object sender, EventArgs e)
        {
            ShowInformationBar(false);
            ActionTimer.StopTimer();
        }

        public virtual void Dispose() { }
    }
}