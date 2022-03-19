using CrytonCoreNext.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows.Threading;

namespace CrytonCoreNext.Abstract
{
    public class InteractiveViewBase : ViewModelBase, IDisposable
    {
        private DispatcherTimer? _timer;

        public InformationPopupViewModel PopupViewModel { get; set; }

        public FilesViewViewModel FilesViewViewModel { get; set; }

        public bool ShowPopup { get; set; }

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

        public void PostPopup(string informationString, TimeSpan delayTime)
        {
            PopupViewModel = new (informationString);
            ShowInformationBar(false);
            ShowInformationBar(true);
            OnPropertyChanged(nameof(PopupViewModel));
            InitializeTimerWithAction(CollapsePopup, delayTime);
        }

        public void UpdateFilesView(string txt)
        {
            FilesViewViewModel = new(txt);
            OnPropertyChanged(nameof(FilesViewViewModel));
        }

        private void ShowInformationBar(bool show)
        {
            ShowPopup = show;
            OnPropertyChanged(nameof(ShowPopup));
        }

        private void InitializeTimerWithAction(Action<object, EventArgs> obj, TimeSpan delayTime)
        {
            _timer = new();
            _timer.Tick += new EventHandler(obj);
            _timer.Interval = delayTime;
            _timer.Start();
        }

        private void CollapsePopup(object sender, EventArgs e)
        {
            ShowInformationBar(false);
            OnPropertyChanged(nameof(PopupViewModel));
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