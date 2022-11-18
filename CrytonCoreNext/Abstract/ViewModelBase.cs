using System.Collections.Generic;

namespace CrytonCoreNext.Abstract
{
    public class ViewModelBase : NotificationBase
    {
        public string PageName { get; set; }

        public bool IsBusy;

        public ViewModelBase(string pageName = "")
        {
            PageName = pageName;
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
    }
}