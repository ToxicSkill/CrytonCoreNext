using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        private void NotifyLoggerChanged(object? o, EventArgs? e)
        {
            OnPropertyChanged(nameof(Logger));
        }

        private static void OnAsyncFailed(Task task)
        {
            if (task != null)
            {
                var ex = task.Exception;
                Console.Write(ex?.Message);
            }
        }
    }
}