using System;
using System.ComponentModel;

namespace CrytonCoreNext.Abstract
{
    public class ViewModelBase : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool InformationBar { get; set; }

        public void ShowInformationBar(bool show)
        {
            InformationBar = show;
            OnPropertyChanged(nameof(InformationBar));
        }

        public virtual void Dispose() { }
    }
}
