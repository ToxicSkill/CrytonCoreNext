using CrytonCoreNext.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Wpf.Ui.Controls;
using TextBlock = Wpf.Ui.Controls.TextBlock;

namespace CrytonCoreNext.Views
{
    public partial class SettingsView : INavigableView<SettingsViewModel>
    {
        private double _firstHeaderHeight = 0d;

        private readonly Dictionary<IconElement, SymbolRegular> _symbolBySymbolWithHeader;

        public SettingsViewModel ViewModel
        {
            get;
        }

        public SettingsView(SettingsViewModel viewModel)
        {
            _symbolBySymbolWithHeader = [];
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
            LoadHeadersSymbols();
            var headers = FindVisualChilds<TextBlock>(stackPanel, false).ToList();
            foreach (var cardControl in FindVisualChilds<CardControl>(stackPanel, false))
            {
                var hasHeader = _symbolBySymbolWithHeader.ContainsKey(cardControl.Icon);
                var headerText = hasHeader ? headers[_symbolBySymbolWithHeader.Keys.ToList().IndexOf(cardControl.Icon)].Text : "";
                SymbolRegular? symbol = hasHeader ? _symbolBySymbolWithHeader[cardControl.Icon] : null;
                ViewModel.RegisterNewUiNavigableElement(cardControl, hasHeader, _firstHeaderHeight, headerText, symbol);
            }
        }

        private void LoadHeadersSymbols()
        {
            _symbolBySymbolWithHeader.Add(new SymbolIcon(SymbolRegular.DarkTheme20), SymbolRegular.Eye20);
            _symbolBySymbolWithHeader.Add(new SymbolIcon(SymbolRegular.LocalLanguage20), SymbolRegular.ReadAloud20);
            _symbolBySymbolWithHeader.Add(new SymbolIcon(SymbolRegular.StarSettings20), SymbolRegular.Rocket20);
            _symbolBySymbolWithHeader.Add(new SymbolIcon(SymbolRegular.PreviewLink20), SymbolRegular.DocumentPdf20);
            _symbolBySymbolWithHeader.Add(new SymbolIcon(SymbolRegular.Connector20), SymbolRegular.CameraDome20);
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