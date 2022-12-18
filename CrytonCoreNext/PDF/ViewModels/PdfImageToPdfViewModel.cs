using CrytonCoreNext.Abstract;
using CrytonCoreNext.PDF.Abstarct;
using CrytonCoreNext.PDF.Interfaces;
using CrytonCoreNext.ViewModels;
using System;

namespace CrytonCoreNext.PDF.ViewModels
{
    public class PdfImageToPdfViewModel : PdfBase
    {
        public ViewModelBase FilesSelectorModel { get; set; }

        public PdfImageToPdfViewModel(InteractiveViewBase pdfManagerViewModel, ViewModelBase filesSelectorModel, IPDFService pdfService) : base(pdfManagerViewModel, pdfService)
        {
            FilesSelectorModel = filesSelectorModel;
            OnUpdate += HandleUpdate;
        }

        private void HandleUpdate(object? sender, EventArgs e)
        {
            foreach (var file in Files)
            {
                (FilesSelectorModel as FilesSelectorViewViewModel).InProgressItemListingViewModel.AddItem(file);
            }
        }

    }
}
