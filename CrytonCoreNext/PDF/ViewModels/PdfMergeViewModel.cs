using CrytonCoreNext.Abstract;
using CrytonCoreNext.Commands;
using CrytonCoreNext.Interfaces;
using CrytonCoreNext.PDF.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace CrytonCoreNext.PDF.ViewModels
{
    public class PdfMergeViewModel : ViewModelBase
    {
        private readonly InteractiveViewBase _pdfManagerViewModel;

        private List<PDFImage> _images;

        public Visibility FileInformationVisibility { get; set; } = Visibility.Hidden;

        public Visibility NavigationButtonsVisibility { get; set; }

        public PDFFile CurrentFile { get; set; }

        public ImageSource Bitmap { get; set; }

        public ICommand ClearFilesCommand { get; set; }

        public ICommand PreviousCommand { get; set; }

        public ICommand NextCommand { get; set; }

        public IImageView ImageViewerViewModel { get; init; }

        public PdfMergeViewModel(InteractiveViewBase pdfManagerViewModel, IImageView imageView)
        {
            _images = new();
            _pdfManagerViewModel = pdfManagerViewModel;
            ImageViewerViewModel = imageView;
            PreviousCommand = new Command(MovePreviousPage, CanExecute);
            NextCommand = new Command(MoveNextPage, CanExecute);
        }

        public override void SendObject(object obj)
        {
            if (obj is PDFFile file)
            {
                CurrentFile = file;
            }
            if (obj is List<PDFImage> images)
            {
                _images = images;
                ImageViewerViewModel.PostImages(_images.Where(x => x.Guid == CurrentFile.Guid).First());
            }

            FileInformationVisibility = obj == null ? Visibility.Hidden : Visibility.Visible;
            NavigationButtonsVisibility = CurrentFile.NumberOfPages > 1 ? Visibility.Visible : Visibility.Collapsed;
            UpdateContent();
            UpdateImages();
        }

        private void UpdateContent()
        {
            OnPropertyChanged(nameof(FileInformationVisibility));
            OnPropertyChanged(nameof(CurrentFile));
            OnPropertyChanged(nameof(ImageViewerViewModel));
            OnPropertyChanged(nameof(NavigationButtonsVisibility));
        }

        private void MovePreviousPage()
        {
            var pageNumber = CurrentFile.LastPage;
            CurrentFile.LastPage = pageNumber > 0 ? --pageNumber : 0;
            UpdateImages();
        }

        private void MoveNextPage()
        {
            var pageNumber = CurrentFile.LastPage;
            CurrentFile.LastPage = pageNumber < CurrentFile.NumberOfPages - 1 ? ++pageNumber : CurrentFile.NumberOfPages - 1;
            UpdateImages();
        }

        private void UpdateImages()
        {
            var pdfImage = _images.Where(x => x.Guid == CurrentFile.Guid).FirstOrDefault();
            if (pdfImage != null)
            {
                Bitmap = pdfImage.Images[CurrentFile.LastPage];
                OnPropertyChanged(nameof(Bitmap));
            }
        }
    }
}
