using CrytonCoreNext.Helpers;
using CrytonCoreNext.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Wpf.Ui.Controls;
using TextBlock = System.Windows.Controls.TextBlock;

namespace CrytonCoreNext.Views
{
    public partial class SettingsView : INavigableView<SettingsViewModel>
    {
        private double _firstHeaderHeight = 0d;

        private readonly Dictionary<SymbolRegular, SymbolRegular> _symbolBySymbolWithHeader;

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
            var headers = VisualHelper.FindVisualChilds<TextBlock>(stackPanel, false).ToList();
            ScrollViewer scrollViewer = GetScrollViewer(stackPanel);
            var verticalPositionByCard = new Dictionary<int, (CardControl card, bool hasHeader, double height, string text, SymbolRegular symbol)>();
            foreach (var cardControl in VisualHelper.FindVisualChilds<CardControl>(stackPanel, false))
            {
                if (cardControl.Icon is SymbolIcon icon)
                {
                    var hasHeader = _symbolBySymbolWithHeader.ContainsKey(icon.Symbol);
                    var headerText = hasHeader ? headers[_symbolBySymbolWithHeader.Keys.ToList().IndexOf(icon.Symbol)].Text : "";
                    var symbol = hasHeader ? _symbolBySymbolWithHeader[icon.Symbol] : icon.Symbol;
                    var transform = cardControl.TransformToVisual(scrollViewer);
                    var positionInScrollViewer = transform.Transform(new Point(0, 0));
                    verticalPositionByCard.Add((int)positionInScrollViewer.Y, (cardControl, hasHeader, _firstHeaderHeight, headerText, symbol));
                }
            }
            foreach (var cardExpander in VisualHelper.FindVisualChilds<CardExpander>(stackPanel, false))
            {
                if (cardExpander.Icon is SymbolIcon icon)
                {
                    var hasHeader = _symbolBySymbolWithHeader.ContainsKey(icon.Symbol);
                    var headerText = hasHeader ? headers[_symbolBySymbolWithHeader.Keys.ToList().IndexOf(icon.Symbol)].Text : "";
                    var symbol = hasHeader ? _symbolBySymbolWithHeader[icon.Symbol] : icon.Symbol;
                    var cardControl = new CardControl
                    {
                        Icon = icon,
                        Header = (cardExpander.Header as Grid).Children[1]
                    };

                    var transform = cardExpander.TransformToVisual(scrollViewer);
                    var positionInScrollViewer = transform.Transform(new Point(0, 0));
                    verticalPositionByCard.Add((int)positionInScrollViewer.Y, (cardControl, hasHeader, _firstHeaderHeight, headerText, symbol));
                }
            }
            foreach (var card in verticalPositionByCard.OrderBy(x => x.Key))
            {
                ViewModel.RegisterNewUiNavigableElement(card.Value.card, card.Value.hasHeader, card.Value.height, card.Value.text, card.Value.symbol);
            }
        }

        public static ScrollViewer GetScrollViewer(DependencyObject depObj)
        {
            var obj = depObj as ScrollViewer;
            if (obj != null) return obj;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                var result = GetScrollViewer(child);
                if (result != null) return result;
            }
            return null;
        }

        private void LoadHeadersSymbols()
        {
            _symbolBySymbolWithHeader.Add(SymbolRegular.DarkTheme20, SymbolRegular.Eye20);
            _symbolBySymbolWithHeader.Add(SymbolRegular.LocalLanguage20, SymbolRegular.ReadAloud20);
            _symbolBySymbolWithHeader.Add(SymbolRegular.StarSettings20, SymbolRegular.Rocket20);
            _symbolBySymbolWithHeader.Add(SymbolRegular.PreviewLink20, SymbolRegular.DocumentPdf20);
            _symbolBySymbolWithHeader.Add(SymbolRegular.Connector20, SymbolRegular.CameraDome20);
        }

        private void stackPanel_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.Source is CardControl card)
            {
                ViewModel.UpdateSelectedTreeViewItem(card);
            }
            if (e.Source is CardExpander cardExpander)
            {
                var cardControl = new CardControl
                {
                    Icon = cardExpander.Icon
                };
                ViewModel.UpdateSelectedTreeViewItem(cardControl);
            }
        }
    }
}