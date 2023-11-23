using CommunityToolkit.Mvvm.Input;
using CrytonCoreNext.Abstract;
using CrytonCoreNext.PDF.Interfaces;
using CrytonCoreNext.PDF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace CrytonCoreNext.PDF.Abstarct
{
    public abstract partial class PdfBase : ViewModelBase
    {
        private int _currentPage = 0;

        private List<PDFImage> _images;

        protected readonly IPDFService PdfService;

        protected readonly InteractiveViewBase PdfManagerViewModel;

        protected List<PDFFile> Files;

        protected event EventHandler OnUpdate;

        public Visibility FileInformationVisibility { get; set; } = Visibility.Hidden;

        public Visibility NavigationButtonsVisibility { get; set; }

        public Visibility PreviousImageVisibility { get; set; }

        public Visibility NextImageVisibility { get; set; }

        public PDFFile? CurrentFile { get; set; }

        public int CurrentPageDisplay { get => _currentPage + 1; }

        public int MaxPageDisplay { get => CurrentFile == null ? 0 : CurrentFile.NumberOfPages + 1; }

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
                OnPropertyChanged(nameof(CurrentPageDisplay));
            }
        }

        public ImageSource Bitmap { get; set; }

        public ImageSource PreviousImage { get; set; }

        public ImageSource NextImage { get; set; }

        public ICommand PreviousCommand { get; set; }

        public ICommand NextCommand { get; set; }

        public PdfBase(InteractiveViewBase pdfManagerViewModel, IPDFService pdfService)
        {
            _images = [];
            Files = [];

            PdfService = pdfService;
            PdfManagerViewModel = pdfManagerViewModel;
        }

        //public override void SendObject(object obj)
        //{
        //    if (obj is bool)
        //    {
        //        Files.Clear();
        //        CurrentFile = null;
        //    }
        //    if (obj is PDFFile file)
        //    {
        //        CurrentFile = file;
        //        OnPropertyChanged(nameof(MaxPageDisplay));
        //    }
        //    if (obj is List<PDFImage> images)
        //    {
        //        _images = images;
        //        //ImageViewerViewModel.PostImages(_images.Where(x => x.Guid == CurrentFile.Guid).First());
        //        UpdateImages();
        //    }
        //    if (obj is List<PDFFile> pdfFiles)
        //    {
        //        Files = pdfFiles;
        //    }
        //    UpdateContent();
        //}

        protected void ResetCurrentPage()
        {
            CurrentPage = 0;
        }

        [RelayCommand]
        private void MovePreviousPage()
        {
            var pageNumber = CurrentFile?.LastPage ?? 0;
            CurrentPage = pageNumber > 0 ? --pageNumber : 0;
            UpdateImages();
        }

        [RelayCommand]
        private void MoveNextPage()
        {
            var pageNumber = CurrentFile.LastPage;
            CurrentPage = pageNumber < CurrentFile.NumberOfPages ? ++pageNumber : CurrentFile.NumberOfPages;
            UpdateImages();
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
            OnUpdate.Invoke(null, null);
            if (CurrentFile == null)
            {
                FileInformationVisibility = Visibility.Hidden;
                NavigationButtonsVisibility = Visibility.Hidden;
            }
            else
            {
                NavigationButtonsVisibility = CurrentFile.NumberOfPages > 0 ? Visibility.Visible : Visibility.Hidden;
                FileInformationVisibility = Visibility.Visible;
            }
            OnPropertyChanged(nameof(FileInformationVisibility));
            OnPropertyChanged(nameof(CurrentFile));
            OnPropertyChanged(nameof(NavigationButtonsVisibility));
        }
    }
}
