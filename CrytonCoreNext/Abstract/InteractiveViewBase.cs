using CrytonCoreNext.Enums;
using CrytonCoreNext.Interfaces;
using CrytonCoreNext.Models;
using CrytonCoreNext.ViewModels;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace CrytonCoreNext.Abstract
{
    public class InteractiveViewBase : ViewModelBase, IDisposable
    {
        private readonly IFilesManager _filesManager;

        private DispatcherTimer? _timer;

        public InformationPopupViewModel PopupViewModel { get; private set; }

        public FilesViewViewModel FilesViewViewModel { get; init; }

        public File? CurrentFile { get; private set; }

        public Visibility FileInformationVisibility { get; private set; } = Visibility.Hidden;

        public InteractiveViewBase(IFilesManager filesManager)
        {
            _filesManager = filesManager;
            PopupViewModel = new ();
            FilesViewViewModel = new (_filesManager);
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
            var newFiles = _filesManager.LoadFiles(EDialogFilters.DialogFilters.All, "Open files", true, filesCount);
            if (newFiles != null)
            {
                var newFilesCollection = filesCount > 0 ?
                    FilesViewViewModel.FilesView?.ToList().Concat(newFiles) :
                    newFiles;

                FilesViewViewModel.Update(newFilesCollection);
                PostPopup("File(s) where loaded successfuly", 2, EPopopColor.Information);
            }
        }

        public void SaveFile()
        {
            var result = _filesManager.SaveFile(EDialogFilters.DialogFilters.All, "Save file", CurrentFile);
            if (result)
            {
                PostPopup("File has been saved successfuly", 2, EPopopColor.Information);
            }
            if (!result)
            {
                PostPopup("Error when saving file", 2, EPopopColor.Error);
            }
        }

        public bool ModifyFile(byte[] bytes, bool status)
        {
            var result =_filesManager.ModifyFile(FilesViewViewModel.FilesView, FilesViewViewModel.CurrentFile.Guid, bytes, status);
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