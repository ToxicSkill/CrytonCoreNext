using CommunityToolkit.Mvvm.ComponentModel;
using CrytonCoreNext.Abstract;

namespace CrytonCoreNext.ViewModels.Settings
{
    public partial class AppearanceViewModel : ViewModelBase
    {
        [ObservableProperty]
        public bool isChecked;

        public AppearanceViewModel()
        {

        }

        partial void OnIsCheckedChanged(bool value)
        {
            var tt = false;
        }
    }
}
