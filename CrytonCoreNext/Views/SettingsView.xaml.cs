using CrytonCoreNext.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Wpf.Ui.Common;
using Wpf.Ui.Common.Interfaces;
using Wpf.Ui.Controls;

namespace CrytonCoreNext.Views
{
    public partial class SettingsView : INavigableView<SettingsViewModel>
    {
        private double _firstHeaderHeight = 0d;

        private Dictionary<SymbolRegular, SymbolRegular> _symbolBySymbolWithHeader;

        public SettingsViewModel ViewModel
        {
            get;
        }

        public SettingsView(SettingsViewModel viewModel)
        {
            _symbolBySymbolWithHeader = new ();
            ViewModel = viewModel;
            InitializeComponent();
            DataContext = ViewModel;
            ViewModel.SetVerticalScrollUpdateFunction(SetScrollVerticalOffset);
        }

        private static IEnumerable<T> FindVisualChilds<T>(DependencyObject depObj, bool deeper) where T : DependencyObject
        {
            if (depObj == null) yield return (T)Enumerable.Empty<T>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject ithChild = VisualTreeHelper.GetChild(depObj, i);
                if (ithChild == null) continue;
                if (ithChild is T t) yield return t;
                if (deeper)
                {
                    foreach (T childOfChild in FindVisualChilds<T>(ithChild, deeper)) yield return childOfChild;
                }
            }
        }

        private void SetScrollVerticalOffset(double offset)
        {
            scrollViewer.ScrollToVerticalOffset(offset);
        }

        private void UiPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (ViewModel.MembersInitialized)
            {
                return;
            }
            _firstHeaderHeight = firstHeader.ActualHeight;
            _symbolBySymbolWithHeader.Add(SymbolRegular.DarkTheme20, SymbolRegular.Eye20);
            _symbolBySymbolWithHeader.Add(SymbolRegular.LocalLanguage20, SymbolRegular.ReadAloud20);
            _symbolBySymbolWithHeader.Add(SymbolRegular.StarSettings20, SymbolRegular.Rocket20);
            var headers = FindVisualChilds<TextBlock>(stackPanel, false).ToList();
            foreach (var cardControl in FindVisualChilds<CardControl>(stackPanel, false))
            {
                var hasHeader = _symbolBySymbolWithHeader.ContainsKey(cardControl.Icon);
                var headerText = hasHeader ? headers[_symbolBySymbolWithHeader.Keys.ToList().IndexOf(cardControl.Icon)].Text : "";
                var symbol = hasHeader ? _symbolBySymbolWithHeader[cardControl.Icon] : cardControl.Icon;
                ViewModel.RegisterNewUiNavigableElement(cardControl, hasHeader, _firstHeaderHeight, headerText, symbol);
            } 
        }

        private void stackPanel_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.Source is CardControl card)
            {
                ViewModel.UpdateSelectedTreeViewItem(card);
            }
        }
    }
}