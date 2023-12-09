using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrytonCoreNext.Abstract;
using CrytonCoreNext.Models;
using CrytonCoreNext.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using Wpf.Ui.Appearance;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using Wpf.Ui.Mvvm.Contracts;

namespace CrytonCoreNext.ViewModels
{
    public partial class SettingsViewModel : ViewModelBase
    {
        private const int MinimalWindowsBuildNumber = 22523;

        private readonly IThemeService _themeService;

        private List<UiViewElement<CardControl>> _elements;

        private Dictionary<TreeViewItemModel, CardControl> _cardByTreeViewItem;

        private bool _lockTreeViewItem = false;

        private delegate void _verticalOffsetScrollUpdateDelegate(double offset);

        private _verticalOffsetScrollUpdateDelegate VerticalOffsetScrollUpdate;

        public delegate void OnThemeStyleChanged(BackgroundType value);

        public event OnThemeStyleChanged ThemeStyleChanged;

        [ObservableProperty]
        public ObservableCollection<BackgroundType> themeStylesItemsSource;

        [ObservableProperty]
        public BackgroundType selectedThemeStyle;

        [ObservableProperty]
        public ObservableCollection<TreeViewItemModel> treeViewItemSource;

        [ObservableProperty]
        public TreeViewItemModel selectedTreeViewItem;

        [ObservableProperty]
        public bool isThemeSwitchChecked = true;

        [ObservableProperty]
        public bool isFullscreenOnStart = false;

        [ObservableProperty]
        public bool isThemeStyleAvailable;

        [ObservableProperty]    
        public int pdfDpiValue;

        public bool MembersInitialized { get => TreeViewItemSource.Any(); }

        public SettingsViewModel(IThemeService themeService)
        {
            _themeService = themeService;
            _elements = [];
            _cardByTreeViewItem = [];

            TreeViewItemSource = [];

            InitializeThemes();
            InitializeSettings(); 
        }

        public void OnStartup()
        {
            if (Properties.Settings.Default.FirstRun)
            {
                Properties.Settings.Default.FirstRun = false;
                SelectedThemeStyle = BackgroundType.Mica;
            }
        }

        private void InitializeThemes()
        {
            ThemeStylesItemsSource = new ObservableCollection<BackgroundType>
                (
                    [
                        BackgroundType.Acrylic,
                        BackgroundType.Mica,
                        BackgroundType.Tabbed
                    ]
                );
            IsThemeStyleAvailable = WindowsAPIService.GetWindowsBuild() >= MinimalWindowsBuildNumber;
        }

        private void InitializeSettings()
        {
            IsFullscreenOnStart = Properties.Settings.Default.FullscreenOnStart;
            if (Enum.TryParse(Properties.Settings.Default.Style, out BackgroundType backgroundTypeStyle))
            {
                SelectedThemeStyle = backgroundTypeStyle;
            }
            else
            {
                SelectedThemeStyle = BackgroundType.Mica;
            }
            IsThemeSwitchChecked = Properties.Settings.Default.Theme;
            PdfDpiValue = Properties.Settings.Default.PdfRenderDpi;
            SetSettings();
        }

        partial void OnPdfDpiValueChanged(int value)
        {
            SetSettings();
        }

        partial void OnSelectedThemeStyleChanged(BackgroundType value)
        {
            ThemeStyleChanged?.Invoke(value);
            Properties.Settings.Default.Style = value.ToString();
            SetSettings();
        }

        partial void OnIsFullscreenOnStartChanged(bool value)
        {
            Properties.Settings.Default.FullscreenOnStart = value;
            SetSettings();
        }

        partial void OnIsThemeSwitchCheckedChanged(bool value)
        {
            _themeService.SetTheme(value ? ThemeType.Dark : ThemeType.Light);
            Properties.Settings.Default.Theme = value;
            SetSettings();
        }

        public void UpdateSelectedTreeViewItem(CardControl card)
        {
            var newSelectedItem = _cardByTreeViewItem.First(c => c.Value == card).Key;
            if (newSelectedItem.Childs != null)
            {
                newSelectedItem = newSelectedItem.Childs[0];
            }
            _lockTreeViewItem = true;
            SelectedTreeViewItem = newSelectedItem;
            _lockTreeViewItem = false;
        }

        public void SetVerticalScrollUpdateFunction(Action<double> update)
        {
            VerticalOffsetScrollUpdate = new(update);
        }

        partial void OnSelectedTreeViewItemChanged(TreeViewItemModel value)
        {
            if (!_lockTreeViewItem)
            {
                var newSelectedItem = value;
                if (value.Childs != null)
                {
                    newSelectedItem = value.Childs.First();
                }
                SelectedTreeViewItem = newSelectedItem;
                VerticalOffsetScrollUpdate.Invoke(GetYOffset(_cardByTreeViewItem[value]));
            }
        }

        public void RegisterNewUiNavigableElement(CardControl card, bool hasHeader, double headerHeight, string headerTitle, SymbolRegular mainItemSymbol)
        {
            _elements.Add(new(card, hasHeader, headerHeight));
            RegisterTreeViewItem(card, hasHeader, headerTitle, mainItemSymbol);
        }

        public double GetYOffset(CardControl cardControl)
        {
            var offset = 0d;
            foreach (var card in _elements)
            {
                if (card.Object == cardControl)
                {
                    break;
                }
                offset += card.Height;
                offset += card.HasHeader ? card.HeaderHeight + 20 : 0;
                offset += 8;
            }
            return offset;
        }

        private void UpdateTreeView(TreeViewItemModel selectedTreeViewItem)
        {
            foreach (var treeViewItem in TreeViewItemSource)
            {
                treeViewItem.IsExpanded = false;
                foreach (var subItem in treeViewItem.Childs)
                {
                    subItem.IsExpanded = false;
                    if (subItem == selectedTreeViewItem)
                    {
                        treeViewItem.IsExpanded = true;
                        subItem.IsExpanded = true;
                    }
                }
            }
        }

        private void RegisterTreeViewItem(CardControl card, bool hasHeader, string headerTitle, SymbolRegular mainItemSymbol)
        {
            if (hasHeader)
            {
                var newTreeViewItem = new TreeViewItemModel()
                {
                    Title = headerTitle,
                    Symbol = mainItemSymbol,
                    IsExpanded = true,
                    Childs = []
                };
                TreeViewItemSource.Add(newTreeViewItem);
                _cardByTreeViewItem.Add(newTreeViewItem, card);
            }
            var newSubTreeViewItem = new TreeViewItemModel()
            {
                Title = ((card.Header as StackPanel)!.Children[0] as TextBlock)!.Text,
                IsExpanded = true,
                Symbol = card.Icon
            };
            TreeViewItemSource.Last().Childs.Add(newSubTreeViewItem);
            _cardByTreeViewItem.Add(newSubTreeViewItem, card);
        }

        [RelayCommand]
        private void HandleHoveredPicureClicked()
        {
            IsThemeSwitchChecked = !IsThemeSwitchChecked;
        }
    }
}
