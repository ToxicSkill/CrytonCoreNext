using CrytonCoreNext.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Wpf.Ui.Common;
using Wpf.Ui.Common.Interfaces;
using Wpf.Ui.Controls;
using static iTextSharp.text.pdf.AcroFields;

namespace CrytonCoreNext.Views
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class SettingsView : INavigableView<SettingsViewModel>
    {
        private int _firstHeaderHeight;

        private List<SymbolRegular> _symbolsWithHeader;

        public SettingsViewModel ViewModel
        {
            get;
        }

        public SettingsView(SettingsViewModel viewModel)
        {
            _symbolsWithHeader = new ();
            ViewModel = viewModel;
            InitializeComponent();
            DataContext = ViewModel;
        }

        private static IEnumerable<T> FindVisualChilds<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) yield return (T)Enumerable.Empty<T>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject ithChild = VisualTreeHelper.GetChild(depObj, i);
                if (ithChild == null) continue;
                if (ithChild is T t) yield return t;
                foreach (T childOfChild in FindVisualChilds<T>(ithChild)) yield return childOfChild;
            }
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //this.defaultEncryptionSuffix.BringIntoView();
            
            scrollViewver.ScrollToVerticalOffset(ViewModel.GetYOffset(al));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            scrollViewver.ScrollToVerticalOffset(ViewModel.GetYOffset(tr));
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            scrollViewver.ScrollToVerticalOffset(ViewModel.GetYOffset(acc));
        }

        private void UiPage_Loaded(object sender, RoutedEventArgs e)
        {
            _firstHeaderHeight = (int)Math.Ceiling(firstHeader.ActualHeight);
            _symbolsWithHeader.Add(SymbolRegular.DarkTheme20);
            _symbolsWithHeader.Add(SymbolRegular.LocalLanguage20); 
            _symbolsWithHeader.Add(SymbolRegular.StarSettings20); 
            foreach (var cardControl in FindVisualChilds<CardControl>(stackPanel))
            {
                ViewModel.RegisterNewUiNavigableElement(cardControl, _symbolsWithHeader.Contains(cardControl.Icon), _firstHeaderHeight);
            }
        }

        private void UiPage_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            txt.Text = scrollViewver.VerticalOffset.ToString();
        }
    }
}