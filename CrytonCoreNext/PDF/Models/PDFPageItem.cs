using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrytonCoreNext.Abstract;
using System;
using Wpf.Ui.Common;
using Wpf.Ui.Common.Interfaces;
using static CrytonCoreNext.Helpers.Delegate;

namespace CrytonCoreNext.PDF.Models
{
    [ObservableObject]
    public partial class PDFPageItem
    {
        public INavigableView<ViewModelBase> View { get; set; }

        public SymbolRegular Icon { get; set; }

        public string ShortDescription { get; set; }

        public string Description { get; set; }

        public Type Type { get; set; }

        public string Title { get => View.ViewModel.PageName; }

        private NavigationDelegate Delegate;

        public PDFPageItem(NavigationDelegate action)
        {
            Delegate = action;
        }

        [RelayCommand]
        private void Clicked()
        {
            Delegate(View);
        }
    }
}
