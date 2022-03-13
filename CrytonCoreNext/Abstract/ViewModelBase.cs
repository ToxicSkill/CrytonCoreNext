using CrytonCoreNext.ViewModels;
using System;
using System.ComponentModel;

namespace CrytonCoreNext.Abstract
{
    public class ViewModelBase : INotifyPropertyChanged, IDisposable
    {
        public ViewModelBase PopupViewModel { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool InformationBar { get; set; }

        public ViewModelBase()
        {
            PopupViewModel = new InformationPopupViewModel();
        }

        protected void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void PostPopup(string informationString)
        {
            PopupViewModel = new InformationPopupViewModel(informationString);
            ShowInformationBar(false);
            ShowInformationBar(true);
            OnPropertyChanged(nameof(PopupViewModel));
        }

        private void ShowInformationBar(bool show)
        {
            InformationBar = show;
            OnPropertyChanged(nameof(InformationBar));
        }

        public virtual void Dispose() { }
    }
}