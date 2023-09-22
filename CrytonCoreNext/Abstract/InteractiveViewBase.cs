using CrytonCoreNext.Dictionaries;
using CrytonCoreNext.Interfaces.Files;
using CrytonCoreNext.Models;
using System.Collections.Generic;
using System.Linq;
using Wpf.Ui.Common;
using Wpf.Ui.Mvvm.Contracts;

namespace CrytonCoreNext.Abstract
{
    public abstract class InteractiveViewBase : ViewModelBase
    {
        private readonly ISnackbarService _snackbarService;

        private readonly Interfaces.IDialogService _dialogService;

        private readonly IFileService _fileService;

        public InteractiveViewBase(IFileService fileService, Interfaces.IDialogService dialogService, ISnackbarService snackbarService)
        {
            _fileService = fileService;
            _dialogService = dialogService;
            _snackbarService = snackbarService;
        }

        protected void PostSuccessSnackbar(string text)
        {
            _snackbarService.Show("Success", text, SymbolRegular.CheckmarkCircle20, ControlAppearance.Success);
        }

        protected void PostErrorSnackbar(string text)
        {
            _snackbarService.Show("Error", text, SymbolRegular.ErrorCircle20, ControlAppearance.Dark);
        }

        protected void PostWarningSnackbar(string text)
        {
            _snackbarService.Show("Warning", text, SymbolRegular.Warning20, ControlAppearance.Caution);
        }

        protected async IAsyncEnumerable<File> LoadFiles()
        {
            await foreach (var file in LoadFiles(Static.Extensions.DialogFilters.All))
            {
                yield return file;
            }
        }

        protected string GetFileFromDialog(Static.Extensions.DialogFilters filters = Static.Extensions.DialogFilters.All)
        {
            var selectedFiles = _dialogService.GetFilesNamesToOpen(filters, Language.Post("OpenFiles"));
            if (selectedFiles.Any())
            {
                return selectedFiles.First();
            }
            return string.Empty;
        }

        protected async IAsyncEnumerable<File> LoadFiles(Static.Extensions.DialogFilters filters = Static.Extensions.DialogFilters.All)
        {
            var filesPaths = _dialogService.GetFilesNamesToOpen(filters, Language.Post("OpenFiles"), true);
            var loadedFilesCounter = 0;
            await foreach (var file in _fileService.LoadFiles(filesPaths))
            {
                if (file != null)
                {
                    loadedFilesCounter++;
                    yield return file;
                }
            }
            //if ((filesPaths.Count - loadedFilesCounter) > 0 && loadedFilesCounter > 0)
            //{
            //    PostSnackbar("Warning", $"{Language.Post("NotAllFilesLoadedWarning")} ({filesPaths.Count - loadedFilesCounter})", SymbolRegular.Warning20, ControlAppearance.Caution);
            //}
            //else if (loadedFilesCounter == 1)
            //{
            //    PostSnackbar("Information", Language.Post("FileLoaded"), SymbolRegular.Checkmark20, ControlAppearance.Success);
            //}
            //else if (loadedFilesCounter > 1)
            //{
            //    PostSnackbar("Information", Language.Post("FilesLoaded"), SymbolRegular.Checkmark20, ControlAppearance.Success);
            //}
            //else if (loadedFilesCounter == 0 && filesPaths.Any())
            //{
            //    PostSnackbar("Error", Language.Post("FilesLoadingError"), SymbolRegular.ErrorCircle20, ControlAppearance.Danger);
            //}
        }

        protected void SaveFile(File? file)
        {
            if (file == null)
            {
                return;
            }

            var filePath = _dialogService.GetFilesNamesToSave(Static.Extensions.DialogFilters.All, Language.Post("SaveFile"), file);
            if (filePath == null)
            {
                return;
            }
            var result = _fileService.SaveFile(filePath.First(), file);
            if (result)
            {
                PostSuccessSnackbar(Language.Post("FilesSaved"));
            }
            if (!result)
            {
                PostErrorSnackbar(Language.Post("FilesSavingError"));
            }
        }
    }
}