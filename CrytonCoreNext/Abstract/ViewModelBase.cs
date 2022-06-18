using System.Collections.Generic;
using System.ComponentModel;

namespace CrytonCoreNext.Abstract
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual Dictionary<string, object> GetObjects()
        {
            return default;
        }
    }
}