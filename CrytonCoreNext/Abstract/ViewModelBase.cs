using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace CrytonCoreNext.Abstract
{
    [ObservableObject]
    public partial class ViewModelBase
    {
        [ObservableProperty]
        public string pageName;

        [ObservableProperty]
        public bool isBusy;

        public ViewModelBase(string name = "")
        {
            this.PageName = name;
        }

        public virtual Dictionary<string, object> GetObjects()
        {
            return new Dictionary<string, object>();
        }

        public virtual bool CanExecute()
        {
            return !IsBusy;
        }

        public virtual void SendObject(object obj)
        {

        }


        public virtual void Lock()
        {
            IsBusy = true;
        }

        public virtual void Unlock()
        {
            IsBusy = false;
        }
    }
}