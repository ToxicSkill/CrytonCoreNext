using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrytonCoreNext.Abstract;
using Wpf.Ui.Appearance;
using Wpf.Ui.Mvvm.Contracts;

namespace CrytonCoreNext.ViewModels
{
    public partial class SettingsViewModel : ViewModelBase
    {
        private readonly IThemeService _themeService;

        [ObservableProperty]
        public bool isThemeSwitchChecked = true;

        public SettingsViewModel(IThemeService themeService)
        {
            _themeService = themeService;
        }

        partial void OnIsThemeSwitchCheckedChanged(bool value)
        {
            _themeService.SetTheme(value ? ThemeType.Dark : ThemeType.Light);
        }

        [RelayCommand]
        private void HandleHoveredPicureClicked()
        {
            IsThemeSwitchChecked = !IsThemeSwitchChecked;
        }
    }
}
