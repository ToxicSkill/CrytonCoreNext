using CrytonCoreNext.Dictionaries;
using CrytonCoreNext.Interfaces.Files;
using CrytonCoreNext.Models;
using CrytonCoreNext.Services;
using CrytonCoreNext.Sound;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using Wpf.Ui.Common;
using Wpf.Ui.Mvvm.Contracts;
using Wpf.Ui.Mvvm.Services;

namespace CrytonCoreNext.Abstract
{
    public abstract class InteractiveViewBase(IFileService fileService, ISnackbarService snackbarService, CrytonCoreNext.Services.DialogService dialogService) : ViewModelBase
    {
        protected void PostSuccessSnackbar(string text)
        {
            snackbarService.Show("Success", text, SymbolRegular.CheckmarkCircle20, ControlAppearance.Success);
            NotificationPlayer.PlayNotificationSound();
        }

        protected void PostErrorSnackbar(string text)
        {
            snackbarService.Show("Error", text, SymbolRegular.ErrorCircle20, ControlAppearance.Dark);
        }

        protected void PostWarningSnackbar(string text)
        {
            snackbarService.Show("Warning", text, SymbolRegular.Warning20, ControlAppearance.Caution);
            SystemSounds.Exclamation.Play();
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
            return dialogService.GetFileNameToOpen(filters, Environment.SpecialFolder.Recent); 
        }

        protected async IAsyncEnumerable<File> LoadFiles(Static.Extensions.DialogFilters filters = Static.Extensions.DialogFilters.All)
        {
            var filesPaths = dialogService.GetFilesNamesToOpen(filters, Environment.SpecialFolder.Desktop);
            var loadedFilesCounter = 0;
            await foreach (var file in fileService.LoadFiles(filesPaths))
            {
                if (file != null)
                {
                    loadedFilesCounter++;
                    yield return file;
                }
            }
        }

        protected void SaveFile(File? file)
        {
            if (file == null)
            {
                return;
            }

            var filePath = dialogService.GetFileNameToSave(file.Extension, System.Environment.SpecialFolder.Recent);
            if (filePath == string.Empty)
            {
                PostErrorSnackbar(Language.Post("FilesSavingError"));
            }
            var result = fileService.SaveFile(filePath, file);
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