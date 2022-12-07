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
            PopupViewModel = new();
            FilesViewModel.CurrentFileChanged += UpdateFilesVisibility;
        }

        protected void PostPopup(string informationString, int seconds, Color color = default)
        {
            PopupViewModel = new(informationString, color);
            ShowInformationBar(true);
            OnPropertyChanged(nameof(PopupViewModel));
            ActionTimer.InitializeTimerWithAction(CollapsePopup, seconds);
        }

        protected void PostPopup(string message, Color status, int seconds = 2)
        {
            PostPopup(Language.Post(message), seconds, status);
        }

        protected async IAsyncEnumerable<File> LoadFiles()
        {
            await foreach (var file in LoadFiles(Static.Extensions.DialogFilters.All))
            {
                yield return file;
            }
        }

        protected async IAsyncEnumerable<File> LoadFiles(Static.Extensions.DialogFilters filters = Static.Extensions.DialogFilters.All)
        {
            var filesCount = FilesViewModel.GetFilesCount();
            var filesPaths = _dialogService.GetFilesNamesToOpen(filters, Language.Post("OpenFiles"), true);
            await foreach (var file in _fileService.LoadFiles(filesPaths, filesCount))
            {
                yield return file;
            }
        }

        protected List<Guid> GetFilesOrder()
        {
            var guids = new List<Guid>();

            foreach (var fileGuid in FilesViewModel.GetFilesGuids())
            {
                guids.Add(fileGuid);
            }

            return guids;
        }

        protected void SaveFile(File? file)
        {
            if (file == null)
            {
                return;
            }

            var filePath = _dialogService.GetFilesNamesToSave(Static.Extensions.DialogFilters.All, Language.Post("SaveFile"), file.Extension);
            if (filePath == null)
            {
                return;
            }
            var result = _fileService.SaveFile(filePath.First(), file);
            if (result)
            {
                PostPopup(Language.Post("FilesSaved"), 2, ColorStatus.Blue);
            }
            if (!result)
            {
                PostPopup(Language.Post("FilesError"), 2, ColorStatus.Red);
            }
        }

        private void UpdateFilesVisibility(object? obj = null, EventArgs? a = null)
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
        }

        public virtual void Dispose() { }
    }
}