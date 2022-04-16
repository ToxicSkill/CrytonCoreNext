using CrytonCoreNext.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;
using System.Windows.Threading;

namespace CrytonCoreNext.Abstract
{
    public class InteractiveViewBase : ViewModelBase, IDisposable
    {
        private DispatcherTimer? _timer;

        public InformationPopupViewModel PopupViewModel { get; set; }

        public FilesViewViewModel FilesViewViewModel { get; set; }

        public string FileSize { get; set; }

        public InteractiveViewBase()
        {
            PopupViewModel = new ();
            FilesViewViewModel = new ();

            NotifyObjectChangeByName(new List<string>()
            { 
                nameof(PopupViewModel),
                nameof(FilesViewViewModel)
            }.ToArray());
        }

        public void PostPopup(string informationString, int seconds, Color color = default)
        {
            PopupViewModel = new (informationString, color);
            ShowInformationBar(true);
            OnPropertyChanged(nameof(PopupViewModel));
            InitializeTimerWithAction(CollapsePopup, seconds);
        }

        public void UpdateFilesView(ObservableCollection<Models.File>? files = null)
        {
            FilesViewViewModel = new (files);
            FilesViewViewModel.PropertyChanged += SelectedItem_PropertyChanged;
            OnPropertyChanged(nameof(FilesViewViewModel));
        }

        private void SelectedItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            FileSize = FilesViewViewModel.FilesView.ElementAt(FilesViewViewModel.SelectedItemIndex).Size;
            OnPropertyChanged(nameof(FileSize));
        }

        public void ShowFilesView(bool show)
        {
            FilesViewViewModel.ShowFilesView = show;
        }

        private void ShowInformationBar(bool show)
        {
            PopupViewModel.ShowPopup = show;
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

        public virtual void Dispose() { }
    }
}