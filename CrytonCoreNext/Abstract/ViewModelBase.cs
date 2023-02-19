using CommunityToolkit.Mvvm.ComponentModel;

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

        public virtual bool CanExecute()
        {
            return !IsBusy;
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