using CrytonCoreNext.Enums;
using CrytonCoreNext.Logger;
using System;
using System.Collections.Generic;

namespace CrytonCoreNext.Abstract
{
    public class ViewModelBase : NotificationBase
    {
        public string PageName { get; set; }

        public Log Logger { get; set; }

        public bool IsBusy;

        public ViewModelBase(string pageName = "")
        {
            Logger = new Log();
            PageName = pageName;
            Logger.OnLoggerChanged += NotifyLoggerChanged;
        }

        public virtual Dictionary<string, object> GetObjects()
        {
            return default;
        }

        public virtual void SetObjects(Dictionary<string, object> objects)
        {
            return;
        }

        public virtual bool CanExecute()
        {
            return !IsBusy;
        }

        public void Log(ELogLevel level, string message)
        {
            Logger.Post(level, message);
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
    }
}