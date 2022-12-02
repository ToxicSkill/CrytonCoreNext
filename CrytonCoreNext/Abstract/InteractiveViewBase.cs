using CrytonCoreNext.Dictionaries;
using CrytonCoreNext.Helpers;
using CrytonCoreNext.Interfaces;
using CrytonCoreNext.Models;
using CrytonCoreNext.Static;
using CrytonCoreNext.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace CrytonCoreNext.Abstract
{
    public abstract class InteractiveViewBase : ViewModelBase, IDisposable
    {
        protected readonly IDialogService _dialogService;

        protected readonly IFileService _fileService;

        public InformationPopupViewModel PopupViewModel { get; private set; }

        public IProgressView ProgressViewModel { get; init; }

        public IFilesView FilesViewModel { get; init; }

        public Visibility FileInformationVisibility { get; private set; } = Visibility.Hidden;

        public InteractiveViewBase(IFileService fileService, IDialogService dialogService, IFilesView filesView, IProgressView progressView)
        {
            _fileService = fileService;
            _dialogService = dialogService;
            ProgressViewModel = progressView;
            FilesViewModel = filesView;
            FilesViewModel.CurrentFileChanged += HandleFileChanged;
            PopupViewModel = new();
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
            UpdateFilesVisibility();
        }

        public List<File> LoadFiles()
        {
            return LoadFiles(Static.Extensions.DialogFilters.All);
        }

        public List<File> LoadFiles(Static.Extensions.DialogFilters filters = Static.Extensions.DialogFilters.All)
        {
            var filesCount = FilesViewModel.GetFilesCount();
            var filesPaths = _dialogService.GetFilesNamesToOpen(filters, Language.Post("OpenFiles"), true);
            return _fileService.LoadFiles(filesPaths, filesCount);
        }

        public List<Guid> GetFilesOrder()
        {
            var guids = new List<Guid>();

            foreach (var file in FilesViewModel.GetFiles())
            {
                guids.Add(file.Guid);
            }

            return guids;
        }

        public void PostPopup(string message, Color status, int seconds = 2)
        {
            PostPopup(Language.Post(message), seconds, status);
        }

        public void SaveFile(File? file)
        {
            if (file == null)
            {
                return;
            }

            var filePath = _dialogService.GetFilesNamesToSave(Static.Extensions.DialogFilters.All, Language.Post("SaveFile"), file.Extension);
            var result = _fileService.SaveFile(filePath.First(), file);
            if (result)
            {
                PostPopup(Language.Post("FilesSaved"), 2, ColorStatus.Information);
            }
            if (!result)
            {
                PostPopup(Language.Post("FilesError"), 2, ColorStatus.Error);
            }
        }

        private void UpdateFilesVisibility()
        {
            FileInformationVisibility =
                FilesViewModel.GetSelectedFileIndex() != -1 && FilesViewModel != null ?
                Visibility.Visible :
                Visibility.Hidden;
            OnPropertyChanged(nameof(FileInformationVisibility));
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