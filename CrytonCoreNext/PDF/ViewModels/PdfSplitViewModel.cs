using CrytonCoreNext.Abstract;
using CrytonCoreNext.Commands;
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
        public int FromPage { get; set; }

        public int ToPage { get; set; }

        public Visibility SplitButtonVisibility { get; set; } = Visibility.Hidden;

        public ICommand SplitCommand { get; set; }

        public PdfSplitViewModel(InteractiveViewBase pdfManagerViewModel, IPDFService pdfService) : base(pdfManagerViewModel, pdfService)
        {
            OnUpdate += HandleUpdate;
            SplitCommand = new AsyncCommand(Split, CanExecute);
        }

        private void HandleUpdate(object? sender, EventArgs e)
        {
            if (CurrentFile == null)
            {
                SplitButtonVisibility = Visibility.Hidden;
            }
            else
            {
                SplitButtonVisibility = Visibility.Hidden;
            }
            OnPropertyChanged(nameof(SplitButtonVisibility));
        }

        private async Task Split()
        {
            if (CurrentFile != null)
            {
                var newFile = await PdfService.Split(CurrentFile, FromPage, ToPage);
                PdfManagerViewModel.SendObject(newFile);
            }
        }
    }
}
