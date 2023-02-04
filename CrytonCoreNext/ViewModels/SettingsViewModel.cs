using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrytonCoreNext.Abstract;
using CrytonCoreNext.Models;
using System.Collections.Generic;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using Wpf.Ui.Mvvm.Contracts;

namespace CrytonCoreNext.ViewModels
{
    public partial class SettingsViewModel : ViewModelBase
    {
        private readonly IThemeService _themeService;

        private List<UiViewElement<CardControl>> _elements;

        [ObservableProperty]
        public bool isThemeSwitchChecked = true;

        public SettingsViewModel(IThemeService themeService)
        {
            _themeService = themeService;
            _elements = new();
        }

        public void RegisterNewUiNavigableElement(CardControl obj, bool hasHeader, int headerHeight)
        {
            _elements.Add(new (obj, hasHeader, headerHeight));
        }

        public int GetYOffset(CardControl cardControl)
        {
            var offset = 50;
            foreach (var card in _elements)
            {
                if (card.Object == cardControl)
                {
                    break;
                }
                offset += card.Height+9;
                offset += card.HasHeader ? card.HeaderHeight + 21 : 2;
            }
            return offset;
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
