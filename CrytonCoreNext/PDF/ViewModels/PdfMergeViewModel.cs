using CrytonCoreNext.Abstract;
using CrytonCoreNext.Commands;
using CrytonCoreNext.Dictionaries;
using CrytonCoreNext.Interfaces;
using CrytonCoreNext.PDF.Interfaces;
using CrytonCoreNext.PDF.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace CrytonCoreNext.PDF.ViewModels
{
    public class PdfMergeViewModel : ViewModelBase
    {
        private readonly IPDFService _pdfService;

        private readonly InteractiveViewBase _pdfManagerViewModel;

        private List<PDFImage> _images;

        private List<PDFFile> _files;

        public Visibility FileInformationVisibility { get; set; } = Visibility.Hidden;

        public Visibility MergeButtonVisibility { get; set; } = Visibility.Hidden;

        public Visibility NavigationButtonsVisibility { get; set; }

        public PDFFile? CurrentFile { get; set; }

        public ImageSource Bitmap { get; set; }

        public ICommand PreviousCommand { get; set; }

        public ICommand NextCommand { get; set; }

        public ICommand MergeCommand { get; set; }

        public IImageView ImageViewerViewModel { get; init; }

        public PdfMergeViewModel(InteractiveViewBase pdfManagerViewModel, IPDFService pdfService, IImageView imageView)
        {
            _images = new();
            _files = new();
            _pdfService = pdfService;
            _pdfManagerViewModel = pdfManagerViewModel;
            ImageViewerViewModel = imageView;
            PreviousCommand = new Command(MovePreviousPage, CanExecute);
            NextCommand = new Command(MoveNextPage, CanExecute);
            MergeCommand = new AsyncCommand(Merge, CanExecute);
        }

        public override void SendObject(object? obj)
        {
            if (obj == null)
            {
                _files.Clear();
                CurrentFile = null;
            }
            if (obj is PDFFile file)
            {
                CurrentFile = file;
            }
            if (obj is List<PDFImage> images)
            {
                _images = images;
                ImageViewerViewModel.PostImages(_images.Where(x => x.Guid == CurrentFile.Guid).First());
                UpdateImages();
            }
            if (obj is List<PDFFile> pdfFiles)
            {
                _files = pdfFiles;
            }
            UpdateContent();
        }

        private void MovePreviousPage()
        {
            var pageNumber = CurrentFile?.LastPage ?? 0;
            CurrentFile.LastPage = pageNumber > 0 ? --pageNumber : 0;
            UpdateImages();
        }

        private void MoveNextPage()
        {
            var pageNumber = CurrentFile.LastPage;
            CurrentFile.LastPage = pageNumber < CurrentFile.NumberOfPages - 1 ? ++pageNumber : CurrentFile.NumberOfPages - 1;
            UpdateImages();
        }

        private async Task Merge()
        {
            if (_files.Count < 1)
            {
                Log(Enums.ELogLevel.Information, Language.Post("NotEnoughFiles"));
                return;
            }

            var newFile = await _pdfService.Merge(_files);
            _pdfManagerViewModel.SendObject(newFile);
        }

        private void UpdateImages()
        {
            var pdfImage = _images.Where(x => x.Guid == CurrentFile?.Guid).FirstOrDefault();
            if (pdfImage != null)
            {
                Bitmap = pdfImage.Images[CurrentFile.LastPage];
                OnPropertyChanged(nameof(Bitmap));
            }
        }

        private void UpdateContent()
        {
            if (CurrentFile == null)
            {
                FileInformationVisibility = Visibility.Hidden;
                NavigationButtonsVisibility = Visibility.Hidden;
                MergeButtonVisibility = Visibility.Hidden;
            }
            else
            {
                NavigationButtonsVisibility = CurrentFile.NumberOfPages > 1 ? Visibility.Visible : Visibility.Hidden;
                FileInformationVisibility = Visibility.Visible;
                MergeButtonVisibility = Visibility.Hidden;
            }
            if (_files.Count > 1)
            {
                MergeButtonVisibility = Visibility.Visible;
            }

            OnPropertyChanged(nameof(MergeButtonVisibility));
            OnPropertyChanged(nameof(FileInformationVisibility));
            OnPropertyChanged(nameof(CurrentFile));
            OnPropertyChanged(nameof(ImageViewerViewModel));
            OnPropertyChanged(nameof(NavigationButtonsVisibility));
        }
    }
}
