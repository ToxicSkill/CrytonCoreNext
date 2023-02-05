using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrytonCoreNext.Abstract;
using CrytonCoreNext.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using Wpf.Ui.Appearance;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using Wpf.Ui.Mvvm.Contracts;
using Wpf.Ui.Mvvm.Interfaces;

namespace CrytonCoreNext.ViewModels
{
    public partial class SettingsViewModel : ViewModelBase
    {
        private readonly IThemeService _themeService;

        private List<UiViewElement<CardControl>> _elements;

        private Dictionary<TreeViewItemModel, CardControl> _cardByTreeViewItem;

        private bool _lockTreeViewItem = false;

        private delegate void _verticalOffsetScrollUpdateDelegate(double offset);

        private _verticalOffsetScrollUpdateDelegate VerticalOffsetScrollUpdate;

        [ObservableProperty]
        public ObservableCollection<TreeViewItemModel> treeViewItemSource;

        [ObservableProperty]
        public TreeViewItemModel selectedTreeViewItem;

        [ObservableProperty]
        public bool isThemeSwitchChecked = true;

        public bool MembersInitialized { get => TreeViewItemSource.Any(); }

        public SettingsViewModel(IThemeService themeService)
        {
            _themeService = themeService;
            _elements = new();
            _cardByTreeViewItem = new();
            TreeViewItemSource = new();
        }

        public void UpdateSelectedTreeViewItem(CardControl card)
        {
            var newSelectedItem = _cardByTreeViewItem.First(c => c.Value == card).Key;
            if (newSelectedItem.Childs != null)
            {
                newSelectedItem = newSelectedItem.Childs[0];
            }
            if (SelectedTreeViewItem != newSelectedItem)
            {
                _lockTreeViewItem = true;
                SelectedTreeViewItem = newSelectedItem;
                _lockTreeViewItem = false;
                UpdateTreeView(SelectedTreeViewItem);
            }
        }

        public void SetVerticalScrollUpdateFunction(Action<double> update)
        {
            VerticalOffsetScrollUpdate = new (update);
        }

        partial void OnSelectedTreeViewItemChanged(TreeViewItemModel value)
        {
            if (!_lockTreeViewItem)
            {
                VerticalOffsetScrollUpdate.Invoke(GetYOffset(_cardByTreeViewItem[value]));
            }
        }

        public void RegisterNewUiNavigableElement(CardControl card, bool hasHeader, double headerHeight, string headerTitle, SymbolRegular mainItemSymbol)
        {
            _elements.Add(new (card, hasHeader, headerHeight));
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
            var tempTreeView = new ObservableCollection<TreeViewItemModel>();
            foreach (var treeViewItem in TreeViewItemSource)
            {
                treeViewItem.IsExpanded = false;
                foreach (var subItems in treeViewItem.Childs)
                {
                    subItems.IsSelected = false;
                    if (subItems == selectedTreeViewItem)
                    {
                        subItems.IsSelected = true;
                        treeViewItem.IsExpanded = true;
                    }
                }
                tempTreeView.Add(treeViewItem);
            }
            TreeViewItemSource.Clear();
            TreeViewItemSource = tempTreeView;
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
                    Childs = new()
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
