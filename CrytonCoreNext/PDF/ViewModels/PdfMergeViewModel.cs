using CrytonCoreNext.Abstract;
using CrytonCoreNext.Commands;
using CrytonCoreNext.Dictionaries;
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

        private int _currentPage = 0;

        private List<PDFImage> _images;

        private List<PDFFile> _files;

        public Visibility FileInformationVisibility { get; set; } = Visibility.Hidden;

        public Visibility MergeButtonVisibility { get; set; } = Visibility.Hidden;

        public Visibility NavigationButtonsVisibility { get; set; }

        public Visibility PreviousImageVisibility { get; set; }

        public Visibility NextImageVisibility { get; set; }

        public PDFFile? CurrentFile { get; set; }

        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (value == _currentPage || CurrentFile == null) return;
                _currentPage = value;
                CurrentFile.LastPage = _currentPage;
                UpdateImages();
                OnPropertyChanged(nameof(CurrentPage));
            }
        }

        public ImageSource Bitmap { get; set; }

        public ImageSource PreviousImage { get; set; }

        public ImageSource NextImage { get; set; }

        public ICommand PreviousCommand { get; set; }

        public ICommand NextCommand { get; set; }

        public ICommand MergeCommand { get; set; }

        public PdfMergeViewModel(InteractiveViewBase pdfManagerViewModel, IPDFService pdfService)
        {
            _images = new();
            _files = new();

            _pdfService = pdfService;
            _pdfManagerViewModel = pdfManagerViewModel;

            PreviousCommand = new Command(MovePreviousPage, CanExecute);
            NextCommand = new Command(MoveNextPage, CanExecute);
            MergeCommand = new AsyncCommand(Merge, CanExecute);
        }

        public override void SendObject(object obj)
        {
            if (obj is bool clear == true)
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
                //ImageViewerViewModel.PostImages(_images.Where(x => x.Guid == CurrentFile.Guid).First());
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
            CurrentPage = pageNumber > 0 ? --pageNumber : 0;
            UpdateImages();
        }

        private void MoveNextPage()
        {
            var pageNumber = CurrentFile.LastPage;
            CurrentPage = pageNumber < CurrentFile.NumberOfPages ? ++pageNumber : CurrentFile.NumberOfPages;
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
            if (pdfImage != null && CurrentFile != null)
            {
                var newPageNumber = CurrentFile.LastPage;
                Bitmap = pdfImage.Images[newPageNumber];
                NextImageVisibility = Visibility.Hidden;
                PreviousImageVisibility = Visibility.Hidden;
                if (newPageNumber > 0)
                {
                    PreviousImage = pdfImage.Images[newPageNumber - 1];
                    PreviousImageVisibility = Visibility.Visible;
                    OnPropertyChanged(nameof(PreviousImage));
                }
                if (newPageNumber < CurrentFile.NumberOfPages)
                {
                    NextImage = pdfImage.Images[newPageNumber + 1];
                    NextImageVisibility = Visibility.Visible;
                    OnPropertyChanged(nameof(NextImage));
                }
                OnPropertyChanged(nameof(Bitmap));
                OnPropertyChanged(nameof(PreviousImageVisibility));
                OnPropertyChanged(nameof(NextImageVisibility));
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
            OnPropertyChanged(nameof(NavigationButtonsVisibility));
        }
    }
}
