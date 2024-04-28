using CommunityToolkit.Mvvm.ComponentModel;

namespace CrytonCoreNext.Abstract
{
    public partial class ViewModelBase : ObservableObject
    {
        [ObservableProperty]
        public string pageName = "";

        [ObservableProperty]
        public int progressValue;

        [ObservableProperty]
        public bool isBusy;

        public void UpdateProgress(double value)
        {
            ProgressValue = (int)(value * 100);
        }

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

        public static void SetSettings()
        {
            Properties.Settings.Default.Save();
        }
    }
}