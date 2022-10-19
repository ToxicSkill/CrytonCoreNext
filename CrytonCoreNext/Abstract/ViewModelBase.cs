using System.Collections.Generic;

namespace CrytonCoreNext.Abstract
{
    public class ViewModelBase : NotificationBase
    {
        public string? PageName { get; set; }

        public ViewModelBase(string? pageName = null)
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
    }
}