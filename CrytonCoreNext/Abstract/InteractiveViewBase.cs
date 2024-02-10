using CrytonCoreNext.Dictionaries;
using CrytonCoreNext.Interfaces.Files;
using CrytonCoreNext.Models;
using CrytonCoreNext.Sound;
using System;
using System.Collections.Generic;
using System.Media;
using Wpf.Ui;
using Wpf.Ui.Controls;


namespace CrytonCoreNext.Abstract
{
    public abstract class InteractiveViewBase(IFileService fileService, ISnackbarService snackbarService, CrytonCoreNext.Services.DialogService dialogService) : ViewModelBase
    {
        protected void PostSuccessSnackbar(string text)
        {
            snackbarService.Show("Success", text, ControlAppearance.Success, new SymbolIcon(SymbolRegular.CheckmarkCircle20), TimeSpan.FromSeconds(2));
            NotificationPlayer.PlayNotificationSound();
        }

        protected void PostErrorSnackbar(string text)
        {
            snackbarService.Show("Error", text, ControlAppearance.Danger, new SymbolIcon(SymbolRegular.ErrorCircle20), TimeSpan.FromSeconds(2));
        }

        protected void PostWarningSnackbar(string text)
        {
            snackbarService.Show("Warning", text, ControlAppearance.Caution, new SymbolIcon(SymbolRegular.Warning20), TimeSpan.FromSeconds(2));
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

            var filePath = dialogService.GetFileNameToSave(file.Extension, Environment.SpecialFolder.Recent);
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