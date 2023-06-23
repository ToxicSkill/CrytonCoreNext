using CommunityToolkit.Mvvm.ComponentModel;

namespace CrytonCoreNext.Abstract
{
    public partial class ViewModelBase : ObservableObject
    {
        [ObservableProperty]
        public string pageName = "";

        [ObservableProperty]
        public bool isBusy;

        public ViewModelBase(string name = "")
        {
            PageName = name;
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