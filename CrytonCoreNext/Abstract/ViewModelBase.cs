using CrytonCoreNext.Enums;
using CrytonCoreNext.Logger;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            return new Dictionary<string, object>();
        }

        public virtual bool CanExecute()
        {
            return !IsBusy;
        }

        public void Log(ELogLevel level, string message)
        {
            Logger.Post(level, message).ContinueWith(OnAsyncFailed, TaskContinuationOptions.OnlyOnFaulted);
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