using CrytonCoreNext.ViewModels;
using System;
using System.ComponentModel;
using System.Windows.Threading;

namespace CrytonCoreNext.Abstract
{
    public class ViewModelBase : INotifyPropertyChanged, IDisposable
    {
        private DispatcherTimer _timer;

        public ViewModelBase PopupViewModel { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool InformationBar { get; set; }

        protected void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void PostPopup(string informationString, TimeSpan delayTime)
        {
            PopupViewModel = new InformationPopupViewModel(informationString);
            ShowInformationBar(false);
            ShowInformationBar(true);
            OnPropertyChanged(nameof(PopupViewModel));
            InitializeTimerWithAction(CollapsePopup, delayTime);
        }

        private void InitializeTimerWithAction(Action<object, EventArgs> obj, TimeSpan delayTime)
        {
            _timer = new();
            _timer.Tick += new EventHandler(obj);
            _timer.Interval = delayTime;
            _timer.Start();
        }

        private void ShowInformationBar(bool show)
        {
            InformationBar = show;
            OnPropertyChanged(nameof(InformationBar));
        }

        private void CollapsePopup(object sender, EventArgs e)
        {
            ShowInformationBar(false);
            OnPropertyChanged(nameof(PopupViewModel));
            _timer.Stop();
        }

        public virtual void Dispose() { }
    }
}