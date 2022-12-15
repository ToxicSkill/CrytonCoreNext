using CrytonCoreNext.Abstract;
using CrytonCoreNext.Commands;
using CrytonCoreNext.Dictionaries;
using CrytonCoreNext.PDF.Abstarct;
using CrytonCoreNext.PDF.Interfaces;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CrytonCoreNext.PDF.ViewModels
{
    public class PdfMergeViewModel : PdfBase
    {
        public Visibility MergeButtonVisibility { get; set; } = Visibility.Hidden;

        public ICommand MergeCommand { get; set; }

        public PdfMergeViewModel(InteractiveViewBase pdfManagerViewModel, IPDFService pdfService) : base(pdfManagerViewModel, pdfService)
        {
            OnUpdate += HandleUpdate;
            MergeCommand = new AsyncCommand(Merge, CanExecute);
        }

        private void HandleUpdate(object? sender, EventArgs e)
        {
            MergeButtonVisibility = Files.Count > 1 ? Visibility.Visible : Visibility.Hidden;
            OnPropertyChanged(nameof(MergeButtonVisibility));
        }

        private async Task Merge()
        {
            if (Files.Count < 1)
            {
                Log(Enums.ELogLevel.Information, Language.Post("NotEnoughFiles"));
                return;
            }

            var newFile = await PdfService.Merge(Files);
            PdfManagerViewModel.SendObject(newFile);
            ResetCurrentPage();
        }
    }
}
