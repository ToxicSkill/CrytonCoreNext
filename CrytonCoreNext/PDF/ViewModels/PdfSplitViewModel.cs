using CrytonCoreNext.Abstract;
using CrytonCoreNext.PDF.Abstarct;
using CrytonCoreNext.PDF.Interfaces;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CrytonCoreNext.PDF.ViewModels
{
    public class PdfSplitViewModel : PdfBase
    {
        private int _fromPage = 1;

        private int _toPage = 1;

        public int FromPage
        {
            get => _fromPage;
            set
            {
                var maxPage = CurrentFile.NumberOfPages + 1;
                if (_fromPage == value || value > maxPage) return;
                _fromPage = value;
                if (_fromPage == maxPage)
                {
                    ToPage = maxPage;
                }
                OnPropertyChanged(nameof(FromPage));
            }
        }

        public int ToPage
        {
            get => _toPage;
            set
            {
                if (_toPage == value || value < _fromPage || value > CurrentFile.NumberOfPages + 1) return;
                _toPage = value;
                OnPropertyChanged(nameof(ToPage));
            }
        }

        public Visibility SplitButtonVisibility { get; set; } = Visibility.Hidden;

        public ICommand SplitCommand { get; set; }

        public PdfSplitViewModel(InteractiveViewBase pdfManagerViewModel, IPDFService pdfService) : base(pdfManagerViewModel, pdfService)
        {
            OnUpdate += HandleUpdate;
        }

        private void HandleUpdate(object? sender, EventArgs e)
        {
            if (CurrentFile == null)
            {
                SplitButtonVisibility = Visibility.Hidden;
            }
            else
            {
                SplitButtonVisibility = Visibility.Visible;
                FromPage = 1;
                _toPage = CurrentFile.NumberOfPages + 1;
                OnPropertyChanged(nameof(ToPage));
            }
            OnPropertyChanged(nameof(SplitButtonVisibility));
        }

        private async Task Split()
        {
            if (CurrentFile != null)
            {
                var newFile = await PdfService.Split(CurrentFile, FromPage - 1, ToPage - 1, Files.Count + 1);
                PdfManagerViewModel.SendObject(newFile);
                ResetCurrentPage();
            }
        }
    }
}
